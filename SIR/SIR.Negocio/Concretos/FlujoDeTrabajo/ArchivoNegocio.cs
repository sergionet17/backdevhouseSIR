using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Datos.Fabrica;
using SIR.Negocio.Abstractos;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Archivos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    public class ArchivoNegocio : PasoBase
    {
        private MODFlujo _flujo;

        public override void Configurar(MODFlujo flujo)
        {
            _flujo = flujo;
        }

        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            IArchivoNegocio archivoNegocio = FabricaNegocio.CrearArchivoNegocio;

            MODResultado resultado = new MODResultado();
            var errores = new List<string>();
            archivo.Periodo = tarea.Value.Periodo;
            archivo.IdFlujo = tarea.Value.IdFlujo;
            archivo.IdElementoFlujo = tarea.Value.IdElemento;
            archivo.Periodicidad = _flujo.Periodicidad;
            archivo.DatoPeriodo = _flujo.DatoPeriodo;

            var lista = tarea.Value.Registros;
            string rutaDestino = "";
            if((tarea.Value.Registros != null && tarea.Value.Registros.Count > 0) || tarea.Previous.Value.Proceso == Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar)
            {
                if (archivoNegocio.CrearArchivo(archivo, lista, ref rutaDestino))
                {
                    archivoNegocio.CrearLogGeneracionArchivo(new MODLogGeneracionArchivo()
                    {
                        IdArchivo = archivo.IdArchivo,
                        FechaGeneracion = DateTime.Now,
                        RutaDestino = rutaDestino,
                        IdFlujo = archivo.IdFlujo
                    });                    
                }
                else
                {
                    //resultado = false;
                    //mensajesError.Add($"Ha ocurrido un error al intentar crear el archivo");
                    resultado.Errores.Add("Ha ocurrido un error al intentar crear el archivo");
                }
            }else{
                archivoNegocio.GenerarArchivo(archivo, ref errores);
                resultado.Errores = errores;
            }
            AisgnarRegistros(ref tarea);
            return resultado;
        }
    }
}
