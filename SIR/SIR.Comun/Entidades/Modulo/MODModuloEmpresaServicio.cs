using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Modulo
{
    [DataContract]
    [Serializable]
    public class MODModuloEmpresaServicio : MODModulo
    {
        [DataMember]
        public int IdModuloEmpresaServicio { get; set; }
        [DataMember]
        public int IdServicio { get; set; }
        [DataMember]
        public string DescripcionServicio { get; set; }
        [DataMember]
        public int IdEmpresa { get; set; }
    }
}
