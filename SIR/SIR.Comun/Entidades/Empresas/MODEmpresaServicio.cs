using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Empresas
{
    [DataContract]
    [Serializable]
    public class MODEmpresaServicio
    {
        [DataMember]
        public int IdEmpresaServicio { get; set; }
        [DataMember]
        public int IdEmpresa { get; set; }
        [DataMember]
        public int IdServicio { get; set; }
        [DataMember]
        public string DescripcionServicio { get; set; }
    }
}
