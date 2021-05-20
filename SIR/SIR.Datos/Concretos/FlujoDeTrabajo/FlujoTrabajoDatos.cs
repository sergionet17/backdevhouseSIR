using Dapper;
using Newtonsoft.Json;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Enumeradores;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.FlujoDeTrabajo;
using SIRBackend.SIR.SIR.Comun.Entidades.FlujoDeTrabajo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SIR.Datos.Concretos.FlujoDeTrabajo
{
    public class FlujoTrabajoDatos : Dal_Base, IFlujoTrabajoDatos
    {
        public List<MODFlujo> ObtenerFlujos()
        {
            List<MODFlujo> resultado = new List<MODFlujo>();

            using (var conn = ObtenerConexionPrincipal())
            {
                var reader = conn.QueryMultiple("StpObtenerFlujos",
                    null,
                    null,
                    int.MaxValue,
                    System.Data.CommandType.StoredProcedure);

                resultado = reader.Read<MODFlujo>().ToList();

                if (resultado != null)
                {
                    var _tareas = reader.Read<MODTarea, MODRelGrupoEjecucion, MODTarea>((t, r) => { t.RelGrupoEjecucion = r; return t; }, splitOn: "IdRelGrupoEjecucion").ToList();

                    if (_tareas != null)
                    {
                        _tareas.ForEach(x =>
                        {
                            if (!String.IsNullOrEmpty(x.ConsultaFinal))
                            {
                                x.ConsultaFinal = Criptografia.Desincriptar(x.ConsultaFinal);
                            }
                        });

                        var configBD = reader.Read<MODOrigenDatos>().ToList();
                        if (configBD != null)
                        {
                            configBD.ForEach(x =>
                            {
                                x.UsuarioBD = Criptografia.Desincriptar(x.UsuarioBD);
                                x.ClaveBD = Criptografia.Desincriptar(x.ClaveBD);
                                x.Servidor = Criptografia.Desincriptar(x.Servidor);
                                x.Sid = Criptografia.Desincriptar(x.Sid);
                                x.consulta = Criptografia.Desincriptar(x.consulta);
                            });
                            _tareas.ForEach(y => y.ConfiguracionBD = configBD.FirstOrDefault(x => x.IdTarea == y.Id && new EnumProceso[] { EnumProceso.Obtener, EnumProceso.Combinacion }.Contains(y.Proceso)));
                        }


                        var homologacionBD = reader.Read<MODHomologacion>().ToList();
                        if (homologacionBD != null)
                        {
                            var condiconesHomoligacion = reader.Read<MODCondiciones>();
                            homologacionBD.ForEach(y => y.Condiciones = condiconesHomoligacion.Where(x => x.IdHomologacion == y.Id).ToList());
                            _tareas.ForEach(y => y.Homologaciones = homologacionBD.Where(x => x.IdTarea == y.Id && new EnumProceso[] { EnumProceso.Homologar }.Contains(y.Proceso)).ToList());
                        }

                        var reportesBD = reader.Read<MODReporte>().ToList();
                        if (reportesBD != null)
                        {
                            var camposBD = reader.Read<MODCampos>();
                            reportesBD.ForEach(y => y.campos = camposBD.Where(x => x.IdReporte == y.Id).ToList());
                        }

                        var archivosBD = reader.Read<MODArchivo>().ToList();

                        if (archivosBD != null)
                        {
                            var camposArchivoBD = reader.Read<MODCamposArchivo>().ToList();
                            archivosBD.ForEach(x => x.Campos = camposArchivoBD.Where(y => y.IdArchivo == x.IdArchivo).ToList());
                        }

                        _tareas.ForEach(x =>
                        {
                            x.Reporte = reportesBD.FirstOrDefault(y => x.Id == y.IdTarea);
                            x.IdReporte = x.Reporte?.Id ?? 0;
                            x.Archivo = archivosBD.FirstOrDefault(y => x.Id == y.IdTarea);
                        });

                        resultado.ForEach(x => x.Tareas = new LinkedList<MODTarea>(_tareas.Where(y => y.IdFlujo == x.Id).ToList()));
                    }

                }

                conn.Close();
            }

            return resultado;
        }
        public MODResultado Historico(ref MODFlujoHistorico registro, EnumAccionBaseDatos enumAccion)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                using (var coneccion = ObtenerConexionPrincipal())
                {
                    var _sp = coneccion.Query("StpFlujoHistorico", new
                    {
                        accion = (int)enumAccion,
                        IdHistorico = registro.Id,
                        IdEmpresa = registro.IdEmpresa,
                        IdServicio = registro.IdServicio,
                        IdElemento = registro.IdElemento,
                        TipoFlujo = (int)registro.TipoFlujo,
                        IdTarea = registro.IdTarea,
                        FechaCreacion = registro.TareaFechaCreacion,
                        FechaFinalizacion = registro.TareaFechaFinalizacion,
                        EsValido = registro.TareaEsValido,
                        DescripcionError = registro.DescripcionError,
                        IdFlujo = registro.IdFlujo,
                        Periodicidad = registro.Periodicidad,
                        Periodo = registro.Periodo,
                        EstadoFlujo = (int)registro.EstadoFlujo,
                        StrPeriodo = registro.StrPeriodo,
                        TareaProceso = registro.Proceso.ToString()
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    registro.Id = (int)_sp.IdHistorico;
                    resultado.DatosAdicionales.Add("Version", Convert.ToString(_sp.IdHistorico));

                    if (_sp.IdHistorico == 0)
                    {
                        Log.WriteLog(new Exception(_sp.Error), this.GetType().Namespace,
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(registro)),
                             ErrorType.Error);
                        resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().Namespace,
                    Newtonsoft.Json.JsonConvert.SerializeObject(registro),
                    ErrorType.Error);
            }
            return resultado;
        }
        public MODResultado CrearCategoria(MODCategoria categoria)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                var a = coneccion.Query("StpFlujos", new
                {
                    accion = 3,
                    Id = categoria.Id,
                    Nombre = categoria.Nombre
                }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(categoria), ErrorType.Error);
            }
            return resultado;
        }
        public MODResultado EditarCategoria(MODCategoria categoria)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                var a = coneccion.Query("StpFlujos", new
                {
                    accion = 4,
                    Id = categoria.Id,
                    Nombre = categoria.Nombre
                }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                if (a == null)
                {
                    resultado.Errores.Add("FLUJOS.ERRORES.CREAR");
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().FullName, Newtonsoft.Json.JsonConvert.SerializeObject(categoria), ErrorType.Error);
            }
            return resultado;
        }
        public List<MODCategoria> ObtenerCategorias()
        {
            List<MODCategoria> resultado = new List<MODCategoria>();
            try
            {
                var con = ObtenerConexionPrincipal();
                resultado = con.Query<MODCategoria>("StpFlujos", new { accion = 1 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName, "Consulta", ErrorType.Error);
            }
            return resultado;
        }
        public MODResultado Registrar(MODFlujo registro)
        {
            MODResultado resultado = new MODResultado();
            using (var conn = (SqlConnection)ObtenerConexionPrincipal())
            {
                var _reporte = conn.Query("StpFlujoTrabajo", new
                {
                    accion = 1
                }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                if (_reporte.IdFlujo == 0)
                {
                    Log.WriteLog(new Exception(_reporte.Error), "SIR.Datos.Concretos.FlujoDeTrabajo - Registrar",
                         String.Format(@"registro:{0}", System.Text.Json.JsonSerializer.Serialize(registro)),
                         ErrorType.Error);
                    resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                }
                else
                {
                    List<MODCampos> _tablaTareas = ObtenerCampos<MODFlujo>();
                    var tbFlujos = new List<MODFlujo>();
                    tbFlujos.Add(registro);
                    var _tabla = ConvertirATabla(tbFlujos);
                    InsertarBloque(conn, "##TempFlujos", _tablaTareas, _tabla);

                    _tablaTareas = ObtenerCampos<MODFlujoPrerequisito>();
                    _tabla = ConvertirATabla(registro.Prerequisitos);
                    InsertarBloque(conn, "##TempFlujosPrerequisito", _tablaTareas, _tabla);

                    _tablaTareas = ObtenerCampos<MODTarea>();
                    var tbTareas = tbFlujos.SelectMany(y => y.Tareas).ToList();

                    tbTareas.ForEach(x =>
                     {
                         if (!String.IsNullOrEmpty(x.ConsultaFinal))
                         {
                             x.ConsultaFinal = Criptografia.Incriptar(x.ConsultaFinal);
                         }
                     });

                    GenerarTablasFlujo(tbTareas, conn);
                    GenerarTablasAudFlujo(tbTareas, conn);
                    _tabla = ConvertirATabla(tbTareas);
                    InsertarBloque(conn, "##TempTareas", _tablaTareas, _tabla);

                    _tablaTareas = ObtenerCampos<MODHomologacion>();
                    var tbHomologaciones = tbTareas.SelectMany(x => x.Homologaciones).ToList();
                    _tabla = ConvertirATabla(tbHomologaciones);
                    InsertarBloque(conn, "##TempHomologaciones", _tablaTareas, _tabla);

                    _tablaTareas = ObtenerCampos<MODCondiciones>();
                    _tabla = ConvertirATabla(tbHomologaciones.SelectMany(y => y.Condiciones).ToList());
                    InsertarBloque(conn, "##TempCondiciones", _tablaTareas, _tabla);

                    _tablaTareas = ObtenerCampos<MODOrigenDatos>();
                    var tbOrigenDatos = new List<MODOrigenDatos>();
                    foreach (var _ODtarea in tbTareas)
                    {
                        if (_ODtarea.ConfiguracionBD != null && !string.IsNullOrEmpty(_ODtarea.ConfiguracionBD.Nombre))
                        {
                            _ODtarea.ConfiguracionBD.UsuarioBD = Criptografia.Incriptar(_ODtarea.ConfiguracionBD.UsuarioBD);
                            _ODtarea.ConfiguracionBD.ClaveBD = Criptografia.Incriptar(_ODtarea.ConfiguracionBD.ClaveBD);
                            _ODtarea.ConfiguracionBD.Servidor = Criptografia.Incriptar(_ODtarea.ConfiguracionBD.Servidor);
                            _ODtarea.ConfiguracionBD.Sid = Criptografia.Incriptar(_ODtarea.ConfiguracionBD.Sid);
                            _ODtarea.ConfiguracionBD.consulta = Criptografia.Incriptar(_ODtarea.ConfiguracionBD.consulta);
                            tbOrigenDatos.Add(_ODtarea.ConfiguracionBD);
                        }
                    }
                    _tabla = ConvertirATabla(tbOrigenDatos);
                    InsertarBloque(conn, "##TempOrigenDeDatos", _tablaTareas, _tabla);



                    _reporte = conn.Query("StpFlujoTrabajo", new
                    {
                        accion = 2
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (_reporte.IdFlujo == 0)
                    {
                        Log.WriteLog(new Exception(_reporte.Error), "SIR.Datos.Concretos.FlujoDeTrabajo - Registrar",
                             String.Format(@"registro:{0}", System.Text.Json.JsonSerializer.Serialize(registro)),
                             ErrorType.Error);
                        resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }

                    conn.Close();
                }

            }
            return resultado;
        }
        public MODResultado ProbarConeccion(MODOrigenDatos origen)
        {
            EnumBaseDatosOrigen dbtype = (EnumBaseDatosOrigen)origen.TipoOrigen;
            string coneccion = "";
            SqlConnectionStringBuilder builder = null;
            Object parametros = null;
            switch (dbtype)
            {
                case EnumBaseDatosOrigen.Oracle:
                    coneccion = Configuraciones.ObtenerConfiguracion("Conexiones", "ORA");
                    builder = new SqlConnectionStringBuilder(coneccion);
                    builder.DataSource = string.Format("{0}{1}{2}", origen.Servidor, (string.IsNullOrEmpty(origen.Puerto) && origen.Puerto != "0" ? ":" + origen.Puerto.ToString() : ""), (!string.IsNullOrEmpty(origen.Sid) ? "/" + origen.Sid : ""));
                    builder.UserID = origen.UsuarioBD;
                    builder.Password = origen.ClaveBD;
                    break;
                case EnumBaseDatosOrigen.SqlServer:
                    coneccion = Configuraciones.ObtenerConfiguracion("Conexiones", EnumBaseDatos.SIR.ToString());
                    builder = new SqlConnectionStringBuilder(coneccion);
                    builder.DataSource = string.Format("{0}{1}", origen.Servidor, (string.IsNullOrEmpty(origen.Puerto) && origen.Puerto != "0" ? ":" + origen.Puerto.ToString() : ""));
                    builder.InitialCatalog = origen.Sid;
                    builder.UserID = origen.UsuarioBD;
                    builder.Password = origen.ClaveBD;
                    break;
            }
            MODResultado resultado = new MODResultado();
            try
            {
                IDbConnection con = this.ObtenerConexion(builder.ConnectionString, dbtype);
                if (!string.IsNullOrEmpty(origen.consulta))
                {
                    var consulta = (CommandType.Text == (CommandType)origen.tipoMando) ? origen.consulta.ToLower().Replace("where", "where rownum = 1 and") : origen.consulta;
                    consulta = consulta.Replace("@periodo", "'" + DateTime.Now.ToString("yyyyMMdd") + "'");
                    var reader = con.ExecuteReader(consulta, parametros, commandType: (CommandType)origen.tipoMando);
                    var schema = reader.GetSchemaTable();
                    reader.Close();
                    List<MODCampos> campos = new List<MODCampos>();
                    foreach (DataRow row in schema.Rows)
                    {
                        campos.Add(new MODCampos
                        {
                            Nombre = row["ColumnName"].ToString(),
                            Largo = row["ColumnSize"].ToString(),
                            Ordinal = Convert.ToInt32(row["ColumnOrdinal"])
                        });
                    }
                    resultado.DatosAdicionales.Add("campos", JsonConvert.SerializeObject(campos));
                }
            }
            catch (Exception exp)
            {
                resultado.Errores.Add(exp.Message);
                Log.WriteLog(exp, this.GetType().FullName + "-" + nameof(ProbarConeccion),
                             String.Format(@"origen:{0}", System.Text.Json.JsonSerializer.Serialize(origen)),
                             ErrorType.Error);
            }
            return resultado;
        }
        public List<IDictionary<string, object>> Consultar(List<MODCampos> campos, string tabla, Dictionary<string, object> parametros)
        {
            List<IDictionary<string, object>> result = null;
            try
            {
                using (var coneccion = this.ObtenerConexionPrincipal())
                {
                    string consulta = string.Format("SELECT * FROM {0} WHERE VERSION_SIR = @version AND PERIODO_SIR = @periodoSIR", tabla);
                    var lector = coneccion.ExecuteReader(consulta,
                                            ObtenerParametros(parametros),
                                            commandType: System.Data.CommandType.Text);
                    if (!campos.Any(x => x.Nombre.ToUpper().Trim().Equals("ID_TABLA")))
                    {
                        campos.Add(new MODCampos { Nombre = "CONFIRMACION_SIR", Tipo = EnumTipoDato._int });
                        campos.Add(new MODCampos { Nombre = "DESCRIPCION_SIR", Tipo = EnumTipoDato._string });
                        campos.Add(new MODCampos { Nombre = "IDCAUSA_SIR", Tipo = EnumTipoDato._int });
                        campos.Add(new MODCampos { Nombre = "ID_TABLA", Tipo = EnumTipoDato._int });
                    }
                    result = ConvertirADiccionario(ref lector, campos);
                    coneccion.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.ConfiguracionOrigenDatos - Ejecutar",
                             String.Format(@"campos:{0} ", System.Text.Json.JsonSerializer.Serialize(campos)),
                             ErrorType.Error);
            }
            return result;
        }
        private DynamicParameters ObtenerParametros(Dictionary<string, object> origen)
        {
            DynamicParameters parametros = new DynamicParameters();
            if (origen != null)
            {
                foreach (var a in origen)
                {
                    parametros.Add(a.Key, a.Value);
                }
            }
            else
            {
                parametros = null;
            }
            return parametros;
        }

        private void GenerarTablasFlujo(List<MODTarea> tareas, SqlConnection conn)
        {
            StringBuilder sql = null;
            foreach (var tarea in tareas)
            {
                if (!string.IsNullOrEmpty(tarea.NombreTablaSIR))
                {
                    sql = new StringBuilder();
                    sql.AppendLine("BEGIN TRAN");
                    sql.AppendLine("BEGIN TRY");
                    sql.AppendFormat("IF NOT EXISTS(SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{0}')", tarea.NombreTablaSIR).AppendLine();
                    sql.AppendLine("BEGIN");
                    sql.AppendFormat("CREATE TABLE {0}", tarea.NombreTablaSIR).AppendLine();
                    sql.AppendLine("( ID_TABLA INT NOT NULL IDENTITY(1,1) PRIMARY KEY,");
                    sql.AppendLine("VERSION_SIR INT NOT NULL,");
                    sql.AppendLine("IDFLUJO_SIR INT NOT NULL,");
                    sql.AppendLine("PERIODO_SIR NVARCHAR(50) NOT NULL,");
                    sql.AppendLine("IDCAUSA_SIR INT NOT NULL,");
                    sql.AppendLine("DESCRIPCION_SIR NVARCHAR(MAX) NULL,");
                    sql.AppendLine("DESCRIPCION_CONF_SIR NVARCHAR(MAX) NULL,");
                    sql.AppendLine("CONFIRMACION_SIR INT NULL DEFAULT(0)");
                    sql.AppendLine(")");
                    sql.AppendLine("END");

                    foreach (var campo in tarea.Reporte.campos)
                    {
                        sql.AppendFormat("IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = '{0}' AND TABLE_NAME = '{1}')",
                            campo.Nombre,
                            tarea.NombreTablaSIR).AppendLine();
                        sql.AppendLine("BEGIN");
                        sql.AppendFormat("ALTER TABLE {0} ADD {1} {2} {3} NULL",
                            tarea.NombreTablaSIR,
                            campo.Nombre,
                            ObtenerTipoOLargo(campo),
                             ObtenerTipoOLargo(campo, false)).AppendLine();
                        sql.AppendLine("END");
                    }
                    sql.AppendLine("COMMIT TRAN");
                    sql.AppendLine("END TRY");
                    sql.AppendLine("BEGIN CATCH");
                    sql.AppendLine("ROLLBACK TRAN");
                    sql.AppendLine("END CATCH");
                    using (SqlCommand comando = new SqlCommand(sql.ToString(), conn))
                    {
                        comando.ExecuteNonQuery();
                    }
                }
            }
        }
        private void GenerarTablasAudFlujo(List<MODTarea> tareas, SqlConnection conn)
        {
            StringBuilder sql = null;
            foreach (var tarea in tareas)
            {
                if (!string.IsNullOrEmpty(tarea.NombreTablaSIR))
                {
                    sql = new StringBuilder();
                    sql.AppendLine("BEGIN TRAN");
                    sql.AppendLine("BEGIN TRY");
                    sql.AppendFormat("IF NOT EXISTS(SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{0}_AUD')", tarea.NombreTablaSIR).AppendLine();
                    sql.AppendLine("BEGIN");
                    sql.AppendFormat("CREATE TABLE {0}_AUD", tarea.NombreTablaSIR).AppendLine();
                    sql.AppendLine("(ID_TABLA INT NOT NULL IDENTITY(1,1) PRIMARY KEY,");
                    sql.AppendLine("ID_ORIGEN INT NOT NULL,");
                    sql.AppendLine("MARCA VARCHAR(2) NOT NULL,");
                    sql.AppendLine("FECHA DATETIME NOT NULL,");
                    sql.AppendLine(")");
                    sql.AppendLine("END");

                    foreach (var campo in tarea.Reporte.campos)
                    {
                        sql.AppendFormat("IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = '{0}' AND TABLE_NAME = '{1}_AUD')",
                            campo.Nombre,
                            tarea.NombreTablaSIR).AppendLine();
                        sql.AppendLine("BEGIN");
                        sql.AppendFormat("ALTER TABLE {0}_AUD ADD {1} {2} {3} NULL",
                            tarea.NombreTablaSIR,
                            campo.Nombre,
                            ObtenerTipoOLargo(campo),
                             ObtenerTipoOLargo(campo, false)).AppendLine();
                        sql.AppendLine("END");
                    }
                    sql.AppendLine("COMMIT TRAN");
                    sql.AppendLine("END TRY");
                    sql.AppendLine("BEGIN CATCH");
                    sql.AppendLine("ROLLBACK TRAN");
                    sql.AppendLine("END CATCH");
                    using (SqlCommand comando = new SqlCommand(sql.ToString(), conn))
                    {
                        comando.ExecuteNonQuery();
                    }
                }
            }
        }

        public void GenerarTablaTemporal(MODTarea tarea)
        {
            using (var conn = (SqlConnection)ObtenerConexionPrincipal())
            {
                StringBuilder sql = null;

                if (!string.IsNullOrEmpty(tarea.NombreTablaSIR))
                {
                    sql = new StringBuilder();
                    sql.AppendLine("BEGIN TRAN");
                    sql.AppendLine("BEGIN TRY");
                    sql.AppendFormat("IF NOT EXISTS(SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{0}')", tarea.NombreTablaSIR).AppendLine();
                    sql.AppendLine("BEGIN");
                    sql.AppendFormat("CREATE TABLE {0}", tarea.NombreTablaSIR).AppendLine();
                    sql.AppendLine("( ID_TABLA_COMBINACION INT NOT NULL IDENTITY(1,1) PRIMARY KEY,");
                    sql.AppendLine("VERSION_SIR INT NOT NULL,");
                    sql.AppendLine("IDFLUJO_SIR INT NOT NULL,");
                    sql.AppendLine("PERIODO_SIR NVARCHAR(50) NOT NULL,");
                    sql.AppendLine("IDCAUSA_SIR INT NOT NULL,");
                    sql.AppendLine("DESCRIPCION_SIR NVARCHAR(MAX) NULL,");
                    sql.AppendLine("CONFIRMACION_SIR INT NULL DEFAULT(0)");
                    sql.AppendLine(")");
                    sql.AppendLine("END");

                    foreach (var campo in tarea.Reporte.campos)
                    {
                        sql.AppendFormat("IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = '{0}' AND TABLE_NAME = '{1}')",
                            campo.Nombre,
                            tarea.NombreTablaSIR).AppendLine();
                        sql.AppendLine("BEGIN");
                        sql.AppendFormat("ALTER TABLE {0} ADD {1} {2} {3} NULL",
                            tarea.NombreTablaSIR,
                            campo.Nombre.ToUpper(),
                            ObtenerTipoOLargo(campo),
                             ObtenerTipoOLargo(campo, false)).AppendLine();
                        sql.AppendLine("END");
                    }
                    sql.AppendLine("COMMIT TRAN");
                    sql.AppendLine("END TRY");
                    sql.AppendLine("BEGIN CATCH");
                    sql.AppendLine("ROLLBACK TRAN");
                    sql.AppendLine("END CATCH");
                    using (SqlCommand comando = new SqlCommand(sql.ToString(), conn))
                    {
                        comando.ExecuteNonQuery();
                    }
                }
            }
        }

        private string ObtenerTipoOLargo(MODCampos _campo, bool esTipo = true)
        {
            string resultado = string.Empty;
            switch (_campo.Tipo)
            {
                case EnumTipoDato._int:
                    resultado = esTipo ? "NUMERIC" : "(18, 0)";
                    break;
                case EnumTipoDato._string:
                    if (string.IsNullOrEmpty(_campo.Largo) && !esTipo) resultado = "(120)";
                    else resultado = esTipo ? "NVARCHAR" : string.Format("({0})", _campo.Largo);
                    break;
                case EnumTipoDato._datetime:
                    resultado = esTipo ? "DATETIME" : string.Empty;
                    break;
                case EnumTipoDato._bool:
                    resultado = esTipo ? "BIT" : string.Empty;
                    break;
                case EnumTipoDato._decimal:
                    resultado = esTipo ? "DECIMAL" : "(18, 8)";
                    break;
                default:
                    break;
            }
            return resultado;
        }

        public IEnumerable<MODGruposDeEjecucion> ObtenerPasos(MODFlujoFiltro filtro)
        {
            IEnumerable<MODGruposDeEjecucion> resultado;
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                Dictionary<int, MODGruposDeEjecucion> pasos = new Dictionary<int, MODGruposDeEjecucion>();
                coneccion.Query<MODGruposDeEjecucion, MODTarea, MODFlujo, MODGruposDeEjecucion>("StpFlujos", (g, t, f) =>
                {
                    MODGruposDeEjecucion grupo;
                    if (!pasos.TryGetValue(g.Id, out grupo))
                    {
                        grupo = g;
                        grupo.Tareas = new List<MODTarea>();
                        pasos.Add(g.Id, grupo);
                    }
                    if (t != null)
                    {
                        t.Flujo = f;
                        grupo.Tareas.Add(t);
                    }
                    return grupo;
                },
                    new
                    {
                        accion = 2,
                        IdEmpresa = filtro.IdEmpresa,
                        IdServicio = filtro.IdServicio,
                        IdReporte = filtro.IdElemento
                    }, commandType: System.Data.CommandType.StoredProcedure);
                resultado = pasos.Values.ToList();
            }
            catch (Exception e)
            {
                //Implementar Log
                resultado = null;
                Log.WriteLog(e, this.GetType().FullName, "", ErrorType.Error);
            }
            return resultado;
        }

        public List<MODGruposDeEjecucion> ObtenerGruposEjecucion()
        {
            List<MODGruposDeEjecucion> resultado = new List<MODGruposDeEjecucion>();
            try
            {
                var con = ObtenerConexionPrincipal();
                resultado = con.Query<MODGruposDeEjecucion>("StpFlujos",
                    new { accion = 5 },
                    commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName, "Consulta", ErrorType.Error);
            }
            return resultado;
        }

        public List<MODGruposDeEjecucion> ObtenerGruposEjecucion(int prmIdCategoria)
        {
            List<MODGruposDeEjecucion> grupos = null;
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdCategoria", prmIdCategoria, System.Data.DbType.Int32);
                var reader = conn.QueryMultiple("StpConsultarGruposEjecucion", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);
                if (HasRows(reader))
                {
                    grupos = reader.Read<MODGruposDeEjecucion>().ToList();
                }
                conn.Close();
            }
            return grupos;
        }

        public List<IDictionary<string, object>> EjecutarScirpt(List<MODCampos> campos, string consulta, Dictionary<string, object> parametros)
        {
            List<IDictionary<string, object>> result = null;
            try
            {
                using (var coneccion = this.ObtenerConexionPrincipal())
                {
                    var lector = coneccion.ExecuteReader(consulta,
                                            ObtenerParametros(parametros),
                                            commandType: System.Data.CommandType.Text);
                    result = ConvertirADiccionario(ref lector, campos);
                    coneccion.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.ConfiguracionOrigenDatos - Ejecutar",
                             String.Format(@"campos:{0} ", System.Text.Json.JsonSerializer.Serialize(campos)),
                             ErrorType.Error);
            }
            return result;
        }

        public Dictionary<int, string> ObtenerVersiones(MODFlujoFiltro filtro)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            try
            {
                using (var coneccion = this.ObtenerConexionPrincipal())
                {
                    result = coneccion.Query<dynamic>("StpFlujoHistorico",
                                            new
                                            {
                                                accion = 4,
                                                IdEmpresa = filtro.IdEmpresa,
                                                IdServicio = filtro.IdServicio,
                                                IdElemento = filtro.IdElemento,
                                                StrPeriodo = filtro.StrPeriodo
                                            },
                                            commandType: System.Data.CommandType.StoredProcedure).ToDictionary(x => (int)x.Version, x => String.Format("v.{0}", (int)x.Version));
                    coneccion.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.ConfiguracionOrigenDatos - Ejecutar",
                             String.Format(@"campos:{0} ", System.Text.Json.JsonSerializer.Serialize(filtro)),
                             ErrorType.Error);
            }
            return result;
        }

        public List<string> conPrerequisito(MODFlujoFiltro filtro)
        {
            List<string> result = new List<string>();
            try
            {
                using (var coneccion = this.ObtenerConexionPrincipal())
                {
                    result = coneccion.Query<string>("StpFlujoHistorico",
                                            new
                                            {
                                                accion = 5,
                                                IdFlujo = filtro.Id,
                                                IdEmpresa = filtro.IdEmpresa,
                                                IdServicio = filtro.IdServicio,
                                                IdElemento = filtro.IdElemento,
                                                Periodicidad = filtro.Periodicidad,
                                                StrPeriodo = filtro.StrPeriodo
                                            },
                                            commandType: System.Data.CommandType.StoredProcedure).ToList();
                    coneccion.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.ConfiguracionOrigenDatos - Ejecutar",
                             String.Format(@"campos:{0} ", System.Text.Json.JsonSerializer.Serialize(filtro)),
                             ErrorType.Error);
            }
            return result;
        }

        public List<MODOpciones> ObtenerOpciones(MODFlujoFiltro filtro)
        {
            List<MODOpciones> grupos = null;
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@accion", 6);
                parametros.Add("@IdEmpresa", filtro.IdEmpresa);
                parametros.Add("@IdReporte", filtro.IdElemento);
                parametros.Add("@IdServicio", filtro.IdServicio);
                parametros.Add("@IdTarea", filtro.IdTarea);
                var reader = conn.QueryMultiple("StpFlujos", parametros, null, commandTimeout: int.MaxValue, System.Data.CommandType.StoredProcedure);
                if (HasRows(reader))
                {
                    grupos = reader.Read<MODOpciones>().ToList();
                }
                conn.Close();
            }
            return grupos;
        }
        public List<MODOpciones> ObtenerAcciones()
        {
            List<MODOpciones> grupos = null;
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@accion", 6);
                grupos = conn.Query<MODOpciones>("StpFlujos", parametros, commandType: System.Data.CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return grupos;
        }

        public MODResultado GuardarConf(List<Dictionary<string, object>> lista)
        {
            MODResultado grupos = new MODResultado();
            using (var conn = ObtenerConexionPrincipal())
            {
                foreach (var accion in lista)
                {
                    try
                    {
                        var parametros = new DynamicParameters();
                        parametros.Add("@accion", (1 == Convert.ToInt32(accion["accion"].ToString())) ? 7 : 8);
                        parametros.Add("@IdTarea", Convert.ToInt32(accion["IdTarea"].ToString()));
                        parametros.Add("@IdAccion", Convert.ToInt32(accion["IdAccion"].ToString()));
                        var res = conn.Query<dynamic>("StpFlujos", parametros, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    }
                    catch (Exception e)
                    {
                        grupos.Errores.Add(e.Message);
                        Log.WriteLog(e, this.GetType().FullName + "- GuardarConf",
                        String.Format(@"campos:{0} ", System.Text.Json.JsonSerializer.Serialize(accion)),
                        ErrorType.Error);
                    }
                }
                conn.Close();
            }
            return grupos;
        }

        public List<MODOpciones> ObtenerAccionesSeleccionadas(MODFlujoFiltro filtro)
        {
            List<MODOpciones> grupos = null;
            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@accion", 9);
                parametros.Add("@IdEmpresa", filtro.IdEmpresa);
                parametros.Add("@IdReporte", filtro.IdElemento);
                parametros.Add("@IdServicio", filtro.IdServicio);
                parametros.Add("@IdTarea", filtro.IdTarea);
                grupos = conn.Query<MODOpciones, MODProcTipoOpcion, MODOpciones>("StpFlujos", (p, o) => { p.procTipoOpcion = o; return p; }, parametros, commandTimeout: int.MaxValue, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return grupos;
        }

        public MODResultado GuardarCorrecciones(string sql, List<Dictionary<string, object>> lista)
        {
            MODResultado grupos = new MODResultado();
            using (var conn = ObtenerConexionPrincipal())
            {

                foreach (var accion in lista)
                {
                    try
                    {

                        var parametros = new DynamicParameters();
                        parametros.Add("@IDCAUSA_SIR",(accion["id_causa_sir"]!=null)?accion["id_causa_sir"].ToString():null);
                        parametros.Add("@DESCRIPCION_SIR",(accion["descripcion_sir"] !=null)?accion["descripcion_sir"].ToString():null);
                        parametros.Add("@CONFIRMACION_SIR", 1);
                        parametros.Add("@ID_TABLA", accion["id_tabla"].ToString());
                        if (accion.ContainsKey("f_alta"))
                        {
                            parametros.Add("@F_ALTA", (accion["f_alta"]!=null)?accion["f_alta"].ToString():null);
                        }
                        if (accion.ContainsKey("f_reposicion"))
                        {
                            parametros.Add("@F_REPOSICION", accion["f_reposicion"]!=null?accion["f_reposicion"].ToString():null);
                        }
                        var res = conn.Query<dynamic>(sql, parametros, commandType: System.Data.CommandType.Text).FirstOrDefault();
                    }
                    catch (Exception e)
                    {
                        grupos.Errores.Add(e.Message);
                        Log.WriteLog(e, this.GetType().FullName + "- GuardarConf",
                        String.Format(@"campos:{0} ", System.Text.Json.JsonSerializer.Serialize(accion)),
                        ErrorType.Error);
                    }
                }
                conn.Close();
            }
            return grupos;
        }

        public MODResultado ConfirmarCorrecciones(string sql, List<Dictionary<string, object>> lista)
        {
            MODResultado grupos = new MODResultado();
            using (var conn = ObtenerConexionPrincipal())
            {
                foreach (var accion in lista)
                {
                    try
                    {
                        var parametros = new DynamicParameters();
                        parametros.Add("@ID_TABLA", accion["id_tabla"].ToString());
                        var res = conn.Query<dynamic>(sql, parametros, commandType: System.Data.CommandType.Text).FirstOrDefault();
                    }
                    catch (Exception e)
                    {
                        grupos.Errores.Add(e.Message);
                        Log.WriteLog(e, this.GetType().FullName + "- GuardarConf",
                        String.Format(@"campos:{0} ", System.Text.Json.JsonSerializer.Serialize(accion)),
                        ErrorType.Error);
                    }
                }
                conn.Close();
            }
            return grupos;
        }
    }
}