using concox_v3.concox.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace concox_v3.Concox.Read
{
    public class ConcoxRead
    {
        public ConcoxProtocolModel Convert(string msg, Concox_Versions concoxModel)
        {
            var model = new ConcoxProtocolModel();
            switch (concoxModel)
            { // Treatment function
                case Concox_Versions.Trmsg787801:
                    model.imei = Tr787801.Convert(msg);
                    break;
                case Concox_Versions.Trmsg787812:
                    string[] pacote = (string[])Tr787812.Convert(msg);
                    model.datahora = pacote[0];
                    model.latitude = pacote[1];
                    model.longitude = pacote[2];
                    model.velocidade = pacote[3];
                    model.direcaoang = pacote[4];
                    model.direcao = pacote[5];
                    model.ignicao = pacote[6];
                    model.qtdsatelites = pacote[7];
                    model.alerta = "";
                    model.protocol = pacote[8];
                    model.serial = pacote[9];
                    break;
                case Concox_Versions.Trmsg787813:
                    string[] dados = (string[])Tr787813.Convert(msg);
                    model.bateria = dados[0];
                    model.sinalgsm = dados[1];
                    model.alimentacao = dados[2];
                    model.statusgps = dados[3];
                    model.carregando = dados[4];
                    model.defesa = dados[6];
                    break;
                case Concox_Versions.Trmsg787816:
                    string[] alarmes = (string[])Tr787816.Convert(msg);
                    model.datahora = alarmes[0];
                    model.latitude = alarmes[1];
                    model.longitude = alarmes[2];
                    model.velocidade = alarmes[3];
                    model.direcaoang = alarmes[4];
                    model.direcao = alarmes[5];
                    model.ignicao = alarmes[6];
                    model.qtdsatelites = alarmes[7];
                    model.bateria = alarmes[8];
                    model.sinalgsm = alarmes[9];
                    model.alimentacao = alarmes[10];
                    model.statusgps = alarmes[11];
                    model.alerta = alarmes[12];
                    model.carregando = alarmes[13];
                    model.defesa = alarmes[16];
                    model.protocol = alarmes[17];
                    model.serial = alarmes[18];
                    break;
                case Concox_Versions.Trmsg797994:
                    model.protocol = "94";
                    model.digitalinput = Tr797994.Convert(msg);
                    break;
            }
            return model;
        }

        public Array GetMsg(Socket client)
        {
            byte[] msg = new byte[1536]; // Array de byte criado parareceber o pacote de informações (suporta comodamente até 20 pacotes 7878)
            int sizeMsg = 0;

            if (client.ReceiveTimeout <= 10000)
            { // Recebe o tamanho do byte recebido.
                try
                {
                    sizeMsg = client.Receive(msg);
                }
                catch
                {
                    sizeMsg = 0;
                    // Insere registro de log no arquivo criado
                    string dir = "EventosLog/" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
                    using (StreamWriter wnl = File.AppendText(dir))
                    {
                        wnl.WriteLine(" > Device Desconectado - Time overflow");
                    }
                }
            }
            else
            {
                sizeMsg = 0;
            }
            string dataMsg = "";
            Array.Resize(ref msg, sizeMsg);
            for (int i = 0; i < sizeMsg; i++)
            { // Inicia a varredura de todas as posições da array
                string x = System.Convert.ToString(msg[i]);
                string y = System.Convert.ToString(long.Parse(x), 16); // Converte as posições da array para hexadecimal
                if (y.Length == 1)
                {
                    dataMsg += "0" + y;
                }
                else
                {
                    dataMsg += y;
                }
            }

            dataMsg = dataMsg.ToUpper(); // Aqui contem o pacote recebido do rastreador.
            var pacotes = GetMessageToQueue(dataMsg);
            return pacotes;
        }

        // getMessageToQueue --------------------------------------------------- //
        // - Função destinada a tratar as mensagem vinda do rastreador e ...     //
        // retornalas em um array consistente.                                   //
        public Array GetMessageToQueue(string data)
        {
            int size = data.Length; // Tamanho da MSG
            string msg = "";
            string terminal = "";
            // Array para armazenar temporariamente as mensagens obtidas do rastreador.
            string[] pacotes = new string[25];
            int position = 0;
            for (int i = 0; i < size; i = i + 2)
            {
                string aux = data.Substring(i, 2); // Pega um byte por vez da msg.
                msg += aux; // Concatena as posições lidas em uma string para formar a msg correta.
                int sizeMsg = msg.Length; // Verifica o tamanho da msg montada
                if (sizeMsg >= 10)
                    terminal = msg.Substring(msg.Length - 4);
                if (terminal == "0D0A")
                {
                    if (msg.Substring(0, 4) == "7878" || msg.Substring(0, 4) == "7979")
                    {

                        if (position < 25)
                        {
                            pacotes[position] = msg;
                            position++;
                        }
                    }
                    msg = "";
                    terminal = "";
                }
                else
                {
                    int cont = i + 2;
                    if (cont >= size)
                    {
                        i = cont;
                        data = "";
                        msg = "";
                        terminal = "";
                    }
                }
            }
            return pacotes;
        }


    }
    public enum Concox_Versions
    {
        Trmsg787801,
        Trmsg78788A,
        Trmsg787812,
        Trmsg787813,
        Trmsg787816,
        Trmsg797994

    }
}
