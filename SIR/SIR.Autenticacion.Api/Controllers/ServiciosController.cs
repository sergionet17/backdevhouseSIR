using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Empresas;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Funcionalidades;
using SIR.Negocio.Concretos;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Empresa;
using System;
using System.Collections.Generic;
using System.Net;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
    public class ServiciosController : ControllerBase
    {
         // POST: api/Servicios/ObtenerServicios
        /// <summary>
        /// Obtiene el listado de Servicios
        /// </summary>
        /// <remarks>
        /// Metodo encargado de listar los Servicios
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public IEnumerable<MODServicio> ObtenerServicios()
        {
            IServicioNegocio negocio = FabricaNegocio.CrearServicioNegocio;
            return negocio.ObtenerServicios();
        }

        // POST: api/Servicios/ObtenerServicioPorId
        /// <summary>
        /// Obtiene un objecto servicio por id de servicio
        /// </summary>
        /// <remarks>
        /// Metodo encargado obtener un servico por el id del servicio
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODServicio ObtenerServicioPorId(int idServicio)
        {
            IServicioNegocio negocio = FabricaNegocio.CrearServicioNegocio;
            return negocio.ObtenerServicioPorId(idServicio);
        }

        // POST: api/Servicios/CrearServicio
        /// <summary>
        /// Crear un servicio
        /// </summary>
        /// <remarks>
        /// Metodo encargado de crear un servicio
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado CrearServicio([FromBody] MODServicio servicio)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = servicio.IdUsuario,
                UserName = servicio.Usuario,
                IP = servicio.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                IServicioNegocio negocio = FabricaNegocio.CrearServicioNegocio;
                return negocio.CrearServicio(servicio);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
            
        }

        // POST: api/Servicios/ActualizarServicio
        /// <summary>
        /// Actualiza la informacion registrada del servicio
        /// </summary>
        /// <remarks>
        /// Metodo encargado actualizar la informacion del servicio
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado ActualizarServicio(MODServicio servicio)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = servicio.IdUsuario,
                UserName = servicio.Usuario,
                IP = servicio.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                IServicioNegocio negocio = FabricaNegocio.CrearServicioNegocio;
                return negocio.ActualizarServicio(servicio);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
        }

        // POST: api/Servicios/BorrarServicio
        /// <summary>
        /// Borra un servicio
        /// </summary>
        /// <remarks>
        /// Metodo encargado de borrar
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado BorrarServicio ([FromBody] MODServicio servicio)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = servicio.IdUsuario,
                UserName = servicio.Usuario,
                IP = servicio.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                IServicioNegocio negocio = FabricaNegocio.CrearServicioNegocio;
                return negocio.BorrarServicio(servicio);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
            
        }

        /// <summary>
        /// Consulta los servicios asociados a una empresa
        /// </summary>
        /// <param name="prmIdEmpresa">Id de la empresa</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="500">Error interno</response>
        /// <response code="400">Error de validacion</response>
        /// <response code="401">No autorizado</response>
        [HttpPost]
        public MODRespuestaAPI<List<MODEmpresaServicio>> ObtenerServicioEmpresa(int prmIdEmpresa)
        {
            MODRespuestaAPI<List<MODEmpresaServicio>> respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";

            try
            {
                IServicioNegocio servicioNegocio = FabricaNegocio.CrearServicioNegocio;

                var servicios = servicioNegocio.ObtenerServiciosPorEmpresa(prmIdEmpresa);

                if (servicios != null && servicios.Count > 0)
                {
                    respuesta = new MODRespuestaAPI<List<MODEmpresaServicio>>(servicios);
                }
                else
                {
                    respuesta = new MODRespuestaAPI<List<MODEmpresaServicio>>(HttpStatusCode.BadRequest, "SERVICIOS.MENSAJES.MEN001");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "UsuarioController.ConsultarPermisosUsuario", String.Empty, ErrorType.Error);
                respuesta = new MODRespuestaAPI<List<MODEmpresaServicio>>(HttpStatusCode.InternalServerError, codigoMensaje);
            }
            finally
            {
                //Auditoria
            }

            return respuesta;
        }
    }
}