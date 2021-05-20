using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.UsuarioPerfil;
using System.Net;

namespace SIR.Autenticacion.Api.Models.Response
{
    public class LoginResponse : MODResultado
    {
        public MODUsuario Usuario { get; set; }

        public string Token { get; set; }

        public LoginResponse(HttpStatusCode prmCodigoRespuesta, string prmCodigoMensaje)
        {
            this.CodigoRespuesta = prmCodigoRespuesta;
            this.CodigoMensaje = prmCodigoMensaje;
        }

        public LoginResponse(MODUsuario prmUsuario, string prmToken)
        {
            this.CodigoRespuesta = HttpStatusCode.OK;
            this.CodigoMensaje = "RTA000";
            this.Usuario = prmUsuario;
            this.Token = prmToken;
        }
    }
}
