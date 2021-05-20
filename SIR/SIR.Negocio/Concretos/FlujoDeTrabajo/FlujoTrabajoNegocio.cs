using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Enumeradores;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Auditoria;
using SIR.Negocio.Interfaces.FlujoDeTrabajo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    public class FlujoTrabajoNegocio : IFlujoTrabajoNegocio
    {
        #region Metodos Publicos
        public MODResultado CrearCategoria(MODCategoria categoria)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                if (categoria.Id == 0 && ConfiguracionNegocio.CategoriasFlujos.Any(x => x.Nombre.ToUpper().Equals(categoria.Nombre.ToUpper()) && x.Activo))
                {
                    resultado.Errores.Add("EMPRESAS.ERROR.NOMBREREPETIDO");
                }
                else
                {
                    var data = FabricaDatos.CrearFlujoTrabajoDatos;
                    resultado = data.CrearCategoria(categoria);
                    if (resultado.esValido)
                    {

                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.categoriasFlujos);
                        /*********************auditoria*****************************/
                        IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                        MODResultado mODResultado = audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear, categoria.Id.ToString(), "Categorias flujos", new MODCategoria(), categoria, categoria.Usuario, categoria.Ip);
                        /**********************fin auditoria***********************/
                    }
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().FullName, Newtonsoft.Json.JsonConvert.SerializeObject(categoria), ErrorType.Error);
            }

            return resultado;
        }

        public MODResultado EditarCategoria(MODCategoria categoria)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                if (categoria.Id != 0 && ConfiguracionNegocio.CategoriasFlujos.Any(x => x.Nombre.ToUpper().Equals(categoria.Nombre.ToUpper()) && x.Id != categoria.Id && x.Activo))
                {
                    resultado.Errores.Add("EMPRESAS.ERROR.NOMBREREPETIDO");
                }
                else
                {
                    var data = FabricaDatos.CrearFlujoTrabajoDatos;
                    resultado = data.EditarCategoria(categoria);
                    if (resultado.esValido)
                    {
                        /*********************auditoria*****************************/
                        MODCategoria anterior = ConfiguracionNegocio.CategoriasFlujos.Where(x => x.Id == categoria.Id).FirstOrDefault();
                        IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                        audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.actualizar, categoria.Id.ToString(), "Categorias Flujos", anterior, categoria, categoria.Usuario, categoria.Ip);
                        /**********************fin auditoria***********************/

                        ConfiguracionNegocio.RefrescarConfiguracion(Comun.Enumeradores.EnumTipoConfiguracion.categoriasFlujos);
                    }
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add(e.Message);
                Log.WriteLog(e, this.GetType().FullName, Newtonsoft.Json.JsonConvert.SerializeObject(categoria), ErrorType.Error);
            }

            return resultado;
        }

        public MODResultado Ejecutar(MODFlujoFiltro filtro)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var _flujo = ConfiguracionNegocio.Flujos.FirstOrDefault(y =>
                    y.IdEmpresa == filtro.IdEmpresa &&
                    y.IdServicio == filtro.IdServicio &&
                    y.IdElemento == filtro.IdElemento &&
                    y.Tipo == filtro.TipoFlujo);
                if (_flujo != null)
                {
                    var _flujohistorico = FabricaDatos.CrearFlujoTrabajoDatos;
                    _flujo.Periodo = filtro.Periodo;
                    _flujo.DatoPeriodo = filtro.DatoPeriodo;

                    var _historico = new MODFlujoHistorico
                    {
                        IdEmpresa = _flujo.IdEmpresa,
                        IdServicio = _flujo.IdServicio,
                        IdElemento = _flujo.IdElemento,
                        TipoFlujo = _flujo.Tipo,
                        IdFlujo = _flujo.Id,
                        Periodicidad = _flujo.Periodicidad,
                        EstadoFlujo = EnumEstado.Ejecutando,
                        Periodo = FijarPeriodoPorPeriodicidad(_flujo.Periodo, _flujo.Periodicidad, _flujo.DatoPeriodo)
                    };
                    var prerequisitos = _flujohistorico.conPrerequisito(filtro);
                    if (prerequisitos.Count() > 0)
                    {
                        resultado.DatosAdicionales = prerequisitos.ToDictionary(x => x, x => x);
                        resultado.Errores.Add("FLUJOS.ERRORES.FLU001");
                        return resultado;
                    }
                    for (LinkedListNode<MODTarea> _tarea = _flujo.Tareas.First; _tarea != null;)
                    {
                        _historico.IdTarea = _tarea.Value.Id;
                        _historico.Proceso = _tarea.Value.Proceso;
                        _historico.FlujoFechaCreacion = DateTime.Now;
                        _historico.TareaFechaCreacion = DateTime.Now;
                        _tarea.Value.Periodo = _flujo.Periodo;
                        _tarea.Value.Periodicidad = _flujo.Periodicidad;
                        _tarea.Value.IdElemento = _flujo.IdElemento;
                        try
                        {
                            if (_tarea.Value.IdGrupoEjecucion == filtro.IdGrupoEjecucion && resultado.esValido)
                            {

                                _flujohistorico.Historico(ref _historico, Comun.Enumeradores.EnumAccionBaseDatos.Insertar);

                                var paso = GenerarInstancia(_tarea.Value.Proceso);
                                paso.Configurar(_flujo);
                                resultado = paso.Ejecutar(ref _tarea, _tarea.Value.Reporte, _tarea.Value.Archivo);

                                _historico.TareaFechaFinalizacion = DateTime.Now;
                                _historico.TareaEsValido = resultado.esValido;
                                _historico.EstadoFlujo = resultado.esValido ? EnumEstado.Ejecutando : EnumEstado.Error;
                                _historico.DescripcionError = string.Join('-', resultado.Errores);
                                if (_tarea.Value.Proceso == EnumProceso.Finalizar)
                                    _historico.EstadoFlujo = EnumEstado.Ok;

                                _flujohistorico.Historico(ref _historico, Comun.Enumeradores.EnumAccionBaseDatos.Actualizar);
                                if (filtro.Notificar != null)
                                {
                                    var ninfo = new { 
                                        proceso = _tarea.Value.Proceso, 
                                        odata = (_tarea.Value.ConfiguracionBD!=null)?_tarea.Value.ConfiguracionBD.Nombre:"",
                                        desc = _tarea.Value.Reporte.Descripcion
                                    };                                    
                                    filtro.Notificar(filtro.Usuario,JsonSerializer.Serialize(ninfo));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog(e, "SIR.Negocio.Concretos.FlujoDeTrabajo.FlujoTrabajoNegocio - Paso",
                                JsonSerializer.Serialize(_tarea.Value),
                                ErrorType.Error);
                            resultado.Errores.Add(e.Message);
                            _historico.TareaFechaFinalizacion = DateTime.Now;
                            _historico.FlujoFechaFinalizacion = DateTime.Now;
                            _historico.TareaEsValido = false;
                            _historico.DescripcionError = e.Message;
                            _historico.EstadoFlujo = EnumEstado.Error;
                            _flujohistorico.Historico(ref _historico, Comun.Enumeradores.EnumAccionBaseDatos.Actualizar);
                        }
                        _tarea = _tarea.Next;
                    }
                }
                else { resultado.Errores.Add("FLUJO.NOEXISTENTE"); }

            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.FlujoDeTrabajo.FlujoTrabajoNegocio - Ejecutar",
                    JsonSerializer.Serialize(filtro),
                    ErrorType.Error);
                resultado.Errores.Add(e.Message);
            }
            return resultado;
        }

        public MODResultado Registrar(MODFlujo registro)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                registro.Tareas.ToList().ForEach(x =>
                {
                    if (x.IdReporte != 0)
                    {
                        x.Reporte = ConfiguracionNegocio.Reportes.FirstOrDefault(y => y.Id == x.IdReporte);
                    }
                    if (x.Proceso == EnumProceso.Obtener)
                    {
                        x.NombreTablaSIR = string.Format(@"TB_{0}_{1}_{2}", registro.Elemento.Replace(" ", "_").ToUpper(), registro.NombreEmpresa.Replace(" ", "_").ToUpper(), !String.IsNullOrEmpty(x.ConfiguracionBD.NombreTabla) ? x.ConfiguracionBD.NombreTabla.Replace(" ", "_").ToUpper() : x.ConfiguracionBD.Nombre.Replace(" ", "_").ToUpper());
                    }
                    else if (x.Proceso == EnumProceso.Registrar)
                    {
                        x.NombreTablaSIR = string.Format(@"TB_{0}_{1}_{2}", registro.Elemento.Replace(" ", "_").ToUpper(),
                            registro.NombreEmpresa.Replace(" ", "_").ToUpper(),
                             x.Reporte.Nombre.Replace(" ", "_").ToUpper()
                            );
                    }
                });
                resultado = data.Registrar(registro);
                if (resultado.esValido)
                {
                    ConfiguracionNegocio.RefrescarConfiguracion(EnumTipoConfiguracion.flujo);
                    /*********************auditoria*****************************/
                    IAuditoriaNegocio audit = FabricaNegocio.CrearAuditoriaNegocio;
                    MODResultado mODResultado = audit.CrearRastroAuditoria(EnumTipoAccionAuditoria.crear,
                        registro.codigo_Externo.ToString(), "flujos", new MODFlujo(), registro, registro.Usuario, registro.Ip);
                    /**********************fin auditoria***********************/
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.FlujoDeTrabajo.FlujoTrabajoNegocio - Registrar",
                    JsonSerializer.Serialize(registro),
                    ErrorType.Error);
                resultado.Errores.Add(e.Message);
            }
            return resultado;
        }

        public List<MODFlujo> Obtener(MODFlujoFiltro filtro)
        {
            List<MODFlujo> resultado = new List<MODFlujo>();
            try
            {
                resultado = ConfiguracionNegocio.Flujos;
                if (filtro.IdServicio != 0)
                    resultado = resultado.Where(y => y.IdServicio == filtro.IdServicio).ToList();
                if (filtro.IdEmpresa != 0)
                    resultado = resultado.Where(y => y.IdEmpresa == filtro.IdEmpresa).ToList();
                if (filtro.Id != 0)
                    resultado = resultado.Where(y => y.Id == filtro.Id).ToList();
                if (!string.IsNullOrEmpty(filtro.Nombre))
                    resultado = resultado.Where(y => y.Nombre.Contains(filtro.Nombre)).ToList();

                resultado.ForEach(y => y.Categoria = y.IdCategoria == 0 ? "" : ConfiguracionNegocio.CategoriasFlujos.FirstOrDefault(x => x.Id == y.IdCategoria).Nombre);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.Reportes - Obtener",
                             String.Format(@"filtro:{0}", System.Text.Json.JsonSerializer.Serialize(filtro)),
                             ErrorType.Error);
            }
            return resultado;
        }

        public List<IDictionary<string, object>> Consultar(MODFlujoFiltro filtro)
        {
            try
            {
                var _tarea = ConfiguracionNegocio.Flujos.FirstOrDefault(y =>
                   y.IdEmpresa == filtro.IdEmpresa &&
                   y.IdServicio == filtro.IdServicio &&
                   y.IdElemento == filtro.IdElemento).Tareas.FirstOrDefault(y => y.Id == filtro.IdTarea);
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@version", filtro.Version);
                parametros.Add("@periodoSIR", filtro.StrPeriodo);

                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                return data.Consultar(_tarea.Reporte.campos, _tarea.NombreTablaSIR, parametros);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.FlujoDeTrabajo.FlujoTrabajoNegocio - Consultar",
                    JsonSerializer.Serialize(filtro),
                    ErrorType.Error);
            }
            return new List<IDictionary<string, object>>();
        }
        #endregion


        #region Metodos Privados
        private static IPasos GenerarInstancia(EnumProceso _proceso)
        {
            switch (_proceso)
            {
                case EnumProceso.Obtener:
                    return new ObtenerNegocio();
                case EnumProceso.Registrar:
                    return new RegistrarNegocio();
                case EnumProceso.Validar:
                    return new ValidarNegocio();
                case EnumProceso.Homologar:
                    return new HomologarNegocio();
                case EnumProceso.Ejecutar:
                    return new EjecutarNegocio();
                case EnumProceso.Confirmar:
                    return new ConfirmarNegocio();
                case EnumProceso.Notificar:
                    return new NotificarNegocio();
                case EnumProceso.Archivo:
                    return new ArchivoNegocio();
                case EnumProceso.Combinacion:
                    return new CombinacionNegocio();
                case EnumProceso.Finalizar:
                    return new FinalizarFlujoNegocio();
                default:
                    return null;
            }
        }
        public MODResultado ProbarConeccion(MODOrigenDatos origen)
        {
            var data = FabricaDatos.CrearFlujoTrabajoDatos;
            return data.ProbarConeccion(origen);
        }

        public IEnumerable<MODGruposDeEjecucion> ObtenerPasos(MODFlujoFiltro filtro)
        {
            IEnumerable<MODGruposDeEjecucion> resultado = new List<MODGruposDeEjecucion>();
            try
            {
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                return data.ObtenerPasos(filtro);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - ObtenerPasos",
                             String.Format(@"filtro:{0}", System.Text.Json.JsonSerializer.Serialize(filtro)),
                             ErrorType.Error);
            }
            return resultado;
        }
        public MODResultado CrearFlujo(MODFlujo mod)
        {
            MODResultado resultado = new MODResultado();
            try
            {
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                return data.Registrar(mod);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - CrearFlujo",
                             String.Format(@"flujo:{0}", System.Text.Json.JsonSerializer.Serialize(mod)),
                             ErrorType.Error);
            }
            return resultado;
        }

        public List<MODGruposDeEjecucion> ObtenerGrupos(int idCategoria)
        {
            List<MODGruposDeEjecucion> resultado = new List<MODGruposDeEjecucion>();
            try
            {
                resultado = ConfiguracionNegocio.GruposDeEjecucion.Where(x => x.IdCategoria == idCategoria).ToList();
            }
            catch (Exception e)
            {
                Log.WriteLog(e, "SIR.Negocio.Concretos.Reportes - Obtener",
                             String.Format(@"idCategoria:{0}", System.Text.Json.JsonSerializer.Serialize(idCategoria)), ErrorType.Error);

            }
            return resultado;
        }
        public List<MODGruposDeEjecucion> ObtenerGruposEjecucion(int prmIdCategoria)
        {
            List<MODGruposDeEjecucion> resultado = new List<MODGruposDeEjecucion>();
            try
            {
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                return data.ObtenerGruposEjecucion(prmIdCategoria);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - ObtenerPasos",
                             String.Format(@"IdCategoria:{0}", System.Text.Json.JsonSerializer.Serialize(prmIdCategoria)),
                             ErrorType.Error);
            }
            return resultado;
        }

        public Dictionary<int, string> ObtenerVersiones(MODFlujoFiltro filtro)
        {
            Dictionary<int, string> retorno = new Dictionary<int, string>();
            try
            {
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                return data.ObtenerVersiones(filtro);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - ObtenerPasos",
                             String.Format(@"filtro:{0}", System.Text.Json.JsonSerializer.Serialize(filtro)),
                             ErrorType.Error);
            }
            return retorno;
        }

        public List<object> ObtenerOrigenes(MODFlujoFiltro filtro)
        {
            IEnumerable<MODTarea> tareas = new List<MODTarea>();
            MODTarea tarea = ConfiguracionNegocio.Flujos.SelectMany(x => x.Tareas, (x, y) => y).Where(x => x.Id == filtro.IdTarea).FirstOrDefault();

            tareas = ConfiguracionNegocio.Flujos.Where(x => x.IdEmpresa == filtro.IdEmpresa && x.IdServicio == filtro.IdServicio && x.IdElemento == filtro.IdElemento).SelectMany(x => x.Tareas, (x, y) => y);
            tareas = tareas.Where(x => x.Proceso == EnumProceso.Obtener || x.Proceso == EnumProceso.Registrar);
            tareas = tareas.Where(x => x.Orden < tarea.Orden);
            return tareas.Select(x =>
            {
                object y = null;
                switch (x.Proceso)
                {
                    case EnumProceso.Obtener:
                        y = new { id = x.Id, Nombre = x.Reporte.Descripcion, campos = x.Reporte.campos };
                        break;
                    case EnumProceso.Registrar:
                        y = new { id = x.Id, Nombre = x.Reporte.Descripcion, campos = x.Reporte.campos };
                        break;
                }
                return y;
            }).ToList();
        }

        public List<MODFlujo> ObtonerFlujo(MODFlujoFiltro filtro)
        {
            throw new NotImplementedException();
        }

        public List<MODOpciones> ObtenerOpciones(MODFlujoFiltro filtro)
        {
            List<MODOpciones> resultado = new List<MODOpciones>();
            try
            {
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                return data.ObtenerOpciones(filtro);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - ObtenerOpciones",
                             String.Format(@"filtro:{0}", System.Text.Json.JsonSerializer.Serialize(filtro)),
                             ErrorType.Error);
            }
            return resultado;
        }
        public List<MODOpciones> ObtenerAcciones()
        {
            List<MODOpciones> result = ConfiguracionNegocio.Acciones;
            return result;
        }

        public List<object> ObtenerOrigenesPor(MODFlujoFiltro filtro)
        {
            IEnumerable<MODTarea> tareas = new List<MODTarea>();
            MODTarea tarea = ConfiguracionNegocio.Flujos.SelectMany(x => x.Tareas, (x, y) => y).Where(x => x.Id == filtro.IdTarea).FirstOrDefault();
            tareas = ConfiguracionNegocio.Flujos.Where(x => x.IdEmpresa == filtro.IdEmpresa && x.IdServicio == filtro.IdServicio && x.IdElemento == filtro.IdElemento).SelectMany(x => x.Tareas, (x, y) => y);
            tareas = tareas.Where(x => x.Proceso == EnumProceso.Obtener || x.Proceso == EnumProceso.Registrar);
            tareas = tareas.Where(x => x.IdGrupoEjecucion == tarea.IdGrupoEjecucion);
            return tareas.Select(x =>
            {
                object y = null;
                switch (x.Proceso)
                {
                    case EnumProceso.Obtener:
                        y = new { id = x.Id, Nombre = x.ConfiguracionBD.Nombre, campos = x.Reporte.campos, idGrupoEjecucion = x.IdGrupoEjecucion };
                        break;
                    case EnumProceso.Registrar:
                        y = new { id = x.Id, Nombre = x.Reporte.Nombre, campos = x.Reporte.campos, idGrupoEjecucion = x.IdGrupoEjecucion };
                        break;
                }
                return y;
            }).ToList();
        }
        public MODResultado GuardarConf(List<Dictionary<string, object>> lista)
        {
            MODResultado res = new MODResultado();
            try
            {
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                res = data.GuardarConf(lista);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - ObtenerPasos",
                             String.Format(@"Lista:{0}", System.Text.Json.JsonSerializer.Serialize(lista)),
                             ErrorType.Error);
            }
            return res;
        }

        public List<MODOpciones> ObtenerAccionesSeleccionadas(MODFlujoFiltro filtro)
        {
            List<MODOpciones> resultado = new List<MODOpciones>();
            try
            {
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                return data.ObtenerAccionesSeleccionadas(filtro);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - ObtenerOpciones",
                             String.Format(@"filtro:{0}", System.Text.Json.JsonSerializer.Serialize(filtro)),
                             ErrorType.Error);
            }
            return resultado;
        }

        public MODResultado GuardarCorrecciones(List<Dictionary<string, object>> lista)
        {
            MODResultado res = new MODResultado();
            try
            {
                var IdTarea = Convert.ToInt32(lista[0]["id_tarea"].ToString());
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                MODTarea tarea = ConfiguracionNegocio.Flujos.SelectMany(x => x.Tareas, (x, y) => { return y; }).Where(x => x.Id == IdTarea).FirstOrDefault();
                List<string> campos = new List<string>();
                if (!tarea.Reporte.campos.Any(x => x.Nombre.ToUpper() == "F_ALTA"))
                {
                    lista.ForEach(x => x.Remove("f_alta"));
                }
                else
                {
                    campos.Add("F_ALTA");
                }
                if (!tarea.Reporte.campos.Any(x => x.Nombre.ToUpper() == "F_REPOSICION"))
                {
                    lista.ForEach(x => x.Remove("f_reposicion"));
                }
                else
                {
                    campos.Add("F_REPOSICION");
                }

                string sql = String.Format(@"INSERT INTO {0}_AUD(ID_ORIGEN,MARCA,FECHA,{1})
                                             SELECT ID_TABLA,'O',GETDATE(),{1} FROM {0} WHERE ID_TABLA = @ID_TABLA
                                             UPDATE {0} SET IDCAUSA_SIR = ISNULL(@IDCAUSA_SIR,IDCAUSA_SIR), DESCRIPCION_SIR=ISNULL(@DESCRIPCION_SIR,DESCRIPCION_SIR), CONFIRMACION_SIR = ISNULL(@CONFIRMACION_SIR,CONFIRMACION_SIR) {2} WHERE ID_TABLA = @ID_TABLA
                                             INSERT INTO {0}_AUD(ID_ORIGEN,MARCA,FECHA,{1})
                                             SELECT ID_TABLA,'N',GETDATE(),{1} FROM {0} WHERE ID_TABLA = @ID_TABLA
                                             ", tarea.NombreTablaSIR,string.Join(",",tarea.Reporte.campos.Where(x=>!x.Nombre.EndsWith("_SIR") && x.Nombre != "ID_TABLA").Select(x=>x.Nombre).Distinct()), (campos.Any()) ?","+string.Join(",", campos.Select(x => x + "=ISNULL(@" + x+","+x+")")): "");
                res = data.GuardarCorrecciones(sql, lista);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - ObtenerPasos",
                             String.Format(@"Lista:{0}", System.Text.Json.JsonSerializer.Serialize(lista)),
                             ErrorType.Error);
            }
            return res;
        }

        public MODResultado ConfirmarCorrecciones(List<Dictionary<string, object>> lista)
        {
            MODResultado res = new MODResultado();
            try
            {
                var IdTarea = Convert.ToInt32(lista[0]["id_tarea"].ToString());
                var data = FabricaDatos.CrearFlujoTrabajoDatos;
                MODTarea tarea = ConfiguracionNegocio.Flujos.SelectMany(x => x.Tareas, (x, y) => { return y; }).Where(x => x.Id == IdTarea).FirstOrDefault();                
                string sql = String.Format("UPDATE {0} SET CONFIRMACION_SIR = 2 WHERE ID_TABLA = @ID_TABLA", tarea.NombreTablaSIR);
                res = data.ConfirmarCorrecciones(sql, lista);
            }
            catch (Exception e)
            {
                Log.WriteLog(e, this.GetType().FullName + " - ObtenerPasos",
                             String.Format(@"Lista:{0}", System.Text.Json.JsonSerializer.Serialize(lista)),
                             ErrorType.Error);
            }
            return res;

        }

        private DateTime FijarPeriodoPorPeriodicidad(DateTime periodo, EnumPeriodicidad periodicidad, int datoPeriodo)
        {
            DateTime fecha = periodo;

            if (periodicidad == EnumPeriodicidad.trimestral)
            {
                switch (datoPeriodo)
                {
                    case 1:
                        fecha = new DateTime(periodo.Year, 1, 1);
                        break;
                    case 2:
                        fecha = new DateTime(periodo.Year, 4, 1);
                        break;
                    case 3:
                        fecha = new DateTime(periodo.Year, 7, 1);
                        break;
                    case 4:
                        fecha = new DateTime(periodo.Year, 10, 1);
                        break;
                    default:
                        break;
                }
            }

            return fecha;
        }
        #endregion
    }
}
