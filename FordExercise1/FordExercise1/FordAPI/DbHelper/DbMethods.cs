using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using FordAPI.Helper;
using System.Data.SqlClient;
using System.Collections;

namespace FordAPI.DbHelper
{
    public class DbMethods
    {
        public static DataSet TokenGenration(string OldTokenId, string Ip, string NewTokenId)
        {
            string strSiteId = ConfigurationManager.AppSettings["SiteId"].ToString();
            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                                                                                //DefaultConnection    
                //string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionLive"].ToString();
                //string ConnectionString = ConfigurationManager.ConnectionStrings["DmsContentEsCountries_230"].ToString();
                String sSql = "EXEC [GDOneFeedAPI_GenerateSiteTokenId]";
                sSql = sSql + " @OldTokenId='" + OldTokenId + "',";
                sSql = sSql + " @NewTokenId='" + NewTokenId + "',";
                sSql = sSql + " @IP='" + Ip + "',";
                sSql = sSql + " @SiteId=" + strSiteId + "";
                return SqlHelper.ExecuteDataset(ConnectionString, sSql.TrimEnd(','));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message.ToString());
            }
        }

        public static DataSet LoginTokenGenration(string OldLTokenId, string Ip, string NewLTokenId)
        {
            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                String sSql = "EXEC [GDOneFeedAPI_LoginGenerateTokenId]";
                sSql = sSql + " @OldLTokenId='" + OldLTokenId + "',";
                sSql = sSql + " @NewLTokenId='" + NewLTokenId + "',";
                sSql = sSql + " @IP='" + Ip + "',";
                return SqlHelper.ExecuteDataset(ConnectionString, sSql.TrimEnd(','));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message.ToString());
            }
        }

        public static string CheckTokenId(string TokenId)
        {
            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                String sSql = "EXEC [GDOneFeedAPI_CheckTokenId]";
                sSql = sSql + " @TokenId='" + TokenId.Replace("'", "''") + "',";
                DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, sSql.TrimEnd(','));

                string Token = "";
                if (ds != null)
                {
                    Token = ds.Tables[0].Rows[0][0].ToString();
                }
                return Token;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message.ToString());
            }
        }

       
        public static string Encrypt(string TextToBeEncrypted)
        {

            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            string Password = "DMV";

            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(TextToBeEncrypted);

            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

            //Creates a symmetric encryptor object. 

            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();

            //Defines a stream that links data streams to cryptographic transformations

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(PlainText, 0, PlainText.Length);

            //Writes the final state and clears the buffer

            cryptoStream.FlushFinalBlock();

            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();

            cryptoStream.Close();

            string EncryptedData = Convert.ToBase64String(CipherBytes);

            return EncryptedData;

        }

        private string GenerateRandomNumber()
        {
            string strUID = Guid.NewGuid().ToString();
            strUID = strUID.Substring(0, strUID.IndexOf("-"));
            return strUID;
        }

        

       

        
       

        
       
      

        

       
        

        
        

      

        
        
       

      

        
    }
}