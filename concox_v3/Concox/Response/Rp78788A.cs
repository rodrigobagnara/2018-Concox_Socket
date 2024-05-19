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
    public class Rp78788A
    {
        public static void Send(string data, Socket client)
        { // ------ 8A

            // Data e hora servidor
            DateTime dt = DateTime.Now;
            string dateHour = dt.ToString();

            // CONSTRUINDO RESPOSTA PT1
            string[] dataPacket = new string[12];
            string aux = "";
            dataPacket[0] = "120"; // Start Bit 1/2
            dataPacket[1] = "120"; // Start Bit 2/2
            aux = "0B";
            dataPacket[2] = Convert.ToString(int.Parse(aux, NumberStyles.HexNumber)); // Packet Length
            aux = "8A";
            dataPacket[3] = Convert.ToString(int.Parse(aux, NumberStyles.HexNumber)); // Protocol Number
            dataPacket[4] = Convert.ToString(dateHour.Substring(8, 2)); // Ano
            dataPacket[5] = Convert.ToString(dateHour.Substring(3, 2)); // Mês
            dataPacket[6] = Convert.ToString(dateHour.Substring(0, 2)); // Dia
            dataPacket[7] = Convert.ToString(dateHour.Substring(11, 2)); // Hora
            dataPacket[8] = Convert.ToString(dateHour.Substring(14, 2)); // Min
            dataPacket[9] = Convert.ToString(dateHour.Substring(17, 2)); // Seg
            aux = Convert.ToString(data.Substring(8, 2));
            dataPacket[10] = Convert.ToString(int.Parse(aux, NumberStyles.HexNumber)); // Serial Number 1/2
            aux = Convert.ToString(data.Substring(10, 2));
            dataPacket[11] = Convert.ToString(int.Parse(aux, NumberStyles.HexNumber)); // Serial Number 2/2

            // ORGANIZANDO ARRAY DE BYTES PARA CALCULAR O CRC16-UIT (PRÉ CODIFICA ARRAY)
            byte[] dataPacketByte = new byte[12];
            dataPacketByte[0] = byte.Parse(dataPacket[0]); // Start Bit 1/2
            dataPacketByte[1] = byte.Parse(dataPacket[1]); // Start Bit 2/2
            dataPacketByte[2] = byte.Parse(dataPacket[2]); // Packet Length
            dataPacketByte[3] = byte.Parse(dataPacket[3]); // Protocol Number
            dataPacketByte[4] = byte.Parse(dataPacket[4]); // Ano
            dataPacketByte[5] = byte.Parse(dataPacket[5]); // Mês
            dataPacketByte[6] = byte.Parse(dataPacket[6]); // Dia
            dataPacketByte[7] = byte.Parse(dataPacket[7]); // Hora
            dataPacketByte[8] = byte.Parse(dataPacket[8]); // Min
            dataPacketByte[9] = byte.Parse(dataPacket[9]); // Seg
            dataPacketByte[10] = byte.Parse(dataPacket[10]); // Serial Number 1/2
            dataPacketByte[11] = byte.Parse(dataPacket[11]); // Serial Number 2/2

            // ENVIANDO PARA O CALCULO DO CRC16-ITU
            var crc16 = CRC16.Get(dataPacketByte, 2, 10);
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
            respostaPost += "120"; // Start Bit 1/2 
            respostaPost += "120"; // Start Bit 2/2 
            respostaPost += "0B"; // Packet Length
            respostaPost += "8A"; // Protocol Number
            respostaPost += dateHour.Substring(8, 2); // Ano
            respostaPost += dateHour.Substring(3, 2); // Mês
            respostaPost += dateHour.Substring(0, 2); // dia
            respostaPost += dateHour.Substring(11, 2); // hrs
            respostaPost += dateHour.Substring(14, 2); // min
            respostaPost += dateHour.Substring(17, 2); // seg
            respostaPost += data.Substring(8, 4); // Serial Number
            respostaPost += crcHexadecimal; // Error check
            respostaPost += data.Substring(16, 4); // Stop Bit

            // CONSTRUINDO RESPOSTA PT2 (ARRAY DE BYTES)
            string[] dataPost = new string[16];
            byte[] postInfo = new byte[16];
            string msg = "";
            postInfo[0] = dataPacketByte[0]; // Start Bit 1/2
            postInfo[1] = dataPacketByte[1]; // Start Bit 2/2
            postInfo[2] = dataPacketByte[2]; // Packet Length
            postInfo[3] = dataPacketByte[3]; // Protocol Number
            postInfo[4] = dataPacketByte[4]; // Ano
            postInfo[5] = dataPacketByte[5]; // Mês
            postInfo[6] = dataPacketByte[6]; // Dia
            postInfo[7] = dataPacketByte[7]; // Hora
            postInfo[8] = dataPacketByte[8]; // Min
            postInfo[9] = dataPacketByte[9]; // Seg
            postInfo[10] = dataPacketByte[10]; // Serial Number 1/2
            postInfo[11] = dataPacketByte[11]; // Serial Number 2/2
            msg = Convert.ToString(respostaPost.Substring(24, 2));
            dataPost[12] = Convert.ToString(int.Parse(msg, NumberStyles.HexNumber)); // Error check 1/2
            postInfo[12] = byte.Parse(dataPost[12]);
            msg = Convert.ToString(respostaPost.Substring(26, 2));
            dataPost[13] = Convert.ToString(int.Parse(msg, NumberStyles.HexNumber)); // Error check 2/2
            postInfo[13] = byte.Parse(dataPost[13]);
            msg = Convert.ToString(respostaPost.Substring(28, 2));
            dataPost[14] = Convert.ToString(int.Parse(msg, NumberStyles.HexNumber)); // Stop Bit 1/2
            postInfo[14] = byte.Parse(dataPost[14]);
            msg = Convert.ToString(respostaPost.Substring(30, 2));
            dataPost[15] = Convert.ToString(int.Parse(msg, NumberStyles.HexNumber)); // Stop Bit 2/2
            postInfo[15] = byte.Parse(dataPost[15]);

            // ENVIANDO RESPOSTA PARA O TERMINAL(GPS)
            Array.Resize(ref postInfo, postInfo.Length);
            client.Send(postInfo);
            Thread.Sleep(250);
        }
    }
}
