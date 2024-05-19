using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace concox_v3.Concox.Read
{
    public class Tr797994
    {
        public static string Convert(string data)
        { // --------------------- 94

            string typeIn = data.Substring(11, 1);
            string valueIn = data.Substring(13, 1);
            string digitalIn = "0";

            if (typeIn == "5")
            {
                if (valueIn == "0")
                {
                    digitalIn = "1";
                }
                else if (valueIn == "1")
                {
                    digitalIn = "0";
                }
            }

            return digitalIn;
        }
    }
}
