using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace concox_v3.Concox.Read
{
    public class Tr787801
    {
        public static string Convert(string data)
        {  // ---------------------- 01

            // Extrai IMEI da msg de login.
            string imeiCrx = data.Substring(8, 16);

            // Insere registro de log no arquivo criado
            string dir = Config.Folder + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            using (StreamWriter wnl = File.AppendText(dir))
            {
                wnl.WriteLine($" > Device (imei: {imeiCrx} ) - Conectado!");
            }

            return imeiCrx;
        }
    }
}
