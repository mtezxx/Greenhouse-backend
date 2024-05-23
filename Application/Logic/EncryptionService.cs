using System;
using System.Security.Cryptography;
using System.Text;
using Application.LogicInterfaces;

namespace Application.Logic;

public class EncryptionService : IEncryptionService
    {
        private const string KeyHex = "44dec5ccbdf9c2ec53bfd387df9f47ef";
        private static readonly byte[] Key = Convert.FromHexString(KeyHex);

        public byte[] Decrypt(byte[] encryptedData)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.None;
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.Key = Key;

                return aesAlg.DecryptEcb(encryptedData, PaddingMode.None);
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.None;
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.Key = Key;

                return aesAlg.EncryptEcb(data, PaddingMode.None);
            }
        }

        public ushort ComputeChecksum(byte[] bytes, int offset, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = offset; i < offset + length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ Crc16.Table[index]);
            }
            return crc;
        }

        public byte[] FromHexString(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public string ToHexString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "");
        }

        public byte[] PrepareDataForResponse(ushort boardId, uint timestamp, byte commandCode, byte ledStatus, byte servoStatus)
        {
            ushort transformedBoardId = (ushort)(boardId | 0x8000); // Or with 0x8000 to set the highest bit

            byte[] data = new byte[16];

            byte[] boardIdBytes = BitConverter.GetBytes(transformedBoardId);
            Array.Reverse(boardIdBytes); // Reverse byte order
            Array.Copy(boardIdBytes, 0, data, 0, 2);

            byte[] timestampBytes = BitConverter.GetBytes(timestamp);
            Array.Reverse(timestampBytes); // Reverse byte order
            Array.Copy(timestampBytes, 0, data, 2, 4);

            data[6] = commandCode;
            data[7] = ledStatus;
            data[8] = servoStatus;
            // Unused bytes are already 0 by default in the array initialization

            ushort crc = ComputeChecksum(data, 0, 14);
            byte[] crcBytes = BitConverter.GetBytes(crc);
            Array.Reverse(crcBytes); // Reverse byte order
            Array.Copy(crcBytes, 0, data, 14, 2);

            return data;
        }

        public byte[] PrepareDataForDecryption(ushort boardId, uint timestamp, ushort humidity, ushort temperature, ushort lux, ushort unused)
        {
            byte[] data = new byte[16];

            BitConverter.GetBytes(boardId).CopyTo(data, 0);
            BitConverter.GetBytes(timestamp).CopyTo(data, 2);
            BitConverter.GetBytes(humidity).CopyTo(data, 6);
            BitConverter.GetBytes(temperature).CopyTo(data, 8);
            BitConverter.GetBytes(lux).CopyTo(data, 10);
            BitConverter.GetBytes(unused).CopyTo(data, 12);

            ushort crc = ComputeChecksum(data, 0, 14);
            BitConverter.GetBytes(crc).CopyTo(data, 14);

            return data;
        }

        public (ushort boardId, uint timestamp, ushort humidity, ushort temperature, ushort lux, ushort unused, ushort crc) ParseDataForDecryption(byte[] data)
        {
            if (data.Length < 16)
                throw new ArgumentException("Data is too short!");

            ushort boardId = ReverseBytes(BitConverter.ToUInt16(data, 0));
            uint timestamp = ReverseBytes(BitConverter.ToUInt32(data, 2));
            ushort humidityRaw = ReverseBytes(BitConverter.ToUInt16(data, 6));
            ushort temperatureRaw = ReverseBytes(BitConverter.ToUInt16(data, 8));
            ushort lux = ReverseBytes(BitConverter.ToUInt16(data, 10));
            ushort unused = ReverseBytes(BitConverter.ToUInt16(data, 12));
            ushort crc = ReverseBytes(BitConverter.ToUInt16(data, 14));

            double humidity = (humidityRaw >> 8) + ((humidityRaw & 0xFF) / 100.0); // Extract integer and decimal parts
            double temperature = (temperatureRaw >> 8) + ((temperatureRaw & 0xFF) / 100.0); // Extract integer and decimal parts

            ushort humidityUshort = (ushort)(humidity * 100);
            ushort temperatureUshort = (ushort)(temperature * 100);

            return (boardId, timestamp, humidityUshort, temperatureUshort, lux, unused, crc);
        }
        private ushort ReverseBytes(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        private uint ReverseBytes(uint value)
        {
            return ((value & 0x000000FFU) << 24) |
                   ((value & 0x0000FF00U) << 8) |
                   ((value & 0x00FF0000U) >> 8) |
                   ((value & 0xFF000000U) >> 24);
        }
    }

    public static class Crc16
    {
        public const ushort Polynomial = 0xA001;
        public static readonly ushort[] Table = new ushort[256];

        static Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < Table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ Polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                Table[i] = value;
            }
        }
    }