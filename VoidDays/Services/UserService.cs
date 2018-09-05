using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models.Interfaces;
using VoidDays.Models;
using Sodium;
namespace VoidDays.Services
{
    public class UserService : IUserService
    {
        IRepositoryBase<User> _userRepository;
        IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.UserRepository;
        }
        public void TestEncrypt()
        {
            var dataKey = SecretBox.GenerateKey();
            var intermediateKey = SecretBox.GenerateKey();
            
            var User = new User();
            User.UserName = "mick";
            User.Password = new SecureString();
            User.Password.AppendChar('p');
            User.DataKey = new SecureString();
            User.DataKey.AppendChar('d');
            User.IntermediateKey = new SecureString();
            User.IntermediateKey.AppendChar('i');

            var message = "this is the message";
            var encrypted = EncryptString(message, dataKey);
            User.Message = encrypted;

            var encryptDataKey = EncryptString(Utilities.BinaryToHex(dataKey), intermediateKey);
            User.DataKeyCipher = encryptDataKey;
            var salty = Utilities.BinaryToHex(PasswordHash.ScryptGenerateSalt());
            var bitty = Utilities.HexToBinary(salty);
            var passwordHash = PasswordHash.ScryptHashBinary(User.Password.ToString(),salty,PasswordHash.Strength.Medium);
            var salt = Sodium.SodiumCore.GetRandomBytes(1);
            //var bytes = Encoding.UTF8.GetBytes(passwordHash);
            var encryptInterKey = EncryptString(Utilities.BinaryToHex(intermediateKey),passwordHash);
            User.IntermediateKeyCipher = encryptInterKey;
            
            _userRepository.Insert(User);
            _unitOfWork.Save();

            var mick = _userRepository.Get(x => x.UserName == "mick").FirstOrDefault();
            var unEnInterKey = Decrypt(mick.IntermediateKeyCipher, Encoding.UTF8.GetBytes(User.Password.ToString()));
            var unEnDataKey = Decrypt(mick.DataKeyCipher, Utilities.HexToBinary(unEnInterKey));
            var dec = Decrypt(mick.Message,Utilities.HexToBinary(unEnDataKey));
        }
        public bool Login(string userName, SecureString password)
        {
            var user = _userRepository.Get(x => x.UserName == userName).FirstOrDefault();
            if (user == null)
                return false;
            if (VerifyPasswordHash(password, user.PasswordHash))
                return true;

            return false;
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

        public bool VerifyPasswordHash(SecureString password, string hash)
        {
            if (PasswordHash.ScryptHashStringVerify(hash, password.ToString()))
            {
                return true;
            }
            return false;
        }
    }

    public interface IUserService
    {
        void TestEncrypt();
    }
}
