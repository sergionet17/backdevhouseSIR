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
    class ValidarNegocio : PasoBase
    {
        MODFlujo _flujo;
        public override void Configurar(MODFlujo flujo) { 
            this._flujo = flujo;
        }

        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            MODResultado resultado = new MODResultado();
            var _configuraicon = FabricaDatos.CrearConfiguracionOrigenDatos;
            var _flujoTrabajo = FabricaDatos.CrearFlujoTrabajoDatos;
            List<IDictionary<string,object>> error = null;
            
            if(tarea.Value.ConfiguracionBD != null){
                tarea.Value.ConfiguracionBD.Parametros = CrearParametros(tarea.Value.ConfiguracionBD, _flujo);
                 error = _configuraicon.Ejecutar(tarea.Value.ConfiguracionBD, reporte, ref resultado);
            }else if (!String.IsNullOrEmpty(tarea.Value.ConsultaFinal)){
                error = _flujoTrabajo.EjecutarScirpt(null, tarea.Value.ConsultaFinal, null);
            }
            if(error != null){
                foreach(var e in error){
                    resultado.Errores.Add(e["error"].ToString());
                }
            }
            return resultado;
        }
    }
}
