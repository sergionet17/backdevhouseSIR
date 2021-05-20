using Dapper;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Parametrizacion_SIR;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.Parametrizacion_SIR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SIR.Datos.Concretos.Parametrizacion_SIR
{
    public class Cliente_ExcluidoDatos : Dal_Base, ICliente_ExcluidoDatos
    {
        public MODResultado Borrar(MOD_Cliente_Excluido Cliente)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                using (var conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    var _cliente = conn.Query("StpClienteExcluido", new
                    {
                        accion = 3,
                        IdCliente = Cliente.Id,
                        NUI = Cliente.NUI,
                        ID_CAUSA = Cliente.Id_Causa,
                        OBSERVACION = Cliente.Observacion,
                        FEC_ULT_MODIFICACION = Cliente.Fec_Ult_Modificacion,
                        USUARIO_MOD = Cliente.Usuario_Mod
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (!string.IsNullOrEmpty(_cliente.ERROR))
                    {
                        Log.WriteLog(new Exception(_cliente.Error), this.GetType().Namespace,
                             String.Format(@"Cliente:{0}", System.Text.Json.JsonSerializer.Serialize(Cliente)),
                             ErrorType.Error);
                        resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().Namespace,
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(Cliente)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            return resultado;
        }

        public MODResultado Modificar(MOD_Cliente_Excluido Cliente)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                using (var conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    var _cliente = conn.Query("StpClienteExcluido", new
                    {
                        accion = 2,
                        IdCliente = Cliente.Id,
                        NUI = Cliente.NUI,
                        ID_CAUSA = Cliente.Id_Causa,
                        OBSERVACION = Cliente.Observacion,
                        FEC_ULT_MODIFICACION = Cliente.Fec_Ult_Modificacion,
                        USUARIO_MOD = Cliente.Usuario_Mod
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (!string.IsNullOrEmpty(_cliente.ERROR))
                    {
                        Log.WriteLog(new Exception(_cliente.Error), this.GetType().Namespace,
                             String.Format(@"Cliente:{0}", System.Text.Json.JsonSerializer.Serialize(Cliente)),
                             ErrorType.Error);
                        resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().Namespace,
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(Cliente)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            return resultado;
        }

        public IEnumerable<MOD_Cliente_Excluido> Obtener(MOD_Cliente_ExcluidoFiltro filtro)
        {
            IEnumerable<MOD_Cliente_Excluido> resultado = new List<MOD_Cliente_Excluido>();

            using (var conn = ObtenerConexionPrincipal())
            {
                var reader = conn.QueryMultiple("StpClienteExcluido",
                    new
                    {
                        accion = 4,
                        IdCliente = filtro.Id,
                        NUI = filtro.NUI,
                        ID_CAUSA = filtro.Id_Causa,
                        USUARIO_MOD = filtro.Usuario_Mod
                    },
                    null,
                    int.MaxValue,
                    System.Data.CommandType.StoredProcedure);

                resultado = reader.Read<MOD_Cliente_Excluido>().ToList();
                conn.Close();
            }

            return resultado;
        }

        public MODResultado Registrar(MOD_Cliente_Excluido Cliente)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                using (var conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    var _cliente = conn.Query("StpClienteExcluido", new
                    {
                        accion = 1,
                        IdCliente = Cliente.Id,
                        NUI = Cliente.NUI,
                        ID_CAUSA = Cliente.Id_Causa,
                        OBSERVACION = Cliente.Observacion,
                        FEC_ULT_MODIFICACION = Cliente.Fec_Ult_Modificacion,
                        USUARIO_MOD = Cliente.Usuario_Mod
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (!string.IsNullOrEmpty(_cliente.ERROR))
                    {
                        Log.WriteLog(new Exception(_cliente.Error), this.GetType().Namespace,
                             String.Format(@"Cliente:{0}", System.Text.Json.JsonSerializer.Serialize(Cliente)),
                             ErrorType.Error);
                        resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().Namespace,
                             String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(Cliente)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            return resultado;
        }
    }
}
