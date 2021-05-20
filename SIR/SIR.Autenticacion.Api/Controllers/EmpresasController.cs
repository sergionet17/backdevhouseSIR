using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Genericas;
using SIR.Negocio.Concretos;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Empresa;
using System.Collections.Generic;

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
    public class EmpresasController : ControllerBase
    {

        // POST: api/Empresas/ObtenerEmpresas
        /// <summary>
        /// Obtiene el listado de Empresas
        /// </summary>
        /// <remarks>
        /// Metodo encargado de listar las empresas
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public IEnumerable<MODEmpresa> ObtenerEmpresas()
        {
            IEmpresasNegocio negocio = FabricaNegocio.CrearEmpresaNegocio;
            return negocio.ObtenerEmpresas();
        }

        // POST: api/Empresas/ObtenerEmpresaPorId
        /// <summary>
        /// Obtiene elistado de tipos de solicitud
        /// </summary>
        /// <remarks>
        /// Metodo encargado de lis tipos de Solicitud
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODEmpresa ObtenerEmpresaPorId(int idEmpresa)
        {
            IEmpresasNegocio negocio = FabricaNegocio.CrearEmpresaNegocio;
            return negocio.ObtenerEmpresaPorId(idEmpresa);
        }

        // POST: api/Empresas/CrearEmpresa
        /// <summary>
        /// Obtiene elistado de tipos de solicitud
        /// </summary>
        /// <remarks>
        /// Metodo encargado de lis tipos de Solicitud
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado CrearEmpresa([FromBody] MODEmpresa empresa)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = empresa.IdUsuario,
                UserName = empresa.Usuario,
                IP = empresa.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                IEmpresasNegocio negocio = FabricaNegocio.CrearEmpresaNegocio;
                return negocio.CrearEmpresa(empresa);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
        }

        // POST: api/Empresas/ActualizarEmpresa
        /// <summary>
        /// Obtiene elistado de tipos de solicitud
        /// </summary>
        /// <remarks>
        /// Metodo encargado de lis tipos de Solicitud
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado ActualizarEmpresa(MODEmpresa empresa)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = empresa.IdUsuario,
                UserName = empresa.Usuario,
                IP = empresa.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                IEmpresasNegocio negocio = FabricaNegocio.CrearEmpresaNegocio;
                return negocio.ActualizarEmpresa(empresa);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
        }

        // POST: api/Empresas/BorrarEmpresa
        /// <summary>
        /// Obtiene elistado de tipos de solicitud
        /// </summary>
        /// <remarks>
        /// Metodo encargado de lis tipos de Solicitud
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado BorrarEmpresa ([FromBody] MODEmpresa empresa)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = empresa.IdUsuario,
                UserName = empresa.Usuario,
                IP = empresa.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                IEmpresasNegocio negocio = FabricaNegocio.CrearEmpresaNegocio;
                return negocio.BorrarEmpresa(empresa);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
            
        }
    }
}
