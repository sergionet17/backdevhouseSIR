using SIR.Comun.Entidades.Abstracto;
using SIR.Comun.Entidades.UsuarioPerfil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SIR.Autenticacion.Api.Models.Request
{
    [DataContract]
    [Serializable]
    public class CrearUsuarioRequestModel
    {
        [Required]
        [DataMember]
        public string NombreUsuario { get; set; }

        [Required]
        [DataMember]
        public string Ip { get; set; }

        [Required]
        [DataMember]
        public MODUsuario Usuario { get; set; }
        
        [Required]
        [DataMember]
        public List<MODPermisoUsuario> Permisos { get; set; }
    }
}
