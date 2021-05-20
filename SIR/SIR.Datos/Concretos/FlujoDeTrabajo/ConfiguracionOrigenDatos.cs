using Dapper;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.FlujoDeTrabajo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SIR.Datos.Concretos.FlujoDeTrabajo
{
    public class ConfiguracionOrigenDatos : Dal_Base, IConfiguracionOrigenesDatos
    {
        public List<IDictionary<string, object>> Ejecutar(MODOrigenDatos origen, MODReporte reporte, ref MODResultado resultado)
        {
            List<IDictionary<string, object>> result = null;
            try
            {
                using (var coneccion = ObtenerConexion(origen.cadenaConexion, origen.TipoOrigen))
                {
                    origen.consulta = (origen.TipoOrigen == Comun.Enumeradores.EnumBaseDatosOrigen.Oracle)?origen.consulta.Replace("@periodo",":periodo"):origen.consulta;
                    var lector = coneccion.ExecuteReader(origen.consulta,
                                            ObtenerParametros(origen),
                                            commandType: origen.tipoMando,
                                            commandTimeout: int.MaxValue);
                    result = ConvertirADiccionario(ref lector, reporte.campos);
                    coneccion.Close();
                }
            }
            catch(Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.ConfiguracionOrigenDatos - Ejecutar",
                             String.Format(@"Origen:{0} - reporte:{1}", System.Text.Json.JsonSerializer.Serialize(origen), 
                             System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            return result;
        }
        public MODResultado ProcesarEstracion(string destino, List<IDictionary<string, object>> resultadoOrigenes, List<MODCampos> campos)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var _tabla = ConvertirATabla(resultadoOrigenes, campos);
                using (SqlConnection conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    InsertarBloque(conn, destino, campos, _tabla);
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.ConfiguracionOrigenDatos - ProcesarEstracion",
                             String.Format(@"resultadoOrigenes:{0}", System.Text.Json.JsonSerializer.Serialize(resultadoOrigenes)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            
            return resultado;
        }
        private DynamicParameters ObtenerParametros(MODOrigenDatos origen)
        {
            DynamicParameters parametros = new DynamicParameters();
            try
            {
                if (origen.Parametros != null)
                {
                    foreach (var a in origen.Parametros)
                    {
                        parametros.Add(a.Key, a.Value);
                    }
                }
                else
                {
                    parametros = null;
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Datos.Concretos.ConfiguracionOrigenDatos - ObtenerParametros",
                             String.Format(@"Origen:{0}", System.Text.Json.JsonSerializer.Serialize(origen)),
                             ErrorType.Error);
            }

            return parametros;
        }
    }
}
