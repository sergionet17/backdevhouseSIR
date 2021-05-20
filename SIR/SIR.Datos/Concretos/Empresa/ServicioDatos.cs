using Dapper;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Empresas;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Abstractos;
using SIR.Datos.Interfaces.Empresa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Datos.Concretos.Empresa
{
    public class ServicioDatos : Dal_Base, IServicioDatos
    {
        public MODResultado ActualizarServicio(MODServicio servicio)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                var serv = coneccion.Query("StpServicios",
                    new {
                        accion = 4,
                        IdServicio = servicio.IdServicio,
                        Nombre = servicio.Nombre,
                        Descripcion = servicio.Descripcion
                    },commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();

                if(serv == null){
                   var a = coneccion.Query("StpServicios",new {accion=6,IdServicio = servicio.IdServicio,empresas=string.Join(",",servicio.Empresas.Select(x=>x.IdEmpresa))},commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();
                   if(a != null) resultado.Errores.Add("SERVICIOS.ERRORES.EMPRESAS");
                }else{
                    resultado.Errores.Add("SERVICIOS.ERRORES.CREAR");
                }                
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(servicio),ErrorType.Error);
            }
            return resultado;
        }

        public MODResultado BorrarServicio(int idServicio)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                var serv = coneccion.Query<string>("StpServicios",
                    new {
                        accion = 5,
                        IdServicio = idServicio                        
                    },commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();                        
                if(serv != null){
                    resultado.Errores.Add(serv);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(new {idServicio=idServicio}),ErrorType.Error);
            }
            return resultado;
        }

        public MODResultado CrearServicio(MODServicio servicio)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                var ri = coneccion.Query("StpServicios",new {
                        accion = 3,
                        IdServicio = servicio.IdServicio,
                        Nombre = servicio.Nombre,
                        Descripcion = servicio.Descripcion
                    },commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();

                if(ri!=null && ri.id != null){
                    coneccion.Query("StpServicios",new {
                        accion=6,
                        idServicio=ri.id,
                        empresas=string.Join(",",servicio.Empresas.Select(x=>x.IdEmpresa))},commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(servicio),ErrorType.Error);
            }
            return resultado;
        }

        public MODServicio ObtenerServicioPorId(int idServicio)
        {
            MODServicio resultado;
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                Dictionary<int, MODServicio> servicios = new Dictionary<int, MODServicio>();
                coneccion.Query<MODServicio, MODEmpresa, MODServicio>("StpServicios", (e, s) => {
                    MODServicio ser;
                    if(!servicios.TryGetValue(e.IdServicio,out ser))
                    {
                        ser = e;
                        ser.Empresas = new List<MODEmpresa>();
                        servicios.Add(e.IdServicio, ser);
                    }
                    ser.Empresas.Append(s);
                    return e;
                }, new {accion = 2 ,idServicio = idServicio },splitOn:"IdEmpresa", commandType: System.Data.CommandType.StoredProcedure);
                resultado = servicios[idServicio];
            }
            catch(Exception exp)
            {
                resultado = null;
                Log.WriteLog(exp,this.GetType().Namespace,Newtonsoft.Json.JsonConvert.SerializeObject(new {idServicio=idServicio}),ErrorType.Error);
            }
            return resultado;
        }

        public IEnumerable<MODServicio> ObtenerServicios()
        {
            IEnumerable<MODServicio> resultado;
            try
            {
                var coneccion = this.ObtenerConexionPrincipal();
                Dictionary<int, MODServicio> servicios = new Dictionary<int, MODServicio>();
                coneccion.Query<MODServicio, MODEmpresa, MODServicio>("StpServicios", (e, s) => {
                    MODServicio ser;
                    if(!servicios.TryGetValue(e.IdServicio,out ser))
                    {
                        ser = e;
                        ser.Empresas = new List<MODEmpresa>();
                        servicios.Add(e.IdServicio, ser);
                    }
                    if(s!=null)((List<MODEmpresa>)ser.Empresas).Add(s);
                    return e;
                }, new {accion = 1 },splitOn:"IdEmpresa", commandType: System.Data.CommandType.StoredProcedure);
                resultado = servicios.Values;
            }
            catch(Exception exp)
            {
                resultado = null;
                Log.WriteLog(exp,this.GetType().Namespace,"",ErrorType.Error);
            }
            return resultado;
 
        }

        public List<MODEmpresaServicio> ObtenerServiciosPorEmpresa(int prmIdEmpresa) 
        {
            List<MODEmpresaServicio> servicios = null;

            using (var conn = ObtenerConexionPrincipal())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@prmIdEmpresa", prmIdEmpresa, System.Data.DbType.Int32);

                var reader = conn.QueryMultiple("StpConsultarServiciosxEmpresa", parametros, null, int.MaxValue, System.Data.CommandType.StoredProcedure);

                servicios = reader.Read<MODEmpresaServicio>().ToList();

                conn.Close();
            }

            return servicios;
        }
    }
}