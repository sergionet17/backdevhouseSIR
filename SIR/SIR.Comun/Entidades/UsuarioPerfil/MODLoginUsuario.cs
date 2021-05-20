using System;

namespace SIR.Comun.Entidades.UsuarioPerfil
{
    public class MODLoginUsuario
    {
        public string IP { get; set; }
        public string Token { get; set; }
        public int IdUsuario { get; set; }
        public string UserName { get; set; }
        public DateTime Fecha { get; set; }
    }
}