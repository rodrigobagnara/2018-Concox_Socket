using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace concox_v3.Concox.Read
{
    public class Tr787813
    {
        public static Array Convert(string data)
        { // ---------------------- 13

            // Informações de conexão do rastreador.
            // Extrai o Byte 1 de Terminal information
            string infTrack = data.Substring(8, 2);
            infTrack = System.Convert.ToString(System.Convert.ToInt32(infTrack, 16), 2); // Converte o Byte hexa em binario
            if (infTrack.Length < 8)
            { // Verifica o tamanho daconversão e verifica se ha os 8 bits do byte
                int maxDataTerminal = (8 - infTrack.Length);
                int j = 0;
                for (j = 0; j < maxDataTerminal; j++)
                {
                    infTrack = "0" + System.Convert.ToString(infTrack); // Completa o byte a cada ciclo do for
                }
            }

            string eventSource = infTrack.Substring(0, 1);
            string source = "";
            if (eventSource == "1")
            { // Sem alimentação
                source = "0";
            }
            else if (eventSource == "0")
            { // Com alimentação
                source = "1";
            }

            string eventGps = infTrack.Substring(1, 1);
            string statusGps = "";
            if (eventGps == "1")
            { // Ativado
                statusGps = "1";
            }
            else if (eventGps == "0")
            { // Desativado
                statusGps = "0";
            }

            string eventLoading = infTrack.Substring(5, 1); // Carregando
            string eventIgnition = infTrack.Substring(6, 1); // Ignição
            string eventDefense = infTrack.Substring(7, 1); // Defesa

            // Tensão bateria rastreador.
            string batTrack = data.Substring(10, 2);
            switch (batTrack)
            {
                case "00":
                    batTrack = "0";
                    break;
                case "01":
                    batTrack = "15";
                    break;
                case "02":
                    batTrack = "30";
                    break;
                case "03":
                    batTrack = "40";
                    break;
                case "04":
                    batTrack = "50";
                    break;
                case "05":
                    batTrack = "75";
                    break;
                case "06":
                    batTrack = "100";
                    break;
            }

            // Sinal GSM rastreador.
            string gsmTrack = data.Substring(12, 2);
            switch (gsmTrack)
            {
                case "00":
                    gsmTrack = "0";
                    break;
                case "01":
                    gsmTrack = "25";
                    break;
                case "02":
                    gsmTrack = "50";
                    break;
                case "03":
                    gsmTrack = "75";
                    break;
                case "04":
                    gsmTrack = "100";
                    break;
            }

            //Retorna arrai com informações do pacote 13.
            string[] dados = new string[7];
            dados[0] = batTrack;
            dados[1] = gsmTrack;
            dados[2] = source;
            dados[3] = statusGps;
            dados[4] = eventLoading;
            dados[5] = eventIgnition;
            dados[6] = eventDefense;
            return dados;
        }
    }
}
