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

namespace VoidDays.Services
{
    public class UserService : IUserService
    {
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
        public void Login(string userName, string password, string schema)
        {
            SetConnectionString(userName, password, schema);
        }
        void SetConnectionString(string userName, string password, string schema)
        {
            _databaseService.ConnectionString = "server=localhost;user id=" + userName + ";password=" + password + ";persistsecurityinfo=True;database=" + schema;
        }
        public string GetConnectionString()
        {
            return ConnectionString;
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
    }

    public interface IUserService
    {
        void Login(string userName, string password, string schema);
        void TestEncrypt();
        string GetConnectionString();
    }


}
