using System;
using SIR.Comun.Entidades.Formatos_SIR;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Negocio.Interfaces.Formatos_SIR;

namespace SIR.Negocio.Concretos.Formatos_SIR
{
    public class Carga_FormatoNegocio : ICargaFormatoNegocio
    {
        public MODResultado Cargar(MOD_Carga_Formato Formato)
        {
            MODResultado resultado = new MODResultado();

            try
            {
                var context = FabricaDatos.CargaFormato_Datos;
                context.Cargar(Formato);
            }
            catch (Exception e)
            {
                resultado.Errores.Add("ClI_EXC_ERROR_001");
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(Formato), ErrorType.Error);
            }

            return resultado;
        }
    }
}
