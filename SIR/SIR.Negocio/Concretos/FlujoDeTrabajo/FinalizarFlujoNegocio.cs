using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Negocio.Abstractos;
using System;
using System.Collections.Generic;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    class FinalizarFlujoNegocio : PasoBase
    {
        public override void Configurar(MODFlujo flujo) { }
        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            return new MODResultado();
        }
    }
}
