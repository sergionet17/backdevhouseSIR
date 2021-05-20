using Dapper;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.Empresa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Datos.Concretos.Empresa
{
    public class EmpresaDatos : Dal_Base, IEmpresaDatos
    {        
        public MODResultado ActualizarEmpresa(MODEmpresa empresa)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                
                var a = coneccion.Query<string>("StpEmpresas", new
                {
                    accion=4,
                    IdEmpresa = empresa.IdEmpresa,
                    NumeroIdentificacion = empresa.NumeroIdentificacion,
                    RazonSocial = empresa.RazonSocial,
                    Correo = empresa.Correo,
                    Activo = empresa.Activo,
                    Descripcion = empresa.Descripcion,
                    servicios = string.Join(",", empresa.Servicios.Select(x => x.IdServicio))
                },commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();
                if(a!=null) resultado.Errores.Add(a);
                var b = coneccion.Query<string>("StpEmpresas",new {
                    accion=6,
                    IdEmpresa=empresa.IdEmpresa,
                    servicios = string.Join(",", empresa.Servicios.Select(x => x.IdServicio))},
                    commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();
                if(b!=null) resultado.Errores.Add(b); 
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(empresa),ErrorType.Error);
            }
            return resultado;
        }

        public MODResultado BorrarEmpresa(int idEmpresa)
        {
             MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                
                var a = coneccion.Query<string>("StpEmpresas", new
                {
                    accion=5,
                    IdEmpresa = idEmpresa
                },commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();
                if(a != null){
                    resultado.Errores.Add(a);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(new {idEmpresa=idEmpresa}),ErrorType.Error);
            }
            return resultado;
        }

        public MODResultado CrearEmpresa(MODEmpresa empresa)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                var a = coneccion.Query("StpEmpresas", new
                {
                    accion = 3,
                    IdEmpresa = empresa.IdEmpresa,
                    NumeroIdentificacion = empresa.NumeroIdentificacion,
                    RazonSocial = empresa.RazonSocial,
                    Correo = empresa.Correo,
                    Activo = empresa.Activo,
                    FechaCreacion = DateTime.Now,
                    Descripcion = empresa.Descripcion              
                },commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();                
                if(a != null && a.id != null){
                    empresa.IdEmpresa = a.id;
                    var b = coneccion.Query<string>("StpEmpresas",new {accion=6, IdEmpresa= empresa.IdEmpresa, servicios = string.Join(",", empresa.Servicios.Select(x => x.IdServicio))},
                        commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    if(b!=null) resultado.Errores.Add("EMPRESAS.ERRORES.SERVICIOS");
                }else{
                    resultado.Errores.Add("EMPRESAS.ERRORES.CREAR");
                }
            }catch(Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(empresa),ErrorType.Error);
            }
            return resultado;
        }

        public MODEmpresa ObtenerEmpresaPorId(int idEmpresa)
        {
            MODEmpresa resultado;
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                Dictionary<int, MODEmpresa> empresas = new Dictionary<int, MODEmpresa>();
                coneccion.Query<MODEmpresa, MODServicio, MODEmpresa>("StpEmpresas", (e, s) => {
                    MODEmpresa emp;
                    if(!empresas.TryGetValue(e.IdEmpresa,out emp))
                    {
                        emp = e;
                        emp.Servicios = new List<MODServicio>();
                        empresas.Add(e.IdEmpresa, emp);
                    }
                    if(s!=null) emp.Servicios.Add(s);
                    return e;
                }, new {accion = 2 ,idEmpresa = idEmpresa }, commandType: System.Data.CommandType.StoredProcedure);
                resultado = empresas[idEmpresa];
            }
            catch(Exception e)
            {
                //Implementar Log
                resultado = null;
                Log.WriteLog(e,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(new {idEmpresa=idEmpresa}),ErrorType.Error);
            }
            return resultado;
        }

        public IEnumerable<MODEmpresa> ObtenerEmpresas()
        {
            List<MODEmpresa> resultado;
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                Dictionary<int, MODEmpresa> empresas = new Dictionary<int, MODEmpresa>();
                coneccion.Query<MODEmpresa, MODServicio, MODEmpresa>("StpEmpresas", (e, s) => {
                    MODEmpresa emp;
                    if (!empresas.TryGetValue(e.IdEmpresa, out emp))
                    {
                        emp = e;
                        emp.Servicios = new List<MODServicio>();
                        empresas.Add(e.IdEmpresa, emp);
                    }
                    if(s!=null) emp.Servicios.Add(s);
                    return e;
                }, new { accion = 1 },splitOn:"IdServicio", commandType: System.Data.CommandType.StoredProcedure);
                resultado = empresas.Values.ToList();
            }
            catch (Exception e)
            {
                //Implementar Log
                resultado = null;
                Log.WriteLog(e,this.GetType().Namespace,"",ErrorType.Error);
            }
            return resultado;
        }
    }
}
