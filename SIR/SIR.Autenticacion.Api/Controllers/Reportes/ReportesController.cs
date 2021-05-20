using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Reporte;
using System.Collections.Generic;

namespace SIR.Autenticacion.Api.Controllers.Reportes
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
    public class ReportesController : ControllerBase
    {
        // POST: api/Reportes/Obtener
        /// <summary>
        /// Obtiene la informacion de los reportes por medio de una lista
        /// </summary>
        /// <remarks>
        /// Metodo encargado de obtiene la informacion de los reportes por medio de una lista
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <param name="filtro">Parametros de busqueda para el listado solicitado</param>
        /// <returns>IEnumerable Reporte </returns>
        [HttpPost]
        public IEnumerable<MODReporte> Obtener(MODReporteFiltro filtro)
        {
            IReporteNegocio iReportes = FabricaNegocio.CrearReporteNegocio;
            return iReportes.Obtener(filtro);
        }

        // POST: api/Reportes/Registrar
        /// <summary>
        /// Registra la informacion de los reportes 
        /// </summary>
        /// <remarks>
        /// Metodo encargado de registra la informacion de los reportes 
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <param name="reporte">Informacion de los reportes</param>
        /// <returns>MODResultado Resultado </returns>
        [HttpPost]
        public MODResultado Registrar(MODReporte reporte)
        {
            IReporteNegocio iReportes = FabricaNegocio.CrearReporteNegocio;
            return iReportes.Registrar(reporte);
        }

        // POST: api/Reportes/Modificar
        /// <summary>
        /// Modifica la informacion de los reportes 
        /// </summary>
        /// <remarks>
        /// Metodo encargado de modificar la informacion de los reportes 
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <param name="reporte">Informacion de los reportes</param>
        /// <returns>MODResultado Resultado </returns>
        [HttpPost]
        public MODResultado Modificar(MODReporte reporte)
        {
            IReporteNegocio iReportes = FabricaNegocio.CrearReporteNegocio;
            return iReportes.Modificar(reporte);
        }

        // POST: api/Reportes/Borrar
        /// <summary>
        /// Elimina de forma permanente un reporte
        /// </summary>
        /// <remarks>
        /// Elimina de forma permanente un reporte
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <param name="filtro">Parametros de busqueda para el listado solicitado</param>
        /// <returns>IEnumerable Reporte </returns>
        [HttpPost]
        public MODResultado Borrar(MODReporteFiltro filtro)
        {
            IReporteNegocio iReportes = FabricaNegocio.CrearReporteNegocio;
            return iReportes.Borrar(filtro);
        }

        // POST: api/Reportes/CambiarEstado
        /// <summary>
        /// Cambia el estado del reporte para que puedas filtralo
        /// </summary>
        /// <remarks>
        /// Cambia el estado del reporte para que puedas filtralo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <param name="filtro">Parametros de busqueda para el listado solicitado</param>
        /// <returns>IEnumerable Reporte </returns>
        [HttpPost]
        public MODResultado CambiarEstado(MODReporteFiltro filtro)
        {
            IReporteNegocio iReportes = FabricaNegocio.CrearReporteNegocio;
            return iReportes.CambiarEstado(filtro);
        }

        // POST: api/Reportes/ObtenerLimpio
        /// <summary>
        /// Obtiene la informacion de los reportes por medio de una lista
        /// </summary>
        /// <remarks>
        /// Metodo encargado de obtiene la informacion de los reportes por medio de una lista
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <returns>IEnumerable Reporte </returns>
        [HttpPost]
        public IEnumerable<MODReporte> ObtenerLimpio()
        {
            IReporteNegocio iReportes = FabricaNegocio.CrearReporteNegocio;
            return iReportes.ObtenerReportesLimpio();
        }
    }
}
