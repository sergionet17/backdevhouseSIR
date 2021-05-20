using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Parametrizacion_SIR;
using System.Collections.Generic;

namespace SIR.Negocio.Interfaces.Parametrizacion_SIR
{
    public interface ICliente_ExcluidoNegocio
    {
        IEnumerable<MOD_Cliente_Excluido> Obtener(MOD_Cliente_ExcluidoFiltro filtro);
        MODResultado Registrar(MOD_Cliente_Excluido Cliente);
        MODResultado Modificar(MOD_Cliente_Excluido Cliente);
        MODResultado Borrar(MOD_Cliente_Excluido Cliente);
    }
}
