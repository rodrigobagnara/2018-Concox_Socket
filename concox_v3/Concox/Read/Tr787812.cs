using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace concox_v3.Concox.Read
{
    public class Tr787812
    {
        public static Array Convert(string data)
        { // ---------------------- 12

            // Pega a data e hora do pacotre de localização.
            string dataTimeGps = "";
            string anoDec = System.Convert.ToString(int.Parse(data.Substring(8, 2), NumberStyles.HexNumber));
            string mesDec = System.Convert.ToString(int.Parse(data.Substring(10, 2), NumberStyles.HexNumber));
            string diaDec = System.Convert.ToString(int.Parse(data.Substring(12, 2), NumberStyles.HexNumber));
            string hrsDec = System.Convert.ToString(int.Parse(data.Substring(14, 2), NumberStyles.HexNumber));
            string minDec = System.Convert.ToString(int.Parse(data.Substring(16, 2), NumberStyles.HexNumber));
            string secDec = System.Convert.ToString(int.Parse(data.Substring(18, 2), NumberStyles.HexNumber));

            if (mesDec.Length == 1)
            {
                mesDec = "0" + mesDec;
            }

            if (diaDec.Length == 1)
            {
                diaDec = "0" + diaDec;
            }

            if (hrsDec.Length == 1)
            {
                hrsDec = "0" + hrsDec;
            }

            if (minDec.Length == 1)
            {
                minDec = "0" + minDec;
            }

            if (secDec.Length == 1)
            {
                secDec = "0" + secDec;
            }

            dataTimeGps = "20" + anoDec + "-" + mesDec + "-" + diaDec + " " + hrsDec + ":" + minDec + ":" + secDec;

            // Trata o BYTE-01 de Course Status
            string dataCourseOne = "";
            //string courseOneHex = data.Substring(40, 2); // Extrai o Byte 1 da string
            dataCourseOne = System.Convert.ToString(System.Convert.ToInt32(data.Substring(40, 2), 16), 2); // Converte o Byte hexa em binario
            if (dataCourseOne.Length <= 8)
            { // Verifica o tamanho daconversão e verifica se ha os 8 bits do byte
                int maxCicleCourseOne = (8 - dataCourseOne.Length);
                int i = 0;
                for (i = 0; i < maxCicleCourseOne; i++)
                {
                    dataCourseOne = "0" + System.Convert.ToString(dataCourseOne); // Completa o byte a cada ciclo do for
                }
            }

            // Trata o BYTE-02 de Course Status
            string dataCourseTwo = "";
            //string courseTwoHex = data.Substring(42, 2); // Extrai o Byte 2 da string
            dataCourseTwo = System.Convert.ToString(System.Convert.ToInt32(data.Substring(42, 2), 16), 2); // Converte o Byte hexa em binario
            if (dataCourseTwo.Length <= 8)
            { // Verifica o tamanho daconversão e verifica se ha os 8 bits do byte
                int maxCicleCourseTwo = (8 - dataCourseTwo.Length);
                int j = 0;
                for (j = 0; j < maxCicleCourseTwo; j++)
                {
                    dataCourseTwo = "0" + System.Convert.ToString(dataCourseTwo); // Completa o byte a cada ciclo do for
                }
            }

            // Trata a latitude.
            string dataLat = "";
            string latHex = data.Substring(22, 8); // Estrai os 4 Bytes da string referentes a latitude
            string LatDec = System.Convert.ToString(int.Parse(latHex, NumberStyles.HexNumber)); // Converte para decimal
            // Para se obter o valor da latitude, deve-se dividir o valor decimal por 1800000, porem estoura a memoria da variavel
            // então... esse tratamento é feito em duas etapas.
            decimal auxLatDec = (decimal.Parse(LatDec) / 1800); // Primeira etapa
            auxLatDec = (auxLatDec / 1000); // Segunda etapa
            dataLat = System.Convert.ToString(auxLatDec);
            if (dataLat.Length > 9)
            {
                dataLat = dataLat.Substring(0, 9);
            }
            if (dataCourseOne.Substring(5, 1) == "0")
            { // Verifica o hemisfério (se a leitura for 0, então estamos no hemisfério sul)
                dataLat = "-" + dataLat;
            }
            dataLat = dataLat.Replace(',', '.'); // Troca a virgula por ponto

            // Trata a longitude.
            string dataLon = "";
            string lonHex = data.Substring(30, 8); // Estrai os 4 Bytes da string referentes a longitude
            string LonDec = System.Convert.ToString(int.Parse(lonHex, NumberStyles.HexNumber)); // Converte para decimal
            // Para se obter o valor da longitude, deve-se dividir o valor decimal por 1800000, porem estoura a memoria da variavel
            // então... esse tratamento é feito em duas etapas.
            decimal auxLonDec = (decimal.Parse(LonDec) / 1800); // Primeira etapa
            auxLonDec = (auxLonDec / 1000); // Segunda etapa
            dataLon = System.Convert.ToString(auxLonDec);
            if (dataLon.Length > 9)
            {
                dataLon = dataLon.Substring(0, 9);
            }
            if (dataCourseOne.Substring(4, 1) == "1")
            { // Verifica o hemisfério (se a leitura for 1, então estamos no hemisfério ocidental)
                dataLon = "-" + dataLon;
            }
            dataLon = dataLon.Replace(',', '.'); // Troca a virgula por ponto

            // Trata velocidade do veículo
            string dataSpeed = "";
            string speedHex = data.Substring(38, 2);
            dataSpeed = System.Convert.ToString(int.Parse(speedHex, NumberStyles.HexNumber));

            // Trata direção do veículo
            string dataDirection = dataCourseOne.Substring(6, 2) + dataCourseTwo;
            int auxDirection = System.Convert.ToInt32(dataDirection, 2);
            if (auxDirection < 22.5 || auxDirection >= 337.5)
            {
                dataDirection = "0";
            }
            else if (auxDirection >= 22.5 && auxDirection < 67.5)
            {
                dataDirection = "1";
            }
            else if (auxDirection >= 67.5 && auxDirection < 112.5)
            {
                dataDirection = "2";
            }
            else if (auxDirection >= 112.5 && auxDirection < 157.5)
            {
                dataDirection = "3";
            }
            else if (auxDirection >= 157.5 && auxDirection < 202.5)
            {
                dataDirection = "4";
            }
            else if (auxDirection >= 202.5 && auxDirection < 247.5)
            {
                dataDirection = "5";
            }
            else if (auxDirection >= 247.5 && auxDirection < 292.5)
            {
                dataDirection = "6";
            }
            else if (auxDirection >= 292.5 && auxDirection < 337.5)
            {
                dataDirection = "7";
            }

            // Trata ignição do veículo
            string dataAcc = dataCourseOne.Substring(0, 1);

            // Trata quantidade de satelites de GPS
            string dataSat = data.Substring(21, 1);
            dataSat = System.Convert.ToString(int.Parse(dataSat, NumberStyles.HexNumber)); // Converte para decimal

            // Obtem o protocolo do pacote.
            string dataProtocol = data.Substring(6, 2);

            // Obtem o serial do pacote.
            string dataSerial = System.Convert.ToString(int.Parse(data.Substring(60, 4), NumberStyles.HexNumber)); // Converte para decimal

            Thread.Sleep(250); // TO DO Thread

            //Retorna arrai com informações do pacote 12.
            string[] pacotes = new string[10];
            pacotes[0] = dataTimeGps;
            pacotes[1] = dataLat;
            pacotes[2] = dataLon;
            pacotes[3] = dataSpeed;
            pacotes[4] = System.Convert.ToString(auxDirection);
            pacotes[5] = dataDirection;
            pacotes[6] = dataAcc;
            pacotes[7] = dataSat;
            pacotes[8] = dataProtocol;
            pacotes[9] = dataSerial;
            return pacotes;
        }
    }
}
