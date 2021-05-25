using System;
using SIR.Comun.Entidades.Formatos_SIR;
using SIR.Comun.Entidades.Genericas;

namespace SIR.Negocio.Interfaces.Formatos_SIR
{
    public interface ICargaFormatoNegocio
    {
        MODResultado Cargar(MOD_Carga_Formato Formato);

    }
}
