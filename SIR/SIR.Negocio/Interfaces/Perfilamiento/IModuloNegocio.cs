using SIR.Comun.Entidades.Modulo;
using SIR.Comun.Entidades.UsuarioPerfil;
using System.Collections.Generic;

namespace SIR.Negocio.Interfaces.Perfilamiento
{
    public interface IModuloNegocio
    {
        List<MODModulo> ObtenerModulos();

        List<MODModuloEmpresaServicio> ObtenerModulosEmpresaServicio(int prmIdEmpresa, int? prmIdServicio);

        int InsertarOactualizarModuloEmpresaServicio(MODModuloEmpresaServicio prmModuloEmpresaServicio);

        int InsertarOactualizarReporteEmpresaServicio(MODPermisoReporte prmReporteEmpresaServicio);
    }
}
