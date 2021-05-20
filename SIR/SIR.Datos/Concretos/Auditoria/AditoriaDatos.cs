using System.Collections.Generic;
using SIR.Comun;
using SIR.Comun.Entidades.Auditoria;
using SIR.Datos.Interfaces.Auditoria;
using Dapper;
using SIR.Comun.Entidades.Genericas;
using SIR.Datos.Abstractos;
using System.Linq;
using System;
using SIR.Comun.Funcionalidades;

namespace SIR.Datos.Concretos.Auditoria{
    public class AuditoriaDatos :Dal_Base ,IAuditoriaDatos
    {
        public MODResultado CrearRastroAuditoria(MODRastroAuditoria rastro)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                var a = coneccion.Query("StpAuditoria", new
                {
                    accion = 3,
                    TipoAccionAuditoria = rastro.TipoAccionAuditoria,
                    ModeloDatos = rastro.ModeloDatos,
                    FechaAccion = rastro.FechaAccion,
                    CampoClaveID=rastro.CampoClaveID,
                    ValorAntes = rastro.ValorAntes,
                    ValorDespues=rastro.ValorDespues,
                    Cambios = rastro.Cambios,
                    Usuario = rastro.Usuario,
                    Ip = rastro.Ip
                },commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();
            }catch(Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(rastro), ErrorType.Error);
            }
            return resultado;
        }

        public List<MODRastroAuditoria> ObtenerRastroAuditoria()
        {
            List<MODRastroAuditoria> resultado;
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();                
                resultado = coneccion.Query<MODRastroAuditoria>("StpAuditoria",new {accion = 1}, commandType: System.Data.CommandType.StoredProcedure).ToList();                
            }
            catch(Exception e)
            {
                //Implementar Log
                resultado = null;
                Log.WriteLog(e,this.GetType().Namespace,"",ErrorType.Error);
            }
            return resultado;
        }

        public List<MODRastroAuditoria> ObtenerRastroAuditoriaPorId(int CampoClaveID,string modeloDatos)
        {
            
            List<MODRastroAuditoria> resultado;
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();                
                resultado = coneccion.Query<MODRastroAuditoria>("StpAuditoria",new {accion = 1,CampoClaveID = CampoClaveID,ModeloDatos = modeloDatos}, commandType: System.Data.CommandType.StoredProcedure).ToList();                
            }
            catch(Exception e)
            {
                //Implementar Log
                resultado = null;
                Log.WriteLog(e,this.GetType().Namespace,"",ErrorType.Error);
            }
            return resultado;
        }
        public List<MODRastroAuditoria> ObtenerRastroAuditoriaPor(MODFiltroAuditoria filtro){
            List<MODRastroAuditoria> resultado;
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();                
                resultado = coneccion.Query<MODRastroAuditoria>("StpAuditoria",new {
                    accion = 4,
                    fechaInicio = filtro.FechaInicio,
                    fechaFin = filtro.FechaFin,
                    Usuario = filtro.Usuario,
                    }, commandType: System.Data.CommandType.StoredProcedure).ToList();                
            }
            catch(Exception e)
            {
                //Implementar Log
                resultado = null;
                Log.WriteLog(e,this.GetType().Namespace,"",ErrorType.Error);
            }
            return resultado;
        }
    }
}