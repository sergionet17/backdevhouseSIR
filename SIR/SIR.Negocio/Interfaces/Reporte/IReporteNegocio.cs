using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using System.Collections.Generic;

namespace SIR.Negocio.Interfaces.Reporte
{
    public interface IReporteNegocio
    {
        MODResultado Registrar(MODReporte reporte);
        MODResultado Modificar(MODReporte reporte);
        List<MODReporte> Obtener(MODReporteFiltro filtro); 
        MODResultado Borrar(MODReporteFiltro reporte);
        MODResultado CambiarEstado(MODReporteFiltro reporte);
        List<MODReporte> ObtenerReportesLimpio();
    }
}
