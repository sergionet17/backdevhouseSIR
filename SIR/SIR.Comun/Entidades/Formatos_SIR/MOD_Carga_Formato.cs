using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Formatos_SIR
{
    [DataContract]
    [Serializable]
    public class MOD_Carga_Formato
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string detalle { get; set; }
    }
}
