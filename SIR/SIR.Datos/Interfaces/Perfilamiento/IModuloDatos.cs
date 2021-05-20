using SIR.Comun.Entidades.Modulo;
using SIR.Comun.Entidades.UsuarioPerfil;
using System.Collections.Generic;

namespace SIR.Datos.Interfaces.Perfilamiento
{
    public interface IModuloDatos
    {
        List<MODModulo> ObtenerModulos();

        List<MODModuloEmpresaServicio> ObtenerModulosEmpresaServicio(int prmIdEmpresa, int? prmIdServicio);

        int InsertarOactualizarModuloEmpresaServicio(MODModuloEmpresaServicio prmModuloEmpresaServicio);

        int InsertarOactualizarReporteEmpresaServicio(MODPermisoReporte prmReporteEmpresaServicio);
    }
}
