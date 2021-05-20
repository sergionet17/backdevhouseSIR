using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Parametrizacion_SIR;
using SIR.Negocio.Concretos;
using SIR.Negocio.Fabrica;
using System.Collections.Generic;

namespace SIR.Autenticacion.Api.Controllers.Parametrizacion_SIR
{    
    /// <summary>
     /// Parametrizacion para clientes excluidos que se retirar de los reportes SUI configurados con esto
     /// </summary>
     /// <remarks>
     /// Controlador encargado de ralizar las tareas de Cracion edicion y 
     /// eliminacion de los reportes    
     /// </remarks>
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Cliente_ExcluidoController : ControllerBase
    {
        // POST: api/Cliente_Excluido/ListarClientesExcluidos
        /// <summary>
        /// Obtiene el listado de los clientes excluidos
        /// </summary>
        /// <remarks>
        /// Metodo encargado de obtiene el listado de los clientes excluidos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public IEnumerable<MOD_Cliente_Excluido> ListarClientesExcluidos(MOD_Cliente_ExcluidoFiltro filtro)
        {
            var context = FabricaNegocio.CrearClienteExcluido;
            return context.Obtener(filtro);
        }

        // POST: api/Cliente_Excluido/RegistrarClientesExcluidos
        /// <summary>
        /// Registrar los datos de un cliente excluido para los reportes
        /// </summary>
        /// <remarks>
        /// Metodo encargado de registrar los datos de un cliente excluido para los reportes
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado RegistrarClientesExcluidos([FromBody] MOD_Cliente_Excluido cliente)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = cliente.IdUsuario,
                UserName = cliente.Usuario,
                IP = cliente.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                var context = FabricaNegocio.CrearClienteExcluido;
                return context.Registrar(cliente);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
        }

        // POST: api/Cliente_Excluido/RegistrarClientesExcluidos
        /// <summary>
        /// Modificar los datos de un cliente excluido para los reportes
        /// </summary>
        /// <remarks>
        /// Metodo encargado de modificar los datos de un cliente excluido para los reportes
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado ModificarClientesExcluidos([FromBody] MOD_Cliente_Excluido cliente)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = cliente.IdUsuario,
                UserName = cliente.Usuario,
                IP = cliente.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                var context = FabricaNegocio.CrearClienteExcluido;
                return context.Modificar(cliente);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
        }

        // POST: api/Cliente_Excluido/RegistrarClientesExcluidos
        /// <summary>
        /// Borrar los datos de un cliente excluido para los reportes
        /// </summary>
        /// <remarks>
        /// Metodo encargado de borrar los datos de un cliente excluido para los reportes
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado BorrarClientesExcluidos([FromBody] MOD_Cliente_Excluido cliente)
        {
            if (ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
            {
                IdUsuario = cliente.IdUsuario,
                UserName = cliente.Usuario,
                IP = cliente.Ip
            }, Comun.Enumeradores.EnumSession._peticion))
            {
                var context = FabricaNegocio.CrearClienteExcluido;
                return context.Borrar(cliente);
            }
            return new MODResultado { CodigoRespuesta = System.Net.HttpStatusCode.BadRequest, Errores = new List<string> { "LOGIN.RTA007" } };
        }
    }
}
