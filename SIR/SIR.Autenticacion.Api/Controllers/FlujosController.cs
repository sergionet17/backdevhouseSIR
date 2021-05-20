using Microsoft.AspNetCore.Mvc;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Negocio.Fabrica;
using SIR.Autenticacion.Api.Notificaciones;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Negocio.Concretos;
using SIR.Negocio.Interfaces.FlujoDeTrabajo;

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
    public class FlujosController : ControllerBase
    {
        // POST: api/Flujos/IniciarFlujo
        /// <summary>
        /// Ejecutar flujo por filtro
        /// </summary>
        /// <remarks>
        /// Metodo encargado de ejecutar flujo de tareas;
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpPost]
        public async Task<MODResultado> IniciarFlujoAsync([FromBody] MODFlujoFiltro mod)
        {
            return await Task<MODResultado>.Run(() =>
            {
                mod.Notificar = NotificacionHub.NotificarCliente;
                var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;               
                var resultado = negocio.Ejecutar(mod);
                return resultado;
            });
        }
        // POST: api/Flujos/CrearFlujo
        /// <summary>
        /// Crear flujo
        /// </summary>
        /// <remarks>
        /// Metodo encargado de crear flujo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpPost]
        public MODResultado CrearFlujo([FromBody] MODFlujo mod)
        {
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.Registrar(mod);
        }
        // POST: api/Flujos/ObtenerProcesos
        /// <summary>
        /// Obtener procesos
        /// </summary>
        /// <remarks>
        /// Metodo encargado obtener procesos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public List<MODProceso> ObtenerProcesos()
        {
            List<MODProceso> procesos = new List<MODProceso>();
            foreach (var a in Enum.GetValues<EnumProceso>())
            {
                procesos.Add(new MODProceso { Id = (int)a, Nombre = a.ToString() });
            }
            return procesos;
        }
        // POST: api/Flujos/ObtenerGrupos
        /// <summary>
        /// Obtener grupos
        /// </summary>
        /// <remarks>
        /// Metodo encargado obtener grupos de ejecucion por categoria
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public List<MODGruposDeEjecucion> ObtenerGrupos([FromBody] MODFlujoFiltro filtro)
        {
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerGrupos(filtro.IdCategoria);
        }
        // GET: api/Flujos/ObtenerEnumeradores
        /// <summary>
        /// Obtiene enumeradores
        /// </summary>
        /// <remarks>
        /// Metodo encargado de convertir enumeradores en listas
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpGet]
        public Dictionary<string, List<MODProceso>> ObtenerEnumeradores()
        {
            Dictionary<string, List<MODProceso>> procesos = new Dictionary<string, List<MODProceso>>();
            procesos.Add("operadores", new List<MODProceso>());
            foreach (var a in Enum.GetValues<EnumCondiciones>())
            {
                procesos["operadores"].Add(new MODProceso { Id = (int)a, Nombre = a.ToString() });
            }
            procesos.Add("conectores", new List<MODProceso>());
            foreach (var a in Enum.GetValues<EnumConectores>())
            {
                procesos["conectores"].Add(new MODProceso { Id = (int)a, Nombre = a.ToString() });
            }
            procesos.Add("tiporeemplazo", new List<MODProceso>());
            foreach (var a in Enum.GetValues<EnumTipoReemplazo>())
            {
                procesos["tiporeemplazo"].Add(new MODProceso { Id = (int)a, Nombre = a.ToString() });
            }
            procesos.Add("periodicidad",new List<MODProceso>());
            foreach(var a in Enum.GetValues<EnumPeriodicidad>()){
                procesos["periodicidad"].Add(new MODProceso { Id = (int)a, Nombre = a.ToString() });
            }
            return procesos;
        }
        // POST: api/Flujos/CrearCategoria
        /// <summary>
        /// crear categoria de flujos
        /// </summary>
        /// <remarks>
        /// Metodo encargado de crear categoria de flujos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado EditarCategoria(MODCategoria categoria)
        {
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            MODResultado resutado = negocio.EditarCategoria(categoria);
            return resutado;
        }
        // POST: api/Flujos/EditarCategoria
        /// <summary>
        /// editar categoria de flujos
        /// </summary>
        /// <remarks>
        /// Metodo encargado de crear categoria de flujos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado CrearCategoria(MODCategoria categoria)
        {
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            MODResultado resutado = negocio.CrearCategoria(categoria);
            return resutado;
        }
        // POST: api/Flujos/ObtenerCategorias
        /// <summary>
        /// Obtener categorias de flujos
        /// </summary>
        /// <remarks>
        /// Metodo para obtener categorias de de flujos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public List<MODCategoria> ObtenerCategorias()
        {
            List<MODCategoria> resultado = ConfiguracionNegocio.CategoriasFlujos;
            return resultado;
        }
        // POST: api/Flujos/ProbarConeccion
        /// <summary>
        /// probar coneccion de origen de datos
        /// </summary>
        /// <remarks>
        /// Metodo encargado de probar la informacion del origen de datos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado ProbarConeccion(MODOrigenDatos origen)
        {
            IFlujoTrabajoNegocio negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ProbarConeccion(origen);
        }
        // POST: api/Flujos/ObtenerPasos
        /// <summary>
        /// Obtiene grupos de ejecuion y sus tareas asignadas
        /// </summary>
        /// <remarks>
        /// Obtiene grupos de ejecucion y sus tareas asociadas
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public IEnumerable<MODGruposDeEjecucion> ObtenerPasos(MODFlujoFiltro filtro)
        {
            IFlujoTrabajoNegocio negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerPasos(filtro);
        }
        
        // POST: api/Flujos/ObtenerGruposEjecucion
        /// <summary>
        /// Obtiene los grupos de ejecucion disponibles por categoria
        /// </summary>
        /// <remarks>
        /// Obtiene los grupos de ejecucion disponibles por categoria
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public IEnumerable<MODGruposDeEjecucion> ObtenerGruposEjecucion(int prmIdCategoria)
        {
            IFlujoTrabajoNegocio negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerGruposEjecucion(prmIdCategoria);
        }
        // POST: api/Flujos/ObtenerVersiones
        /// <summary>
        /// Obtiene las versiones de un flujo
        /// </summary>
        /// <remarks>
        /// Obtiene las versiones de un flujo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public Dictionary<int,string> ObtenerVersiones(MODFlujoFiltro filtro){
            IFlujoTrabajoNegocio negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerVersiones(filtro);
        }
        // POST: api/Flujos/ObtenerOrigenes
        /// <summary>
        /// Obtiene los origenes de datos
        /// </summary>
        /// <remarks>
        /// Obtiene las origenes de datos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public List<object> ObtenerOrigenes(MODFlujoFiltro filtro){
            IFlujoTrabajoNegocio negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerOrigenes(filtro);
        }
        // POST: api/Flujos/ObtenerOrigenesPor
        /// <summary>
        /// Obtiene los origenes de datos
        /// </summary>
        /// <remarks>
        /// Obtiene las origenes de datos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public List<object> ObtenerOrigenesPor(MODFlujoFiltro filtro){
            IFlujoTrabajoNegocio negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerOrigenesPor(filtro);
        }
        [HttpPost]
        public MODResultado GuardarConf(List<Dictionary<string,object>> lista){
            IFlujoTrabajoNegocio negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.GuardarConf(lista);
        }

        // POST: api/Flujos/ObtonerFlujo
        /// <summary>
        /// Consultar flujos
        /// </summary>
        /// <remarks>
        /// Consultar flujos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpPost]
        public List<MODFlujo> ObtonerFlujo([FromBody] MODFlujoFiltro filtro)
        {
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.Obtener(filtro);
        }
        // POST: api/Flujos/Consultar
        /// <summary>
        /// Consultar
        /// </summary>
        /// <remarks>
        /// Consultar
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpPost]
        public List<IDictionary<string,object>> Consultar([FromBody] MODFlujoFiltro filtro){
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.Consultar(filtro);
        }
        // POST: api/Flujos/ObtenerOpciones
        /// <summary>
        /// Consultar
        /// </summary>
        /// <remarks>
        /// Consultar
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpPost]
        public List<MODOpciones> ObtenerOpciones(MODFlujoFiltro filtro){
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerOpciones(filtro);
        }

        // POST: api/Flujos/ObtenerAcciones
        /// <summary>
        /// Consultar
        /// </summary>
        /// <remarks>
        /// Consultar
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpGet]
        public List<MODOpciones> ObtenerAcciones(){
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerAcciones();
        }
        // POST: api/Flujos/ObtenerAccionesSeleccionadas
        /// <summary>
        /// Consultar
        /// </summary>
        /// <remarks>
        /// Consultar
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpPost]
        public List<MODOpciones> ObtenerAccionesSeleccionadas(MODFlujoFiltro filtro){
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ObtenerAccionesSeleccionadas(filtro);
        }
        // POST: api/Flujos/GuardarConfiguracion
        /// <summary>
        /// Consultar
        /// </summary>
        /// <remarks>
        /// Consultar
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>  
        [HttpPost]
        public MODResultado GuardarConfiguracion(List<Dictionary<string,object>> lista){
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.GuardarConf(lista);
        }
        // POST: api/Flujos/GuardarCorrecciones
        /// <summary>
        /// Consultar
        /// </summary>
        /// <remarks>
        /// Consultar
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado GuardarCorrecciones(List<Dictionary<string,object>> lista){
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.GuardarCorrecciones(lista);
        }
        
        // POST: api/Flujos/ConfirmarCorrecciones
        /// <summary>
        /// Consultar
        /// </summary>
        /// <remarks>
        /// Consultar
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public MODResultado ConfirmarCorrecciones(List<Dictionary<string,object>> lista){
            var negocio = FabricaNegocio.CrearFlujoTrabajoNegocio;
            return negocio.ConfirmarCorrecciones(lista);
        }

    }
}