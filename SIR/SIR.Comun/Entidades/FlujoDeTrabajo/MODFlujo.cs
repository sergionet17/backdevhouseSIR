using SIR.Comun.Entidades.Abstracto;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.FlujoDeTrabajo
{
    [DataContract]
    [Serializable]
    public class MODFlujo : MODBase
    {
        [DataMember]
        [Column(Order = 1, TypeName = "ID")]
        public int Id { get; set; }
        [DataMember]
        [Column(Order = 9, TypeName = "codigo_Externo")]
        public Guid codigo_Externo { get; set; }
        [DataMember]
        [Column(Order = 2, TypeName = "Accion")]
        public EnumAccion Accion { get; set; }
        [DataMember]
        [Column(Order = 3, TypeName = "IdEmpresa")]
        public int IdEmpresa { get; set; }
        [DataMember]
        [Column(Order = 7, TypeName = "IdServicio")]
        public int IdServicio { get; set; }
        [DataMember]
        public string NombreEmpresa { get; set; }
        [DataMember]
        [Column(Order = 4, TypeName = "IdElemento")]
        public int IdElemento { get; set; }
        [DataMember]
        [Column(Order = 5, TypeName = "Elemento")]
        public string Elemento { get; set; }
        [DataMember]
        public string Nombre { get { return string.Format(@"{0}_{1}_{2}", 
            Accion.ToString(), 
            Elemento, NombreEmpresa); } 
        }
        [DataMember]
        [Column(Order = 6, TypeName = "Tipo")]
        public EnumTipo Tipo { get; set; }
        [DataMember]
        [Column(Order = 8, TypeName = "SubTipo")]
        public EnumSubTipoFlujo SubTipo { get; set; }
        public LinkedList<MODTarea> Tareas { get; set; }
        [DataMember]
        [Column(Order = 10, TypeName = "Periodicidad")]
        public EnumPeriodicidad Periodicidad { get; set; }

        public DateTime Periodo { get; set; }
        
        public int DatoPeriodo { get; set; }

        [DataMember]
        [Column(Order = 11, TypeName = "IdCategoria")]
        public int IdCategoria { get; set; }
        [DataMember]
        public string Categoria { get; set; }

        public List<MODFlujoPrerequisito> Prerequisitos { get; set; }
        public MODFlujo()
        {
            Tareas = new LinkedList<MODTarea>();
            Prerequisitos = new List<MODFlujoPrerequisito>();
        }

        public void nuevo()
        {
            this.codigo_Externo = this.codigo_Externo == Guid.Empty? Guid.NewGuid() : this.codigo_Externo;
            Tareas.ToList().ForEach(x => { 
                x.codigo_Externo_flujo = this.codigo_Externo; 
                x.nuevo();
            });
            Prerequisitos.ForEach(x => x.codigo_Externo_flujo = this.codigo_Externo);
        }
    }
}
