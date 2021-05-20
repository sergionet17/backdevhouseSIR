using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Empresas;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Datos.Interfaces.Empresa;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Auditoria;
using SIR.Negocio.Interfaces.Empresa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Concretos.Empresa
{
    public class ServicioNegocio : IServicioNegocio
    {
        public MODResultado ActualizarServicio(MODServicio servicio)
        {
              MODResultado resultado = new MODResultado();            
            try
            {
                if (servicio.IdServicio == 0 && ConfiguracionNegocio.Servicios.Any(x => x.Descripcion.ToUpper().Equals(servicio.Descripcion.ToUpper()) && x.IdServicio != servicio.IdServicio))
                {
                    resultado.Errores.Add("SERVICIOS.ERROR.NOMBREREPETIDO");
                }
                else
                {
                    var data = FabricaDatos.CrearServicioDatos;
                    resultado = data.ActualizarServicio(servicio);
                    if (resultado.esValido)
                    {
                        /*********************auditoria*****************************/
                        MODServicio anterior = ConfiguracionNegocio.Servicios.Where(x=>x.IdServicio == servicio.IdServicio).FirstOrDefault();
                        IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                        object p = audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar,servicio.IdServicio.ToString(),"Servicios",anterior,servicio,servicio.Usuario,servicio.Ip);
                        /**********************fin auditoria***********************/
                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.servicios);
                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.empresas);    
                        /*********************auditoria*****************************/

                    }
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().FullName,Newtonsoft.Json.JsonConvert.SerializeObject(servicio),ErrorType.Error);              
            }

            return resultado;
        }

        public MODResultado BorrarServicio(MODServicio servicio)
        {
               MODResultado resultado = new MODResultado();            
            try
            {
                
                    var data = FabricaDatos.CrearServicioDatos;
                    resultado = data.BorrarServicio(servicio.IdServicio);
                    if (resultado.esValido)
                    {
                        /*********************auditoria*****************************/
                        MODServicio anterior = ConfiguracionNegocio.Servicios.Where(x=>x.IdServicio == servicio.IdServicio).FirstOrDefault();
                        IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                        object p = audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.borrar,servicio.IdServicio.ToString(),"Servicios",anterior,new MODServicio(),servicio.Usuario,servicio.Ip);
                        /**********************fin auditoria***********************/
                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.servicios);
                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.empresas);    
                        /*********************auditoria*****************************/
                    }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().FullName,Newtonsoft.Json.JsonConvert.SerializeObject(servicio),ErrorType.Error);              
            }

            return resultado;
        }

        public MODResultado CrearServicio(MODServicio servicio)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                if(servicio.IdServicio == 0 && ConfiguracionNegocio.Servicios.Any(x => x.Descripcion.ToUpper().Equals(servicio.Descripcion.ToUpper())))
                {
                    resultado.Errores.Add("SERVICIOS.ERROR.NOMBREREPETIDO");
                }
                else
                {
                    var data = FabricaDatos.CrearServicioDatos;
                    resultado = data.CrearServicio(servicio);
                    if(resultado.esValido)
                    {
                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.servicios);
                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.empresas);    
                        /*********************auditoria*****************************/                        
                        IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                        object p = audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear,servicio.IdServicio.ToString(),"Servicios",new MODServicio(),servicio,servicio.Usuario,servicio.Ip);
                        /**********************fin auditoria***********************/
                    }
                }
            }
            catch (Exception e) {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().FullName,Newtonsoft.Json.JsonConvert.SerializeObject(servicio),ErrorType.Error);
            }

            return resultado;
        }

        public MODServicio ObtenerServicioPorId(int idServicio)
        {
            return ConfiguracionNegocio.Servicios.Where(x=>x.IdServicio == idServicio).FirstOrDefault();
        }

        public IEnumerable<MODServicio> ObtenerServicios()
        {
            return ConfiguracionNegocio.Servicios;
        }

        public List<MODEmpresaServicio> ObtenerServiciosPorEmpresa(int prmIdEmpresa) 
        {
            IServicioDatos servicioDatos = FabricaDatos.CrearServicioDatos;

            return servicioDatos.ObtenerServiciosPorEmpresa(prmIdEmpresa);
        }
    }
}