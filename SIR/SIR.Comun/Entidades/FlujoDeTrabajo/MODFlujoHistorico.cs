using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using System;

namespace SIR.Comun.Entidades.FlujoDeTrabajo
{
    public class MODFlujoHistorico
    {
        public int Id { get; set; }
        public int IdEmpresa { get; set; }
        public int IdServicio { get; set; }
        public int IdElemento { get; set; }
        public EnumTipo TipoFlujo { get; set; }
        public DateTime? FlujoFechaCreacion { get; set; }
        public DateTime? FlujoFechaFinalizacion { get; set; }
        public int IdFlujo { get; set; }
        public EnumPeriodicidad Periodicidad { get; set; }
        public DateTime Periodo { get; set; }
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
                        resultado = Periodo.ToString("yyyy") + trimester.ToString();
                        break;
                    case EnumPeriodicidad.semestral:
                        int semestre = (Periodo.Month - 1) / 3 + 1;
                        resultado = semestre > 2 ? "2":"1";
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
        public string StrPeriodoOracle
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
                    case EnumPeriodicidad.semestral:
                    case EnumPeriodicidad.anual:
                        resultado = Periodo.ToString("yyyyMM");
                        break;
                    default:
                        break;
                }
                return resultado;
            }
        }
        public EnumEstado EstadoFlujo { get; set; }

        public int IdTarea { get; set; }
        public DateTime? TareaFechaCreacion { get; set; }
        public DateTime? TareaFechaFinalizacion { get; set; }
        public bool TareaEsValido { get; set; }
        public string DescripcionError { get; set; }
        public EnumProceso Proceso { get; set; }

    }
}
