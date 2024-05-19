using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace concox_v3.concox.Model
{
    public class ConcoxProtocolModel
    {
        
        public string alerta { get; set; }
        public string alimentacao { get; set; }
        public string bateria { get; set; }
        public string carregando { get; set; }
        public string datahora { get; set; }
        public string defesa { get; set; }
        public string direcao { get; set; }
        public string direcaoang { get; set; }
        public string ignicao { get; set; }
        public string imei { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string protocol { get; set; }
        public string qtdsatelites { get; set; }
        public string serial { get; set; }
        public string sinalgsm { get; set; }
        public string statusgps { get; set; }
        public string velocidade { get; set; }
        public string digitalinput { get; set; }
    }
}
