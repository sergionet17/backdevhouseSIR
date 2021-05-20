using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Datos.Fabrica;
using SIR.Negocio.Abstractos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    class ObtenerNegocio : PasoBase
    {
        private MODFlujo _flujo;
        public override void Configurar(MODFlujo flujo) { _flujo = flujo; }
        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            MODResultado resultado = new MODResultado();
            var _configuraicon = FabricaDatos.CrearConfiguracionOrigenDatos;
            var _flujoTrabajo = FabricaDatos.CrearFlujoTrabajoDatos;

            tarea.Value.ConfiguracionBD.Parametros = new Dictionary<string, object>();

            tarea.Value.ConfiguracionBD.Parametros = CrearParametros(tarea.Value.ConfiguracionBD, _flujo);
            tarea.Next.Value.Registros = _configuraicon.Ejecutar(tarea.Value.ConfiguracionBD, reporte, ref resultado);

            if (tarea.Next.Value.Registros != null)
            {
                resultado = this.Registrar(tarea.Next, tarea.Value.Reporte);
            }
            if (!String.IsNullOrEmpty(tarea.Value.ConsultaFinal))
                _flujoTrabajo.EjecutarScirpt(null, tarea.Value.ConsultaFinal, null);

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
