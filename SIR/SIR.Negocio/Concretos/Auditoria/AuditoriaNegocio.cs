using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using SIR.Comun.Entidades.Auditoria;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Enumeradores;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Datos.Interfaces.Auditoria;
using SIR.Negocio.Interfaces.Auditoria;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SIR.Negocio.Concretos.Auditoria
{
    public class AuditoriaNegocio : IAuditoriaNegocio
    {
        public MODResultado CrearRastroAuditoria(EnumTipoAccionAuditoria Accion, string CampoClaveID, string modeloDatos, Object AnteriorObject, Object NuevoObject, string Usuario, string ip)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                // get the differance  
                CompareLogic compObjects = new CompareLogic();
                compObjects.Config.MaxDifferences = 99;
                compObjects.Config.TreatStringEmptyAndNullTheSame = true;
                compObjects.Config.MaxStructDepth = 5;
                ComparisonResult compResult = compObjects.Compare(AnteriorObject, NuevoObject);
                List<DeltaAuditoria> DeltaList = new List<DeltaAuditoria>();
                foreach (var change in compResult.Differences)
                {
                    DeltaAuditoria delta = new DeltaAuditoria();
                    if (change.PropertyName != "" && change.PropertyName.Substring(0, 1) == ".") delta.NombreCampo = change.PropertyName.Substring(1, change.PropertyName.Length - 1);
                    else delta.NombreCampo = change.PropertyName;
                    delta.ValorAntes = change.Object1Value;
                    delta.ValorDespues = change.Object2Value;
                    DeltaList.Add(delta);
                }
                MODRastroAuditoria audit = new MODRastroAuditoria();
                audit.TipoAccionAuditoria = Accion;
                audit.ModeloDatos = modeloDatos;
                audit.FechaAccion = DateTime.Now;
                audit.CampoClaveID = CampoClaveID;
                audit.ValorAntes = JsonConvert.SerializeObject(AnteriorObject);
                audit.ValorDespues = JsonConvert.SerializeObject(NuevoObject);
                audit.Cambios = JsonConvert.SerializeObject(DeltaList);
                audit.Usuario = Usuario;
                IAuditoriaDatos dal = FabricaDatos.CrearAuditoriaDatos;
                resultado = dal.CrearRastroAuditoria(audit);
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().Namespace, Newtonsoft.Json.JsonConvert.SerializeObject(new { Accion = Accion, CampoClaveID = CampoClaveID, modeloDatos = modeloDatos, AnteriorObject = AnteriorObject, NuevoObject = NuevoObject }), ErrorType.Error);
            }
            return resultado;
        }

        public List<MODRastroAuditoria> ObtenerRastroAuditoriaPorId(int CampoClaveID, string ModeloDatos)
        {
            List<MODRastroAuditoria> rslt = new List<MODRastroAuditoria>();
            try
            {
                IAuditoriaDatos dal = FabricaDatos.CrearAuditoriaDatos;
                rslt = dal.ObtenerRastroAuditoriaPorId(CampoClaveID, ModeloDatos);                
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName, Newtonsoft.Json.JsonConvert.SerializeObject(new { CampoClaveID = CampoClaveID, ModeloDatos = ModeloDatos }), ErrorType.Error);
            }
            return rslt;
        }
        public List<MODRastroAuditoria> ObtenerRastroAuditoriaPor(MODFiltroAuditoria filtro)
        {
            List<MODRastroAuditoria> rslt = new List<MODRastroAuditoria>();
            try
            {
                IAuditoriaDatos dal = FabricaDatos.CrearAuditoriaDatos;
                rslt = dal.ObtenerRastroAuditoriaPor(filtro);                
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName, Newtonsoft.Json.JsonConvert.SerializeObject(filtro), ErrorType.Error);
            }
            return rslt;
        }
    }
}