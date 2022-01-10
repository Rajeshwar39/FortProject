using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using FordAPI.Helper;
using System.Net.Mail;
using System.Net;

/// <summary>
/// Summary description for DBMethods
/// </summary>
public class DBMethods
{
    
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

    

    public static bool isDate(string date)
    {
        DateTime n;
        bool isdate = DateTime.TryParse(date, out n);
        return isdate;
    }

    
}