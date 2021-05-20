using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Funcionalidades;
using SIR.Comun.Enumeradores;
using SIR.Datos.Fabrica;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Empresa;
using SIR.Negocio.Interfaces.Auditoria;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Concretos.Empresa
{
    public class EmpresasNegocio : IEmpresasNegocio
    {
        public MODResultado ActualizarEmpresa(MODEmpresa empresa)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                if (empresa.IdEmpresa != 0 && ConfiguracionNegocio.Empresas.Any(x => x.RazonSocial.ToUpper().Equals(empresa.RazonSocial.ToUpper()) && x.IdEmpresa != empresa.IdEmpresa && x.Activo))
                {
                    resultado.Errores.Add("EMPRESAS.ERROR.NOMBREREPETIDO");
                }
                else
                {
                    var data = FabricaDatos.CrearEmpresaDatos;
                    resultado = data.ActualizarEmpresa(empresa);
                    if (resultado.esValido)
                    {
                        /*********************auditoria*****************************/
                        MODEmpresa anterior = ConfiguracionNegocio.Empresas.Where(x => x.IdEmpresa == empresa.IdEmpresa).FirstOrDefault();
                        IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                        audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar, empresa.IdEmpresa.ToString(), "Empresas", anterior, empresa, empresa.Usuario, empresa.Ip);
                        /**********************fin auditoria***********************/

                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.servicios);
                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.empresas);
                    }
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(empresa), ErrorType.Error);
            }

            return resultado;
        }

        public MODResultado BorrarEmpresa(MODEmpresa empresa)
        {
            MODResultado resultado = new MODResultado();
            try
            {

                var data = FabricaDatos.CrearEmpresaDatos;
                resultado = data.BorrarEmpresa(empresa.IdEmpresa);
                if (resultado.esValido)
                {
                    /*********************auditoria*****************************/
                    MODEmpresa anterior = ConfiguracionNegocio.Empresas.Where(x => x.IdEmpresa == empresa.IdEmpresa).FirstOrDefault();
                    IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                    audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.borrar,empresa.IdEmpresa.ToString(), "Empresas", anterior, new MODEmpresa(), empresa.Usuario, empresa.Ip);
                    /**********************fin auditoria***********************/

                    ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.servicios);
                    ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.empresas);
                }

            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(empresa), ErrorType.Error);
            }

            return resultado;
        }

        //[RastroAuditoria(EnumTipoAccionAuditoria.crear)]
        public MODResultado CrearEmpresa(MODEmpresa empresa)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                if (empresa.IdEmpresa == 0 && ConfiguracionNegocio.Empresas.Any(x => x.RazonSocial.ToUpper().Equals(empresa.RazonSocial.ToUpper()) && x.Activo))
                {
                    resultado.Errores.Add("EMPRESAS.ERROR.NOMBREREPETIDO");
                }
                else
                {
                    var data = FabricaDatos.CrearEmpresaDatos;
                    resultado = data.CrearEmpresa(empresa);
                    if (resultado.esValido)
                    {

                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.servicios);
                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.empresas);
                        /*********************auditoria*****************************/
                        IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                        audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear, empresa.IdEmpresa.ToString(), "Empresas", new MODEmpresa(), empresa, empresa.Usuario, empresa.Ip);
                        /**********************fin auditoria***********************/
                    }
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(empresa), ErrorType.Error);
            }

            return resultado;
        }

        public MODEmpresa ObtenerEmpresaPorId(int idEmpresa)
        {

            return ConfiguracionNegocio.Empresas.Where(x => x.IdEmpresa == idEmpresa).FirstOrDefault();
        }

        public IEnumerable<MODEmpresa> ObtenerEmpresas()
        {
            return ConfiguracionNegocio.Empresas;
        }
    }
}
