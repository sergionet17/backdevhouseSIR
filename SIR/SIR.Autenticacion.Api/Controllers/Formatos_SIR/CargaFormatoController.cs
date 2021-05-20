using System;
using Microsoft.AspNetCore.Mvc;
using SIR.Comun.Entidades.Formatos_SIR;

namespace SIR.Autenticacion.Api.Controllers.Formatos_SIR
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CargaFormatoController : ControllerBase
    {
        [HttpPost]
        public MOD_Carga_Formato CargarFormatos(string filtro)
        {
            MOD_Carga_Formato mOD_Carga = new MOD_Carga_Formato();
            mOD_Carga.Id = "4";
            mOD_Carga.detalle = "este es el valor";
            //var context = FabricaNegocio.CrearClienteExcluido;
            return mOD_Carga;
        }

    }
}
