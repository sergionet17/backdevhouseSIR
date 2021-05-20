using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Funcionalidades;
using SIR.Negocio.Abstractos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    class HomologarNegocio : PasoBase
    {
        public override void Configurar(MODFlujo flujo) { }
        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                ExprParser ep = new ExprParser();
                LambdaExpression lambda = null;
                List<string> parametros_valores = null;

                foreach (var registro in tarea.Value.Registros)
                {
                    foreach (var _homolg in tarea.Value.Homologaciones)
                    {
                        if (_homolg.TipoReemplazo == Comun.Enumeradores.FlujoDeTrabajo.EnumTipoReemplazo.Constante)
                        {
                            registro[_homolg.NombreCampo] = ConvertirObjeto(_homolg.TipoCampo, _homolg.ValorSi);
                        }
                        else if (_homolg.TipoReemplazo == Comun.Enumeradores.FlujoDeTrabajo.EnumTipoReemplazo.Variable)
                        {
                            parametros_valores = new List<string>();
                            foreach (string _nombreCampo in _homolg.Condiciones.Select(y => y.Campo).Distinct()) { parametros_valores.Add(registro[_nombreCampo].ToString()); }
                             
                            lambda = ep.Parse(_homolg.Ecuacion);
                            if ((bool)ep.Run(lambda, parametros_valores.ToArray()))
                            {
                                registro[_homolg.NombreCampo] = ConvertirObjeto(_homolg.TipoCampo, _homolg.ValorSi);
                            }
                            else
                            {
                                string _posiblevalor = _homolg.ValorNo.Equals("@valorOriginal") ? registro[_homolg.NombreCampo].ToString() : _homolg.ValorNo;
                                registro[_homolg.NombreCampo] = ConvertirObjeto(_homolg.TipoCampo, _posiblevalor);
                            }
                        }
                        
                    }
                }
                AisgnarRegistros(ref tarea);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.FlujoDeTrabajo.HomologarNegocio - Ejecutar",
                             String.Format(@"tarea:{0} - reporte:{1}", System.Text.Json.JsonSerializer.Serialize(tarea),
                             System.Text.Json.JsonSerializer.Serialize(reporte)),
                             ErrorType.Error);
                resultado.Errores.Add("COMUNES.ERRORSERVICIO");
            }
            return resultado;
        }
    }
}
