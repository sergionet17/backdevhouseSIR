using SIR.Comun.Entidades.UsuarioPerfil;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Datos.Interfaces.Perfilamiento;
using SIR.Negocio.Interfaces.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIR.Negocio.Concretos.Perfilamiento
{
    public class UsuarioNegocio : IUsuarioNegocio
    {
        public MODUsuario ObtenerUsuario(string prmUsername) 
        {
            IUsuarioDatos usuarioDatos = FabricaDatos.CrearUsuarioDatos;

            return usuarioDatos.ObtenerUsuario(prmUsername);
        }

        public MODUsuario ObtenerUsuarioPorId(int prmIdUsuario)
        {
            IUsuarioDatos usuarioDatos = FabricaDatos.CrearUsuarioDatos;

            return usuarioDatos.ObtenerUsuarioPorId(prmIdUsuario);
        }


        public bool ActualizarUltimaFechaLogin(string prmUsername) 
        {
            IUsuarioDatos usuarioDatos = FabricaDatos.CrearUsuarioDatos;

            return usuarioDatos.ActualizarUltimaFechaLogin(prmUsername);
        }

        public int CrearUsuario(MODUsuario prmUsuario, List<MODPermisoUsuario> prmPermisos) 
        {
            IUsuarioDatos usuarioDatos = FabricaDatos.CrearUsuarioDatos;

            var idUsuario = usuarioDatos.CrearUsuario(prmUsuario);

            if (idUsuario != 0)
            {
                prmPermisos.ForEach(x => x.IdUsuario = idUsuario);

                if (!this.InsertarPermisosUsuario(prmPermisos))
                    throw new System.Exception("Ha ocurrido un error intentando crear el usuario, no se han insertado correctamente los permisos");
            }
            else
                throw new System.Exception("Ha ocurrido un error intentando crear el usuario, no se ha obtenido el ID");

            return idUsuario;
        }

        public bool InsertarPermisosUsuario(List<MODPermisoUsuario> prmPermisos) 
        {
            IUsuarioDatos usuarioDatos = FabricaDatos.CrearUsuarioDatos;

            return usuarioDatos.InsertarPermisosUsuario(prmPermisos);
        }

        public List<MODUsuarioBasico> ObtenerUsuarios(MODUsuarioFiltro filtro) 
        {
            IUsuarioDatos usuarioDatos = FabricaDatos.CrearUsuarioDatos;
            return usuarioDatos.ObtenerUsuarios(filtro);
        }

        public bool ActualizarUsuario(MODUsuario prmUsuario, List<MODPermisoUsuario> prmPermisos)
        {
            IUsuarioDatos usuarioDatos = FabricaDatos.CrearUsuarioDatos;

            if (usuarioDatos.ActualizarUsuario(prmUsuario))
            {
                if (prmPermisos != null && prmPermisos.Count > 0)
                {
                    prmPermisos.ForEach(x => x.IdUsuario = prmUsuario.IdUsuario);

                    this.InsertarPermisosUsuario(prmPermisos);
                }
 
                return true;
            }
            else 
            {
                return false;
            }
        }

        public bool EliminarUsuario(int prmIdUsuario)
        {
            IUsuarioDatos usuarioDatos = FabricaDatos.CrearUsuarioDatos;
            if (usuarioDatos.EliminarUsuario(prmIdUsuario))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Tuple<bool,string>> ValidarUsuarioServicioExterno(string prmUsuario, string prmPassword, bool prmValidarExiste = false) 
        {
            string codigoAplicacion = Configuraciones.ObtenerConfiguracion("Keys", "CodigoAplicacion");
            string codigoMensaje = "COMUNES.ERRORSERVICIO";

            bool usuarioOk = false;
            bool _validarExterna = Convert.ToBoolean(Configuraciones.ObtenerConfiguracion("Contexto", "AutenticacionSCP"));

            byte[] newBytes = Convert.FromBase64String(prmPassword);
            prmPassword = System.Text.Encoding.UTF8.GetString(newBytes);

            using (WCFAutenticacionExterna.AuthenticationClient serviceAuth = new WCFAutenticacionExterna.AuthenticationClient())
            {
                var user = _validarExterna ? await serviceAuth.loginAsync(prmUsuario, prmPassword, codigoAplicacion) : "Ok";

                if (prmValidarExiste)
                {
                    if (user.ToUpper().Equals("OK") || user.ToUpper().Contains("CONTRASEÑA INCORRECTA"))
                    {
                        usuarioOk = true;
                        codigoMensaje = "USUARIO.RESPUESTAS.RTA000";
                    }
                    else if (user.Contains("Usuario no existe"))
                    {
                        codigoMensaje = "USUARIO.RESPUESTAS.RTA001";
                    }
                }
                else 
                {
                    if (user.Equals("Ok"))
                        usuarioOk = true;
                    else if (user.Contains("Contraseña incorrecta"))
                        codigoMensaje = "LOGIN.RTA001";
                    else if (user.Contains("Usuario no existe"))
                        codigoMensaje = "LOGIN.RTA002";
                    else if (user.Contains("Cuenta Bloqueada"))
                        codigoMensaje = "LOGIN.RTA008";
                    else if (user.Contains("no tiene acceso"))
                        codigoMensaje = "LOGIN.RTA009";
                }
            }

            return new Tuple<bool,string>(usuarioOk, codigoMensaje);
        }
    }
}
