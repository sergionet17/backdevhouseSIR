using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Enumeradores;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Datos.Fabrica;
using SIR.Negocio.Interfaces.FlujoDeTrabajo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Abstractos
{
    public abstract class PasoBase : IPasos
    {
        #region Metodos
        public abstract void Configurar(MODFlujo flujo);
        public abstract MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo);

        public void AisgnarRegistros(ref LinkedListNode<MODTarea> tarea)
        {
            if (tarea.Next != null)
            {
                tarea.Next.Value.Registros = tarea.Value.Registros;
                tarea.Value.Registros = null;
            }
        }

        public object ConvertirObjeto(EnumTipoDato tipo, string dato)
        {
            switch (tipo)
            {
                case EnumTipoDato._int:
                    return Convert.ToInt32(dato ?? "0");
                case EnumTipoDato._string:
                    return dato.ToString();
                case EnumTipoDato._datetime:
                    return Convert.ToDateTime(dato ?? "01-01-1990");
                default:
                    return dato.ToString();
            }
        }

        public DateTime FijarPeriodoPorPeriodicidad(DateTime periodo, EnumPeriodicidad periodicidad, int datoPeriodo)
        {
            DateTime fecha = periodo;

            if (periodicidad == EnumPeriodicidad.trimestral)
            {
                switch (datoPeriodo)
                {
                    case 1:
                        fecha = new DateTime(periodo.Year, 1, 1);
                        break;
                    case 2:
                        fecha = new DateTime(periodo.Year, 4, 1);
                        break;
                    case 3:
                        fecha = new DateTime(periodo.Year, 7, 1);
                        break;
                    case 4:
                        fecha = new DateTime(periodo.Year, 10, 1);
                        break;
                    default:
                        break;
                }
            }

            return fecha;
        }

        public IDictionary<string, object> CrearParametros(MODOrigenDatos configuracion, MODFlujo flujo)
        {
            Dictionary<string, object> resultado = new Dictionary<string, object>();
            var _historico = new MODFlujoHistorico { Periodo = FijarPeriodoPorPeriodicidad(flujo.Periodo, flujo.Periodicidad, flujo.DatoPeriodo), Periodicidad = flujo.Periodicidad };

            if (configuracion != null)
            {
                switch (flujo.Periodicidad)
                {
                    case EnumPeriodicidad.diario:
                        if (configuracion == null || configuracion.TipoOrigen == Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer)
                            resultado.Add("periodo", _historico.StrPeriodo);
                        else
                            resultado.Add("periodo", _historico.StrPeriodoOracle);
                        break;
                    case EnumPeriodicidad.mensual:
                    case EnumPeriodicidad.trimestral:
                        if (configuracion.TipoOrigen == Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer && configuracion.Sid == "SIR2")
                            resultado.Add("periodo", _historico.StrPeriodo);
                        else
                            resultado.Add("periodo", _historico.StrPeriodoOracle);
                        break;
                    case EnumPeriodicidad.semestral:
                        break;
                    case EnumPeriodicidad.anual:
                        var fechaInicio = new DateTime(Convert.ToInt32(_historico.StrPeriodo), 1, 1, 0, 0, 0);
                        var fechaFinal = new DateTime(Convert.ToInt32(_historico.StrPeriodo), 12, 31, 23, 59, 59);
                        resultado.Add("fecha_ini", fechaInicio.ToString("yyyyMMdd HH:mm:ss"));
                        resultado.Add("fecha_fin", fechaFinal.ToString("yyyyMMdd HH:mm:ss"));
                        break;
                    default:
                        break;
                }
            }
            else 
            {
                resultado.Add("periodo", _historico.StrPeriodo);
            }

            return resultado;
        }

        public List<MODCampos> AdicionarCamposControl(List<MODCampos> campos)
        {
            if (!campos.Any(y => y.Nombre == "VERSION_SIR"))
                campos.Add(new MODCampos { Nombre = "VERSION_SIR", Tipo = EnumTipoDato._int });
            if (!campos.Any(y => y.Nombre == "IDFLUJO_SIR"))
                campos.Add(new MODCampos { Nombre = "IDFLUJO_SIR", Tipo = EnumTipoDato._int });
            if (!campos.Any(y => y.Nombre == "IDCAUSA_SIR"))
                campos.Add(new MODCampos { Nombre = "IDCAUSA_SIR", Tipo = EnumTipoDato._int });
            if (!campos.Any(y => y.Nombre == "DESCRIPCION_SIR"))
                campos.Add(new MODCampos { Nombre = "DESCRIPCION_SIR", Tipo = EnumTipoDato._string });
            if (!campos.Any(y => y.Nombre == "PERIODO_SIR"))
                campos.Add(new MODCampos { Nombre = "PERIODO_SIR", Tipo = EnumTipoDato._string });
            if (!campos.Any(y => y.Nombre == "CONFIRMACION_SIR"))
                campos.Add(new MODCampos { Nombre = "CONFIRMACION_SIR", Tipo = EnumTipoDato._int });
            return campos;
        }

        public List<IDictionary<string, object>> RegistrarCamposControl(List<IDictionary<string, object>> registros, MODFlujo _flujo, int idTarea)
        {
            var _flujohistorico = FabricaDatos.CrearFlujoTrabajoDatos;
            var _historico = new MODFlujoHistorico
            {
                IdEmpresa = _flujo.IdEmpresa,
                IdServicio = _flujo.IdServicio,
                IdElemento = _flujo.IdElemento,
                TipoFlujo = _flujo.Tipo,
                IdTarea = idTarea,
                FlujoFechaCreacion = DateTime.Now,
                TareaFechaCreacion = DateTime.Now,
                IdFlujo = _flujo.Id,
                Periodicidad = _flujo.Periodicidad,
                Periodo = FijarPeriodoPorPeriodicidad(_flujo.Periodo, _flujo.Periodicidad, _flujo.DatoPeriodo)
            };
            var _conteo = _flujohistorico.Historico(ref _historico, Comun.Enumeradores.EnumAccionBaseDatos.Consulta_1);

            foreach (var x in registros)
            {
                if (!x.ContainsKey("IDFLUJO_SIR"))
                    x.Add("IDFLUJO_SIR", _flujo.Id);
                if (!x.ContainsKey("VERSION_SIR"))
                    x.Add("VERSION_SIR", _conteo.DatosAdicionales["Version"]);
                if (!x.ContainsKey("IDCAUSA_SIR"))
                    x.Add("IDCAUSA_SIR", 0);
                if (!x.ContainsKey("DESCRIPCION_SIR"))
                    x.Add("DESCRIPCION_SIR", "");
                if (!x.ContainsKey("PERIODO_SIR"))
                    x.Add("PERIODO_SIR", _historico.StrPeriodo);
                if (!x.ContainsKey("CONFIRMACION_SIR"))
                    x.Add("CONFIRMACION_SIR", 0);
            };
            return registros;
        }

        #endregion
    }
}
