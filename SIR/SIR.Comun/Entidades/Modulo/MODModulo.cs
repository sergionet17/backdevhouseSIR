using SIR.Comun.Entidades.Abstracto;
using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Modulo
{
    [DataContract]
    [Serializable]
    public class MODModulo : MODBase
    {
        [DataMember]
        public int IdModulo { get; set; }
        [DataMember]
        public string DescripcionModulo { get; set; }
        [DataMember]
        public bool Activo { get; set; }
        [DataMember]
        public string Ruta { get; set; }
        [DataMember]
        public string Titulo { get; set; }
        [DataMember]
        public int? IdPadre { get; set; }
    }
}
