using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SIR.Autenticacion.Api.Models.Request;
using SIR.Autenticacion.Api.Models.Response;
using SIR.Comun.Entidades.UsuarioPerfil;
using SIR.Comun.Funcionalidades;
using SIR.Negocio.Concretos;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Perfilamiento;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SIR.Autenticacion.Api.Controllers
{
    /// <summary>
    /// Session de configuracion de reportes
    /// </summary>
    /// <remarks>
    /// Controlador encargado de ralizar las tareas de Cracion edicion y 
    /// eliminacion de los reportes    
    /// </remarks>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AutenticacionController : ControllerBase
    {
        private IConfiguration _config;

        public AutenticacionController(IConfiguration config)
        {
            this._config = config;
        }

        /// <summary>
        /// Metodo encargado de validar un usuario contra el SCP y contra SIR, encargado del Login
        /// </summary>
        /// <param name="prmLogin">Modelo LoginRequestModel</param>
        /// <returns>Modelo LoginResponse</returns>
        /// <response code="200">Logueo de forma exitosa</response>
        /// <response code="500">No fue posible loguearse validar</response>
        [HttpPost]
        public async Task<LoginResponse> Login([FromBody]LoginRequestModel prmLogin)
        {
            LoginResponse respuesta = null;
            string codigoMensaje = "COMUNES.ERRORSERVICIO";
            IUsuarioNegocio usuarioNegocio = FabricaNegocio.CrearUsuarioNegocio;

            try
            {
                if (String.IsNullOrEmpty(prmLogin.Username) || String.IsNullOrEmpty(prmLogin.Password))
                    respuesta = new LoginResponse(HttpStatusCode.BadRequest, "LOGIN.RTA004");
                else
                {
                    var usuarioExterno = await usuarioNegocio.ValidarUsuarioServicioExterno(prmLogin.Username, prmLogin.Password, false);
                    bool usuarioOk = usuarioExterno.Item1;
                    codigoMensaje = usuarioExterno.Item2;

                    if (usuarioOk)
                    {
                        var usuarioBD = usuarioNegocio.ObtenerUsuario(prmLogin.Username);

                        if (usuarioBD != null)
                        {
                            if (usuarioBD.Activo)
                            {
                                respuesta = new LoginResponse(usuarioBD, this.GenerateTokenJWT(prmLogin.Username));
                                var usrLogin = new Comun.Entidades.UsuarioPerfil.MODLoginUsuario { 
                                    IdUsuario = usuarioBD.IdUsuario,
                                    UserName = usuarioBD.UserName,
                                    IP = prmLogin.Ip,
                                    Token = respuesta.Token};
                                if (ConfiguracionNegocio.SessionUsuario(usrLogin, Comun.Enumeradores.EnumSession._inicio))
                                {
                                    usuarioNegocio.ActualizarUltimaFechaLogin(prmLogin.Username);
                                }
                                else
                                {
                                    respuesta = new LoginResponse(HttpStatusCode.BadRequest, "LOGIN.RTA006");
                                }
                                var aud = FabricaNegocio.CrearAuditoriaNegocio;
                                var usr = new MODLoginUsuario();
                                aud.CrearRastroAuditoria(Comun.Enumeradores.EnumTipoAccionAuditoria.login,usuarioBD.IdUsuario.ToString(),"Autenticacion",usr,usrLogin,prmLogin.Username,prmLogin.Ip);
                            }
                            else 
                            {
                                respuesta = new LoginResponse(HttpStatusCode.BadRequest, "LOGIN.RTA005");
                            }
                        }
                        else 
                        {
                            respuesta = new LoginResponse(HttpStatusCode.BadRequest, "LOGIN.RTA003");
                        }
                    }
                    else 
                    {
                        respuesta = new LoginResponse(HttpStatusCode.BadRequest, codigoMensaje);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex, "AutenticacionController.Login", JsonSerializer.Serialize(prmLogin), ErrorType.Error);
                respuesta = new LoginResponse(HttpStatusCode.InternalServerError, codigoMensaje);
            }

            return respuesta;
        }

        /// <summary>
        /// Metodo que se encarga de retirar el usuario de la lista de sessiones activas
        /// </summary>
        /// <param name="filtro">Usuario</param>
        /// <remarks>
        /// Cierre de sessión
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>              
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        [HttpPost]
        public void CerrarSession([FromBody] LoginRequestModel filtro)
        {
            if (!string.IsNullOrEmpty(filtro.Username))
            {
                var usrlogin = ConfiguracionNegocio.Logueos.Where(x => x.UserName == filtro.Username).FirstOrDefault();
                var aud = FabricaNegocio.CrearAuditoriaNegocio;
                var usr = new MODLoginUsuario();
                aud.CrearRastroAuditoria(Comun.Enumeradores.EnumTipoAccionAuditoria.logout, usrlogin.IdUsuario.ToString(), "Autenticacion", usr, usrlogin, filtro.Username, filtro.Ip);

                ConfiguracionNegocio.SessionUsuario(new Comun.Entidades.UsuarioPerfil.MODLoginUsuario
                {
                    UserName = filtro.Username,
                    IP = filtro.Ip
                }, Comun.Enumeradores.EnumSession._cierre);
            }
        }

        #region Metodos privados
        private string GenerateTokenJWT(string prmUsername)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, prmUsername),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_config["Jwt:ExpirationMinutes"])),
                signingCredentials: credentials);

            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodetoken;
        }
        #endregion
    }
}