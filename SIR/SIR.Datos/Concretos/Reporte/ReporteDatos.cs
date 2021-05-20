using Dapper;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.Reporte;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SIR.Datos.Concretos.Reporte
{
    public class ReporteDatos : Dal_Base, IReporteDatos
    {
        public MODResultado Actualizar(MODReporte reporte)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                using (var conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    var _reporte = conn.Query("StpReporte", new
                    {
                        accion = 2,
                        IdReporte = reporte.Id,
                        Descripcion = reporte.Nombre,
                        Norma = reporte.Descripcion,
                        Activo = reporte.Activo,
                        ActivoEmpresa = reporte.ActivoEmpresa,
                        IdEmpresa = reporte.IdEmpresa,
                        IdServicio = reporte.IdServicio,
                        EsReporte = reporte.EsReporte
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (_reporte.IdReporte == 0)
                    {
                        Log.WriteLog(new Exception(_reporte.Error), "SIR.Datos.Concretos.Reporte - Registrar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                        resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }
                    else
                    {
                        reporte.campos.ForEach(x => {
                            x.IdEmpresa = reporte.IdEmpresa;
                            x.IdServicio = reporte.IdServicio;
                            x.IdReporte = _reporte.IdReporte;
                        });

                        List<MODCampos> _tablaCampos = new List<MODCampos>();
                        EstructuraTablaCampo(ref _tablaCampos);
                        var _tabla = ConvertirATabla(reporte.campos);
                        InsertarBloque(conn, "##CampoTemporal", _tablaCampos, _tabla);

                        conn.Query("StpReporte", new { accion = 3,
                            IdEmpresa = reporte.IdEmpresa,
                            IdServicio = reporte.IdServicio,
                            IdReporte = reporte.Id
                        }, commandType: System.Data.CommandType.StoredProcedure);
                        conn.Close();
                    }

                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.Reporte - Actualizar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            return resultado;
        }

        public MODResultado Registrar(MODReporte reporte)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                using (var conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    var _reporte = conn.Query("StpReporte", new
                    {
                        accion = 1,
                        IdReporte = reporte.Id,
                        Descripcion = reporte.Nombre,
                        Norma = reporte.Descripcion,
                        Activo = reporte.Activo,
                        ActivoEmpresa = reporte.ActivoEmpresa,
                        IdEmpresa = reporte.IdEmpresa,
                        IdServicio = reporte.IdServicio,
                        IdCategoria = reporte.IdCategoria,
                        EsReporte = reporte.EsReporte
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (_reporte.IdReporte == 0)
                    {
                        Log.WriteLog(new Exception(_reporte.Error), "SIR.Datos.Concretos.Reporte - Registrar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                        resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }
                    else
                    {
                        reporte.campos.ForEach(x => { 
                            x.IdEmpresa = reporte.IdEmpresa;
                            x.IdServicio = reporte.IdServicio;
                            x.IdReporte = _reporte.IdReporte; 
                        });

                        List<MODCampos> _tablaCampos = new List<MODCampos>();
                        EstructuraTablaCampo(ref _tablaCampos);

                        var _tabla = ConvertirATabla(reporte.campos);

                        InsertarBloque(conn, "Campo", _tablaCampos, _tabla);
                        conn.Close();
                    }

                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.Reporte - Registrar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            return resultado;
        }

        public List<MODReporte> ObtenerReportes()
        {
            List<MODReporte> resultado = new List<MODReporte>();

            using (var conn = ObtenerConexionPrincipal())
            {
                var reader = conn.QueryMultiple("StpReporte",
                    new
                    {
                        accion = 4
                    },
                    null,
                    int.MaxValue,
                    System.Data.CommandType.StoredProcedure);

                resultado = reader.Read<MODReporte>().ToList();
                if (resultado != null)
                {
                    var camposBD = reader.Read<MODCampos>().ToList();
                    resultado.ForEach(y => y.campos = camposBD.Where(x => x.IdEmpresa == y.IdEmpresa && x.IdServicio == y.IdServicio && x.IdReporte == y.Id).ToList());
                }

                conn.Close();
            }

            return resultado;
        }

        private void EstructuraTablaCampo(ref List<MODCampos> resultado)
        {
            resultado.Add(new MODCampos { Nombre = "Id", Tipo = EnumTipoDato._int, Largo = "0", Ordinal = 1 });
            resultado.Add(new MODCampos { Nombre = "IdEmpresa", Tipo = EnumTipoDato._int, Largo = "0", Ordinal = 2 });
            resultado.Add(new MODCampos { Nombre = "IdServicio", Tipo = EnumTipoDato._int, Largo = "0", Ordinal = 3 });
            resultado.Add(new MODCampos { Nombre = "IdReporte", Tipo = EnumTipoDato._int, Largo = "0", Ordinal = 4 });
            resultado.Add(new MODCampos { Nombre = "Nombre", Tipo = EnumTipoDato._string, Largo = "500", Ordinal = 5 });
            resultado.Add(new MODCampos { Nombre = "Tipo", Tipo = EnumTipoDato._int, Largo = "0", Ordinal = 6 });
            resultado.Add(new MODCampos { Nombre = "largo", Tipo = EnumTipoDato._string, Largo = "10", Ordinal = 7 });
            resultado.Add(new MODCampos { Nombre = "Ordinal", Tipo = EnumTipoDato._int, Largo = "0", Ordinal = 8 });
            resultado.Add(new MODCampos { Nombre = "esVersionable", Tipo = EnumTipoDato._bool, Largo = "0", Ordinal = 9 });
        }

        public MODResultado Borrar(MODReporteFiltro reporte)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                using (var conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    var _reporte = conn.Query("StpReporte", new
                    {
                        accion = 5,
                        IdReporte = reporte.Id,
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    conn.Close();
                    if (_reporte.IdReporte == 0)
                    {
                        resultado.Errores.Add("REPORTES.MENSAJES.ELIMINADONOVALIDO");
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.Reporte - Borrar",
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            return resultado;
        }

        public List<MODReporte> ObtenerReportesLimpio()
        {
            List<MODReporte> reportes = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var reader = conn.QueryMultiple("StpObtenerReportes", null, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                reportes = reader.Read<MODReporte>().ToList();

                conn.Close();
            }

            return reportes;
        }
    }
}
