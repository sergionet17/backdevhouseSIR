using SIR.Datos.Concretos.Auditoria;
using SIR.Datos.Concretos.Empresa;
using SIR.Datos.Concretos.FlujoDeTrabajo;
using SIR.Datos.Concretos.Perfilamiento;
using SIR.Datos.Concretos.Reporte;
using SIR.Datos.Interfaces.Empresa;
using SIR.Datos.Interfaces.FlujoDeTrabajo;
using SIR.Datos.Interfaces.Perfilamiento;
using SIR.Datos.Interfaces.Reporte;
using SIR.Datos.Interfaces.Auditoria;
using SIR.Datos.Concretos.Archivo;
using SIR.Datos.Interfaces.Archivo;
using SIR.Datos.Interfaces.Parametrizacion_SIR;
using SIR.Datos.Concretos.Parametrizacion_SIR;
using SIR.Datos.Interfaces.Formatos_SIR;
using SIR.Datos.Concretos.Formato_SIR;

namespace SIR.Datos.Fabrica
{
    public class FabricaDatos
    {
        public static IEmpresaDatos CrearEmpresaDatos => new EmpresaDatos();
        public static IUsuarioDatos CrearUsuarioDatos => new UsuarioDatos();
        public static IServicioDatos CrearServicioDatos => new ServicioDatos();
        public static IFlujoTrabajoDatos CrearFlujoTrabajoDatos => new FlujoTrabajoDatos();
        public static IReporteDatos CrearReporteDatos => new ReporteDatos();
        public static IModuloDatos CrearModuloDatos => new ModuloDatos();
        public static IConfiguracionOrigenesDatos CrearConfiguracionOrigenDatos => new ConfiguracionOrigenDatos();

        public static IAuditoriaDatos CrearAuditoriaDatos => new AuditoriaDatos();
        public static IArchivoDatos CrearArchivoDatos => new ArchivoDatos();
        public static ICliente_ExcluidoDatos CrearCliente_Excluido => new Cliente_ExcluidoDatos();
        public static ICargaFormatoDatos CargaFormato_Datos => new FormatoDatos();
    }
}
