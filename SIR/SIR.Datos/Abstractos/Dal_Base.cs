using SIR.Comun.Entidades;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using static Dapper.SqlMapper;

namespace SIR.Datos.Abstractos
{
    public abstract class Dal_Base
    {
        protected IDbConnection ObtenerConexionPrincipal()
        {
            return Conexion.CrearConexion();
        }

        protected IDbConnection ObtenerConexion(string cadenaConeccion, EnumBaseDatosOrigen _origen)
        {
            return Conexion.CrearConexion(cadenaConeccion, _origen);
        }

        object ConvertirObjeto(EnumTipoDato tipo)
        {
            switch (tipo)
            {
                case EnumTipoDato._int:
                    return new int();
                case EnumTipoDato._string:
                    return new String("");
                case EnumTipoDato._datetime:
                    return new DateTime();
                case EnumTipoDato._bool:
                    return new bool();
                case EnumTipoDato._decimal:
                    return new Decimal();
                default:
                    return new String("");
            }
        }

        public List<IDictionary<string, object>> ConvertirADiccionario(ref System.Data.IDataReader lector, List<MODCampos> campos)
        {
            var _rows = new List<IDictionary<string, object>>();
            var dtscheme = lector.GetSchemaTable() != null ? lector.GetSchemaTable().AsEnumerable().Select(x => new { Nombre = x["ColumnName"], Orden = x["ColumnOrdinal"] }).ToDictionary(x => x.Nombre.ToString().ToLower(), x => (int)x.Orden) : null;
            while (lector.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                if (campos != null && campos.Count > 0)
                {
                    foreach (var campo in campos)
                    {
                        int ord = -1;
                        if (dtscheme.TryGetValue(campo.Nombre.ToLower(), out ord) && !row.ContainsKey(campo.Nombre.ToUpper()))
                        {
                            row.Add(campo.Nombre.ToUpper(), lector.GetValue(ord));
                        }
                    }
                }
                else
                {
                    //TODO:Leer reader y generar la _rows
                }

                _rows.Add(row);
            }
            return _rows;
        }

        public DataTable ConvertirATabla(List<IDictionary<string, object>> origenes, List<MODCampos> campos)
        {
            var _tabla = new DataTable();
            foreach (var _campo in campos)
            {
                var _objeto = ConvertirObjeto(_campo.Tipo);
                var _col = new DataColumn(_campo.Nombre.ToUpper(), ConvertirObjeto(_campo.Tipo).GetType());
                _col.AllowDBNull = true;
                _tabla.Columns.Add(_col);
            }
            foreach (var _registro in origenes)
            {
                DataRow _fila = _tabla.NewRow();
                foreach (var _campo in campos)
                {
                     object valor = null;
                     if(_registro.TryGetValue(_campo.Nombre.ToUpper(), out valor)){
                         _fila[_campo.Nombre] = ConvertirAObjeto(ConvertirObjeto(_campo.Tipo).GetType(),valor.ToString());
                     }                    
                }
                _tabla.Rows.Add(_fila);
            }
            return _tabla;
        }
        public DataTable ConvertirATabla<T>(IList<T> data)
        {
            System.ComponentModel.PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (System.ComponentModel.PropertyDescriptor prop in properties)
            {
                var _attr = prop.Attributes.OfType<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>();
                if (_attr.Any())
                {
                    var _col = new DataColumn(_attr.First().TypeName.ToUpper(), Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    _col.AllowDBNull = true;
                    table.Columns.Add(_col);
                }
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                {
                    var _attr = prop.Attributes.OfType<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>();
                    if (_attr.Any())
                    {
                        row[_attr.First().TypeName.ToUpper()] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }

                table.Rows.Add(row);
            }
            return table;
        }

        public List<MODCampos> ObtenerCampos<T>()
        {
            var resultado = new List<MODCampos>();
            System.ComponentModel.PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            foreach (System.ComponentModel.PropertyDescriptor prop in properties)      //
            {
                var _attr = prop.Attributes.OfType<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>();
                if (_attr.Any())
                {
                    resultado.Add(new MODCampos { Nombre = _attr.First().TypeName.ToUpper(), Ordinal = _attr.First().Order });
                }
            }

            return resultado;

        }

        public void InsertarBloque(SqlConnection conn, string nombreTabla, List<MODCampos> campos, DataTable tabla)
        {
            using (SqlBulkCopy copy = new SqlBulkCopy(conn))
            {
                copy.DestinationTableName = nombreTabla;
                copy.BulkCopyTimeout = 660;
                foreach (var _campo in campos)
                {
                    copy.ColumnMappings.Add(_campo.Nombre.ToUpper(), _campo.Nombre.ToUpper());
                }                
                copy.WriteToServer(tabla);
                copy.Close();       
            }
        }
        protected bool HasRows(GridReader reader)
        {
            SqlDataReader internalReader = (SqlDataReader)typeof(GridReader).
                GetField
                ("reader",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(reader);
            return internalReader.HasRows;
        }
        protected object ConvertirAObjeto(Type tipo, string valor)
        {
            if (tipo == valor.GetType())
            {
                return valor;
            }
            else
            {
                MethodInfo tryparce = tipo.GetMethod("TryParse", new Type[] { typeof(string), tipo.MakeByRefType() });
                object[] parametros = new object[]{valor,null};
                tryparce.Invoke(null, parametros);
                if (tipo == typeof(DateTime) && ((DateTime)parametros[1]) == DateTime.MinValue)
                {
                    return DBNull.Value; 
                }
                return parametros[1];
            }
        }
    }
}
