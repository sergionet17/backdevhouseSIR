using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.FlujoDeTrabajo
{
    [DataContract]
    [Serializable]
    public class MODTarea
    {
        [DataMember]
        [Column(Order = 1, TypeName = "ID")]
        public int Id { get; set; }
        [DataMember]
        [Column(Order = 6, TypeName = "codigo_Externo")]
        public Guid codigo_Externo { get; set; }
        [DataMember]
        [Column(Order = 4, TypeName = "IdFlujo")]
        public int IdFlujo { get; set; }
        [DataMember]
        public MODFlujo Flujo { get; set; }
        [DataMember]
        [Column(Order = 7, TypeName = "codigo_Externo_flujo")]
        public Guid codigo_Externo_flujo { get; set; }
        [DataMember]
        [Column(Order = 2, TypeName = "Proceso")]
        public EnumProceso Proceso { get; set; }
        [DataMember]
        [Column(Order = 3, TypeName = "IdPadre")]
        public int IdPadre { get; set; }
        [DataMember]
        [Column(Order = 5, TypeName = "Orden")]
        public int Orden { get; set; }

        /** Estos 3 campos se crean para cubrir la necesidad de LAC */
        [DataMember]
        [Column(Order = 8, TypeName = "Agrupador")]
        public string Agrupador { get; set; }
        [DataMember]
        [Column(Order = 9, TypeName = "ConsultarAgrupador")]
        public string ConsultarAgrupador { get; set; }
        [Column(Order = 10, TypeName = "NombreTablaSIR")]
        public string NombreTablaSIR { get; set; }
        [Column(Order = 11, TypeName = "IdCategoria")]
        public int IdCategoria{ get; set; }
        /** La tarea solo soporta un tipo de configuración - El tipo puede variar */
        [DataMember]
        public MODOrigenDatos ConfiguracionBD { get; set; }
        [DataMember]
        public List<MODHomologacion> Homologaciones { get; set; }

        [DataMember]
        [Column(Order = 12, TypeName = "IdReporte")]
        public int IdReporte { get; set; }
        [DataMember]
        public MODReporte Reporte { get; set; }

        [DataMember]
        [Column(Order = 13, TypeName = "IdArchivo")]
        public int IdArchivo { get; set; }
        [DataMember]
        [Column(Order = 14, TypeName = "IdGrupoEjecucion")]
        public int IdGrupoEjecucion { get; set; }
        public MODRelGrupoEjecucion RelGrupoEjecucion { get; set; }
        [DataMember]
        public MODArchivo Archivo { get; set; }
        [DataMember]
        public DateTime Periodo { get; set; }
        [DataMember]
        public MODResultado Respuestas { get; set; }
        [DataMember]
        public EnumPeriodicidad Periodicidad{ get; set; }
        [DataMember]
        public List<IDictionary<string, object>> Registros { get; set; }
        [Column(Order = 15, TypeName = "ConsultaFinal")]
        [DataMember]
        public string ConsultaFinal { get; set; }
        [DataMember]
        public int IdElemento { get; set; }

        public MODTarea()
        {
            Homologaciones = new List<MODHomologacion>();
        }
        public void nuevo()
        {
            this.codigo_Externo = this.codigo_Externo == Guid.Empty ? Guid.NewGuid() : this.codigo_Externo;
            if (ConfiguracionBD != null)
            {
                ConfiguracionBD.codigo_Externo_tarea = this.codigo_Externo;
            }
            Homologaciones.ForEach(x => {
                x.codigo_Externo_tarea = this.codigo_Externo;
                x.codigo_Externo = x.codigo_Externo == Guid.Empty ? Guid.NewGuid() : x.codigo_Externo;
                x.nuevo();
            });
        }
    }
}
