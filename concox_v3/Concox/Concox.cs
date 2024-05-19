using concox_v3.concox.Model;
using concox_v3.Concox.Manager;
using concox_v3.Concox.Read;
using concox_v3.Concox.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace concox_v3.concox
{
    public class Concox
    {
        Socket listener;
        IPAddress ipaddr;
        IPEndPoint ipep;
        public Concox()
        {
            // Instancia um novo Socket á listener, e define suas propriedades
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ipaddr = IPAddress.Parse(Config.ServerIp); // Converte a string IP para um IPAddress
            ipep = new IPEndPoint(ipaddr, Config.ServerPort); // Instancia o endereço de conexão.

            listener.Bind(ipep); // Associa o Socket listener ao local EndPoint ipep.
            listener.Listen(100000); // Numero de equipamentos em fila para a conexão.
        }
        public void Start_Concox()
        {
            int cont = 0; // Conta o numero de conexões
            // Faça sempre... (Sempre será executada essa rotina).
            while (true)
            {
                string data = DateTime.Now.ToString("ddMMyyyy");
                string caminho = Config.Folder + "/" + data + ".txt";
                // Verifica se o diretório principal existe.
                if (!Directory.Exists(Config.Folder))
                {
                    Directory.CreateDirectory(Config.Folder); // Cria um novo diretório.
                }
                // Verifica se o arquivo de log do dia existe.
                if (!File.Exists(caminho))
                {
                    File.CreateText(caminho); // Cria um novo arquivo de log do dia
                }
                Socket client = listener.Accept(); // Cria um novo Socket para a conexão listener recém-criada.
                if (client.Connected == true)
                {
                    Thread threadClient = new Thread(() => { ThreadForConnection(client); });
                    threadClient.Start();
                }
                cont++;
            }
        }
        public static void Disconnect_Gateway(Socket client)
        {
            client.Disconnect(true);
            client.Close();
        }
        public static void ThreadForConnection(Socket newClient)
        {
            var imei = "";
            while (newClient.Connected == true)
            {
                var read = new ConcoxRead();
                // Se o cliente estiver conectado, trate as mensagens recebidas.
                string[] data = (string[])read.GetMsg(newClient);
                int sizeData = data.Length;
                // Instancia a fila.
                Queue<string> fifoMsg = new Queue<string>();
                // Varredura das informações recebidas do rastreador.
                for (int i = 0; i < sizeData; i++)
                {
                    if (data[i] != "" && data[i] != null)
                    {
                        fifoMsg.Enqueue(data[i]);
                    }
                }
                if (fifoMsg.Count == 0)
                {
                    // Insere registro de log no arquivo criado
                    string dir = "EventosLog/" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
                    using (StreamWriter wnl = File.AppendText(dir))
                    {
                        wnl.WriteLine(" > Erro ao conectar Device!");
                    }
                    Disconnect_Gateway(newClient);
                    continue;
                }
                // Executa e trata os pacotes da fila --------------- //
                int cont = 0;
                foreach (string msg in fifoMsg)
                {
                    cont++;
                    string startBit = "";
                    string protocolNumber = "";
                    startBit = Convert.ToString(msg.Substring(0, 4)); // Separa os dois primeiros bytes da msg
                    if (startBit == "7878")
                    { // Mensagem com um Byte de protocol Number.
                        protocolNumber = Convert.ToString(msg.Substring(6, 2));
                    }
                    else if (startBit == "7979")
                    { // Mensagem com dois Byte de protocol Number.
                        protocolNumber = Convert.ToString(msg.Substring(8, 2));
                    }
                    string fT = "trmsg" + startBit + protocolNumber;
                    string fA = "respmsg" + startBit + protocolNumber;
                    if (msg.Length >= 10)
                    {
                        // Aqui se verifica o tipo do pacote.
                        switch (fT)
                        { // Treatment function
                            case "trmsg787801":
                                imei = read.Convert(msg, Concox_Versions.Trmsg787801).imei;
                                break;  
                            case "trmsg787812":
                                var pacote = read.Convert(msg, Concox_Versions.Trmsg787812);
                                // Monta o JSON com as informações recolhidas.
                                string jsonTrack12 = JsonConvert.SerializeObject(pacote);
                                imei = pacote.imei;
                                new Rabbit_MQ().Publish(jsonTrack12);// Envia json para fila.
                                break;
                            case "trmsg787813":
                                var dados = read.Convert(msg, Concox_Versions.Trmsg787813);
                                string jsonTrack13 = JsonConvert.SerializeObject(dados);
                                imei = dados.imei;
                                new Rabbit_MQ().Publish(jsonTrack13);// Envia json para fila.
                                break;
                            case "trmsg787816":
                                var alarmes = read.Convert(msg, Concox_Versions.Trmsg787816);
                                string jsonTrack16 = JsonConvert.SerializeObject(alarmes);
                                imei = alarmes.imei;
                                new Rabbit_MQ().Publish(jsonTrack16);// Envia json para fila.
                                break;
                            case "trmsg797994":
                                var digitalInput = read.Convert(msg, Concox_Versions.Trmsg797994);
                                // Monta o JSON com as informações recolhidas.
                                if (!string.IsNullOrEmpty(digitalInput.datahora))
                                {
                                    imei = digitalInput.imei;
                                    string jsonTrack94 = JsonConvert.SerializeObject(digitalInput);
                                    new Rabbit_MQ().Publish(jsonTrack94);// Envia json para fila.
                                }
                                break;
                        }
                        
                        switch (fA) // Aqui o pacote é encaminhado para tratamento.
                        { 
                            // Answer function
                            case "respmsg787801":
                                Rp787801.Send(msg, newClient);
                                break;
                            case "respmsg78788A":
                                Rp78788A.Send(msg, newClient);
                                break;
                            case "respmsg787813":
                                Rp787813.Send(msg, newClient);
                                break;
                            case "respmsg787816":
                                Rp787816.Send(msg, newClient);
                                break;
                        }
                    }
                    else
                    {
                        // Insere registro de log no arquivo criado
                        string dir = "EventosLog/" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
                        using (StreamWriter wnl = File.AppendText(dir))
                        {
                            wnl.WriteLine(" > Device (imei " + imei + " ) - Desconectado!");
                        }
                        // Se alguma mensagem do pacote estiver inconsistente, reset a connexão.
                        Disconnect_Gateway(newClient);
                    }
                }
            }
        }
    }
}