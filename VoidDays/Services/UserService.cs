using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models.Interfaces;
using VoidDays.Models;
using Sodium;
using VoidDays.Services;
using VoidDays.ServiceReference2;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace VoidDays.Services
{
    public class UserService : IUserService
    {
        private VoidDaysLoginServiceClient _loginClient;
        IDatabaseService _databaseService;
        public UserService(IDatabaseService databaseService)
        {
            
            _databaseService = databaseService;
        }
        public static string ConnectionString { get; private set; }

        public void TestEncrypt()
        {
            /*
            var dataKey = SecretBox.GenerateKey();
            var intermediateKey = SecretBox.GenerateKey();

            var User = new User();
            User.UserName = "mick";
            User.Password = "password";
            User.DataKey = "datakey";
            User.IntermediateKey = "interkey";

            var message = "this is the message";
            var encrypted = EncryptString(message, dataKey);
            User.Message = encrypted;

            var pass = User.Password.ToString();
            var encryptDataKey = EncryptString(Utilities.BinaryToHex(dataKey), intermediateKey);
            User.DataKeyCipher = encryptDataKey;

            //returns a 32 byte hash
            var passHashKey = GenericHash.Hash(pass, (byte[])null, 32);
            var passwordHash = PasswordHash.ScryptHashBinary(Encoding.UTF8.GetBytes(pass), passHashKey, PasswordHash.Strength.Medium);
            var encryptInterKey = EncryptString(Utilities.BinaryToHex(intermediateKey), passwordHash);
            User.IntermediateKeyCipher = encryptInterKey;
            var actualPasswordHash = PasswordHash.ScryptHashString(pass, PasswordHash.Strength.Medium);
            User.PasswordHash = actualPasswordHash;

            //_userRepository.Insert(User);
            _unitOfWork.Save();

            //var mick = _userRepository.Get(x => x.UserName == "mick").FirstOrDefault();
            var unEnInterKey = Decrypt(mick.IntermediateKeyCipher, passwordHash);
            var unEnDataKey = Decrypt(mick.DataKeyCipher, Utilities.HexToBinary(unEnInterKey));

            if (PasswordHash.ScryptHashStringVerify(mick.PasswordHash, pass))
            {
                Console.WriteLine("true");
                //correct password
            }
            var dec = Decrypt(mick.Message, Utilities.HexToBinary(unEnDataKey));
            */

        }

        public bool CreateUser(string username, string password)
        {
            try
            {
                _loginClient = new VoidDaysLoginServiceClient();
                _loginClient.CreateUser(username, password);
                _loginClient.Close();
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        public bool Login(string userName, string password, string server, out string message) //should be a security token or something from server
        {
            _loginClient = new VoidDaysLoginServiceClient();
            message = "Login successful.";
            var schema = _loginClient.LoginUser(userName, password);
            var connectionString = SetConnectionString(userName, password, schema, server);
            if (CheckIfDatabaseExists("server = " + server + "; Port = 3306; user id = " + userName + "; password = " + password + "; persistsecurityinfo = True;"))
                return true;
            message = "Cannot connect to server";
            _loginClient.Close();
            return false;

        }
        string SetConnectionString(string userName, string password, string schema, string server)
        {
            _databaseService.ConnectionString = "server=" + server + "; Port=3306;user id=" + userName + ";password=" + password + ";persistsecurityinfo=True;database=" + schema;
            return _databaseService.ConnectionString;
        }
        public string GetConnectionString()
        {
            return ConnectionString;
        }
        bool CheckIfDatabaseExists(string connectionString)
        {
            using (EFDbContext dbContext = new EFDbContext(connectionString))
            {
                try
                {
                    dbContext.Database.Connection.Open();
                    dbContext.Database.Connection.Close();
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }
        public string EncryptString(string message, byte[] key)
        {
            var nonce = SecretBox.GenerateNonce();
            var secretMessage = SecretBox.Create(message, nonce, key);

            byte[] rv = new byte[nonce.Length + secretMessage.Length];
            System.Buffer.BlockCopy(nonce, 0, rv, 0, nonce.Length);
            System.Buffer.BlockCopy(secretMessage, 0, rv, nonce.Length, secretMessage.Length);
            return Utilities.BinaryToHex(rv);
        }
        public string Decrypt(string cipher, byte[] key)
        {
            byte[] cipherBytes = Utilities.HexToBinary(cipher);
            var nonce = cipherBytes.Take(24).ToArray();
            var messageBytes = cipherBytes.Skip(24).ToArray();
            var message = Utilities.BinaryToHex(messageBytes);

            var secret = SecretBox.Open(messageBytes, nonce, key);
            var secretMessage = Encoding.UTF8.GetString(secret);
            return secretMessage;
        }

        public bool VerifyPasswordHash(string password, string hash)
        {
            if (PasswordHash.ScryptHashStringVerify(hash, password))
            {
                return true;
            }
            return false;
        }
        
        public string CreateAppDataFolder()
        {
            var directory = GetUserDirectoryPath();
            var directoryInfo = Directory.CreateDirectory(directory);
            return directoryInfo.FullName;
        }
        public void RecreateAppSettingsFile()
        {
            CreateAppDataFolder();
            var fullFilename = GetAppSettingsFilePath();
            if (File.Exists(fullFilename))
                File.Delete(fullFilename);
            var settings = new AppSettings();
            var serialize = new XmlSerializer(typeof(AppSettings));
            using (TextWriter tw = new StreamWriter(fullFilename))
            {
                serialize.Serialize(tw, settings);
            }
        }
        public string GetAppSettingsFilePath()
        {
            return GetUserDirectoryPath() + "settings.xml";
        }
        public string GetUserDirectoryPath()
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            directory += "VoidDays";
            return directory;
        }

        AppSettings _appSettings;
        public AppSettings GetAppSettings()
        {
            if (_appSettings != null)
                return _appSettings;
            var settings = new AppSettings();
            var serialize = new XmlSerializer(typeof(AppSettings));
            if (!File.Exists(GetAppSettingsFilePath()))
                RecreateAppSettingsFile();
            using (TextReader tr = new StreamReader(GetAppSettingsFilePath()))
            {
                _appSettings = (AppSettings)serialize.Deserialize(tr);
            }
            return _appSettings;
        }
        public void SaveAppSettingsFile(AppSettings appSettings)
        {
            var fullFilename = GetAppSettingsFilePath();
            var serialize = new XmlSerializer(typeof(AppSettings));
            using (TextWriter tw = new StreamWriter(fullFilename))
            {
                serialize.Serialize(tw, appSettings);
            }
        }
        public void SaveDefaultServerAddress(string serverAddress)
        {
            _appSettings = null;
            GetAppSettings();
            _appSettings.ServerAddress = serverAddress;
            SaveAppSettingsFile(_appSettings);
        }
        public void SetLastUser(string username)
        {
            _appSettings = null;
            GetAppSettings();
            _appSettings.LastUser = username;
            SaveAppSettingsFile(_appSettings);
        }
    }

    public interface IUserService
    {
        bool Login(string userName, string password, string server, out string message);
        void TestEncrypt();
        string GetConnectionString();
        bool CreateUser(string username, string password);
        AppSettings GetAppSettings();
        void SaveDefaultServerAddress(string serverAddress);
        void SetLastUser(string username);
    }

    public interface IUserServiceFactory
    {
        UserService CreateUserService();
    }

}
