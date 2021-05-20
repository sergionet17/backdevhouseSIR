using Oracle.ManagedDataAccess.Client;
using SIR.Comun;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SIR.Datos.Fabrica
{
    public class Conexion
    {
        public static IDbConnection CrearConexion(EnumBaseDatos _baseDatos = EnumBaseDatos.SIR, EnumBaseDatosOrigen _origen = EnumBaseDatosOrigen.SqlServer)
        {
            IDbConnection conn = null;
            switch (_origen)
            {
                case EnumBaseDatosOrigen.SqlServer:
                    conn = new SqlConnection(ObtenerNombre(_baseDatos));
                    break;
                case EnumBaseDatosOrigen.Oracle:
                    conn = new OracleConnection(ObtenerNombre(_baseDatos));
                    break;
                default:
                    throw new Exception("Base de datos no soportada");
            }
            conn.Open();
            return conn;
        }
        public static IDbConnection CrearConexion(string cadenaConeccion, EnumBaseDatosOrigen _origen = EnumBaseDatosOrigen.SqlServer)
        {
            IDbConnection conn = null;
            switch (_origen)
            {
                case EnumBaseDatosOrigen.SqlServer:
                    conn = new SqlConnection(cadenaConeccion);
                    break;
                case EnumBaseDatosOrigen.Oracle:
                    conn = new OracleConnection(cadenaConeccion);
                    break;
                default:
                    throw new Exception("Base de datos no soportada");
            }
            conn.Open();
            return conn;
        }


        #region Privado
        private static string ObtenerNombre(EnumBaseDatos _baseDatos)
        {
            return Configuraciones.ObtenerConfiguracion("Conexiones", _baseDatos.ToString());
        }
        #endregion
    }
}
