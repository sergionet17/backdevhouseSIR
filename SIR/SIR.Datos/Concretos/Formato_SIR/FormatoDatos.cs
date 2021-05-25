using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SIR.Comun.Entidades.Formatos_SIR;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.Formatos_SIR;

namespace SIR.Datos.Concretos.Formato_SIR
{
    public class FormatoDatos : Dal_Base, ICargaFormatoDatos
    {
        public MODResultado Cargar(MOD_Carga_Formato Formato)
        {
            MODResultado resultado = new MODResultado();

            try
            {
                using (var conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    var _cliente = conn.Query("StpCargaFormatos", new
                    {

                        Id_formato = Formato.Id,
                        Descripcion = Formato.detalle
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    if (!string.IsNullOrEmpty(_cliente.ERROR))
                    {
                        Log.WriteLog(new Exception(_cliente.Error), this.GetType().Namespace,
                             String.Format(@"Formato:{0}", System.Text.Json.JsonSerializer.Serialize(Formato)),
                             ErrorType.Error);
                        resultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }
                    conn.Close();
                }
            }catch(Exception e  )
            {
                Log.WriteLog(e, this.GetType().Namespace,
                                String.Format(@"reporte:{0}", System.Text.Json.JsonSerializer.Serialize(Formato)),
                                ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");

            }

            return resultado;
        }
    }
}
