using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Datos
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();

        private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();

        public void Add(string name, object value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null)
        {
            dynamicParameters.Add(name, value, dbType, direction, size);
        }

        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction)
        {
            var oracleParameter = new OracleParameter(name, oracleDbType, direction);
            oracleParameters.Add(oracleParameter);
        }
        public void AddArray<T>(string name,string dbType,T[] array,ParameterDirection dir,T emptyArrayValue)
        {
            var parameter = new OracleParameter
            {
                ParameterName = name,
                UdtTypeName = dbType,
                CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                DbType = DbType.Object
            };
            
            // oracle does not support passing null or empty arrays.
            // so pass an array with exactly one element
            // with a predefined value and use it to check
            // for empty array condition inside the proc code
            if (array == null || array.Length == 0)
            {
                parameter.Value = new T[1] { emptyArrayValue };
                parameter.Size = 1;
            }
            else
            {
                parameter.Value = array;
                parameter.Size = array.Length;
            }
            oracleParameters.Add(parameter);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);

            var oracleCommand = command as OracleCommand;

            if (oracleCommand != null)
            {
                oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
            }
        }
    }
}
