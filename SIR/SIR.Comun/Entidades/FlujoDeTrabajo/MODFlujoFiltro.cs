using SIR.Comun.Entidades.Abstracto;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.FlujoDeTrabajo
{
    [DataContract]
    [Serializable]
    public class MODFlujoFiltro : MODBase
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdEmpresa { get; set; }
        [DataMember]
        public int IdServicio { get; set; }
        [DataMember]
        public int IdElemento { get; set; }
        [DataMember]
        public EnumTipo TipoFlujo { get; set; }
        [DataMember]
        public EnumAccion Accion { get; set; }
        [DataMember]
        public bool? Activo { get; set; }
        [DataMember]
        public int IdCategoria{ get; set; }
        [DataMember]
        public int IdGrupoEjecucion{ get; set; }
        [DataMember]
        public int IdTarea { get; set; }
        [DataMember]
        public EnumPeriodicidad Periodicidad { get; set; }
        [DataMember]
        public DateTime Periodo { get; set; }
        [DataMember]
        public int DatoPeriodo { get; set; }
        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public string Nombre { get; set; }
        public string StrPeriodo
        {
            get
            {
                string resultado = string.Empty;
                switch (Periodicidad)
                {
                    case EnumPeriodicidad.diario:
                        resultado = Periodo.ToString("yyyyMMdd");
                        break;
                    case EnumPeriodicidad.mensual:
                        resultado = Periodo.ToString("yyyyMM");
                        break;
                    case EnumPeriodicidad.trimestral:
                        int trimester = (Periodo.Month - 1) / 3 + 1;
                        resultado = trimester.ToString();
                        break;
                    case EnumPeriodicidad.semestral:
                        int semestre = (Periodo.Month - 1) / 3 + 1;
                        resultado = semestre > 2 ? "2" : "1";
                        break;
                    case EnumPeriodicidad.anual:
                        resultado = Periodo.ToString("yyyy");
                        break;
                    default:
                        break;
                }
                return resultado;
            }
        }
    }
}
