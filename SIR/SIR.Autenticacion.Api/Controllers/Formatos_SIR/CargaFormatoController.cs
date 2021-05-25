using System;
using Microsoft.AspNetCore.Mvc;
using SIR.Comun.Entidades.Formatos_SIR;
using SIR.Negocio.Fabrica;

namespace SIR.Autenticacion.Api.Controllers.Formatos_SIR
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CargaFormatoController : ControllerBase
    {
        [HttpPost]
        public MOD_Carga_Formato CargarFormatos([FromBody] MOD_Carga_Formato Formato)
        {
            MOD_Carga_Formato mOD_Carga = new MOD_Carga_Formato();
            var context = FabricaNegocio.CargarFormatoNegocio;
            context.Cargar(Formato);

            mOD_Carga.Id = Formato.Id;
            mOD_Carga.detalle = "Se cargo correctamente";
            return mOD_Carga;
        }

    }
}
