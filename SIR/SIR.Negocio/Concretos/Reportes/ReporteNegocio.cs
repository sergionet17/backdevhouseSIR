using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Datos.Interfaces.Reporte;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Auditoria;
using SIR.Negocio.Interfaces.Reporte;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Concretos.Reportes
{
    public class ReporteNegocio : IReporteNegocio
    {
        public MODResultado Modificar(MODReporte reporte)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var data = FabricaDatos.CrearReporteDatos;
                resultado = data.Actualizar(reporte);
                ConfiguracionNegocio.RefrescarConfiguracion(EnumTipoConfiguracion.reportes);
                /*********************auditoria*****************************/
                MODReporte anterior = ConfiguracionNegocio.Reportes.Where(x => x.Id == reporte.Id).FirstOrDefault();
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar, reporte.Id.ToString(), "Reporte", anterior, reporte, reporte.Usuario, reporte.Ip);
                /**********************fin auditoria***********************/
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.Reportes - Modificar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }

            return resultado;
        }

        public MODResultado Registrar(MODReporte reporte)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var data = FabricaDatos.CrearReporteDatos;
                resultado = data.Registrar(reporte);

                ConfiguracionNegocio.RefrescarConfiguracion(EnumTipoConfiguracion.reportes);
                /*********************auditoria*****************************/
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear, reporte.Nombre, "Reporte", new MODReporte(), reporte, reporte.Usuario, reporte.Ip);
                /**********************fin auditoria***********************/
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.Reportes - Registrar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }

            return resultado;
        }

        public List<MODReporte> Obtener(MODReporteFiltro filtro)
        {
            List<MODReporte> resultado = new List<MODReporte>();
            try
            {
                IReporteDatos iReporte = FabricaDatos.CrearReporteDatos;
                resultado = iReporte.ObtenerReportes().ToList();

                if (filtro.Activo != null)
                    resultado = resultado.Where(y => y.Activo == filtro.Activo).ToList();
                if (filtro.IdServicio != 0)
                    resultado = resultado.Where(y => y.IdServicio == filtro.IdServicio).ToList();
                if (filtro.IdEmpresa != 0)
                    resultado = resultado.Where(y => y.IdEmpresa == filtro.IdEmpresa).ToList();
                if (filtro.Id != 0)
                    resultado = resultado.Where(y => y.Id == filtro.Id).ToList();
                if (!string.IsNullOrEmpty(filtro.Nombre))
                    resultado = resultado.Where(y => y.Nombre.ToUpper().Contains(filtro.Nombre.ToUpper())).ToList();
                if (filtro.IdCategoria != 0)
                    resultado = resultado.Where(y => y.IdCategoria == filtro.IdCategoria).ToList();
                if (filtro.esReporte != null)
                    resultado = resultado.Where(y => y.EsReporte == filtro.esReporte).ToList();
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.Reportes - Obtener",
                             String.Format(@"filtro:{0}", System.Text.Json.JsonSerializer.Serialize(filtro)),
                             ErrorType.Error);
            }
            return resultado;
        }

        public MODResultado Borrar(MODReporteFiltro reporte)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var data = FabricaDatos.CrearReporteDatos;
                resultado = data.Borrar(reporte);

                ConfiguracionNegocio.RefrescarConfiguracion(EnumTipoConfiguracion.reportes);
                /*********************auditoria*****************************/
                MODReporte anterior = ConfiguracionNegocio.Reportes.Where(x => x.Id == reporte.Id).FirstOrDefault();
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar, reporte.Id.ToString(), "Reporte", null, reporte, reporte.Usuario, reporte.Ip);
                /**********************fin auditoria***********************/
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.Reportes - Modificar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }

            return resultado;
        }

        public MODResultado CambiarEstado(MODReporteFiltro reporte)
        {
           
            MODResultado resultado = new MODResultado();
            try
            {
                MODReporte actual = ConfiguracionNegocio.Reportes.Where(x => x.Id == reporte.Id).FirstOrDefault();
                actual.Activo = (bool)reporte.Activo;
                var data = FabricaDatos.CrearReporteDatos;
                resultado = data.Actualizar(actual);
                
                ConfiguracionNegocio.RefrescarConfiguracion(EnumTipoConfiguracion.reportes);
                /*********************auditoria*****************************/
                MODReporte anterior = ConfiguracionNegocio.Reportes.Where(x => x.Id == reporte.Id).FirstOrDefault();
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar, reporte.Id.ToString(), "Reporte", anterior, actual, reporte.Usuario, reporte.Ip);
                /**********************fin auditoria***********************/
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.Reportes - Modificar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }

            return resultado;
        }

        public List<MODReporte> ObtenerReportesLimpio()
        {
            List<MODReporte> resultado = new List<MODReporte>();
            try
            {
                IReporteDatos iReporte = FabricaDatos.CrearReporteDatos;
                return iReporte.ObtenerReportesLimpio().Where(x => x.EsReporte).ToList();
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.Reportes - Obtener",
                             String.Format(@"filtro:{0}", String.Empty),
                             ErrorType.Error);
            }
            return resultado;
        }
    }
}
