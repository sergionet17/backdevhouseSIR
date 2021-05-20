using SIR.Comun.Entidades.Abstracto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Genericas
{
    [DataContract]
    [Serializable]
    public class MODResultado : BaseRespuesta
    {
        public MODResultado() { Errores = new List<string>(); DatosAdicionales = new Dictionary<string, string>(); }
        [DataMember]
        public List<string> Errores { get; set; }
        [DataMember]
        public bool esValido { get => !Errores.Any(); }
        [DataMember]
        public Dictionary<string, string> DatosAdicionales { get; set; }
    }
}
