using Dapper;
using SIR.Comun.Entidades.UsuarioPerfil;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SIR.Datos.Concretos.Perfilamiento
{
    public class UsuarioDatos : Dal_Base, IUsuarioDatos
    {
        public MODUsuario ObtenerUsuario(string prmUsername) 
        {
            MODUsuario usuario = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmUsuario", prmUsername, System.Data.DbType.String);

                var reader = conn.QueryMultiple("StpConsultarUsuarioAccesos", parametros, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                if (base.HasRows(reader))
                {
                    usuario = reader.Read<MODUsuario>().FirstOrDefault();

                    if (usuario != null)
                    {
                        var permisosUsuario = reader.Read<MODPermisoUsuario>();

                        if (permisosUsuario != null)
                            usuario.PermisosUsuario = permisosUsuario.ToList();

                        var permisosReporte = reader.Read<MODPermisoReporte>();

                        if (permisosReporte != null)
                            usuario.PermisosReportes = permisosReporte.ToList();
                    }

                }

                conn.Close();
            }

            return usuario;
        }

        public MODUsuario ObtenerUsuarioPorId(int prmIdUsuario)
        {
            MODUsuario usuario = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdUsuario", prmIdUsuario, System.Data.DbType.Int32);

                var reader = conn.QueryMultiple("StpConsultarUsuarioAccesosPorId", parametros, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                if (base.HasRows(reader))
                {
                    usuario = reader.Read<MODUsuario>().FirstOrDefault();

                    if (usuario != null)
                    {
                        var permisosUsuario = reader.Read<MODPermisoUsuario>();

                        if (permisosUsuario != null)
                            usuario.PermisosUsuario = permisosUsuario.ToList();

                        var permisosReporte = reader.Read<MODPermisoReporte>();

                        if (permisosReporte != null)
                            usuario.PermisosReportes = permisosReporte.ToList();
                    }

                }

                conn.Close();
            }

            return usuario;
        }

        public bool ActualizarUltimaFechaLogin(string prmUsername) 
        {
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmUsuario", prmUsername, System.Data.DbType.String);

                var scalar = conn.ExecuteScalar("StpActualizarUltimoAccesoUsuario", parametros, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                conn.Close();
            }

            return true;
        }

        public int CrearUsuario(MODUsuario prmUsuario) 
        {
            int idUsuario = 0;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmUsuario", prmUsuario.UserName, System.Data.DbType.String);
                parametros.Add("@prmNombre", prmUsuario.Nombre, System.Data.DbType.String);
                parametros.Add("@prmApellido", prmUsuario.Apellido, System.Data.DbType.String);
                parametros.Add("@prmIdUsuario", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);

                conn.QueryMultiple("StpInsertarUsuario", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);

                idUsuario = parametros.Get<int>("@prmIdUsuario");

                conn.Close();
            }

            return idUsuario;
        }

        public bool InsertarPermisosUsuario(List<MODPermisoUsuario> prmPermisos) 
        {
            using (var conn = ObtenerConexionPrincipal())
            {
                var dt = new DataTable();
                dt.Columns.Add("IdUsuario", typeof(int));
                dt.Columns.Add("IdModuloEmpresaServicio", typeof(int));
                dt.Columns.Add("TipoPermiso", typeof(int));
                dt.Columns.Add("IdReporteEmpresaServicio", typeof(int));

                prmPermisos.ForEach(x => {
                    dt.Rows.Add(x.IdUsuario, x.IdModuloEmpresaServicio, x.TipoPermiso, x.IdReporteEmpresaServicio);
                });

                var scalar = conn.ExecuteScalar("StpInsertarPermisosUsuario", new { @prmPermisosUsuario = dt }, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                conn.Close();
            }

            return true;
        }

        public List<MODUsuarioBasico> ObtenerUsuarios(MODUsuarioFiltro filtro)
        {
            List<MODUsuarioBasico> usuarios = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmActivo", filtro.Activo, System.Data.DbType.Boolean);
                parametros.Add("@prmUsuario", filtro.Usuario, System.Data.DbType.String);
                parametros.Add("@prmIdEmpresa", filtro.IdEmpresa, System.Data.DbType.Int32);
                parametros.Add("@prmNombre", filtro.Nombre, System.Data.DbType.String);
                parametros.Add("@prmApellido", filtro.Apellido, System.Data.DbType.String);
                parametros.Add("@prmDesdeRegitro", filtro.Desde, System.Data.DbType.Int32);
                parametros.Add("@prmHastaRegistro", filtro.Hasta, System.Data.DbType.Int32);
                if (filtro.IdUsuario != 0)
                    parametros.Add("@prmIdUsuario", filtro.IdUsuario, DbType.Int32);

                var reader = conn.QueryMultiple("StpConsultarUsuarios", parametros, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                if (base.HasRows(reader))
                {
                    usuarios = reader.Read<MODUsuarioBasico>().ToList();
                }

                conn.Close();
            }

            return usuarios;
        }

        public bool ActualizarUsuario(MODUsuario prmUsuario)
        {
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdUsuario", prmUsuario.IdUsuario, System.Data.DbType.String);
                parametros.Add("@prmNombre", prmUsuario.Nombre, System.Data.DbType.String);
                parametros.Add("@prmApellido", prmUsuario.Apellido, System.Data.DbType.String);
                parametros.Add("@prmActivo", prmUsuario.Activo, System.Data.DbType.Boolean);

                conn.QueryMultiple("StpActualizarUsuario", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);

                conn.Close();
            }

            return true;
        }

        public bool EliminarUsuario(int prmIdUsuario)
        {
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdUsuario", prmIdUsuario, System.Data.DbType.Int32);
                conn.QueryMultiple("StpEliminarUsuario", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);
                conn.Close();
            }
            return true;
        }
    }
}
