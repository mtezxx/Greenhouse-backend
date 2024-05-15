using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Application.LogicInterfaces;

namespace Application.Logic;

public class CryptoLogic : ICryptoLogic
{
    private readonly byte[] key = { 0x44, 0xde, 0xc5, 0xcc, 0xbd, 0xf9, 0xc2, 0xec, 0x53, 0xbf, 0xd3, 0x87, 0xdf, 0x9f, 0x47, 0xef };

    public string Encrypt(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = aes.IV;
            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            var encrypted = ms.ToArray();
            var result = new byte[iv.Length + encrypted.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }
    }

    public string Decrypt(string cipherTextHex)
    {
        // Convert hex string to byte array
        Console.WriteLine(cipherTextHex);
        var fullCipher = HexStringToByteArray(cipherTextHex);
        
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Assume first 16 bytes are the IV
            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var decryptor = aes.CreateDecryptor(aes.Key, iv);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            Console.WriteLine(string.Join(", ", cipher));
            string result = Encoding.UTF8.GetString(cipher);
            Console.WriteLine("Decoded Text: " + result);
            return sr.ReadToEnd();
        }
    }

    public byte[] HexStringToByteArray(string hex)
    {
        Console.WriteLine(hex);
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        Console.WriteLine(string.Join(", ", bytes));

        return bytes;
    }
    
    
    
}
