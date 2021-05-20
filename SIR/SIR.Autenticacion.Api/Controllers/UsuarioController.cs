using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIR.Autenticacion.Api.Models.Request;
using SIR.Comun.Entidades.Genericas;
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
using System.Threading.Tasks;

namespace SIR.Autenticacion.Api.Controllers
{
    /// <summary>
    /// Session de configuracion de reportes
    /// </summary>
    /// <remarks>
    /// Controlador encargado de ralizar las tareas de Cracion edicion y 
    /// eliminacion de los reportes    
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsuarioController : ControllerBase
    {
        // POST: api/Usuario/ValidarUsuarioServicioExterno
        /// <summary>
        /// Valida si un usuario existe en el servicio externo
        /// </summary>
        /// <param name="prmUsuario">Usuario</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Error interno</response>
        /// <response code="400">Error de validacion</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public async Task<MODRespuestaAPI<bool>> ValidarUsuarioServicioExterno(string prmUsuario)
        {
            MODRespuestaAPI<bool> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            bool _validarExterna = Convert.ToBoolean(Configuraciones.ObtenerConfiguracion("Contexto", "AutenticacionSCP"));
            try
            {
                if (String.IsNullOrEmpty(prmUsuario))
                    respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, "USUARIO.VALIDADORES.VAL001");
                else 
                {
                    string codigoAplicacion = Configuraciones.ObtenerConfiguracion("Keys", "CodigoAplicacion");
                    bool existeUsuario = false;

                    IUsuarioNegocio usuarioNegocio = FabricaNegocio.CrearUsuarioNegocio;

                    var usuarioBD = usuarioNegocio.ObtenerUsuario(prmUsuario);

                    if (usuarioBD != null)
                    {
                        codigoMensaje = "USUARIO.RESPUESTAS.RTA002";
                        respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, codigoMensaje);
                    }
                    else
                    {
                        var usuarioExterno = await usuarioNegocio.ValidarUsuarioServicioExterno(prmUsuario, String.Empty, true);
                        bool usuarioOk = usuarioExterno.Item1;
                        codigoMensaje = usuarioExterno.Item2;

                        if (usuarioOk)
                        {
                            existeUsuario = true;
                            codigoMensaje = "USUARIO.RESPUESTAS.RTA000";
                            respuesta = new MODRespuestaAPI<bool>(existeUsuario, codigoMensaje);
                        }
                        else
                        {
                            codigoMensaje = "USUARIO.RESPUESTAS.RTA001";
                            respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, codigoMensaje);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.ValidarUsuarioServicioExterno", JsonSerializer.Serialize(new { Usuario = prmUsuario }), ErrorType.Error);
                respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria
            }

            return respuesta;
        }
        // POST: api/Usuario/CrearUsuario
        /// <summary>
        /// Crea un usuario y asigna los permisos
        /// </summary>
        /// <param name="prmUsuario">Usuario y permisos</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Error interno</response>
        /// <response code="400">Error de validacion</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<int> CrearUsuario([FromBody] CrearUsuarioRequestModel prmUsuario)
        {
            MODRespuestaAPI<int> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            int idUsuario = 0;

            try
            {
                if (prmUsuario.Usuario != null && prmUsuario.Permisos != null && prmUsuario.Permisos.Count > 0)
                {
                    IUsuarioNegocio usuarioNegocio = FabricaNegocio.CrearUsuarioNegocio;

                    idUsuario = usuarioNegocio.CrearUsuario(prmUsuario.Usuario, prmUsuario.Permisos);

                    respuesta = new MODRespuestaAPI<int>(idUsuario);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<int>(HttpStatusCode.BadRequest, "USUARIO.RESPUESTAS.RTA003");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.CrearUsuario", JsonSerializer.Serialize(prmUsuario), ErrorType.Error);
                respuesta = new MODRespuestaAPI<int>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear, idUsuario.ToString(), "Usuario", new MODUsuario(), prmUsuario.Usuario, prmUsuario.NombreUsuario.ToString(), prmUsuario.Ip);
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear, idUsuario.ToString(), "PermisoUsuario", new List<MODPermisoUsuario>(), prmUsuario.Permisos, prmUsuario.NombreUsuario.ToString(), prmUsuario.Ip);
            }


            return respuesta;
        }
        // POST: api/Usuario/ObtenerUsuarios
        /// <summary>
        /// Obtiene el listado de usuarios
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Error interno</response>
        /// <response code="400">Error de validacion</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODUsuarioBasico>> ObtenerUsuarios([FromBody] MODUsuarioFiltro filtro)
        {
            MODRespuestaAPI<List<MODUsuarioBasico>> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";

            try
            {
                IUsuarioNegocio usuarioNegocio = FabricaNegocio.CrearUsuarioNegocio;

                var usuarios = usuarioNegocio.ObtenerUsuarios(filtro);

                if (usuarios != null && usuarios.Count > 0)
                    respuesta = new MODRespuestaAPI<List<MODUsuarioBasico>>(usuarios);
                else
                {
                    codigoMensaje = "USUARIO.RESPUESTAS.RTA004";
                    respuesta = new MODRespuestaAPI<List<MODUsuarioBasico>>(HttpStatusCode.BadRequest, codigoMensaje);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.CrearUsuario", String.Empty, ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODUsuarioBasico>>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria
            }

            return respuesta;
        }
        // POST: api/Usuario/ObtenerPermisosUsuario
        /// <summary>
        /// Obtiene los permisos de un usuario
        /// </summary>
        /// <param name="prmUsuario">Usuario</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Error interno</response>
        /// <response code="400">Error de validacion</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODPermisoUsuario>> ObtenerPermisosUsuario(string prmUsuario)
        {
            MODRespuestaAPI<List<MODPermisoUsuario>> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";

            try
            {
                IUsuarioNegocio usuarioNegocio = FabricaNegocio.CrearUsuarioNegocio;

                var usuarioBD = usuarioNegocio.ObtenerUsuario(prmUsuario);

                if (usuarioBD != null)
                {
                    respuesta = new MODRespuestaAPI<List<MODPermisoUsuario>>(usuarioBD.PermisosUsuario);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<List<MODPermisoUsuario>>(HttpStatusCode.BadRequest, codigoMensaje);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.ConsultarPermisosUsuario", String.Empty, ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODPermisoUsuario>>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria
            }

            return respuesta;
        }
        // POST: api/Usuario/ObtenerPermisosReporte
        /// <summary>
        /// Obtiene los permisos de un usuario por reporte
        /// </summary>
        /// <param name="prmUsuario">Usuario</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Error interno</response>
        /// <response code="400">Error de validacion</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODPermisoReporte>> ObtenerPermisosReporte(string prmUsuario)
        {
            MODRespuestaAPI<List<MODPermisoReporte>> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";

            try
            {
                IUsuarioNegocio usuarioNegocio = FabricaNegocio.CrearUsuarioNegocio;

                var usuarioBD = usuarioNegocio.ObtenerUsuario(prmUsuario);

                if (usuarioBD != null)
                {
                    respuesta = new MODRespuestaAPI<List<MODPermisoReporte>>(usuarioBD.PermisosReportes);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<List<MODPermisoReporte>>(HttpStatusCode.BadRequest, codigoMensaje);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.ObtenerPermisosReporte", String.Empty, ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODPermisoReporte>>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria
            }

            return respuesta;
        }
        // POST: api/Usuario/ActualizarUsuario
        /// <summary>
        /// Metodo encargado de actualizar un usuario y sus respectivos permisos
        /// </summary>
        /// <param name="prmUsuario">Usuario</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Error interno</response>
        /// <response code="400">Error de validacion</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<bool> ActualizarUsuario([FromBody] CrearUsuarioRequestModel prmUsuario)
        {
            MODRespuestaAPI<bool> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            IUsuarioNegocio usuarioNegocio = FabricaNegocio.CrearUsuarioNegocio;

            try
            {
                if (prmUsuario.Usuario != null)
                {
                    var validacion = usuarioNegocio.ActualizarUsuario(prmUsuario.Usuario, prmUsuario.Permisos);

                    if(validacion)
                        respuesta = new MODRespuestaAPI<bool>(true);
                    else
                        respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, codigoMensaje);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, "USUARIO.RESPUESTAS.RTA003");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.ActualizarUsuario", JsonSerializer.Serialize(prmUsuario), ErrorType.Error);
                respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                MODUsuario anteriorUsuario = usuarioNegocio.ObtenerUsuario(prmUsuario.Usuario.UserName);
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar, prmUsuario.Usuario.IdUsuario.ToString(), "Usuario", anteriorUsuario, prmUsuario.Usuario, prmUsuario.NombreUsuario.ToString(), prmUsuario.Ip);
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear, prmUsuario.Usuario.IdUsuario.ToString(), "PermisoUsuario", anteriorUsuario.PermisosUsuario, prmUsuario.Permisos, prmUsuario.NombreUsuario.ToString(), prmUsuario.Ip);
            }

            return respuesta;
        }
        // POST: api/Usuario/EliminarUsuario
        /// <summary>
        /// Metodo encargado de eliminar un usuario y sus respectivos permisos
        /// </summary>
        /// <param name="prmUsuario">Usuario</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Error interno</response>
        /// <response code="400">Error de validacion</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<bool> EliminarUsuario(int prmIdUsuario, string ip)
        {
            MODRespuestaAPI<bool> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            IUsuarioNegocio usuarioNegocio = FabricaNegocio.CrearUsuarioNegocio;
            MODUsuario anteriorUsuario = null;

            try
            {
                anteriorUsuario = usuarioNegocio.ObtenerUsuarioPorId(prmIdUsuario);

                if (prmIdUsuario > 0)
                {
                    
                    var validacion = usuarioNegocio.EliminarUsuario(prmIdUsuario);
                    if (validacion)
                        respuesta = new MODRespuestaAPI<bool>(true);
                    else
                        respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, codigoMensaje);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, "USUARIO.RESPUESTAS.RTA004");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.EliminarUsuario", JsonSerializer.Serialize(prmIdUsuario), ErrorType.Error);
                respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.borrar, prmIdUsuario.ToString(), "Usuario", anteriorUsuario, new MODUsuario(), anteriorUsuario.UserName, ip);
            }

            return respuesta;
        }

    }
}
