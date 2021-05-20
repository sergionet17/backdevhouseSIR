using ProjectoPruebas.Interfaces;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectoPruebas.Formatos
{
    public class T15 : FormatoSUI
    {
        public override void Ejecutar()
        {
            var reporte = contextReporte.Obtener(new MODReporteFiltro { Nombre = "T15", IdEmpresa = idEmpresa, IdServicio = idServicio });
            contextFlujo.Ejecutar(new MODFlujoFiltro
            {
                Id = 12,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = reporte.FirstOrDefault(rep => rep.EsReporte).Id,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
                Periodo = new DateTime(2020, 3, 1),
                DatoPeriodo = 0,
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 7
            });
        }
        public override void Estructuras()
        {

        }
        public override void Flujo()
        {
            base.Flujo();
        }
        public override void Generar(bool crear = false)
        {
            base.Generar(crear);
        }
        public override void Inicializar()
        {
            base.Inicializar();
        }
        public override IEnumerable<MODReporte> ObtenerEstructuras()
        {
            return base.ObtenerEstructuras();
        }
    }
}
