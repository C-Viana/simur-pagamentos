using System.Text;

namespace simur_backend.Utilities
{
    public class AlgorithmCRC16
    {
        public static string EncodeString(string stringValue)
        {
            byte[] data = Encoding.UTF8.GetBytes(stringValue);
            ushort crc = 0xFFFF; // Initial value

            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                    {
                        // Left shift and XOR with polynomial 0x1021
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            return crc.ToString("X");
        }
    }
}
