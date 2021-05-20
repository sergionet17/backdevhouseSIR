using SIR.Comun.Entidades.Modulo;
using SIR.Comun.Entidades.UsuarioPerfil;
using SIR.Datos.Fabrica;
using SIR.Datos.Interfaces.Perfilamiento;
using SIR.Negocio.Interfaces.Perfilamiento;
using System.Collections.Generic;

namespace SIR.Negocio.Concretos.Perfilamiento
{
    public class ModuloNegocio : IModuloNegocio
    {
        public List<MODModulo> ObtenerModulos()
        {
            IModuloDatos moduloDatos = FabricaDatos.CrearModuloDatos;

            return moduloDatos.ObtenerModulos();
        }

        public List<MODModuloEmpresaServicio> ObtenerModulosEmpresaServicio(int prmIdEmpresa, int? prmIdServicio)
        {
            IModuloDatos moduloDatos = FabricaDatos.CrearModuloDatos;

            return moduloDatos.ObtenerModulosEmpresaServicio(prmIdEmpresa, prmIdServicio);
        }

        public int InsertarOactualizarModuloEmpresaServicio(MODModuloEmpresaServicio prmModuloEmpresaServicio)
        {
            IModuloDatos moduloDatos = FabricaDatos.CrearModuloDatos;

            var idModuloEmpresaServicio = moduloDatos.InsertarOactualizarModuloEmpresaServicio(prmModuloEmpresaServicio);

            if (idModuloEmpresaServicio == 0)
            {
                    throw new System.Exception("Ha ocurrido un error intentando crear el moduloEmpresaServicio, no se han insertado correctamente los registros");
            }
            return idModuloEmpresaServicio;
        }

        public int InsertarOactualizarReporteEmpresaServicio(MODPermisoReporte prmReporteEmpresaServicio)
        {
            IModuloDatos moduloDatos = FabricaDatos.CrearModuloDatos;

            var idModuloEmpresaServicio = moduloDatos.InsertarOactualizarReporteEmpresaServicio(prmReporteEmpresaServicio);

            return idModuloEmpresaServicio;
        }
    }
}
