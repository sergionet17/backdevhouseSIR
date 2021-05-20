using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Datos.Fabrica;
using SIR.Negocio.Abstractos;
using System;
using System.Collections.Generic;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    public class CombinacionNegocio : PasoBase
    {
        private MODFlujo _flujo;

        public override void Configurar(MODFlujo flujo)
        {
            _flujo = flujo;
        }

        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            MODResultado resultado = new MODResultado();
            var _configuraicon = FabricaDatos.CrearConfiguracionOrigenDatos;
            var _flujoTrabajo = FabricaDatos.CrearFlujoTrabajoDatos;

            if (tarea.Value.Reporte != null && tarea.Value.ConfiguracionBD != null)
            {
                tarea.Value.ConfiguracionBD.Parametros = CrearParametros(tarea.Value.ConfiguracionBD, _flujo);

                tarea.Value.NombreTablaSIR = $"Temp_Combinacion_{tarea.Value.Reporte.Nombre.Replace(" ", "_")}";
                tarea.Next.Value.Registros = _configuraicon.Ejecutar(tarea.Value.ConfiguracionBD, reporte, ref resultado);

                _flujoTrabajo.GenerarTablaTemporal(tarea.Value);
                resultado = this.Registrar(tarea.Next, tarea.Value.Reporte);
            }

            if (!String.IsNullOrEmpty(tarea.Value.ConsultaFinal))
            {
                if (tarea.Value.ConfiguracionBD != null)
                    tarea.Next.Value.Registros = _flujoTrabajo.EjecutarScirpt(reporte.campos, tarea.Value.ConsultaFinal, (Dictionary<string, object>)CrearParametros(tarea.Value.ConfiguracionBD, _flujo));
                else
                {
                    var parametros = new Dictionary<string, object>();
                    var campos = new List<MODCampos>();

                    if (tarea.Value.ConsultaFinal.Contains("@periodo"))
                        parametros.Add("periodo", tarea.Value.Periodo);

                    if (reporte != null)
                        campos = reporte.campos;

                    tarea.Next.Value.Registros = _flujoTrabajo.EjecutarScirpt(campos, tarea.Value.ConsultaFinal, parametros);
                }
                    
            }

            if (tarea.Value.Reporte != null && tarea.Value.ConfiguracionBD != null)
            {
                _flujoTrabajo.EjecutarScirpt(null, $"DROP TABLE {tarea.Value.NombreTablaSIR}", null);
            }

            return resultado;
        }

        private MODResultado Registrar(LinkedListNode<MODTarea> tarea, MODReporte reporte)
        {
            MODResultado resultado = new MODResultado();
            var _configuraicon = FabricaDatos.CrearConfiguracionOrigenDatos;

            reporte.campos = AdicionarCamposControl(reporte.campos);
            tarea.Value.Registros = RegistrarCamposControl(tarea.Value.Registros, _flujo, tarea.Value.Id);
            resultado = _configuraicon.ProcesarEstracion(tarea.Previous.Value.NombreTablaSIR, tarea.Value.Registros, reporte.campos);

            return resultado;
        }
    }
}
