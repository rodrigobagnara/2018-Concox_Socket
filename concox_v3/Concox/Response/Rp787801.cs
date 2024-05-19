using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace concox_v3.Concox.Response
{
    public class Rp787801
    {
        public static void Send(string data, Socket client)
        { // ---------------------- 01

            // CONSTRUINDO RESPOSTA PT1
            string[] dataPacket = new string[6];
            string aux = "";
            dataPacket[0] = "120"; // Start Bit 1/2
            dataPacket[1] = "120"; // Start Bit 2/2
            dataPacket[2] = "05"; // Packet Length
            dataPacket[3] = "01"; // Protocol Number
            aux = Convert.ToString(data.Substring(24, 2));
            dataPacket[4] = Convert.ToString(int.Parse(aux, NumberStyles.HexNumber)); // Serial Number 1/2
            aux = Convert.ToString(data.Substring(26, 2));
            dataPacket[5] = Convert.ToString(int.Parse(aux, NumberStyles.HexNumber)); // Serial Number 2/2

            // ORGANIZANDO ARRAY DE BYTES PARA CALCULAR O CRC16-UIT (PRÉ CODIFICA ARRAY)
            byte[] dataPacketByte = new byte[6];
            dataPacketByte[0] = byte.Parse(dataPacket[0]);
            dataPacketByte[1] = byte.Parse(dataPacket[1]);
            dataPacketByte[2] = byte.Parse(dataPacket[2]);
            dataPacketByte[3] = byte.Parse(dataPacket[3]);
            dataPacketByte[4] = byte.Parse(dataPacket[4]);
            dataPacketByte[5] = byte.Parse(dataPacket[5]);

            // ENVIANDO PARA O CALCULO DO CRC16-ITU
            var crc16 = CRC16.Get(dataPacketByte, 2, 4);
            string crcDecimal = Convert.ToString(crc16);
            string crcHexadecimal = "";
            crcHexadecimal = Convert.ToString(long.Parse(crcDecimal), 16).ToUpper();
            if (crcHexadecimal.Length == 3)
            {
                crcHexadecimal = "0" + crcHexadecimal; // COMPLETA O BYTE COM UM ZERO 0
            }
            else if (crcHexadecimal.Length == 2)
            {
                crcHexadecimal = "00" + crcHexadecimal; // COMPLETA O BYTE COM DOIS ZEROS 0
            }

            // CONSTRUINDO RESPOSTA (STRING)
            string respostaPost = "";
            respostaPost += "78"; // Start Bit 1/2 
            respostaPost += "78"; // Start Bit 2/2 
            respostaPost += "05"; // Packet Length
            respostaPost += "01"; // Protocol Number
            respostaPost += data.Substring(24, 4); // Serial Number
            respostaPost += crcHexadecimal; // Error check
            respostaPost += data.Substring(32, 4); // Stop Bit

            // CONSTRUINDO RESPOSTA PT2 (ARRAY DE BYTES)
            string[] dataPost = new string[10];
            byte[] postInfo = new byte[10];
            string msg = "";
            postInfo[0] = dataPacketByte[0]; // Start Bit 1/2
            postInfo[1] = dataPacketByte[1]; // Start Bit 2/2
            postInfo[2] = dataPacketByte[2]; // Packet Length
            postInfo[3] = dataPacketByte[3]; // Protocol Number
            postInfo[4] = dataPacketByte[4]; // Serial Number 1/2
            postInfo[5] = dataPacketByte[5]; // Serial Number 2/2
            msg = Convert.ToString(respostaPost.Substring(12, 2));
            dataPost[6] = Convert.ToString(int.Parse(msg, NumberStyles.HexNumber)); // Error check 1/2
            postInfo[6] = byte.Parse(dataPost[6]);
            msg = Convert.ToString(respostaPost.Substring(14, 2));
            dataPost[7] = Convert.ToString(int.Parse(msg, NumberStyles.HexNumber)); // Error check 2/2
            postInfo[7] = byte.Parse(dataPost[7]);
            msg = Convert.ToString(respostaPost.Substring(16, 2));
            dataPost[8] = Convert.ToString(int.Parse(msg, NumberStyles.HexNumber)); // Stop Bit 1/2
            postInfo[8] = byte.Parse(dataPost[8]);
            msg = Convert.ToString(respostaPost.Substring(18, 2));
            dataPost[9] = Convert.ToString(int.Parse(msg, NumberStyles.HexNumber)); // Stop Bit 2/2
            postInfo[9] = byte.Parse(dataPost[9]);

            // ENVIANDO RESPOSTA PARA O TERMINAL(GPS)
            Array.Resize(ref postInfo, postInfo.Length);
            client.Send(postInfo);
            Thread.Sleep(250);
        }
    }
}
