using Microsoft.AspNetCore.Mvc;
using SIR.Comun.Entidades.Auditoria;
using SIR.Comun.Funcionalidades;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Auditoria;
using System;
using System.Collections.Generic;

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
    public class AuditoriaController : ControllerBase
    {
        // POST: api/Auditoria/ObtenerRastroAuditoriaPor
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
        public List<MODRastroAuditoria> ObtenerRastroAuditoriaPor([FromBody] MODFiltroAuditoria filtro)
        {
            List<MODRastroAuditoria> rslt = new List<MODRastroAuditoria>();
            try
            {
                IAuditoriaNegocio negocio = FabricaNegocio.CrearAuditoriaNegocio;
                rslt = negocio.ObtenerRastroAuditoriaPor(filtro);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName, Newtonsoft.Json.JsonConvert.SerializeObject(filtro), ErrorType.Error);
            }
            return rslt;
        }
    }
}