namespace Application.LogicInterfaces;

public interface IEncryptionService
{
    byte[] Decrypt(byte[] encryptedData);
    byte[] Encrypt(byte[] data);
    ushort ComputeChecksum(byte[] bytes, int offset, int length);
    byte[] FromHexString(string hex);
    string ToHexString(byte[] data);
    byte[] PrepareDataForResponse(ushort boardId, uint timestamp, byte commandCode, byte ledStatus, byte servoStatus);
    byte[] PrepareDataForDecryption(ushort boardId, uint timestamp, ushort humidity, ushort temperature, ushort lux, ushort unused);
    (ushort boardId, uint timestamp, ushort humidity, ushort temperature, ushort lux, ushort unused, ushort crc) ParseDataForDecryption(byte[] data);
}