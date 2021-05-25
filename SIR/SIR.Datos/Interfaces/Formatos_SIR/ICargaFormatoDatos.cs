using System;
using SIR.Comun.Entidades.Formatos_SIR;
using SIR.Comun.Entidades.Genericas;

namespace SIR.Datos.Interfaces.Formatos_SIR
{
    public interface ICargaFormatoDatos
    {
        MODResultado Cargar(MOD_Carga_Formato Formato);
    }
}
