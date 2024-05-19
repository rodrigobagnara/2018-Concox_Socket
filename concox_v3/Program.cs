using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using concox_v3.concox;
using concox_v3.concox.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using static System.Console;

namespace concox_v3
{
    class MainClass
    {
        public static void Main()
        {
            // Título do terminal.
            //Title = "Concox - V 0.3.0 Beta";

            //CursorVisible = false; // Desabilita cursor no terminal.

            new concox.Concox().Start_Concox();
        }
    } 
}
