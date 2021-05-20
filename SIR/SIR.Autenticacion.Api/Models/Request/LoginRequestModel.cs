using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SIR.Autenticacion.Api.Models.Request
{
    /// <summary>
    /// Modelo para petición de login
    /// </summary>
    [DataContract]
    public class LoginRequestModel
    {
        /// <summary>
        /// Usuario
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Ip
        /// </summary>
        [DataMember]
        public string Ip { get; set; }
    }
}
