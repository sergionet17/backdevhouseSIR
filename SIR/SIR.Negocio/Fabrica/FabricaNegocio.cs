using SIR.Negocio.Concretos.Archivos;
using SIR.Negocio.Concretos.Auditoria;
using SIR.Negocio.Concretos.Empresa;
using SIR.Negocio.Concretos.FlujoDeTrabajo;
using SIR.Negocio.Concretos.Parametrizacion_SIR;
using SIR.Negocio.Concretos.Perfilamiento;
using SIR.Negocio.Concretos.Reportes;
using SIR.Negocio.Interfaces.Archivos;
using SIR.Negocio.Interfaces.Auditoria;
using SIR.Negocio.Interfaces.Empresa;
using SIR.Negocio.Interfaces.FlujoDeTrabajo;
using SIR.Negocio.Interfaces.Parametrizacion_SIR;
using SIR.Negocio.Interfaces.Perfilamiento;
using SIR.Negocio.Interfaces.Reporte;
using ArchivoNegocio = SIR.Negocio.Concretos.Archivos.ArchivoNegocio;

namespace SIR.Negocio.Fabrica
{
    public class FabricaNegocio
    {
        public static IUsuarioNegocio CrearUsuarioNegocio => new UsuarioNegocio();
        public static IEmpresasNegocio CrearEmpresaNegocio => new EmpresasNegocio();
        public static IServicioNegocio CrearServicioNegocio => new ServicioNegocio();
        public static IFlujoTrabajoNegocio CrearFlujoTrabajoNegocio => new FlujoTrabajoNegocio();
        public static IReporteNegocio CrearReporteNegocio => new ReporteNegocio();
        public static IModuloNegocio CrearModuloNegocio => new ModuloNegocio();

        public static IAuditoriaNegocio CrearAuditoriaNegocio => new AuditoriaNegocio();
        public static IArchivoNegocio CrearArchivoNegocio => new Concretos.Archivos.ArchivoNegocio();

        public static ICliente_ExcluidoNegocio CrearClienteExcluido => new Cliente_ExcluidoNegocio();
    }
}
