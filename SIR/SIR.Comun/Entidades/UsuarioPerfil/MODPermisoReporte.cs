using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.UsuarioPerfil
{
    [DataContract]
    [Serializable]
    public class MODPermisoReporte
    {
        [DataMember] 
        public int IdEmpresa { get; set; }
        [DataMember] 
        public int IdServicio { get; set; }
        [DataMember] 
        public int IdReporte { get; set; }
        [DataMember] 
        public string Descripcion { get; set; }
        [DataMember]
        public bool Activo { get; set; }
        [DataMember]
        public int TipoPermiso { get; set; }
        [DataMember]
        public int IdReporteEmpresaServicio { get; set; }
        [DataMember]
        public int IdCategoria { get; set; }
    }
}
