using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Parametrizacion_SIR;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Auditoria;
using SIR.Negocio.Interfaces.Parametrizacion_SIR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Concretos.Parametrizacion_SIR
{
    public class Cliente_ExcluidoNegocio : ICliente_ExcluidoNegocio
    {
        public MODResultado Borrar(MOD_Cliente_Excluido Cliente)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var context = FabricaDatos.CrearCliente_Excluido;
                var anterior = this.Obtener(new MOD_Cliente_ExcluidoFiltro { Id = Cliente.Id }).FirstOrDefault();
                context.Borrar(Cliente);
                /*********************auditoria*****************************/
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.borrar, Cliente.Id.ToString(), "Cliente Excluido", anterior, Cliente, Cliente.Usuario, Cliente.Ip);
                /**********************fin auditoria***********************/
            }
            catch (Exception e)
            {
                resultado.Errores.Add("ClI_EXC_ERROR_001");
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(Cliente), ErrorType.Error);
            }
            return resultado;
        }

        public MODResultado Modificar(MOD_Cliente_Excluido Cliente)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var context = FabricaDatos.CrearCliente_Excluido;
                var anterior = this.Obtener(new MOD_Cliente_ExcluidoFiltro { Id = Cliente.Id }).FirstOrDefault();
                context.Modificar(Cliente);
                /*********************auditoria*****************************/
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar, Cliente.Id.ToString(), "Cliente Excluido", anterior, Cliente, Cliente.Usuario, Cliente.Ip);
                /**********************fin auditoria***********************/
            }
            catch (Exception e)
            {
                resultado.Errores.Add("ClI_EXC_ERROR_001");
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(Cliente), ErrorType.Error);
            }
            return resultado;
        }

        public IEnumerable<MOD_Cliente_Excluido> Obtener(MOD_Cliente_ExcluidoFiltro filtro)
        {
            IEnumerable<MOD_Cliente_Excluido> resultado = new List<MOD_Cliente_Excluido>();
            try
            {
                var context = FabricaDatos.CrearCliente_Excluido;
                context.Obtener(filtro);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(filtro), ErrorType.Error);
            }
            return resultado;
        }

        public MODResultado Registrar(MOD_Cliente_Excluido Cliente)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var context = FabricaDatos.CrearCliente_Excluido;
                context.Modificar(Cliente);
                /*********************auditoria*****************************/
                IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear, Cliente.Id.ToString(), "Cliente Excluido", null, Cliente, Cliente.Usuario, Cliente.Ip);
                /**********************fin auditoria***********************/
            }
            catch (Exception e)
            {
                resultado.Errores.Add("ClI_EXC_ERROR_001");
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(Cliente), ErrorType.Error);
            }
            return resultado;
        }
    }
}
