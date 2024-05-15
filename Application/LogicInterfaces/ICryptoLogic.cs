namespace Application.LogicInterfaces;

public interface ICryptoLogic
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    byte[] HexStringToByteArray(string hex);
}