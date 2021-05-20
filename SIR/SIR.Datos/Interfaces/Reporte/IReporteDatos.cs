using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using System.Collections.Generic;

namespace SIR.Datos.Interfaces.Reporte
{
    public interface IReporteDatos
    {
        MODResultado Registrar(MODReporte reporte);
        MODResultado Actualizar(MODReporte reporte);
        List<MODReporte> ObtenerReportes();
        MODResultado Borrar(MODReporteFiltro reporte);
        List<MODReporte> ObtenerReportesLimpio();
    }
}
