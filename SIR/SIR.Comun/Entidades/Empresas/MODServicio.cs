using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIR.Comun.Entidades.Abstracto;
using Newtonsoft.Json;
namespace SIR.Comun.Entidades
{
    public class MODServicio : MODBase
    {
        [JsonProperty(PropertyName = "IdServicio")]
        public int IdServicio { get; set; }
        [JsonProperty(PropertyName = "Nombre")]
        public string Nombre{ get; set; }
        [JsonProperty(PropertyName = "Descripcion")]
        public string Descripcion { get; set; }
        [JsonProperty(PropertyName = "Empresas")]
        public IEnumerable<MODEmpresa> Empresas{ get; set; }
    }
}
