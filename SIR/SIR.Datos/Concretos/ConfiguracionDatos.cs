using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Datos.Fabrica;
using SIR.Datos.Interfaces.FlujoDeTrabajo;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Datos.Concretos
{
    public class ConfiguracionDatos : IConfiguracionDatos
    {
        public List<MODEmpresa> ObtenerEmpresas()
        {
            return FabricaDatos.CrearEmpresaDatos.ObtenerEmpresas().ToList();
        }

        public List<MODServicio> ObtenerServicios()
        {
            return FabricaDatos.CrearServicioDatos.ObtenerServicios().ToList();
        }

        public List<MODFlujo> ObtenerFlujos()
        {
            return FabricaDatos.CrearFlujoTrabajoDatos.ObtenerFlujos();
        }
    }
}
