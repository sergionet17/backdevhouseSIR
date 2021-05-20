using SIR.Comun.Entidades.Abstracto;
using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Reportes
{
    [DataContract]
    [Serializable]
    public class MODReporteFiltro: MODBase
    {
        [DataMember] 
        public int Id { get; set; }
        [DataMember]
        public int IdServicio { get; set; }
        [DataMember]
        public int IdEmpresa { get; set; }
        [DataMember]
        public bool? Activo { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public int IdCategoria { get; set; }
        [DataMember]
        public bool? esReporte { get; set; }
    }
}
