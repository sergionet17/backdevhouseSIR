using System;
using System.Runtime.Serialization;
using SIR.Comun.Entidades.Abstracto;

namespace SIR.Comun.Entidades.Auditoria{
    [Serializable]
    [DataContract]
    public class MODFiltroAuditoria : MODBase{
        [DataMember]
        public DateTime? FechaInicio { get; set; }
        [DataMember]
        public DateTime? FechaFin { get; set; }
    }
}