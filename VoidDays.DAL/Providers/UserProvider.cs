using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Sodium;
namespace VoidDays.DAL.Providers
{
    public class UserProvider
    {
        string _rootDb = "voiddaysroot";
        string _connectionString = "Server=localhost; Port=3306; Uid=crate;Database=voiddaysroot; Pwd=Sprint100;convert zero datetime=True;max pool size=200;sslmode=none;";
        MySqlConnection _dbConn;
        public string EncryptString(string message, byte[] key)
        {
            var nonce = SecretBox.GenerateNonce();
            var secretMessage = SecretBox.Create(message, nonce, key);

            byte[] rv = new byte[nonce.Length + secretMessage.Length];
            System.Buffer.BlockCopy(nonce, 0, rv, 0, nonce.Length);
            System.Buffer.BlockCopy(secretMessage, 0, rv, nonce.Length, secretMessage.Length);
            return Utilities.BinaryToHex(rv);
        }
        public string EncryptPassword(string password)
        {
            return PasswordHash.ScryptHashString(password, PasswordHash.Strength.Medium);
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

            Console.WriteLine("User service");
            Console.WriteLine(password);
            Console.WriteLine(hash);
            try
            {
                if (PasswordHash.ScryptHashStringVerify(hash, password))
                {
                    Console.WriteLine("return true from passowrd");
                    return true;
                }
                Console.WriteLine("return false from passwd");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception after hash verifiy --");
                Console.WriteLine(e.Message);

            }
            return false;
        }

        public bool CreateUser(string username, string password)
        {
            try
            {
                Console.WriteLine("create user " + username + " " + password);
                _dbConn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                Console.WriteLine("connection " + _connectionString);
                MySqlTransaction transaction;

                MySqlCommand newUserCommand = _dbConn.CreateCommand();
                newUserCommand.CommandText = "create user @user identified by @password;";
                newUserCommand.Parameters.AddWithValue("@user", username);
                newUserCommand.Parameters.AddWithValue("@password", password);

                string passwordHash = EncryptPassword(password);

                MySqlCommand getSuffixCommand = _dbConn.CreateCommand();
                getSuffixCommand.CommandText = "select MAX(tablesuffix) FROM  " + _rootDb + ".users";


                try
                {
                    _dbConn.Open();
                }
                catch (Exception error)
                {
                    Console.WriteLine("Could not connect\n" + error.Message);
                }

                string schemaPrefix = "voiddays_";
                int suffix = 0;

                newUserCommand.ExecuteNonQuery();
                var reader = getSuffixCommand.ExecuteReader();
                Console.WriteLine("before read ");
                if (reader.Read())
                {
                    try
                    {
                        suffix = reader.GetInt32(0);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                reader.Close();
                ++suffix;
                Console.WriteLine("suffix " + suffix);
                var schemaName = schemaPrefix + suffix;
                MySqlCommand createSchemaCommand = _dbConn.CreateCommand();
                createSchemaCommand.CommandText = "create schema " + schemaName;

                MySqlCommand dropSchemaCommand = _dbConn.CreateCommand();
                dropSchemaCommand.CommandText = "drop schema " + schemaName;

                MySqlCommand grantCommand = _dbConn.CreateCommand();
                grantCommand.CommandText = "GRANT ALL ON " + schemaName + ".* To @username";
                grantCommand.Parameters.AddWithValue("@username", username);

                MySqlCommand flushCommand = _dbConn.CreateCommand();
                flushCommand.CommandText = "Flush privileges";

                MySqlCommand insertUserTable = _dbConn.CreateCommand();
                insertUserTable.CommandText = "INSERT INTO `" + _rootDb + "`.`users` (`username`, `password`, `schemaname`, `tablesuffix`) VALUES (@username, @passwordHash, @schemaName, @suffix);";
                insertUserTable.Parameters.AddWithValue("@username", username);
                insertUserTable.Parameters.AddWithValue("@passwordHash", passwordHash);
                insertUserTable.Parameters.AddWithValue("@schemaName", schemaName);
                insertUserTable.Parameters.AddWithValue("@suffix", suffix);



                transaction = _dbConn.BeginTransaction();

                createSchemaCommand.Connection = _dbConn;
                createSchemaCommand.Transaction = transaction;

                insertUserTable.Connection = _dbConn;
                insertUserTable.Transaction = transaction;

                grantCommand.Connection = _dbConn;
                grantCommand.Transaction = transaction;

                flushCommand.Connection = _dbConn;
                flushCommand.Transaction = transaction;

                dropSchemaCommand.Connection = _dbConn;
                dropSchemaCommand.Transaction = transaction;
                try
                {

                    createSchemaCommand.ExecuteNonQuery();
                    Console.WriteLine("schema ");
                    grantCommand.ExecuteNonQuery();
                    Console.WriteLine("grant ");
                    insertUserTable.ExecuteNonQuery();
                    Console.WriteLine("insert user ");
                    flushCommand.ExecuteNonQuery();
                    Console.WriteLine("flush ");
                    dropSchemaCommand.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine("commit ");
                }
                catch (Exception e)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (MySqlException ex)
                    {
                        if (transaction.Connection != null)
                        {
                            Console.WriteLine("An exception of type " + ex.GetType() +
                            " was encountered while attempting to roll back the transaction.");
                        }
                    }
                    Console.WriteLine("An exception of type " + e.GetType() + " was encountered while inserting the data.");
                    Console.WriteLine("Neither record was written to database.");
                }
                finally
                {
                    _dbConn.Close();
                }
                Console.WriteLine("created user!");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
