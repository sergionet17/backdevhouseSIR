using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Modulo;
using SIR.Comun.Entidades.UsuarioPerfil;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Auditoria;
using SIR.Negocio.Interfaces.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace SIR.Autenticacion.Api.Controllers
{
    /// <summary>
    /// Session de configuracion de reportes
    /// </summary>
    /// <remarks>
    /// Controlador encargado de ralizar las tareas de Cracion edicion y 
    /// eliminacion de los reportes    
    /// </remarks>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ModulosController : ControllerBase
    {
        private IConfiguration _config;

        public ModulosController(IConfiguration config)
        {
            this._config = config;
        }
        // POST: api/Modulos/ObtenerModulos
        /// <summary>
        /// Consulta los modulos activos
        /// </summary>
        /// <returns>Listado de modulos</returns>
        /// <response code="200">Consulta de forma exitosa</response>
        /// <response code="500">Ha ocurrido un error</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODModulo>> ObtenerModulos()
        {
            MODRespuestaAPI<List<MODModulo>> respuesta = null;

            try
            {
                IModuloNegocio moduloNegocio = FabricaNegocio.CrearModuloNegocio;

                var modulos = moduloNegocio.ObtenerModulos();

                if(modulos.Count > 0)
                    respuesta = new MODRespuestaAPI<List<MODModulo>>(moduloNegocio.ObtenerModulos());
                else
                    respuesta = new MODRespuestaAPI<List<MODModulo>>(HttpStatusCode.BadRequest, "MODULOS.RESPUESTAS.RTA001");
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ModulosController.ObtenerModulos", String.Empty, ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODModulo>>(HttpStatusCode.InternalServerError, "COMUNES.ERRORSERVICIO");
            }
            finally
            {
                //Auditoria
            }

            return respuesta;
        }
        // POST: api/Modulos/ObtenerModulosEmpresaServicio
        /// <summary>
        /// Consulta los modulos asociados a la empresa y servicio
        /// </summary>
        /// <param name="prmIdEmpresa">Id de la empresa</param>
        /// <returns></returns>
        /// <response code="200">Consulta de forma exitosa</response>
        /// <response code="500">Ha ocurrido un error</response>
        /// <response code="401">No autorizado</response>
        [Authorize]
        [HttpPost]
        public MODRespuestaAPI<List<MODModuloEmpresaServicio>> ObtenerModulosEmpresaServicio(int prmIdEmpresa, int? prmIdServicio)
        {
            MODRespuestaAPI<List<MODModuloEmpresaServicio>> respuesta = null;

            try
            {
                IModuloNegocio moduloNegocio = FabricaNegocio.CrearModuloNegocio;

                var modulos = moduloNegocio.ObtenerModulosEmpresaServicio(prmIdEmpresa, prmIdServicio);

                if (modulos.Count > 0)
                    respuesta = new MODRespuestaAPI<List<MODModuloEmpresaServicio>>(modulos);
                else
                    respuesta = new MODRespuestaAPI<List<MODModuloEmpresaServicio>>(HttpStatusCode.BadRequest, "MODULOS.RESPUESTAS.RTA001");
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ModulosController.ObtenerModulosEmpresaServicio", JsonSerializer.Serialize(new { IdEmpresa = prmIdEmpresa }), ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODModuloEmpresaServicio>>(HttpStatusCode.InternalServerError, "COMUNES.ERRORSERVICIO");
            }
            finally
            {
                //Auditoria
            }

            return respuesta;
        }
        // POST: api/Modulos/InsertarActualizarModuloEmpresaServicio
        /// <summary>
        /// Inserta o actualiza un modulo asociado a la empresa y el servicio
        /// </summary>
        /// <param name="prmModuloEmpresaServicio"></param>
        /// <returns></returns>
        /// <response code="200">Consulta de forma exitosa</response>
        /// <response code="500">Ha ocurrido un error</response>
        /// <response code="401">No autorizado</response>
        [Authorize]
        [HttpPost]
        public MODRespuestaAPI<int> InsertarActualizarModuloEmpresaServicio([FromBody] MODModuloEmpresaServicio prmModuloEmpresaServicio)
        {
            MODRespuestaAPI<int> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            IModuloNegocio moduloNegocio = FabricaNegocio.CrearModuloNegocio;
            List<MODModuloEmpresaServicio> permisosAnterior = null;
            List<MODModuloEmpresaServicio> permisosActual = null;

            try
            {
                permisosAnterior = moduloNegocio.ObtenerModulosEmpresaServicio(prmModuloEmpresaServicio.IdEmpresa, prmModuloEmpresaServicio.IdServicio);

                if (prmModuloEmpresaServicio != null)
                {
                    int idModuloEmpresaServicio = moduloNegocio.InsertarOactualizarModuloEmpresaServicio(prmModuloEmpresaServicio);

                    respuesta = new MODRespuestaAPI<int>(idModuloEmpresaServicio);

                    permisosActual = moduloNegocio.ObtenerModulosEmpresaServicio(prmModuloEmpresaServicio.IdEmpresa, prmModuloEmpresaServicio.IdServicio);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<int>(HttpStatusCode.BadRequest, "MODULOS.RESPUESTAS.RTA003");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ModuloController.InsertarOactualizarModuloEmpresaServicio", JsonSerializer.Serialize(prmModuloEmpresaServicio), ErrorType.Error);
                respuesta = new MODRespuestaAPI<int>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar, prmModuloEmpresaServicio.IdModuloEmpresaServicio.ToString(), "ModuloEmpresaServicio", permisosAnterior, permisosActual, prmModuloEmpresaServicio.Usuario.ToString(), prmModuloEmpresaServicio.Ip);
            }

            return respuesta;
        }
        // POST: api/Modulos/InsertarOactualizarReporteEmpresaServicio
        /// <summary>
        /// Inserta o actualiza un reporte asociado a la empresa y el servicio
        /// </summary>
        /// <param name="prmModuloEmpresaServicio"></param>
        /// <returns></returns>
        /// <response code="200">Consulta de forma exitosa</response>
        /// <response code="500">Ha ocurrido un error</response>
        /// <response code="401">No autorizado</response>
        [Authorize]
        [HttpPost]
        public MODRespuestaAPI<int> InsertarOactualizarReporteEmpresaServicio([FromBody] MODPermisoReporte prmReporteEmpresaServicio)
        {
            MODRespuestaAPI<int> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            IModuloNegocio moduloNegocio = FabricaNegocio.CrearModuloNegocio;

            try
            {
                int idModuloEmpresaServicio = moduloNegocio.InsertarOactualizarReporteEmpresaServicio(prmReporteEmpresaServicio);

                respuesta = new MODRespuestaAPI<int>(idModuloEmpresaServicio);
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ModuloController.InsertarOactualizarModuloEmpresaServicio", JsonSerializer.Serialize(prmReporteEmpresaServicio), ErrorType.Error);
                respuesta = new MODRespuestaAPI<int>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {

            }

            return respuesta;
        }
    }
}
