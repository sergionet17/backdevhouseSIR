using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Comun.Entidades
{
    public class Plantilla
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nombre_tabla { get; set; }
        public ConfiguracionOrigen configuracionOrigen { get; set; }
    }
}
