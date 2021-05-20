using SIR.Comun.Entidades.Abstracto;
using SIR.Comun.Enumeradores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.FlujoDeTrabajo
{
    [DataContract]
    [Serializable]
    public class MODOrigenDatos : MODBase
    {
        [DataMember]
        [Column(Order = 1, TypeName = "ID")]
        public int id { get; set; }
        [DataMember]
        [Column(Order = 2, TypeName = "idTarea")]
        public int IdTarea { get; set; }
        [DataMember]
        [Column(Order = 3, TypeName = "nombre")]
        public string Nombre { get; set; }
        [DataMember]
        [Column(Order = 4, TypeName = "tipoOrigen")]
        public EnumBaseDatosOrigen TipoOrigen { get; set; }
        [DataMember]
        [Column(Order = 5, TypeName = "Usuario")]
        public string UsuarioBD { get; set; }
        [DataMember]
        [Column(Order = 6, TypeName = "Clave")]
        public string ClaveBD { get; set; }
        [DataMember]
        [Column(Order = 7, TypeName = "Servidor")]
        public string Servidor { get; set; }
        [DataMember]
        [Column(Order = 8, TypeName = "SID")]
        public string Sid { get; set; }
        [DataMember]
        [Column(Order = 9, TypeName = "Puerto")]
        public string Puerto { get; set; }
        [DataMember]
        [Column(Order = 11, TypeName = "consulta")]
        public string consulta { get; set; }
        [DataMember]
        [Column(Order = 12, TypeName = "tipoMando")]
        public System.Data.CommandType tipoMando { get; set; }

        [DataMember]
        [Column(Order = 10, TypeName = "codigo_Externo_tarea")]
        public Guid codigo_Externo_tarea { get; set; }

        [DataMember]
        public string NombreTabla { get; set; }

        [DataMember]
        public string cadenaConexion
        {
            get
            {
                string resultado = string.Empty;
                if (this.TipoOrigen == EnumBaseDatosOrigen.Oracle)
                {
                    resultado = string.Format(@"DATA SOURCE={0}{1}{4};PASSWORD={2};PERSIST SECURITY INFO=True;USER ID={3}",
                           this.Servidor,
                           string.IsNullOrEmpty(this.Sid) ? string.Empty : string.Format(@"/{0}", this.Sid),
                           this.ClaveBD,
                           this.UsuarioBD,
                           string.IsNullOrEmpty(this.Puerto) ? string.Empty : string.Format(@":{0}", this.Puerto)
                        );
                }
                else
                {
                    resultado = string.Format(@"Data Source={0};Initial Catalog={1};User ID={3};Password={2};MultipleActiveResultSets=True",
                          this.Servidor,
                          this.Sid,
                          this.ClaveBD,
                          this.UsuarioBD
                       );
                }
                return resultado;
            }
        }
        [DataMember]
        public IDictionary<string, object> Parametros { get; set; }
    }
}