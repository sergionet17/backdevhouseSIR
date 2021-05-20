using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Datos.Fabrica;
using SIR.Negocio.Abstractos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    class RegistrarNegocio : PasoBase
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
            reporte.campos = AdicionarCamposControl(reporte.campos);
            tarea.Value.Registros = RegistrarCamposControl(tarea.Value.Registros, _flujo, tarea.Value.Id);

            resultado = _configuraicon.ProcesarEstracion(tarea.Value.NombreTablaSIR, tarea.Value.Registros, reporte.campos);
            AisgnarRegistros(ref tarea);
            return resultado;
        }
    }
}
