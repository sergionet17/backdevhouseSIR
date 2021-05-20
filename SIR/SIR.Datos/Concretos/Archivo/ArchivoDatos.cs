using Dapper;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Archivos;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.Archivo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SIR.Datos.Concretos.Archivo
{
    public class ArchivoDatos : Dal_Base, IArchivoDatos
    {
        public List<MODTipoArchivo> ObtenerTipoArchivos()
        {
            List<MODTipoArchivo> tipoArchivos = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var reader = conn.QueryMultiple("StpConsultarTipoArchivos", null, null, int.MaxValue, System.Data.CommandType.StoredProcedure);
                tipoArchivos = reader.Read<MODTipoArchivo>().ToList();
                conn.Close();
            }
            return tipoArchivos;
        }

        public List<Comun.Entidades.Archivos.MODArchivo> ObtenerArchivos(MODArchivoFiltro prmFiltro)
        {
            List<Comun.Entidades.Archivos.MODArchivo> archivos = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdArchivo", prmFiltro.IdArchivo, System.Data.DbType.Int32);
                parametros.Add("@prmIdTipoArchivo", prmFiltro.IdTipoArchivo, System.Data.DbType.Int32);
                parametros.Add("@prmIdReporte", prmFiltro.IdReporte, System.Data.DbType.Int32);
                parametros.Add("@prmIdSeparador", prmFiltro.IdSeparador, System.Data.DbType.Int32);
                parametros.Add("@prmNombre", prmFiltro.Nombre, System.Data.DbType.String);
                parametros.Add("@prmDesdeRegitro", prmFiltro.Desde, System.Data.DbType.Int32);
                parametros.Add("@prmHastaRegistro", prmFiltro.Hasta, System.Data.DbType.Int32);

                var reader = conn.QueryMultiple("StpConsultarArchivos", parametros, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                if (HasRows(reader))
                {
                    archivos = reader.Read<Comun.Entidades.Archivos.MODArchivo>().ToList();
                }

                conn.Close();
            }
            return archivos;
        }

        public long CrearArchivo(Comun.Entidades.Archivos.MODArchivo prmArchivo)
        {
            long idArchivo = 0;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdReporte", prmArchivo.IdReporte, System.Data.DbType.Int32);
                parametros.Add("@prmNombre", prmArchivo.Nombre, System.Data.DbType.String);
                parametros.Add("@prmDescripcion", prmArchivo.Descripcion, System.Data.DbType.String);
                parametros.Add("@prmIdTipoArchivo", prmArchivo.IdTipoArchivo, System.Data.DbType.Int32);
                parametros.Add("@prmIdSeparador", prmArchivo.IdSeparador, System.Data.DbType.Int32);
                parametros.Add("@prmIdArchivo", null, System.Data.DbType.Int64, System.Data.ParameterDirection.Output);

                conn.QueryMultiple("StpInsertaArchivo", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);

                idArchivo = parametros.Get<long>("@prmIdArchivo");

                conn.Close();
            }
            return idArchivo;
        }

        public bool ActualizarArchivo(Comun.Entidades.Archivos.MODArchivo prmArchivo)
        {
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdArchivo", prmArchivo.IdArchivo, System.Data.DbType.Int64);
                parametros.Add("@prmNombre", prmArchivo.Nombre, System.Data.DbType.String);
                parametros.Add("@prmDescripcion", prmArchivo.Descripcion, System.Data.DbType.String);
                parametros.Add("@prmIdTipoArchivo", prmArchivo.IdTipoArchivo, System.Data.DbType.Int32);
                parametros.Add("@prmIdSeparador", prmArchivo.IdSeparador, System.Data.DbType.Int32);
                parametros.Add("@prmActivo", prmArchivo.Activo, System.Data.DbType.Boolean);
                conn.QueryMultiple("StpActualizarArchivo", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);
                conn.Close();
            }

            return true;
        }

        public bool EliminarArchivo(long prmIdArchivo)
        {
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdArchivo", prmIdArchivo, System.Data.DbType.Int64);
                conn.QueryMultiple("StpEliminarArchivo", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);
                conn.Close();
            }
            return true;
        }

        public List<MODSeparadorArchivo> ObtenerSeparadorArchivos()
        {
            List<MODSeparadorArchivo> separadorArchivos = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var reader = conn.QueryMultiple("StpConsultarSeparadorArchivos", null, null, int.MaxValue, System.Data.CommandType.StoredProcedure);
                if (HasRows(reader))
                {
                    separadorArchivos = reader.Read<MODSeparadorArchivo>().ToList();
                }
                conn.Close();
            }
            return separadorArchivos;
        }

        public long CrearLogGeneracionArchivo(MODLogGeneracionArchivo prmLogGeneracionArchivo)
        {
            long idLogArchivo = 0;
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdArchivo", prmLogGeneracionArchivo.IdArchivo, System.Data.DbType.Int32);
                parametros.Add("@prmFechaGeneracion", prmLogGeneracionArchivo.FechaGeneracion, System.Data.DbType.DateTime);
                parametros.Add("@prmRutaDestino", prmLogGeneracionArchivo.RutaDestino, System.Data.DbType.String);
                parametros.Add("@prmIdFlujo", prmLogGeneracionArchivo.IdFlujo, System.Data.DbType.Int32);
                parametros.Add("@prmIdLog", null, System.Data.DbType.Int64, System.Data.ParameterDirection.Output);
                conn.QueryMultiple("StpInsertaLogGeneracionArchivo", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);
                idLogArchivo = parametros.Get<long>("@prmIdLog");
                conn.Close();
            }
            return idLogArchivo;
        }

        public List<MODLogGeneracionArchivo> ConsultarLogGeneracionArchivos(int prmIdArchivo)
        {
            List<MODLogGeneracionArchivo> logGeneracionArchivos = null;
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdArchivo", prmIdArchivo, System.Data.DbType.Int32);
                var reader = conn.QueryMultiple("StpConsultarLogGeneracionArchivo", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);
                if (HasRows(reader))
                {
                    logGeneracionArchivos = reader.Read<MODLogGeneracionArchivo>().ToList();
                }
                conn.Close();
            }
            return logGeneracionArchivos;
        }

        public List<MODCamposArchivo> ConsultarCamposArchivo(int prmIdArchivo)
        {
            List<MODCamposArchivo> camposArchivos = null;
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdArchivo", prmIdArchivo, System.Data.DbType.Int32);
                var reader = conn.QueryMultiple("StpConsultarCamposArchivo", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);
                if (HasRows(reader))
                {
                    camposArchivos = reader.Read<MODCamposArchivo>().ToList();
                }
                conn.Close();
            }
            return camposArchivos;
        }

        public bool InsertarCamposArchivo(List<MODCamposArchivo> prmCamposArchivo)
        {
            using (var conn = ObtenerConexionPrincipal())
            {
                var dt = new DataTable();
                dt.Columns.Add("IdCampo", typeof(int));
                dt.Columns.Add("IdArchivo", typeof(int));
                dt.Columns.Add("Orden", typeof(int));

                prmCamposArchivo.ForEach(x => {
                    dt.Rows.Add(x.IdCampo, x.IdArchivo, x.Orden);
                });

                var scalar = conn.ExecuteScalar("StpInsertarCamposArchivo", new { @prmCamposArchivo = dt }, null, int.MaxValue, CommandType.StoredProcedure);
                conn.Close();
            }
            return true;
        }

        public List<IDictionary<string, object>> ObtenerInformacionArchivo(string prmSql, List<MODCampos> prmCampos) 
        {
            List<IDictionary<string, object>> result = null;
            using (var conn = ObtenerConexionPrincipal())
            {
                var reader = conn.ExecuteReader(prmSql, null, null, commandTimeout: int.MaxValue, System.Data.CommandType.Text);

                result = base.ConvertirADiccionario(ref reader, prmCampos);

                conn.Close();
            }
            return result;
        }
    }
}
