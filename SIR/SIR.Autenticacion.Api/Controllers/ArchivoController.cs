using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using SIR.Autenticacion.Api.Models.Request;
using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Funcionalidades;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Archivos;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class ArchivoController : ControllerBase
    {
        private IConfiguration _config;

        public ArchivoController(IConfiguration config)
        {
            _config = config;
        }
        // POST: api/Archivo/ObtenerTipoArchivos
        /// <summary>
        /// Consulta los tipos de archivos
        /// </summary>
        /// <returns>Listado de tipos de archivos permitidos</returns>
        /// <response code="200">Consulta de forma exitosa</response>
        /// <response code="500">Ha ocurrido un error</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODTipoArchivo>> ObtenerTipoArchivos()
        {
            MODRespuestaAPI<List<MODTipoArchivo>> respuesta = null;
            try
            {
                IArchivoNegocio archivoNegocio = FabricaNegocio.CrearArchivoNegocio;

                var archivos = archivoNegocio.ObtenerTipoArchivos();

                if (archivos.Count > 0)
                    respuesta = new MODRespuestaAPI<List<MODTipoArchivo>>(archivos);
                else
                    respuesta = new MODRespuestaAPI<List<MODTipoArchivo>>(HttpStatusCode.BadRequest, "ARCHIVOS.RESPUESTAS.RTA001");
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivosController.ObtenerTipoArchivos", string.Empty, ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODTipoArchivo>>(HttpStatusCode.InternalServerError, "COMUNES.ERRORSERVICIO");
            }
            finally
            {
                //Auditoria
            }

            return respuesta;
        }

        // POST: api/Archivo/ObtenerArchivos
        /// <summary>
        /// Consulta el listado de archivos dados diversos parametros opcionales como IdReporte, nombre, IdTipoArchiv
        /// </summary>
        /// <remarks>
        /// Consulta el listado de archivos dados diversos parametros opcionales como IdReporte, nombre, IdTipoArchiv
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpPost]
        public MODRespuestaAPI<List<MODArchivo>> ObtenerArchivos(MODArchivoFiltro prmObtenerArchivos)
        {
            MODRespuestaAPI<List<MODArchivo>> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            try
            {
                IArchivoNegocio archivoNegocio = FabricaNegocio.CrearArchivoNegocio;
                var archivos = archivoNegocio.ObtenerArchivos(prmObtenerArchivos);
                if (archivos != null && archivos.Count > 0)
                    respuesta = new MODRespuestaAPI<List<MODArchivo>>(archivos);
                else
                {
                    codigoMensaje = "ARCHIVOS.RESPUESTAS.RTA002";
                    respuesta = new MODRespuestaAPI<List<MODArchivo>>(HttpStatusCode.BadRequest, codigoMensaje);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivosController.ObtenerArchivos", string.Empty, ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODArchivo>>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria
            }
            return respuesta;
        }
        // POST: api/Archivo/CrearArchivo
        /// <summary>
        /// Genera la estructura de un archivo
        /// </summary>
        /// <remarks>
        /// Genera la estructura de un archivo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODRespuestaAPI<long> CrearArchivo(MODArchivo prmArchivo)
        {
            MODRespuestaAPI<long> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            try
            {
                if (prmArchivo != null)
                {
                    long idArchivo = 0;
                    IArchivoNegocio usuarioNegocio = FabricaNegocio.CrearArchivoNegocio;
                    idArchivo = usuarioNegocio.CrearArchivo(prmArchivo);
                    respuesta = new MODRespuestaAPI<long>(idArchivo);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<long>(HttpStatusCode.BadRequest, "ARCHIVO.RESPUESTAS.RTA003");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivoController.CrearArchivo", JsonSerializer.Serialize(prmArchivo), ErrorType.Error);
                respuesta = new MODRespuestaAPI<long>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria.
            }
            return respuesta;
        }
        // POST: api/Archivo/ActualizarArchivo
        /// <summary>
        /// Actualiza la estructura de un archivo
        /// </summary>
        /// <remarks>
        /// Actualiza la estructura de un archivo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODRespuestaAPI<bool> ActualizarArchivo(MODArchivo prmArchivo)
        {
            MODRespuestaAPI<bool> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            try
            {
                if (prmArchivo != null)
                {
                    bool isUpdated;
                    IArchivoNegocio usuarioNegocio = FabricaNegocio.CrearArchivoNegocio;
                    isUpdated = usuarioNegocio.ActualizarArchivo(prmArchivo);
                    respuesta = new MODRespuestaAPI<bool>(isUpdated);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, "ARCHIVO.RESPUESTAS.RTA004");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivoController.ActualizarArchivo", JsonSerializer.Serialize(prmArchivo), ErrorType.Error);
                respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria.
            }
            return respuesta;
        }
        // POST: api/Archivo/EliminarArchivo
        /// <summary>
        /// Elimina la estructura de un archivo
        /// </summary>
        /// <remarks>
        /// Elimina la estructura de un archivo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODRespuestaAPI<bool> EliminarArchivo(long prmIdArchivo)
        {
            MODRespuestaAPI<bool> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            IArchivoNegocio archivoNegocio = FabricaNegocio.CrearArchivoNegocio;
            try
            {
                if (prmIdArchivo > 0)
                {
                    var validacion = archivoNegocio.EliminarArchivo(prmIdArchivo);
                    if (validacion)
                        respuesta = new MODRespuestaAPI<bool>(true);
                    else
                        respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, codigoMensaje);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, "ARCHIVO.RESPUESTAS.RTA005");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.EliminarUsuario", JsonSerializer.Serialize(prmIdArchivo), ErrorType.Error);
                respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
            
            }
            return respuesta;
        }
        // POST: api/Archivo/ObtenerSeparadorArchivos
        /// <summary>
        /// Obtener Separador de Archivos
        /// </summary>
        /// <remarks>
        /// Obtener Separador de Archivos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODSeparadorArchivo>> ObtenerSeparadorArchivos()
        {
            MODRespuestaAPI<List<MODSeparadorArchivo>> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            try
            {
                IArchivoNegocio archivoNegocio = FabricaNegocio.CrearArchivoNegocio;
                var separadorArchivos = archivoNegocio.ObtenerSeparadorArchivos();
                if (separadorArchivos != null && separadorArchivos.Count > 0)
                    respuesta = new MODRespuestaAPI<List<MODSeparadorArchivo>>(separadorArchivos);
                else
                {
                    codigoMensaje = "ARCHIVOS.RESPUESTAS.RTA006";
                    respuesta = new MODRespuestaAPI<List<MODSeparadorArchivo>>(HttpStatusCode.BadRequest, codigoMensaje);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivosController.ObtenerSeparadorArchivos", string.Empty, ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODSeparadorArchivo>>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria
            }
            return respuesta;
        }
        // POST: api/Archivo/CrearLogGeneracionArchivo
        /// <summary>
        /// Crear Log Generacion de Archivo
        /// </summary>
        /// <remarks>
        /// Crear Log Generacion de Archivo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODRespuestaAPI<long> CrearLogGeneracionArchivo(MODLogGeneracionArchivo prmLogGeneracionArchivo)
        {
            MODRespuestaAPI<long> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            try
            {
                if (prmLogGeneracionArchivo != null)
                {
                    long idLogArchivo = 0;
                    IArchivoNegocio usuarioNegocio = FabricaNegocio.CrearArchivoNegocio;
                    idLogArchivo = usuarioNegocio.CrearLogGeneracionArchivo(prmLogGeneracionArchivo);
                    respuesta = new MODRespuestaAPI<long>(idLogArchivo);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<long>(HttpStatusCode.BadRequest, "ARCHIVO.RESPUESTAS.RTA007");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivoController.CrearLogGeneracionArchivo", JsonSerializer.Serialize(prmLogGeneracionArchivo), ErrorType.Error);
                respuesta = new MODRespuestaAPI<long>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria.
            }
            return respuesta;
        }
        // POST: api/Archivo/ConsultarLogGeneracionArchivos
        /// <summary>
        /// Consultar Log Generacion de Archivo
        /// </summary>
        /// <remarks>
        /// Consultar Log Generacion de Archivo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODLogGeneracionArchivo>> ConsultarLogGeneracionArchivos(int prmIdArchivo)
        {
            MODRespuestaAPI<List<MODLogGeneracionArchivo>> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            try
            {
                if (prmIdArchivo > 0)
                {
                    IArchivoNegocio usuarioNegocio = FabricaNegocio.CrearArchivoNegocio;
                    var logGeneracionArchivos = usuarioNegocio.ConsultarLogGeneracionArchivos(prmIdArchivo);

                    if(logGeneracionArchivos != null && logGeneracionArchivos.Count > 0)
                        respuesta = new MODRespuestaAPI<List<MODLogGeneracionArchivo>>(logGeneracionArchivos);
                    else
                        respuesta = new MODRespuestaAPI<List<MODLogGeneracionArchivo>>(HttpStatusCode.BadRequest, "ARCHIVOS.RESPUESTAS.RTA003");
                }
                else
                {
                    respuesta = new MODRespuestaAPI<List<MODLogGeneracionArchivo>>(HttpStatusCode.BadRequest, "ARCHIVOS.RESPUESTAS.RTA002");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivoController.ConsultarLogGeneracionArchivos", JsonSerializer.Serialize(prmIdArchivo), ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODLogGeneracionArchivo>>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria.
            }
            return respuesta;
        }
        // POST: api/Archivo/ConsultarCamposArchivos
        /// <summary>
        /// Consultar Campos de Archivos
        /// </summary>
        /// <remarks>
        /// Consultar Campos de Archivos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODCamposArchivo>> ConsultarCamposArchivos(int prmIdArchivo)
        {
            MODRespuestaAPI<List<MODCamposArchivo>> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            try
            {
                if (prmIdArchivo > 0)
                {
                    IArchivoNegocio usuarioNegocio = FabricaNegocio.CrearArchivoNegocio;
                    var camposArchivos = usuarioNegocio.ConsultarCamposArchivos(prmIdArchivo);
                    respuesta = new MODRespuestaAPI<List<MODCamposArchivo>>(camposArchivos);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<List<MODCamposArchivo>>(HttpStatusCode.BadRequest, "ARCHIVO.RESPUESTAS.RTA009");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivoController.ConsultarCamposArchivos", JsonSerializer.Serialize(prmIdArchivo), ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODCamposArchivo>>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria.
            }
            return respuesta;
        }
        // POST: api/Archivo/CrearCamposArchivo
        /// <summary>
        /// Crear Campos de Archivos
        /// </summary>
        /// <remarks>
        /// Crear Campos de Archivos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODRespuestaAPI<bool> CrearCamposArchivo(List<MODCamposArchivo> prmCamposArchivo)
        {
            MODRespuestaAPI<bool> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            try
            {
                if (prmCamposArchivo != null)
                {
                    bool result = false;
                    IArchivoNegocio usuarioNegocio = FabricaNegocio.CrearArchivoNegocio;
                    result = usuarioNegocio.InsertarCamposArchivo(prmCamposArchivo);
                    respuesta = new MODRespuestaAPI<bool>(result);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.BadRequest, "ARCHIVO.RESPUESTAS.RTA010");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "ArchivoController.CrearCamposArchivo", JsonSerializer.Serialize(prmCamposArchivo), ErrorType.Error);
                respuesta = new MODRespuestaAPI<bool>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria.
            }
            return respuesta;
        }
        // POST: api/Archivo/DescargarArchivoLog
        /// <summary>
        /// Descargar Archivo Log
        /// </summary>
        /// <remarks>
        /// Descargar Archivo Log
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public async Task<IActionResult> DescargarArchivoLog(string prmRutaLog)
        {
            if (!System.IO.File.Exists(prmRutaLog))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(prmRutaLog, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            return File(memory, GetContentType(prmRutaLog), Path.GetFileName(prmRutaLog));
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();

            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
