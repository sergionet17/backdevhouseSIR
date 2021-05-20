using Dapper;
using SIR.Comun.Entidades.Modulo;
using SIR.Comun.Entidades.UsuarioPerfil;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.Perfilamiento;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SIR.Datos.Concretos.Perfilamiento
{
    public class ModuloDatos : Dal_Base, IModuloDatos
    {
        public List<MODModulo> ObtenerModulos() 
        {
            List<MODModulo> modulos = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var reader = conn.QueryMultiple("StpConsultarModulos", null, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                modulos = reader.Read<MODModulo>().ToList();

                conn.Close();
            }

            return modulos;
        }

        public List<MODModuloEmpresaServicio> ObtenerModulosEmpresaServicio(int prmIdEmpresa, int? prmIdServicio) 
        {
            List < MODModuloEmpresaServicio> modulos = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdEmpresa", prmIdEmpresa, DbType.Int32);

                if(prmIdServicio != null)
                    parametros.Add("@prmIdServicio", prmIdServicio, System.Data.DbType.Int32);

                var reader = conn.QueryMultiple("StpConsultarModulosServiciosxEmpresa", parametros, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                modulos = reader.Read<MODModuloEmpresaServicio>().ToList();

                conn.Close();
            }

            return modulos;
        }

        public int InsertarOactualizarModuloEmpresaServicio(MODModuloEmpresaServicio prmModuloEmpresaServicio)
        {
            int IdModuloEmpresaServicio = 0;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdServicio", prmModuloEmpresaServicio.IdServicio, DbType.Int32);
                parametros.Add("@prmIdEmpresa", prmModuloEmpresaServicio.IdEmpresa, DbType.Int32);
                parametros.Add("@prmIdModulo", prmModuloEmpresaServicio.IdModulo, DbType.Int32);
                parametros.Add("@prmActivo", prmModuloEmpresaServicio.Activo, DbType.Boolean);
                parametros.Add("@prmIdModuloEmpresaServicio", null,DbType.Int32, ParameterDirection.Output);

                conn.QueryMultiple("StpActualizaInsertaModuloEmpresaServicio", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);

                IdModuloEmpresaServicio = parametros.Get<int>("@prmIdModuloEmpresaServicio");

                conn.Close();
            }
            return IdModuloEmpresaServicio;
        }

        public int InsertarOactualizarReporteEmpresaServicio(MODPermisoReporte prmReporteEmpresaServicio)
        {
            int IdModuloEmpresaServicio = 0;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdServicio", prmReporteEmpresaServicio.IdServicio, DbType.Int32);
                parametros.Add("@prmIdEmpresa", prmReporteEmpresaServicio.IdEmpresa, DbType.Int32);
                parametros.Add("@prmIdReporte", prmReporteEmpresaServicio.IdReporte, DbType.Int32);
                parametros.Add("@prmActivo", prmReporteEmpresaServicio.Activo, DbType.Boolean);
                parametros.Add("@prmIdModuloEmpresaServicio", null, DbType.Int32, ParameterDirection.Output);

                conn.QueryMultiple("StpActualizaInsertaReporteEmpresaServicio", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);

                conn.Close();
            }
            return IdModuloEmpresaServicio;
        }
    }
}
