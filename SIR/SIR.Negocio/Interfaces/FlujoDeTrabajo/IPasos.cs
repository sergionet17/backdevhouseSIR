using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using System.Collections.Generic;

namespace SIR.Negocio.Interfaces.FlujoDeTrabajo
{
    public interface IPasos
    {
        void Configurar(MODFlujo flujo);
        MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo);
    }
}
