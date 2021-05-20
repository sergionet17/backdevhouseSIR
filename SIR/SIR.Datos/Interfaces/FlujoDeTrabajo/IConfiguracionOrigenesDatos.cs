using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using System.Collections.Generic;

namespace SIR.Datos.Interfaces.FlujoDeTrabajo
{
    public interface IConfiguracionOrigenesDatos
    {
        List<IDictionary<string, object>> Ejecutar(MODOrigenDatos origen, MODReporte reporte, ref MODResultado resultado);
        MODResultado ProcesarEstracion(string destino, List<IDictionary<string, object>> resultadoOrigenes, List<MODCampos> campos);
    }
}
