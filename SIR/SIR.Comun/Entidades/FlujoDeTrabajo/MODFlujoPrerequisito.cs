using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.FlujoDeTrabajo
{
    [DataContract]
    [Serializable]
    public class MODFlujoPrerequisito
    {
        [DataMember]
        [Column(Order = 1, TypeName = "CODIGO_EXTERNO_FLUJO")]
        public Guid codigo_Externo_flujo { get; set; }

        [DataMember]
        [Column(Order = 2, TypeName = "IDFLUJOPRINCIPAL")]
        public int IdFlujoPadre { get; set; }

        [DataMember]
        [Column(Order = 3, TypeName = "IdFlujoPrerequisito")]
        public int IdFlujoPrerequisito { get; set; }
    }
}
