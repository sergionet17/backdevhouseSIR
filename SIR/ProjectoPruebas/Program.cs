using ProjectoPruebas.Preparacion_de_ambiente;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Funcionalidades;
using SIR.Negocio.Fabrica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectoPruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            Ejemplo();

            //Console.WriteLine("Requiere Configuracion Inicial: 1 Si / 2 No");
            //string respuesta = Console.ReadLine();
            //if (respuesta.Equals("1"))
            //{
            //    ConfiguracionGeneral.CrearEmpresa();
            //    ConfiguracionGeneral.CrearServico();
            //}
            //Console.WriteLine("Crear Reportes SUI: 1 Si / 2 No");
            //string respuesta = Console.ReadLine();
            //if (respuesta.Equals("1"))
            //{
            //    ReportesSUI.GenerarTC3();
            //}
        }

        
        static void Ejemplo(){
            MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = 2, //LUZ ARTIFICIAL
                IdEmpresa = 1035, //Celsia Colombia
                NombreEmpresa = "Celsia Colombia",
                IdElemento = 2, // REPORTE PRUEBA SIGMA
                Elemento = "DIARIO",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,//Campo nuevo del reporte
                IdCategoria = 1,
                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.LAC,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>()
            };
            //Preparo la consulta para la tarea con proceso de obtener
            var consulta1 = @"WITH T_FINALIZADOS AS 
              (SELECT TICKET_ID, CHANGE_DATE
                  FROM SIG_STATUS_TICKET st
                  WHERE ST.STATUS_ID IN(9, 10) AND ST.CHANGE_DATE BETWEEN ADD_MONTHS(SYSDATE, -9) AND SYSDATE),
                          T_EJECUCION AS(
                SELECT SUM(STATUS_HOUR) AS TIEMPO_DE_EJECUCION, ST.TICKET_ID
                FROM SIG_STATUS_TICKET ST
                INNER JOIN T_FINALIZADOS TF ON ST.TICKET_ID = TF.TICKET_ID
                WHERE STATUS_ID in (2, 6, 7, 8, 12)
                AND ST.ID >= (
                  SELECT IDS FROM
                  (
                    SELECT MAX(ID) AS IDS, A.TICKET_ID
                    FROM SIG_STATUS_TICKET a
                    INNER JOIN T_FINALIZADOS TF ON A.TICKET_ID = TF.TICKET_ID
                    WHERE STATUS_ID = 2
                    GROUP BY a.TICKET_ID
                  )
                  WHERE TICKET_ID = ST.TICKET_ID
                )
                GROUP BY ST.TICKET_ID
              )
              SELECT U_ADMON.ID,
            U_ADMON.FULLNAME GESTOR,
                     COUNT(*) NoTiquetes,
            SUM(CASE WHEN E.TIEMPO_DE_EJECUCION <= RT.TIMEMAX_HOUR THEN 0 ELSE 1 END) Vencidos,
                     SYSDATE FECHA
              FROM SIG_TICKET T
              INNER JOIN T_FINALIZADOS F ON T.ID = F.TICKET_ID
              LEFT JOIN SIG_REQUEST_TYPE RT ON T.REQUEST_TYPE_ID = RT.ID
              LEFT JOIN T_EJECUCION E ON T.ID = E.TICKET_ID
              LEFT JOIN SIG_USERMANAGER_TICKET UT_ADMON ON T.ID = UT_ADMON.TICKET_ID AND UT_ADMON.ISLAST = 1
              LEFT JOIN SIG_USER U_ADMON ON UT_ADMON.USER_ID_MANAGER = U_ADMON.ID
              WHERE RT.ID NOT IN(89, 90, 91, 92)
              GROUP BY U_ADMON.ID,U_ADMON.FULLNAME";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = 4, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "Tiquetes vencidos",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "SIGMA_ADMINIS",
                    ClaveBD = "sigma_adminis",
                    Sid = "ORADEV11",
                    Servidor = "PV30065",
                    Puerto = null,
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta1,
                },
                IdGrupoEjecucion = 4 
            });

            //Prepara la lista de homologaciones para el proceso de validar
            MODHomologacion dato = new MODHomologacion
            {
                Id = 0,
                IdCampo = 4, // NUI
                NombreCampo = "NUI",
                TipoCampo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                ValorSi = "200",
                ValorNo = "@valorOriginal",//debe ir por defecto si no se desea reemplazar el valor
                TipoReemplazo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipoReemplazo.Variable,
                IdTarea = 0,
                Condiciones = new System.Collections.Generic.List<MODCondiciones>()
            };
            dato.Condiciones.Add(new MODCondiciones
            {
                IdCampo = 4,
                Campo = "NUI",
                Condicion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumCondiciones.Mayorque,
                Valor = "@fechaActual",
                TipoCampo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                Nivel = 0 //Se debe colocar el nivel de inicio en cero para evitar confucion a la logica de construccion
                          //El primer registro no debe llevar conector logico
            });

            dato.Condiciones.Add(new MODCondiciones
            {
                IdCampo = 4,
                Campo = "NUI",
                Condicion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumCondiciones.Igualque,
                Valor = "0",
                TipoCampo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                Nivel = 1,
                Conector = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumConectores._And
            });

            dato.Condiciones.Add(new MODCondiciones
            {
                IdCampo = 4,
                Campo = "NUI",
                Condicion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumCondiciones.Mayorque,
                Valor = "50",
                TipoCampo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                Nivel = 1,
                Conector = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumConectores._Or
            });

            var hms = new System.Collections.Generic.List<MODHomologacion>();
            hms.Add(dato);
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Homologar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = 0,
                Homologaciones = hms,
                IdGrupoEjecucion = 4

            });
            
            //Agrego una tarea para registrar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = 4,
                IdGrupoEjecucion = 4
            });

            //Agrego una tarea para finalizar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                IdArchivo = 0,
                IdGrupoEjecucion = 4
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;
            //registrar un flujo de trabajo
            registro.nuevo();
            context.Registrar(registro);

            //Ejecutar el flujo creado, compruebe el identificador
            //context.Ejecutar(new MODFlujoFiltro
            //{
            //    IdEmpresa = 17,
            //    IdServicio = 2,
            //    IdElemento = 1,
            //    TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
            //    Periodo = DateTime.Now,
            //    Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual
            //});

            //context.Consultar(new MODFlujoFiltro {
            //    IdEmpresa = 17,
            //    IdServicio = 2,
            //    IdElemento = 1,
            //    IdTarea = 89, //tarea que requieres obtener
            //    Version = 1, //que version quieres ejecutar
            //    TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
            //    Periodo = DateTime.Now,
            //    Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual
            //});

            //ExprParser ep = new ExprParser();
            //if ((bool)ep.Run(dato.Ecuacion, "2021-03-11", "20", "23, 51, 16, 97"))
            //    Console.WriteLine(string.Format(@"Resultado Positivo"));
            //else
            //    Console.WriteLine(string.Format(@"Resultado Negativo"));

            //var editable = context.Obtener(new MODFlujoFiltro()).FirstOrDefault(y => y.Id == 6);
            //foreach (var item in editable.Tareas.ToList())
            //{
            //    if (item.ConfiguracionBD != null)
            //    {
            //        item.ConfiguracionBD.consulta = "select 1 from dual";
            //    }
            //}
            //context.Registrar(editable);

        }

        static void EjecutarFlujo(){
            
        }

        static void PruebaTareaCombinacion() 
        {
            int idEmpresa = 1036;
            int idServicio = 2;
            int idElemento = 14;

            int idReporteQuery4 = 17;
            int idGrupoEjecucion4 = 12;


            MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = idServicio, //LUZ ARTIFICIAL
                IdEmpresa = idEmpresa, //Celsia Colombia
                NombreEmpresa = "Celsia Colombia",
                IdElemento = idElemento, // REPORTE PRUEBA SIGMA
                Elemento = "PRUEBA_TAREA_COMBINACION",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual, //Campo nuevo del reporte

                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>(),
                Ip = "::1",
                IdUsuario = 1,
                IdCategoria = 2
            };

            //c_Extr_Uquery_Csmos_Recup
            var consulta4 = @"
                SELECT 
                    Nis_Rad,
                    Nvl(SUM(Csmo_Fact), 0) Csmo_Fact,
                    MAX(Imp_Concepto) Imp_Concepto
                FROM (
                        SELECT 
                            Nis_Rad,
                            Csmo_Fact,
                            First_Value(Imp_Concepto) Over(PARTITION BY Nis_Rad ORDER BY f_Fact ASC) Imp_Concepto
                        FROM Imp_Concepto
                        WHERE Co_Concepto = 'VA165'
                        AND f_Fact BETWEEN trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM') AND  LAST_DAY(TO_DATE(@periodo, 'YYYYMM'))
                    )
                GROUP BY Nis_Rad 
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery4, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "CONSUMOS_NF_TARIFA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta4
                },
                IdGrupoEjecucion = idGrupoEjecucion4,
                IdCategoria = 1,
                ConsultaFinal = "DELETE FROM Temp_Combinacion_CONSUMOS_NF_PROM_RECUP WHERE 1 = 1"
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar",
                IdGrupoEjecucion = idGrupoEjecucion4,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            //registrar un flujo de trabajo
            //registro.nuevo();
            //context.Registrar(registro);

            //Ejecutar el flujo creado, compruebe el identificador
            context.Ejecutar(new MODFlujoFiltro
            {
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2019, 1, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 12
            });
        }

        #region Reportes SUI

        #region Consumos

        static void CrearEstructurasReporteConsumos()
        {
            int idEmpresa = 1;
            int idServicio = 1;

            MODReporte EXTR_CONCEP_TARIFA_NF = new MODReporte()
            {
                Descripcion = "CONSUMOS_TARIFA",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "Consumos",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONCEPTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONSUMOS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "9",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TARIFA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "IMP_CONCEPTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "25",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_FACTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_FACT_ANUL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NUM_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "16",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_VAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 9,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_REL_NIS_NIU_NF = new MODReporte()
            {
                Descripcion = "CONSUMOS_REL_NIS_NIU",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "Extracción consumos rel nis niu",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU_PADRE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_CONSUMOS_PROM = new MODReporte()
            {
                Descripcion = "CONSUMOS_PROM",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "Extracción consumos prom",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CSMO_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NUM_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "16",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_CONSUMOS_RECUP = new MODReporte()
            {
                Descripcion = "CONSUMOS_PROM_RECUP",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "Extracción consumos prom recuperacion",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CSMO_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "IMP_CONCEPTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "17",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            var reporteContext = FabricaNegocio.CrearReporteNegocio;
            reporteContext.Registrar(EXTR_CONCEP_TARIFA_NF);
            reporteContext.Registrar(EXTR_REL_NIS_NIU_NF);
            reporteContext.Registrar(EXTR_CONSUMOS_PROM);
            reporteContext.Registrar(EXTR_CONSUMOS_RECUP);
        }

        static void CrearFlujoReporteConsumos()
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 72;


            int idReporteQuery1 = 72;
            int idGrupoEjecucion1 = 12;

            int idReporteQuery2 = 73;

            int idReporteQuery3 = 74;

            int idReporteQuery4 = 75;


            MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = idServicio, //LUZ ARTIFICIAL
                IdEmpresa = idEmpresa, //Celsia Colombia
                NombreEmpresa = "Celsia",
                IdElemento = idElemento, // CONSUMOS
                Elemento = "CONSUMOS",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual, //Campo nuevo del reporte

                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>(),
                Ip = "::1",
                IdUsuario = 1,
                IdCategoria = 2
            };

            //c_Extr_Uquery_Concep_Tarif_Nf
            var consulta1 = @"
            SELECT 
                Rec.Nis_Rad     Nis_Rad,
                i.Co_Concepto   Concepto,
                i.Csmo_Fact     Consumos,
                i.Prec_Concepto Tarifa,
                i.Imp_Concepto  Imp_Concepto,
                Rec.Simbolo_Var Id_Factura,
                i.Cod_Tar,
                Rec.f_Fact_Anul,
                Rec.Num_Fact,
                p.f_Val
            FROM (  
                    SELECT 
                        Rc.Nis_Rad,
                        Rc.Sec_Rec Sec_Rec,
                        Rc.Sec_Nis,
                        Rc.Simbolo_Var,
                        Rc.f_Fact,
                        Cod_Tar,
                        f_Fact_Anul,
                        Num_Fact
                    FROM Recibos Rc
                    WHERE Rc.f_Puesta_Cobro BETWEEN trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM') 
                    AND  LAST_DAY(TO_DATE(@periodo, 'YYYYMM'))
                    AND Rc.Est_Act != 'ER600'
                    AND (Rc.Tip_Serv = 'SV120' OR Rc.Tip_Serv = 'SV100')
                    AND (Tip_Rec = 'TR010' OR Tip_Rec = 'TR020' 
                    OR Tip_Rec = 'TR012')
                ) Rec,
                (
                    SELECT 
                        Nis_Rad,
                        f_Fact,
                        Sec_Nis,
                        Sec_Rec,
                        Co_Concepto,
                        Csmo_Fact,
                        Prec_Concepto,
                        Imp_Concepto,
                        Cod_Tar,
                        Codigo,
                        Sec_Rango
                    FROM Imp_Detalle_Concepto
                    WHERE Substr(Co_Concepto, 1, 3) = 'CC2'
                    OR Substr(Co_Concepto, 1, 3) = 'CC4'
                    OR Co_Concepto IN (SELECT Codigo FROM Sir_Conceptos)
                ) i,
                (
                    SELECT 
                        Cod_Tar,    
                        Co_Concepto, 
                        Codigo, 
                        MAX(f_Val) f_Val, 
                        Sec_Rango
                    FROM Prec_Concepto
                    GROUP BY 
                        Cod_Tar, Co_Concepto, Codigo, Sec_Rango
                ) p
            WHERE Rec.f_Fact = i.f_Fact
            AND Rec.Nis_Rad = i.Nis_Rad
            AND Rec.Sec_Rec = i.Sec_Rec
            AND Rec.Sec_Nis = i.Sec_Nis
            AND i.Cod_Tar = p.Cod_Tar
            AND i.Co_Concepto = p.Co_Concepto
            AND i.Sec_Rango = p.Sec_Rango
            AND Lpad(i.Codigo, 9, '0') = Substr(p.Codigo, 4)
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery1, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "TARIFA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta1
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion1,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //c_Extr_Relac_Nis_Niu_Nf
            var consulta2 = @"
                SELECT b.Nif,
                   b.Nic,
                   b.Nis_Rad,
                   TRIM(a.Niu) AS Niu,
                   TRIM(a.Niu_Padre) AS Niu_Padre,
                   @periodo Periodo
                FROM Niu_Rel_Sumcon a,
                    (   SELECT 
                            Nis_Rad, 
                            Nif, 
                            Nic
                        FROM Sumcon
                        WHERE (Tip_Serv = 'SV100' OR Tip_Serv = 'SV120')
                        AND Est_Sum <> 'EC000') b
                        WHERE b.Nis_Rad = a.Nis_Rad
                        AND a.Niu = a.Niu_Padre

                        UNION 

                        SELECT 
                            b.Nif,
                            b.Nic,
                            b.Nis_Rad,
                            TRIM(a.Niu) AS Niu,
                            TRIM(a.Niu_Padre) AS Niu_Padre,
                            @periodo Periodo
                        FROM 
                        (   
                            SELECT 
                                a.Nis_Rad, 
                                a.Niu, 
                                a.Niu_Padre
                            FROM h_Niu_Rel_Sumcon a
                            WHERE EXISTS
                            (
                                SELECT Nis_Rad, Sec_Nis
                                FROM 
                                (
                                    SELECT 
                                        Nis_Rad, 
                                        MAX(Nvl(Sec_Nis, 0)) Sec_Nis
                                    FROM h_Niu_Rel_Sumcon
                                    GROUP BY Nis_Rad
                                ) x
                                WHERE a.Nis_Rad = x.Nis_Rad
                                AND Nvl(a.Sec_Nis, 0) = x.Sec_Nis
                            )
                        ) a,
                        (
                            SELECT Nis_Rad, Nif, Nic
                            FROM Sumcon
                            WHERE (Tip_Serv = 'SV100' OR Tip_Serv = 'SV120')
                            AND Est_Sum = 'EC000'
                        ) b
                WHERE a.Nis_Rad = b.Nis_Rad
                AND Niu = Niu_Padre
                ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery2, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "REL_NIS_NIU",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta2
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion1,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //c_Extr_Uquery_Csmos_Prom
            var consulta3 = @"
                SELECT 
                    Rc.Nis_Rad, 
                    SUM(i.Csmo_Fact) Csmo_Fact, 
                    Rc.Num_Fact
                FROM Recibos Rc, Imp_Concepto i
                WHERE 
                    Rc.f_Fact BETWEEN Add_Months(To_Date(@periodo || '01 00:00:00', 'YYYYMMDD HH24:MI:SS'), -6) 
                    AND Add_Months(To_Date(@periodo || '01 23:59:59','YYYYMMDD HH24:MI:SS'), 1) - 1
                    AND Rc.Est_Act != 'ER600'
                    AND (Rc.Tip_Serv LIKE 'SV1%')
                    AND (Substr(i.Co_Concepto, 1, 3)) = 'CC2'
                    AND Rc.f_Fact = i.f_Fact
                    AND Rc.Nis_Rad = i.Nis_Rad
                    AND Rc.Sec_Rec = i.Sec_Rec
                    AND Rc.Sec_Nis = i.Sec_Nis
                GROUP BY Rc.Nis_Rad, Rc.Num_Fact    
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery3, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "PROM",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta3
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion1,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //c_Extr_Uquery_Csmos_Recup
            var consulta4 = @"
                SELECT 
                    Nis_Rad,
                    Nvl(SUM(Csmo_Fact), 0) Csmo_Fact,
                    MAX(Imp_Concepto) Imp_Concepto
                FROM (
                        SELECT 
                            Nis_Rad,
                            Csmo_Fact,
                            First_Value(Imp_Concepto) Over(PARTITION BY Nis_Rad ORDER BY f_Fact ASC) Imp_Concepto
                        FROM Imp_Concepto
                        WHERE Co_Concepto = 'VA165'
                        AND f_Fact BETWEEN trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM') AND  LAST_DAY(TO_DATE(@periodo, 'YYYYMM'))
                    )
                GROUP BY Nis_Rad 
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery4, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "PROM_RECUP",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta4
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion1,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //Duplicados
            var consulta5 = @"
                
                DELETE FROM TB_CONSUMOS_CELSIA_CONSUMOS_TARIFA
                WHERE ID_TABLA IN(
                    SELECT ID_TABLA FROM
                        (
			                SELECT ID_TABLA, ROW_NUMBER() 
			                OVER(PARTITION BY
				                Nis_Rad,
				                Concepto,
				                Consumos,
				                Tarifa,
				                Imp_Concepto,
				                Id_Factura,
				                PERIODO_SIR ORDER BY Nis_Rad,
				                Concepto,
				                Consumos,
				                Tarifa,
				                Imp_Concepto,
				                Id_Factura,
				                PERIODO_SIR) Fila
			                FROM TB_CONSUMOS_CELSIA_CONSUMOS_TARIFA 
			                WHERE PERIODO_SIR = @periodo
			                ) T1
                    WHERE Fila > 1           
                )
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = idGrupoEjecucion1,
                IdCategoria = 2,
                ConsultaFinal = consulta5
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar",
                IdGrupoEjecucion = idGrupoEjecucion1,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            //registrar un flujo de trabajo
            registro.nuevo();
            context.Registrar(registro);
        }

        static void EjecutarFlujoReporteConsumos() 
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 72;

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            //Ejecutar el flujo creado, compruebe el identificador
            context.Ejecutar(new MODFlujoFiltro
            {
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2021, 5, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 2 // Revisar y cambiar
            });
        }
        #endregion

        #region Consumos_NF
        static void CrearEstructurasReporteConsumosNF()
        {
            int idEmpresa = 1;
            int idServicio = 1;

            MODReporte EXTR_CONCEP_TARIFA_NF = new MODReporte()
            {
                Descripcion = "CONSUMOS_NF_TARIFA",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "Consumos NF",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONCEPTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONSUMOS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "9",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TARIFA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "IMP_CONCEPTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "25",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_FACTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_FACT_ANUL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NUM_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "16",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_VAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 9,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_REL_NIS_NIU_NF = new MODReporte()
            {
                Descripcion = "CONSUMOS_NF_REL_NIS_NIU",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "Extracción consumos NF rel nis niu",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "8",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU_PADRE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_CONSUMOS_PROM = new MODReporte()
            {
                Descripcion = "CONSUMOS_NF_PROM",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "Extracción consumos NF prom",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CSMO_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NUM_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "16",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_CONSUMOS_RECUP = new MODReporte()
            {
                Descripcion = "CONSUMOS_NF_PROM_RECUP",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "Extracción consumos NF prom recuperacion",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CSMO_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "IMP_CONCEPTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "17",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            var reporteContext = FabricaNegocio.CrearReporteNegocio;
            reporteContext.Registrar(EXTR_CONCEP_TARIFA_NF);
            reporteContext.Registrar(EXTR_REL_NIS_NIU_NF);
            reporteContext.Registrar(EXTR_CONSUMOS_PROM);
            reporteContext.Registrar(EXTR_CONSUMOS_RECUP);
        }

        static void CrearFlujoReporteConsumosNF()
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 76;


            int idReporteQuery1 = 76;
            int idGrupoEjecucion1 = 12;

            int idReporteQuery2 = 77;
            int idGrupoEjecucion2 = 12;

            int idReporteQuery3 = 78;
            int idGrupoEjecucion3 = 12;

            int idReporteQuery4 = 79;
            int idGrupoEjecucion4 = 12;

           MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = idServicio, //LUZ ARTIFICIAL
                IdEmpresa = idEmpresa, //Celsia Colombia
                NombreEmpresa = "Celsia",
                IdElemento = idElemento, // REPORTE PRUEBA SIGMA
                Elemento = "CONSUMOS_NF",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual, //Campo nuevo del reporte
                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>(),
                Ip = "::1",
                IdUsuario = 1,
                IdCategoria = 2
            };

            //c_Extr_Uquery_Concep_Tarif_Nf
            var consulta1 = @"
                SELECT 
                    rec.nis_rad nis_rad,
                    i.co_concepto concepto,
                    i.csmo_fact consumos,
                    i.prec_concepto tarifa,
                    i.imp_concepto imp_concepto,
                    rec.simbolo_var id_factura,
                    rec.cod_tar,
                    rec.f_fact_anul,
                    rec.num_fact,
                    rec.f_Fact,
                    rec.sec_rec,
                    rec.sec_nis
                    FROM  
                    (
                        SELECT 
                            rc.nis_rad,
                            rc.sec_rec sec_rec,
                            rc.sec_nis, 
                            rc.simbolo_Var,
                            rc.f_Fact, 
                            cod_tar, 
                            f_fact_anul, 
                            num_fact
                        FROM   recibos rc
                        WHERE  rc.f_puesta_cobro BETWEEN trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM') 
                        AND  LAST_DAY(TO_DATE(@periodo, 'YYYYMM'))
                        AND    rc.est_act != 'ER600'
                        AND    (rc.tip_serv ='SV120' OR rc.tip_serv ='SV100')
                        and  (tip_rec='TR010' or  tip_rec='TR020' or tip_rec='TR012')
                    ) rec,
                    (
                        select 
                            nis_rad, 
                            f_Fact, 
                            sec_nis, 
                            sec_rec, 
                            co_concepto, 
                            csmo_fact, 
                            prec_concepto, 
                            imp_concepto
                        from imp_concepto
                        where substr(co_concepto,1,3) = 'CC2' OR
                        substr(co_concepto, 1, 3) = 'CC4' OR
                        co_concepto in ( select codigo from sir_conceptos )
                    )i
                WHERE rec.f_Fact=i.f_Fact
                AND rec.nis_rad=i.nis_rad
                AND rec.sec_rec=i.sec_rec
                AND rec.sec_nis=i.sec_nis
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery1, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "TARIFA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta1
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion1,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //c_Extr_Relac_Nis_Niu_Nf
            var consulta2 = @"
                SELECT b.Nif,
                   b.Nic,
                   b.Nis_Rad,
                   TRIM(a.Niu) AS Niu,
                   TRIM(a.Niu_Padre) AS Niu_Padre,
                   @periodo Periodo
                FROM Niu_Rel_Sumcon a,
                    (   SELECT 
                            Nis_Rad, 
                            Nif, 
                            Nic
                        FROM Sumcon
                        WHERE (Tip_Serv = 'SV100' OR Tip_Serv = 'SV120')
                        AND Est_Sum <> 'EC000') b
                        WHERE b.Nis_Rad = a.Nis_Rad
                        AND a.Niu = a.Niu_Padre

                        UNION 

                        SELECT 
                            b.Nif,
                            b.Nic,
                            b.Nis_Rad,
                            TRIM(a.Niu) AS Niu,
                            TRIM(a.Niu_Padre) AS Niu_Padre,
                            @periodo Periodo
                        FROM 
                        (   
                            SELECT 
                                a.Nis_Rad, 
                                a.Niu, 
                                a.Niu_Padre
                            FROM h_Niu_Rel_Sumcon a
                            WHERE EXISTS
                            (
                                SELECT Nis_Rad, Sec_Nis
                                FROM 
                                (
                                    SELECT 
                                        Nis_Rad, 
                                        MAX(Nvl(Sec_Nis, 0)) Sec_Nis
                                    FROM h_Niu_Rel_Sumcon
                                    GROUP BY Nis_Rad
                                ) x
                                WHERE a.Nis_Rad = x.Nis_Rad
                                AND Nvl(a.Sec_Nis, 0) = x.Sec_Nis
                            )
                        ) a,
                        (
                            SELECT Nis_Rad, Nif, Nic
                            FROM Sumcon
                            WHERE (Tip_Serv = 'SV100' OR Tip_Serv = 'SV120')
                            AND Est_Sum = 'EC000'
                        ) b
                WHERE a.Nis_Rad = b.Nis_Rad
                AND Niu = Niu_Padre
                ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery2, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "REL_NIS_NIU",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta2
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion2,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //c_Extr_Uquery_Csmos_Prom
            var consulta3 = @"
                SELECT 
                    Rc.Nis_Rad, 
                    SUM(i.Csmo_Fact) Csmo_Fact, 
                    Rc.Num_Fact
                FROM Recibos Rc, Imp_Concepto i
                WHERE 
                    Rc.f_Fact BETWEEN Add_Months(To_Date(@periodo || '01 00:00:00', 'YYYYMMDD HH24:MI:SS'), -6) 
                    AND Add_Months(To_Date(@periodo || '01 23:59:59','YYYYMMDD HH24:MI:SS'), 1) - 1
                    AND Rc.Est_Act != 'ER600'
                    AND (Rc.Tip_Serv LIKE 'SV1%')
                    AND (Substr(i.Co_Concepto, 1, 3)) = 'CC2'
                    AND Rc.f_Fact = i.f_Fact
                    AND Rc.Nis_Rad = i.Nis_Rad
                    AND Rc.Sec_Rec = i.Sec_Rec
                    AND Rc.Sec_Nis = i.Sec_Nis
                GROUP BY Rc.Nis_Rad, Rc.Num_Fact    
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery3, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "PROM",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta3
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion3,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //c_Extr_Uquery_Csmos_Recup
            var consulta4 = @"
                SELECT 
                    Nis_Rad,
                    Nvl(SUM(Csmo_Fact), 0) Csmo_Fact,
                    MAX(Imp_Concepto) Imp_Concepto
                FROM (
                        SELECT 
                            Nis_Rad,
                            Csmo_Fact,
                            First_Value(Imp_Concepto) Over(PARTITION BY Nis_Rad ORDER BY f_Fact ASC) Imp_Concepto
                        FROM Imp_Concepto
                        WHERE Co_Concepto = 'VA165'
                        AND f_Fact BETWEEN trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM') AND  LAST_DAY(TO_DATE(@periodo, 'YYYYMM'))
                    )
                GROUP BY Nis_Rad 
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = idReporteQuery4, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "PROM_RECUP",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = consulta4
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion4,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //Duplicados
            var consulta5 = @"
                
                DELETE FROM TB_CONSUMOS_NF_CELSIA_CONSUMOS_NF_TARIFA
                WHERE ID_TABLA IN(
                    SELECT ID_TABLA FROM
                        (
			                SELECT ID_TABLA, ROW_NUMBER() 
			                OVER(PARTITION BY
				                Nis_Rad,
				                Concepto,
				                Consumos,
				                Tarifa,
				                Imp_Concepto,
				                Id_Factura,
				                PERIODO_SIR ORDER BY Nis_Rad,
				                Concepto,
				                Consumos,
				                Tarifa,
				                Imp_Concepto,
				                Id_Factura,
				                PERIODO_SIR) Fila
			                FROM TB_CONSUMOS_NF_CELSIA_CONSUMOS_NF_TARIFA 
			                WHERE PERIODO_SIR = @periodo
			                ) T1
                    WHERE Fila > 1           
                )
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = idGrupoEjecucion4,
                IdCategoria = 2,
                ConsultaFinal = consulta5
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar",
                IdGrupoEjecucion = idGrupoEjecucion4,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            //registrar un flujo de trabajo
            registro.nuevo();
            context.Registrar(registro);
        }

        static void EjecutarFlujoReporteConsumosNF() 
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 76;

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            //Ejecutar el flujo creado, compruebe el identificador
            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2021, 6, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 5// revisar y cambiar
            });
        }
        #endregion

        #region TC1
        static void CrearEstructurasReporteTC1()
        {
            int idEmpresa = 1;
            int idServicio = 1;

            MODReporte EXTR_TC1_REGULADOS = new MODReporte()
            {
                Descripcion = "EXTR_TC1_REGULADOS",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_TC1_REGULADOS",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_COMERCIALIZADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIV_TENSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CARGO_INVERSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONEXION_RED",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "SIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 10,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 11,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EST_SUM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 11,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_TC1_NO_REGULADOS = new MODReporte()
            {
                Descripcion = "EXTR_TC1_NO_REGULADOS",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_TC1_NO_REGULADOS",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_COMERCIALIZADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIV_TENSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "7",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CARGO_INVERSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONEXION_RED",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "SIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 10,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 11,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EST_SUM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 11,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_TC1_TEN_PRIMARIA = new MODReporte()
            {
                Descripcion = "EXTR_TC1_TEN_PRIMARIA",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_TC1_TEN_PRIMARIA",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIV_TENSION_PRI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GRUPO_CALIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_UNICO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_ORIGINAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CIRCUITO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LONGITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 10,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_TC1_U_REG_NREG = new MODReporte()
            {
                Descripcion = "EXTR_TC1_U_REG_NREG",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_TC1_U_REG_NREG",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIV_TENSION_PRI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GRUPO_CALIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_UNICO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_ORIGINAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CIRCUITO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LONGITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 10,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_DIVIPOLA = new MODReporte()
            {
                Descripcion = "EXTR_DIVIPOLA",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_DIVIPOLA",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_DIVIPOLA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DESC_TIPO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "80",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NOM_CALLE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "30",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DUPLICADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NUM_PUERTA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CGV_SUM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "23",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIP_FIN",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_DIVIPOLA_TRAD = new MODReporte()
            {
                Descripcion = "EXTR_DIVIPOLA_TRAD",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_DIVIPOLA_TRAD",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_DIVIPOLA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DIRECCION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "80",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIP_FIN",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 3,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_TC1_RELSUM_CATASTRO = new MODReporte()
            {
                Descripcion = "EXTR_TC1_RELSUM_CATASTRO",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_TC1_RELSUM_CATASTRO",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CED_CATASTRAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "100",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_TC1_SUMCON = new MODReporte()
            {
                Descripcion = "EXTR_TC1_SUMCON",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_TC1_SUMCON",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GR_CONCEPTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_SIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_TC1 = new MODReporte()
            {
                Descripcion = "Reporte TC1",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TC1",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIVEL_TENSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIVEL_TENSION_PRIMARIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "PORC_PROP_ACTIV",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONEXION_RED",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "2",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_COMERCIALIZADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GRUPO_CALIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_FRONTERA_COMERCIAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_CIRCUITO_LINEA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 11,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TRANSFORMADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 12,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_DANE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 13,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CEDULA_CATASTRAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "30",
                        Ordinal = 14,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "UBICACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 15,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DIRECCION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "80",
                        Ordinal = 16,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COND_ESPEC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "2",
                        Ordinal = 17,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_AREA_ESPEC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "4",
                        Ordinal = 18,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_AREA_ESPEC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 19,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ESTRATO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 20,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 21,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LONGITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 22,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 23,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "AUTOGENERADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "2",
                        Ordinal = 24,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EXP_ENERGIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 25,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CAPAC_AUTOGEN",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 26,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIP_GENERACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 27,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_FRONTERA_EXPORT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 28,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_ENTRADA_GENERAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "0",
                        Ordinal = 29,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONTR_RESPALDO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 30,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CAPAC_CONTR_RES",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 31,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 32,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_UNICO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 33,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_ORIGINAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 34,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 35,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 36,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 37,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 38,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EST_SUM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 39,
                        esVersionable = false
                    }
                }
            };

            MODReporte TRSF_TC1 = new MODReporte()
            {
                Descripcion = "TRSF_TC1",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TRSF_TC1",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIVEL_TENSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIVEL_TENSION_PRIMARIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "PORC_PROP_ACTIV",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONEXION_RED",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "2",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_COMERCIALIZADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GRUPO_CALIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_FRONTERA_COMERCIAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_CIRCUITO_LINEA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 11,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TRANSFORMADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 12,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_DANE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 13,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CEDULA_CATASTRAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "30",
                        Ordinal = 14,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "UBICACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 15,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DIRECCION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "80",
                        Ordinal = 16,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COND_ESPEC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "2",
                        Ordinal = 17,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_AREA_ESPEC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "4",
                        Ordinal = 18,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_AREA_ESPEC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 19,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ESTRATO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 20,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 21,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LONGITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 22,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 23,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "AUTOGENERADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "2",
                        Ordinal = 24,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EXP_ENERGIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 25,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CAPAC_AUTOGEN",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "60",
                        Ordinal = 26,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIP_GENERACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 27,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_FRONTERA_EXPORT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 28,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_ENTRADA_GENERAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "0",
                        Ordinal = 29,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONTR_RESPALDO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 30,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CAPAC_CONTR_RES",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 31,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 32,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_UNICO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 33,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_ORIGINAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 34,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 35,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 36,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 37,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 38,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EST_SUM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 39,
                        esVersionable = false
                    }
                }
            };

            var reporteContext = FabricaNegocio.CrearReporteNegocio;
            reporteContext.Registrar(EXTR_TC1_REGULADOS);
            reporteContext.Registrar(EXTR_TC1_NO_REGULADOS);
            reporteContext.Registrar(EXTR_TC1_TEN_PRIMARIA);
            reporteContext.Registrar(EXTR_TC1_U_REG_NREG);
            reporteContext.Registrar(EXTR_DIVIPOLA);
            reporteContext.Registrar(EXTR_DIVIPOLA_TRAD);
            reporteContext.Registrar(EXTR_TC1_RELSUM_CATASTRO);
            reporteContext.Registrar(EXTR_TC1_SUMCON);
            reporteContext.Registrar(EXTR_TC1);
            reporteContext.Registrar(TRSF_TC1);
        }

        static void CrearEstructurasArchivoTC1() 
        {
            int idReporte = 105;

            int[] arrayCampos = new int[] {
                320
                ,321
                ,322
                ,323
                ,324
                ,325
                ,326
                ,327
                ,328
                ,329
                ,330
                ,331
                ,332
                ,333
                ,335
                ,336
                ,337
                ,338
                ,339
                ,340
                ,341
                ,342
                ,343
                ,344
                ,345
                ,346
                ,347
                ,348
                ,349
                ,350
                ,351
            };

            int[] arrayCamposControl = new int[]{
                320
                ,321
                ,322
                ,323
                ,324
                ,325
                ,326
                ,327
                ,328
                ,329
                ,330
                ,331
                ,332
                ,333
                ,335
                ,336
                ,337
                ,338
                ,339
                ,340
                ,341
                ,342
                ,343
                ,344
                ,345
                ,346
                ,347
                ,348
                ,349
                ,350
                ,351
                ,353
                ,354
                ,355
                ,356
                ,357
                ,358
                ,359
            }; 

            MODArchivo TC1_561_20437 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_20437",
                Descripcion = "Archivo TC1_561_20437",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_2438 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_2438",
                Descripcion = "Archivo TC1_561_2438",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_637 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_637",
                Descripcion = "Archivo TC1_561_637",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_480 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_480",
                Descripcion = "Archivo TC1_561_480",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_25984 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_25984",
                Descripcion = "Archivo TC1_561_25984",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_31191 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_31191",
                Descripcion = "Archivo TC1_561_31191",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_23442 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_23442",
                Descripcion = "Archivo TC1_561_23442",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_564 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_564",
                Descripcion = "Archivo TC1_561_564",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_597 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_597",
                Descripcion = "Archivo TC1_561_597",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_27631 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_27631",
                Descripcion = "Archivo TC1_561_27631",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_2322 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_2322",
                Descripcion = "Archivo TC1_561_2322",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_694 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_694",
                Descripcion = "Archivo TC1_561_694",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_2020 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_2020",
                Descripcion = "Archivo TC1_561_2020",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_2249 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_2249",
                Descripcion = "Archivo TC1_561_2249",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_536 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_536",
                Descripcion = "Archivo TC1_561_536",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_CONTROL = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_CONTROL",
                Descripcion = "Archivo TC1_561_CONTROL",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_561_TOTAL = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_561_TOTAL",
                Descripcion = "Archivo TC1_561_TOTAL",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_31191 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_31191",
                Descripcion = "Archivo TC1_166_31191",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_564 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_564",
                Descripcion = "Archivo TC1_166_564",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_2020 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_2020",
                Descripcion = "Archivo TC1_166_2020",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_20469 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_20469",
                Descripcion = "Archivo TC1_166_20469",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_637 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_637",
                Descripcion = "Archivo TC1_166_637",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_2438 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_2438",
                Descripcion = "Archivo TC1_166_2438",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_2322 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_2322",
                Descripcion = "Archivo TC1_166_2322",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_2249 = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_2249",
                Descripcion = "Archivo TC1_166_2249",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_CONTROL = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_CONTROL",
                Descripcion = "Archivo TC1_166_CONTROL",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            MODArchivo TC1_166_TOTAL = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TC1_166_TOTAL",
                Descripcion = "Archivo TC1_166_TOTAL",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
            };

            int cont = 0;
            int cont2 = 0;

            foreach (var item in arrayCampos)
            {
                TC1_561_20437.Campos.Add(new MODCamposArchivo(){ IdCampo = item, Orden = cont });
                TC1_561_2438.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_637.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_480.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_25984.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_31191.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_23442.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_564.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_597.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_27631.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_2322.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_694.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_2020.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_2249.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_536.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_561_TOTAL.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_31191.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_564.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_2020.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_20469.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_637.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_2438.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_2322.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_2249.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TC1_166_TOTAL.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });

                cont++;
            }

            foreach (var item in arrayCamposControl)
            {
                TC1_561_CONTROL.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont2 });
                TC1_166_CONTROL.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont2 });

                cont2++;
            }

            var archivoNegocio = FabricaNegocio.CrearArchivoNegocio;
            archivoNegocio.CrearArchivo(TC1_561_20437);
            archivoNegocio.CrearArchivo(TC1_561_2438);
            archivoNegocio.CrearArchivo(TC1_561_637);
            archivoNegocio.CrearArchivo(TC1_561_480);
            archivoNegocio.CrearArchivo(TC1_561_25984);
            archivoNegocio.CrearArchivo(TC1_561_31191);
            archivoNegocio.CrearArchivo(TC1_561_23442);
            archivoNegocio.CrearArchivo(TC1_561_564);
            archivoNegocio.CrearArchivo(TC1_561_597);
            archivoNegocio.CrearArchivo(TC1_561_27631);
            archivoNegocio.CrearArchivo(TC1_561_2322);
            archivoNegocio.CrearArchivo(TC1_561_694);
            archivoNegocio.CrearArchivo(TC1_561_2020);
            archivoNegocio.CrearArchivo(TC1_561_2249);
            archivoNegocio.CrearArchivo(TC1_561_536);
            archivoNegocio.CrearArchivo(TC1_561_TOTAL);
            archivoNegocio.CrearArchivo(TC1_166_31191);
            archivoNegocio.CrearArchivo(TC1_166_564);
            archivoNegocio.CrearArchivo(TC1_166_2020);
            archivoNegocio.CrearArchivo(TC1_166_20469);
            archivoNegocio.CrearArchivo(TC1_166_637);
            archivoNegocio.CrearArchivo(TC1_166_2438);
            archivoNegocio.CrearArchivo(TC1_166_2322);
            archivoNegocio.CrearArchivo(TC1_166_2249);
            archivoNegocio.CrearArchivo(TC1_166_TOTAL);
            archivoNegocio.CrearArchivo(TC1_561_CONTROL);
            archivoNegocio.CrearArchivo(TC1_166_CONTROL);
        }

        static void CrearFlujoReporteTC1() 
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 104;

            int idGrupoEjecucion = 12;
            int idGrupoEjecucionTrsf = 13;
            int ID_REPORTE_EXTR_TC1_REGULADOS = 96;
            int ID_REPORTE_EXTR_TC1_NO_REGULADOS = 97;
            int ID_REPORTE_EXTR_TC1_TEN_PRIMARIA = 98;
            int ID_REPORTE_EXTR_TC1_U_REG_NREG = 99;
            int ID_REPORTE_EXTR_DIVIPOLA = 100;
            int ID_REPORTE_EXTR_DIVIPOLA_TRAD = 101;
            int ID_REPORTE_EXTR_TC1_RELSUM_CATASTRO = 102;
            int ID_REPORTE_EXTR_EXTR_TC1_SUMCON = 103;
            int ID_REPORTE_EXTR_TC1 = 104;
            int ID_REPORTE_TRSF_TC1 = 105;

            int ID_ARCHIVO_TC1_561_20437 = 69;
            int ID_ARCHIVO_TC1_561_2438 = 70;
            int ID_ARCHIVO_TC1_561_637 = 71;
            int ID_ARCHIVO_TC1_561_480 = 72;
            int ID_ARCHIVO_TC1_561_25984 = 73;
            int ID_ARCHIVO_TC1_561_31191 = 74;
            int ID_ARCHIVO_TC1_561_23442 = 75;
            int ID_ARCHIVO_TC1_561_564 = 76;
            int ID_ARCHIVO_TC1_561_597 = 77;
            int ID_ARCHIVO_TC1_561_27631 = 78;
            int ID_ARCHIVO_TC1_561_2322 = 79;
            int ID_ARCHIVO_TC1_561_694 = 80;
            int ID_ARCHIVO_TC1_561_2020 = 81;
            int ID_ARCHIVO_TC1_561_2249 = 82;
            int ID_ARCHIVO_TC1_561_536 = 83;
            int ID_ARCHIVO_TC1_561_TOTAL = 84;
            int ID_ARCHIVO_TC1_166_31191 = 85;
            int ID_ARCHIVO_TC1_166_564 = 86;
            int ID_ARCHIVO_TC1_166_2020 = 87;
            int ID_ARCHIVO_TC1_166_20469 = 88;
            int ID_ARCHIVO_TC1_166_637 = 89;
            int ID_ARCHIVO_TC1_166_2438 = 90;
            int ID_ARCHIVO_TC1_166_2322 = 91;
            int ID_ARCHIVO_TC1_166_2249 = 92;
            int ID_ARCHIVO_TC1_166_TOTAL = 93;
            int ID_ARCHIVO_TC1_561_CONTROL = 94;
            int ID_ARCHIVO_TC1_166_CONTROL = 95;


            MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = idServicio, //LUZ ARTIFICIAL
                IdEmpresa = idEmpresa, //Celsia Colombia
                NombreEmpresa = "Celsia",
                IdElemento = idElemento, // REPORTE PRUEBA SIGMA
                Elemento = "TC1",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual, //Campo nuevo del reporte
                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>(),
                Ip = "::1",
                IdUsuario = 1,
                IdCategoria = 2
            };

            //EXTR_TC1_REGULADOS
            var QUERY_EXTR_TC1_REGULADOS = @"
                SELECT
                    sc.nic as nic,
                    sc.nif as nif,
                    to_char(decode(substr(lpad(sc.bocas_contraincendio,9,0),1,3),'000','536','100','637')) Id_comercializador,
                    sc.tip_tension niv_tension, --TENSION SECUNDARIA
                    substr(lpad(sc.bocas_contraincendio,9,0),9,1) cargo_inversion,
                    substr(lpad(sc.bocas_contraincendio,9,0),8,1) conexion_red,
                    sc.nis_rad nis_rad,
                    decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','561','10','166') id_mercado,
                    trim(sc.codigo_sic) sic,
                    decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','000','10','100') empresa,
                    sc.cod_Tar,
                    sc.est_sum
                 FROM   sumcon sc
                 WHERE  sc.tip_Serv='SV100'
                        AND (sc.est_sum<>'EC000'  and  sc.est_sum<>'EC011')
                        AND (substr(lpad(sc.bocas_contraincendio,9,0),4,2)='00' or substr(lpad(sc.bocas_contraincendio,9,0),4,2)='10' )-- OO->EPSA, 10->CETSA
                        AND sc.tip_suministro != 'SU035'

                 union all

                 select distinct 
                        nic, 
                        nif, 
                        ID_COMERCIALIZADOR, 
                        NIV_TENSION,
                        CARGO_INVERSION,
                        CONEXION_RED,
                        NIS_RAD,
                        ID_MERCADO,
                        SIC,
                        EMPRESA,
                        cod_Tar,
                        est_sum 
                from
                (
                    select
                        sc.nic as nic,
                        sc.nif as nif,
                        to_char(decode(substr(lpad(sc.bocas_contraincendio,9,0),1,3),'000','536','100','637')) Id_comercializador,
                        sc.tip_tension niv_tension, --TENSION SECUNDARIA
                        substr(lpad(sc.bocas_contraincendio,9,0),9,1) cargo_inversion,
                        substr(lpad(sc.bocas_contraincendio,9,0),8,1) conexion_red,
                        sc.nis_rad nis_rad,
                        decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','561','10','166') id_mercado,
                        trim(sc.codigo_sic) sic,
                        decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','000','10','100') empresa,
                        sc.cod_Tar,
                        sc.est_sum
                    from  sumcon sc,recibos rc
                    where sc.nis_RAD=rc.nis_rad
                    AND sc.tip_serv='SV100'
                    AND (sc.est_sum='EC000' or sc.est_sum='EC011')
                    AND (substr(lpad(sc.bocas_contraincendio,9,0),4,2)='00' or substr(lpad(sc.bocas_contraincendio,9,0),4,2)='10' )
                    AND rc.f_puesta_cobro >= trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM')
                    AND rc.f_puesta_cobro <= LAST_DAY(TO_DATE(@periodo, 'YYYYMM'))
                    AND rc.tip_rec='TR010'
                    AND sc.tip_suministro != 'SU035'
                )
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TC1_REGULADOS, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_REGULADOS",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_TC1_REGULADOS
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //EXTR_TC1_NO_REGULADOS
            var QUERY_EXTR_TC1_NO_REGULADOS = @"
                
                SELECT
                    sc.nif as nif,
                    sc.nic as nic,
                    to_char(decode(snr.sumi_ccli_codigo,
                            '001',536,
                            '002',snr.sumi_clie_nit,
                            '003',snr.sumi_clie_nit,
                            '005',2438,
                            '007',536,
                            '008',637,
                            '009',snr.sumi_clie_nit,
                            '012',536,
                            '014',637,
                            '010',536,
                            '050',536,
                            '011',536,
                            '051',536,
                            '052',536,
                            '053',637,
                            '054',536,
                            '055',536,
                             snr.sumi_clie_nit)) id_comercializador,
                    sc.tip_tension niv_tension,
                    substr(lpad(sc.bocas_contraincendio,9,0),9,1) cargo_inversion,
                    substr(lpad(sc.bocas_contraincendio,9,0),8,1) conexion_red,
                    sc.nis_rad nis_rad,
                    decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','561','10','166') id_mercado,
                    sc.codigo_sic SIC,
                    decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','000','10','100') empresa,
                    sc.cod_Tar,
                    sc.est_sum
                FROM  
                (
                    SELECT distinct 
                        SUMI_CLIE_NIT,
                        sumi_ccli_codigo,
                        sumi_sgc_nic, 
                        SUMI_COD_SIC,
                        sumi_nten_codigo
                    FROM snr_suministros 
                    WHERE sumi_estado='ACTIVO'
                ) snr,
                (
                    SELECT 
                        nis_Rad,
                        nic,
                        nif,
                        bocas_contraincendio,
                        tip_tension,
                        codigo_sic,
                        cod_tar,
                        est_sum
                    FROM sumcon A
                    WHERE tip_serv='SV120' 
                            AND est_sum <> 'EC000' 
                            and  est_sum<>'EC011' 
                            AND cod_tar <>'ST4' 
                            AND (substr(lpad(bocas_contraincendio,9,0),4,2)='00' or substr(lpad(bocas_contraincendio,9,0),4,2)='10' )
                            and cod_tar not in('PC2','PE2')
                            AND A.tip_suministro != 'SU035'
                ) sc
                WHERE  sc.nic=snr.sumi_sgc_nic

                UNION all

                SELECT
                    sc.nif as nif,
                    sc.nic as nic,
                    to_char(decode(snr.sumi_ccli_codigo,
                        '001',536,
                        '002',snr.sumi_clie_nit,
                        '003',snr.sumi_clie_nit,
                        '005',2438,
                        '007',536,
                        '008',637,
                        '009',snr.sumi_clie_nit,
                        '012',536,
                        '014',637,
                        '010',536,
                        '050',536,
                        '011',536,
                        '051',536,
                        '052',536,
                        '053',637,
                        '054',536,
                        '055',536,
                        snr.sumi_clie_nit)) id_comercializador,
                    sc.tip_tension niv_tension,
                    substr(lpad(sc.bocas_contraincendio,9,0),9,1) cargo_inversion,
                    substr(lpad(sc.bocas_contraincendio,9,0),8,1) conexion_red,
                    sc.nis_rad nis_rad,
                    decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','561','10','166') id_mercado,
                    sc.codigo_sic SIC,
                    decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','000','10','100') empresa,
                    sc.cod_Tar,
                    sc.est_sum
                FROM   
                (   
                    SELECT distinct 
                        SUMI_CLIE_NIT,
                        sumi_ccli_codigo,
                        sumi_sgc_nic, 
                        SUMI_COD_SIC,
                        sumi_nten_codigo
                    FROM snr_suministros where sumi_sgc_nic in
                    (
                        select sumi_sgc_nic NIC from snr_suministros nr where sumi_estado='INACTIVO' and sumi_sgc_nic is not null
                        minus
                        select sumi_sgc_nic NIC from snr_suministros nr where sumi_estado='ACTIVO' and sumi_sgc_nic is not null
                    )
                ) snr,
                (
                    SELECT nis_Rad,nic,nif,bocas_contraincendio,tip_tension,codigo_sic,cod_Tar,est_sum
                    FROM sumcon A
                    WHERE tip_serv='SV120' AND est_sum <> 'EC000'  and  est_sum<>'EC011' AND cod_tar <>'ST4' AND (substr(lpad(bocas_contraincendio,9,0),4,2)='00' or substr(lpad(bocas_contraincendio,9,0),4,2)='10')
                    and cod_tar not in('PC2','PE2')
                    AND A.tip_suministro != 'SU035'
                ) sc
                WHERE  sc.nic=snr.sumi_sgc_nic

                union all

                select distinct 
                        nif, 
                        nic, 
                        ID_COMERCIALIZADOR, 
                        NIV_TENSION,
                        CARGO_INVERSION,
                        CONEXION_RED,
                        NIS_RAD,
                        ID_MERCADO,
                        SIC,
                        EMPRESA,
                        COD_TAR,
                        EST_SUM 
                from
                (
                    select 
                        sc.nif as nif,
                        sc.nic as nic,
                        to_char(decode(snr.sumi_ccli_codigo,
                            '001',536,
                            '002',snr.sumi_clie_nit,
                            '003',snr.sumi_clie_nit,
                            '005',2438,
                            '007',536,
                            '008',637,
                            '009',snr.sumi_clie_nit,
                            '012',536,
                            '014',637,
                            '010',536,
                            '050',536,
                            '011',536,
                            '051',536,
                            '052',536,
                            '053',637,
                            '054',536,
                            '055',536,
                            snr.sumi_clie_nit)) id_comercializador,
                        sc.tip_tension niv_tension,
                        substr(lpad(sc.bocas_contraincendio,9,0),9,1) cargo_inversion,
                        substr(lpad(sc.bocas_contraincendio,9,0),8,1) conexion_red,
                        sc.nis_rad nis_rad,
                        decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','561','10','166') id_mercado,
                        sc.codigo_sic SIC,
                        decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','000','10','100') empresa,
                        sc.cod_Tar,
                        sc.est_sum
                    FROM  
                    ( 
                        SELECT distinct sumi_clie_nit,sumi_ccli_codigo,SUMI_COD_SIC,sumi_sgc_nic,sumi_nten_codigo
                        FROM snr_suministros WHERE sumi_estado='INACTIVO'
                    ) snr,
                    (
                        SELECT a.nis_rad, a.nif, a.tip_tension, a.bocas_contraincendio, a.codigo_sic, RC.nic,a.cod_Tar,a.est_sum
                        FROM 
                        (
                            select sc.nis_rad,h.nic,sc.nif, sc.tip_tension, sc.bocas_contraincendio, sc.codigo_sic,sc.cod_Tar,sc.est_sum
                            from 
                            (    
                                select nis_Rad, nif,tip_tension,bocas_contraincendio,codigo_sic,cod_Tar,est_sum
                                from sumcon
                                where (est_sum='EC000' or est_sum='EC011' ) 
                                    and tip_Serv='SV120'        
                                    and cod_tar <>'ST4'
                                    and cod_tar not in('PC2','PE2')
                                    AND tip_suministro != 'SU035'
                            ) sc,
                            (
                                select distinct nis_Rad, nic from hsumcon where tip_Serv='SV120' and cod_tar <>'ST4'
                            ) h
                            where sc.nis_Rad=h.nis_Rad(+)
                        ) A,
                        recibos rc
                         WHERE a.nis_RAD=rc.nis_rad
                        AND (substr(lpad(a.bocas_contraincendio,9,0),4,2)='00' or substr(lpad(a.bocas_contraincendio,9,0),4,2)='10' )
                        AND rc.f_puesta_cobro>= trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM')
                        AND rc.f_puesta_cobro<= LAST_DAY(TO_DATE(@periodo, 'YYYYMM'))
                        and rc.tip_rec='TR010'
                    ) sc
                    WHERE  sc.nic=snr.sumi_sgc_nic
                )
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TC1_NO_REGULADOS, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_NO_REGULADOS",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_TC1_NO_REGULADOS
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //EXTR_TC1_TEN_PRIMARIA
            var QUERY_EXTR_TC1_TEN_PRIMARIA = @"

                select 
                    sc.nis_rad nis_rad,
                    case
                        when sc.cod_tar in (select cod_tar from TBL_PAR_ALUM_PUB) then
                            'ALPM0001'
                        when sc.cod_tar in ('SO1','SO5') and f.tip_fin ='TF045' then
                            'ALPM0001'
                        else
                            to_char(ci.codigo_trafo_mb)
                    end as codigo_conexion,
                    case
                        when sc.cod_tar in (select cod_tar from TBL_PAR_ALUM_PUB) then
                            'ALPM0001'
                        when sc.cod_tar in ('SO1','SO5') and f.tip_fin ='TF045' then
                            'ALPM0001'
                        else
                            to_char(trafo_ct.sspdid)
                    end as codigo_conexion_unico,
                    case
                        when sc.cod_tar in (select cod_tar from TBL_PAR_ALUM_PUB) then
                            '2'  
                        when sc.cod_tar in ('SO1','SO5') and f.tip_fin ='TF045' then
                            '2'  
                        else
                            '2'
                        end as tipo_conexion,
                    case
                        when (select niv_tension_sui from TRSF_TENSION_SECUNDARIA where dato_origen=sc.tip_tension)='1' then
                            trafo_ct.niv_tension_pri_trafo
                        else
                            'Default'
                    end as niv_tension_pri,
                    trafo_ct.grupo_calidad_trafo as grupo_calidad,
                    trafo_ct.codigo_trafo as codigo_conexion_original,
                    ci.codigo_salmt as codigo_circuito,
                    trafo_ct.longitud, 
                    trafo_ct.latitud, 
                    trafo_ct.altitud
                    from  
                    (
                        select a.codigo as codigo_trafo, a.sspdid, a.tens_pri niv_tension_pri_trafo, a.grupo grupo_calidad_trafo, SUBSTR(A.INSTALACION_ORIGEN_V10, INSTR(A.INSTALACION_ORIGEN_V10, ':', 1)+1) ins_origen_trafo,
                        b.codigo as codigo_ct, SUBSTR(b.INSTALACION_ORIGEN_V10, INSTR(b.INSTALACION_ORIGEN_V10, ':', 1)+1) ins_origen_ct, c.grupo as grupo_calidad_circuito,
                        b.longitud, b.latitud, b.altitud
                        from   
                        (
                            select codigo,grupo from BDIV10_SGD_SALMT where onis_stat = '0' and instr(onis_ver,'.')=0
                        ) c,
                        (select codigo,INSTALACION_ORIGEN_V10,longitud, latitud, altitud from BDIV10_SGD_CT where onis_stat = '0' and instr(onis_ver,'.')=0) b,
                        (select codigo,sspdid,GC_015 grupo,INSTALACION_ORIGEN_V10,tens_pri from bdiv10_sgd_trafo_mb where onis_stat = '0' and instr(onis_ver,'.')=0) a
                        where substr(A.INSTALACION_ORIGEN_V10, INSTR(A.INSTALACION_ORIGEN_V10, ':', 1)+1) = b.codigo (+)
                        and   substr(b.INSTALACION_ORIGEN_V10, INSTR(b.INSTALACION_ORIGEN_V10, ':', 1)+1) = c.codigo (+)
                    ) trafo_ct,
                    fincas f, gi_cliente_instalacion ci,
                    (
                        select nis_rad, nif, cod_tar, tip_tension from sumcon A where (tip_serv='SV100' or tip_serv='SV120') and cod_tar!='ST4' and cgv_sum!='-'
                        AND (a.est_sum<>'EC000' and a.est_sum<>'EC011')
                        AND (substr(lpad(a.bocas_contraincendio,9,0),4,2)='00' or substr(lpad(a.bocas_contraincendio,9,0),4,2)='10' )
                        AND A.tip_suministro != 'SU035'
                    ) sc
                where sc.nif=f.nif(+)
                and   sc.nis_Rad=ci.nis_rad(+)
                and   ci.codigo_trafo_mb=trafo_ct.codigo_trafo(+)
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TC1_TEN_PRIMARIA, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_TEN_PRIMARIA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_TC1_TEN_PRIMARIA
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //C_EXTR_TC1_TEN_PRIMARIA2
            var QUERY_EXTR_TC1_TEN_PRIMARIA2 = @"
                
                select 
                    sc.nis_rad nis_rad,
                    case
                        when sc.cod_tar in (select cod_tar from TBL_PAR_ALUM_PUB) then
                            'ALPM0001'
                        when sc.cod_tar in ('SO1','SO5') and f.tip_fin ='TF045' then
                            'ALPM0001'
                        else
                            to_char(ci.codigo_trafo_mb)
                    end as codigo_conexion,
                    case
                        when sc.cod_tar in (select cod_tar from TBL_PAR_ALUM_PUB) then
                            'ALPM0001'
                        when sc.cod_tar in ('SO1','SO5') and f.tip_fin ='TF045' then
                            'ALPM0001'
                        else
                            to_char(trafo_ct.sspdid)
                    end as codigo_conexion_unico,
                    case
                        when sc.cod_tar in (select cod_tar from TBL_PAR_ALUM_PUB) then
                            '1'
                        when sc.cod_tar in ('SO1','SO5') and f.tip_fin ='TF045' then
                            '1'
                        else
                            '2'
                    end as tipo_conexion,
                    case
                        when (select niv_tension_sui from TRSF_TENSION_SECUNDARIA where dato_origen=sc.tip_tension)='1' then
                            trafo_ct.niv_tension_pri_trafo
                        else
                            'Default'
                    end as niv_tension_pri,
                    trafo_ct.grupo_calidad_trafo as grupo_calidad,
                    trafo_ct.codigo_trafo as codigo_conexion_original,
                    ci.codigo_salmt as codigo_circuito,
                    trafo_ct.longitud, trafo_ct.latitud, trafo_ct.altitud
                from  
                (
                    select a.codigo as codigo_trafo, a.sspdid, a.tens_pri niv_tension_pri_trafo, a.grupo grupo_calidad_trafo, SUBSTR(A.INSTALACION_ORIGEN_V10, INSTR(A.INSTALACION_ORIGEN_V10, ':', 1)+1) ins_origen_trafo,
                    b.codigo as codigo_ct, SUBSTR(b.INSTALACION_ORIGEN_V10, INSTR(b.INSTALACION_ORIGEN_V10, ':', 1)+1) ins_origen_ct, c.grupo as grupo_calidad_circuito,
                    b.longitud, b.latitud, b.altitud
                    from  
                    (
                        select codigo,grupo from BDIV10_SGD_SALMT where onis_stat = '0' and instr(onis_ver,'.')=0
                    ) c,
                    (select codigo,INSTALACION_ORIGEN_V10,longitud,latitud,altitud from BDIV10_SGD_CT where onis_stat = '0' and instr(onis_ver,'.')=0) b,
                    (select codigo,sspdid,GC_015 grupo,INSTALACION_ORIGEN_V10,tens_pri from bdiv10_sgd_trafo_mb where onis_stat = '0' and instr(onis_ver,'.')=0) a
                    where substr(A.INSTALACION_ORIGEN_V10, INSTR(A.INSTALACION_ORIGEN_V10, ':', 1)+1) = b.codigo (+)
                    and   substr(b.INSTALACION_ORIGEN_V10, INSTR(b.INSTALACION_ORIGEN_V10, ':', 1)+1) = c.codigo (+)
            
                ) trafo_ct,
                fincas f, gi_cliente_instalacion ci,
                (
                    select sc.nis_rad, sc.nif, sc.cod_tar, sc.tip_tension
                    from sumcon sc ,recibos rc
                        where sc.nis_RAD=rc.nis_rad
                    and (sc.tip_serv='SV100' or sc.tip_serv='SV120')
                    AND (sc.est_sum='EC000' or sc.est_sum='EC011')
                    and sc.cod_tar!='ST4'
                    AND sc.tip_suministro != 'SU035'
                    AND (substr(lpad(sc.bocas_contraincendio,9,0),4,2)='00' or substr(lpad(sc.bocas_contraincendio,9,0),4,2)='10' )
                    and rc.f_puesta_cobro>= trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM')
                    and rc.f_puesta_cobro<= LAST_DAY(TO_DATE(@periodo, 'YYYYMM'))
                ) sc
                where sc.nif=f.nif(+)
                and   sc.nis_Rad=ci.nis_rad(+)
                and   ci.codigo_trafo_mb=trafo_ct.codigo_trafo(+)
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TC1_TEN_PRIMARIA, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_TEN_PRIMARIA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_TC1_TEN_PRIMARIA2
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //C_EXTR_DIVIPOLA
            var QUERY_EXTR_DIVIPOLA = @"
                
                select 
                    s.nis_Rad, 
                    di.cod_divipola,
                    t.desc_tipo, 
                    c.nom_calle,
                    f.duplicador,
                    f.num_puerta,
                    s.cgv_sum,
                    f.tip_fin
                from
                    deptos d,
                    fincas f,
                    callejero c,
                    localidades g,
                    tipos t,
                    divipola di,
                    municipios m,
                    (      
                        SELECT nis_rad, nif, nic, cgv_sum, cod_tar, tip_suministro, grupo_famil,est_sum
                        FROM sumcon a
                        WHERE (tip_serv='SV100' OR tip_serv='SV120') and cod_tar <>'ST4')
                     s
                where
                s.nif = f.nif
                AND   f.cod_calle = c.cod_calle
                AND   c.cod_local = g.cod_local
                AND   c.tip_via = t.tipo
                AND   c.cod_depto = d.cod_depto
                AND   c.cod_munic = m.cod_munic
                AND   m.cod_munic = di.cod_munic(+)
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_DIVIPOLA, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_DIVIPOLA",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_DIVIPOLA
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //C_EXTR_TC1_U_REG_NREG
            var QUERY_EXTR_U_REG_NEG = @"
                
                SELECT
	                PERIODO_SIR
	                ,NIU
	                ,ID_COMERCIALIZADOR
	                ,NIV_TENSION
	                ,CARGO_INVERSION
	                ,CONEXION_RED
	                ,NIS_RAD
	                ,ID_MERCADO
	                ,SIC
	                ,EMPRESA
	                ,NIF
	                ,NIC
	                ,COD_TAR
	                ,EST_SUM
                FROM TB_TC1_CELSIA_EXTR_REGULADOS WITH(NOLOCK)
				WHERE PERIODO_SIR = @periodo
                GROUP BY 
	                PERIODO_SIR, NIU, ID_COMERCIALIZADOR, NIV_TENSION, CARGO_INVERSION, CONEXION_RED, NIS_RAD, ID_MERCADO, SIC, EMPRESA, NIF, NIC, COD_TAR, EST_SUM

                UNION ALL

                SELECT
	                PERIODO_SIR
	                ,NIU
	                ,ID_COMERCIALIZADOR
	                ,NIV_TENSION
	                ,CARGO_INVERSION
	                ,CONEXION_RED
	                ,NIS_RAD
	                ,ID_MERCADO
	                ,SIC
	                ,EMPRESA
	                ,NIF
	                ,NIC
	                ,COD_TAR
	                ,EST_SUM
                FROM [dbo].[TB_TC1_CELSIA_EXTR_NO_REGULADOS] WITH(NOLOCK)
				WHERE PERIODO_SIR = @periodo
                GROUP BY 
	                PERIODO_SIR, NIU, ID_COMERCIALIZADOR, NIV_TENSION, CARGO_INVERSION, CONEXION_RED, NIS_RAD, ID_MERCADO, SIC, EMPRESA, NIF, NIC, COD_TAR, EST_SUM
                
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TC1_U_REG_NREG, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "SIR2",
                    Sid = "SIR2",
                    NombreTabla = "EXTR_U_REG_NEG",//colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_EXTR_U_REG_NEG
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //EXTR_DIVIPOLA_TRAD
            var QUERY_EXTR_DIVIPOLA_TRAD = @"
                
                SELECT 
	                T1.NIS_RAD ,
                    T1.COD_DIVIPOLA,
	                CONCAT(
		
		                (CASE WHEN ISNULL(D1.ABREVIATURA, ' ') = ' ' THEN T1.DESC_TIPO ELSE D1.ABREVIATURA END), ' ',
		                T1.NOM_CALLE, ' ',
		                T1.DUPLICADOR, ' ',
		                T1.NUM_PUERTA, ' ',
		
		                (CASE WHEN ISNULL(D2.ABREVIATURA, ' ') = ' ' THEN T1.STR1 ELSE D2.ABREVIATURA END), ' ',
		                 T1.STR2,' ',

		                 (CASE WHEN ISNULL(D3.ABREVIATURA, ' ') = ' ' THEN T1.STR3 ELSE D3.ABREVIATURA END), ' ',
		                 T1.STR4,' ',

		                 (CASE WHEN ISNULL(D4.ABREVIATURA, ' ') = ' ' THEN T1.STR5 ELSE D4.ABREVIATURA END), ' ',
		                 T1.STR6,' ',

		                 (CASE WHEN ISNULL(D5.ABREVIATURA, ' ') = ' ' THEN T1.STR7 ELSE D5.ABREVIATURA END), ' ',
		                 T1.STR8,' ',

		                 (CASE WHEN ISNULL(D6.ABREVIATURA, ' ') = ' ' THEN T1.STR9 ELSE D6.ABREVIATURA END), ' ',
		                 T1.STR10,' '
	
	                ) DIRECCION,
	                T1.TIP_FIN
                FROM (
                SELECT 
	                R.NIS_RAD ,
	                R.COD_DIVIPOLA ,
	                R.DESC_TIPO ,
	                R.NOM_CALLE ,
	                R.DUPLICADOR ,
	                CAST(R.NUM_PUERTA AS INT) AS NUM_PUERTA ,
	                REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' ') CLEAN,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 1)  STR1,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 2)  STR2,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 3)  STR3,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 4)  STR4,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 5)  STR5,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 6)  STR6,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 7)  STR7,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 8)  STR8,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 9)  STR9,
	                (SELECT Data FROM dbo.Split(REPLACE(REPLACE(LTRIM(RTRIM(R.CGV_SUM)), char(35), ''), '  ', ' '), ' ') WHERE Indice = 10) STR10,
	                R.TIP_FIN
                FROM TB_TC1_CELSIA_EXTR_DIVIPOLA R) T1
                LEFT JOIN ParidadDireccion D1 ON UPPER(T1.DESC_TIPO) = UPPER(D1.DESCRIPCION)
                LEFT JOIN ParidadDireccion D2 ON UPPER(T1.STR1) = UPPER(D2.DESCRIPCION)
                LEFT JOIN ParidadDireccion D3 ON UPPER(T1.STR3) = UPPER(D3.DESCRIPCION)
                LEFT JOIN ParidadDireccion D4 ON UPPER(T1.STR5) = UPPER(D4.DESCRIPCION)
                LEFT JOIN ParidadDireccion D5 ON UPPER(T1.STR7) = UPPER(D5.DESCRIPCION)
                LEFT JOIN ParidadDireccion D6 ON UPPER(T1.STR9) = UPPER(D6.DESCRIPCION)
                
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 6,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_DIVIPOLA_TRAD, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "SIR2",   //colocar control no mas 20 caracteres
                    Sid = "SIR2",
                    NombreTabla = "EXTR_DIVIPOLA_TRAD",
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_EXTR_DIVIPOLA_TRAD
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //EXTR_TC1_RELSUM_CATASTRO
            var QUERY_EXTR_TC1_RELSUM_CATASTRO = @"

                select 
                    rsc.NIS_RAD,
                    rsc.CED_CATASTRAL
                from REL_SUM_CATASTRO rsc    
                
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 7,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TC1_RELSUM_CATASTRO, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_RELSUM_CATASTRO",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_TC1_RELSUM_CATASTRO
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //EXTR_TC1_SUMCON
            var QUERY_EXTR_TC1_SUMCON = @"
                
                select 
                     sm.NIS_RAD,
                     SM.COD_TAR,
                     SM.GR_CONCEPTO,
                     sm.CODIGO_SIC
                from SUMCON SM
                
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 8,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_EXTR_TC1_SUMCON, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_SUMCON",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_TC1_SUMCON
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            var QUERY_EXTR_TC1 = @"
                
                SELECT DISTINCT
	                EURN.NIS_RAD,
                    PMNIU.NIU NIU,
                    EURN.NIF,
                    EURN.NIC,
                    ET.TIPO_CONEXION,
                    ET.NIV_TENSION_PRI,
                    ET.GRUPO_CALIDAD,
                    EURN.ID_COMERCIALIZADOR,
                    EURN.NIV_TENSION,
                    EURN.CARGO_INVERSION,
                    EURN.CONEXION_RED,
                    EURN.ID_MERCADO,
                    EURN.SIC,
                    EURN.EMPRESA,
                    ET.CODIGO_CONEXION,
                    ET.CODIGO_CONEXION_UNICO,
                    ET.CODIGO_CONEXION_ORIGINAL,
                    EURN.cod_Tar,
                    EURN.est_sum,
                    ed.COD_DIVIPOLA as COD_DANE,
                    rsc.CED_CATASTRAL AS CEDULA_CATASTRAL,
	                replace(ED.DIRECCION,',',' ') as DIRECCION,
	                CASE
		                WHEN ISNULL(GR.ORDEN_GI, '0') = '7' THEN (CASE WHEN GRUPO_FAMIL > 2 THEN '7' ELSE '0' END)
		                ELSE ISNULL(GR.ORDEN_GI, '0')
	                END AS CONDICION_ESPECIAL,
	                '0000' as codigo_Area,
	                '0' as tipo_Area,
	                sr.codigo_num AS estrato,
	                Convert(CHAR(8),convert(datetimeoffset(7),replace(FECHA_GEN,',','.'),4),112) AS FECHA_GEN,
	                '0' contrato_respaldo,
	                IU.AUTOGENERADOR,
	                IU.EXP_ENERGIA,
	                IU.CAPACIDAD_KW,
	                IU.TIPO_GENEREACION,
	                sm.CODIGO_SIC FRONTERA_EXP,
	                ' ' CONTRATO_RESP,
	                ' ' CAP_RESPALDO,
	                et.CODIGO_CIRCUITO,
	                et.LONGITUD,
	                et.LATITUD,
	                et.ALTITUD
                FROM 
	                [dbo].[TB_TC1_CELSIA_EXTR_TEN_PRIMARIA] ET WITH(NOLOCK) 
	                LEFT JOIN [dbo].[TB_TC1_CELSIA_EXTR_U_REG_NEG] EURN WITH(NOLOCK) ON [EURN].[NIS_RAD] = [ET].[NIS_RAD]
	                LEFT JOIN [dbo].[TB_CONSUMOS_NF_CELSIA_REL_NIS_NIU] PMNIU WITH(NOLOCK) ON  [PMNIU].[NIS_RAD] = [EURN].[NIS_RAD] AND [PMNIU].[PERIODO_SIR] = @periodo
	                LEFT JOIN [dbo].[TB_TC1_CELSIA_EXTR_AUTOGENERADORES] IU WITH(NOLOCK) ON [IU].[NIU] = [PMNIU].[NIU]
	                LEFT JOIN [dbo].[TB_TC1_CELSIA_EXTR_DIVIPOLA_TRAD] ED WITH(NOLOCK) ON [ED].[NIS_RAD] = [EURN].[NIS_RAD]
	                LEFT JOIN [dbo].[TB_TC1_CELSIA_EXTR_RELSUM_CATASTRO] RSC WITH(NOLOCK) ON [RSC].[NIS_RAD] = [ED].[NIS_RAD]
	                LEFT JOIN [dbo].[TransformacionSector] SR WITH(NOLOCK) ON [SR].[SISTEMAS_FUENTE] = [EURN].[COD_TAR]
	                LEFT JOIN [dbo].[TB_TC1_CELSIA_EXTR_SUMCON] SM ON [SM].[NIS_RAD] = [EURN].[NIS_RAD]
	                LEFT JOIN [dbo].[GrupoEstSIR] GR ON [GR].[EST_REC] = CONCAT([SM].[COD_TAR],'-',[SM].[GR_CONCEPTO]) AND [GR].[CO_GRUPO] = 'CS001'
                WHERE
	                ISNULL([EURN].[ID_COMERCIALIZADOR],'') NOT IN (SELECT NIT FROM [dbo].[ExcluidosComercializador] WITH(NOLOCK))
                GROUP BY
	                EURN.NIS_RAD,
                    PMNIU.NIU,
                    EURN.NIF,
                    EURN.NIC,
                    ET.TIPO_CONEXION,
                    ET.NIV_TENSION_PRI,
                    ET.GRUPO_CALIDAD,
                    EURN.ID_COMERCIALIZADOR,
                    EURN.NIV_TENSION,
                    EURN.CARGO_INVERSION,
                    EURN.CONEXION_RED,
                    ET.PERIODO_SIR,
                    EURN.ID_MERCADO,
                    EURN.SIC,
                    EURN.EMPRESA,
                    ET.CODIGO_CONEXION,
                    ET.CODIGO_CONEXION_UNICO,
                    ET.CODIGO_CONEXION_ORIGINAL,
                    EURN.cod_Tar,
                    EURN.est_sum,
	                    ed.COD_DIVIPOLA,
                    rsc.CED_CATASTRAL,
	                ED.DIRECCION,
	                GR.ORDEN_GI,
	                sr.codigo_num,
	                FECHA_GEN,
	                IU.AUTOGENERADOR,
	                IU.EXP_ENERGIA,
	                IU.CAPACIDAD_KW,
	                IU.TIPO_GENEREACION,
	                sm.CODIGO_SIC,
	                et.CODIGO_CIRCUITO,
	                et.LONGITUD,
	                et.LATITUD,
	                et.ALTITUD,
	                GRUPO_FAMIL
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 9,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TC1, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "SIR2",   //colocar control no mas 20 caracteres
                    Sid = "SIR2",
                    NombreTabla = "EXTR_TC1",
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_EXTR_TC1
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            var QUERY_DELETE = @"
                
                TRUNCATE TABLE TB_TC1_CELSIA_EXTR_RELSUM_CATASTRO;
                TRUNCATE TABLE TB_TC1_CELSIA_EXTR_SUMCON

            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 10,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TC1_RELSUM_CATASTRO, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConsultaFinal = QUERY_DELETE,
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            //Finalizar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 11,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //Transformación
            var QUERY_TRSF_TC1_1 = @"
                
                SELECT
	                E.NIU,
                    E.NIS_RAD,
                    E.NIC,
                    E.NIF,
                    E.TIPO_CONEXION,
                    (CASE
		                WHEN E.ID_COMERCIALIZADOR  =  IC.SUMI_CLIE_NIT THEN
			                IC.COD_SUI
		                ELSE
			                E.ID_COMERCIALIZADOR
                    END) ID_COMERCIALIZADOR,
                    E.GRUPO_CALIDAD,
                    E.COD_FRONTERA_COMERCIAL,
                    E.PERIODO_SIR,
	                ISNULL(TP.NIV_TENSION_SUI, REPLACE(e.NIVEL_TENSION_PRIMARIA,'PR','')) NIV_TENSION_SUI,
                    ISNULL(TS.NIV_TENSION_SUI,E.NIVEL_TENSION) NT,
                    ISNULL(FI.FRACCION_INVERSION,e.PORC_PROP_ACTIV) FRACCION_INVERSION,
	                CASE WHEN ISNULL(CR.TIPO_DESTINO,E.CONEXION_RED) = 'A' THEN '1' ELSE '2' END TIPO_DESTINO,
                    E.ID_MERCADO,
	                CASE WHEN E.EMPRESA = '000' THEN 'EPSA' WHEN E.EMPRESA = '100' THEN 'CETSA' ELSE E.EMPRESA END EMPRESA,
                    E.CODIGO_CONEXION,
                    E.CODIGO_CONEXION_UNICO,
                    E.CODIGO_CONEXION_ORIGINAL,
                    E.cod_tar,
                    E.est_sum,
                    E.COD_CIRCUITO_LINEA,
                    E.COD_TRANSFORMADOR,
                    E.COD_DANE,
                    E.CEDULA_CATASTRAL,
                    E.UBICACION,
	                SUBSTRING(E.DIRECCION,1,50) DIRECCION,
                    E.COND_ESPEC,
	                ISNULL(LTRIM(RTRIM(E.COD_AREA_ESPEC)),'0') COD_AREA_ESPEC,
                    E.TIPO_AREA_ESPEC,
                    E.ESTRATO,
                    CAST(CAST(ISNULL(REPLACE(E.ALTITUD,',','.'), 0) AS FLOAT) AS INT) ALTITUD,
                    RTRIM(LTRIM(ISNULL(REPLACE(E.LONGITUD,'.,',','),0))) LONGITUD,
	                RTRIM(LTRIM(ISNULL(REPLACE(E.LATITUD,'.,',','),0))) LATITUD,
	                CASE WHEN E.AUTOGENERADOR = 2 THEN 2 ELSE ISNULL(EA.CODIGO_CREG,3) END AUTOGENERADOR,
	                CASE WHEN (CASE WHEN E.AUTOGENERADOR = 2 THEN 2 ELSE ISNULL(EA.CODIGO_CREG,3) END) = 3 THEN ' ' ELSE E.EXP_ENERGIA END EXP_ENERGIA,
	                CASE WHEN (CASE WHEN E.AUTOGENERADOR = 2 THEN 2 ELSE ISNULL(EA.CODIGO_CREG,3) END) = 3 THEN ' ' ELSE E.CAPAC_AUTOGEN END CAPAC_AUTOGEN,
                    CASE WHEN (CASE WHEN E.AUTOGENERADOR = 2 THEN 2 ELSE ISNULL(EA.CODIGO_CREG,3) END) = 3 THEN ' ' ELSE TG.CODIGO_CREG END TIP_GENERACION,
                    CASE WHEN (CASE WHEN E.AUTOGENERADOR = 2 THEN 2 ELSE ISNULL(EA.CODIGO_CREG,3) END) = 3 THEN ' ' ELSE E.COD_FRONTERA_EXPORT END COD_FRONTERA_EXPORT,
                    E.F_ENTRADA_GENERAR,
	                CASE WHEN (CASE WHEN E.AUTOGENERADOR = 2 THEN 2 ELSE ISNULL(EA.CODIGO_CREG,3) END) = 3 THEN ' ' ELSE E.CONTR_RESPALDO END CONTR_RESPALDO,
                    E.CAPAC_CONTR_RES
                FROM   
	                TB_TC1_CELSIA_EXTR_TC1 E
	                LEFT JOIN TransformacionTensionPrimaria TP ON E.NIVEL_TENSION_PRIMARIA  =  TP.DATO_ORIGEN
                    LEFT JOIN TransformacionTensionSecundaria TS ON E.NIVEL_TENSION  =  TS.DATO_ORIGEN
                    LEFT JOIN TransformacionFraccionInversion FI ON E.PORC_PROP_ACTIV  =  FI.SISTEMAS_FUENTE
                    LEFT JOIN TransformacionConexionRed CR ON E.CONEXION_RED  =  CR.TIPO_ORIGEN
                    LEFT JOIN TransformacionComercializador IC ON E.ID_COMERCIALIZADOR = IC.SUMI_CLIE_NIT
                    LEFT JOIN ParidadAutoGenCreg EA ON E.AUTOGENERADOR = EA.CODIGO_SUI
                    LEFT JOIN ParidadTipoGenCreg TG ON E.TIP_GENERACION = TG.COD_TIPO_GEN
                WHERE
	                 E.PERIODO_SIR = @periodo
	                 AND NOT EXISTS (SELECT ex.NUI from Par_Clientes_Excluidos ex WHERE ex.NUI=E.niu) 
	                 AND E.niu IS NOT NULL
                
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 12,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "SIR2",
                    Sid = "SIR2",
                    NombreTabla = "TRSF_TC1",//colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_TRSF_TC1_1
                },
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            var QUERY_TRSF_TC1_2 = @"
                
                SELECT
	                CONCAT('CALP',COD_DANE,AL.CODIGO_CONEXION)  AS NIU,  
	                AL.CODIGO_CONEXION AS CODIGO_CONEXION,
	                TC1.TIPO_CONEXION, 
	                TC1.NIVEL_TENSION,
	                ISNULL(TP.NIV_TENSION_SUI,AL.NIVEL_TENSION_PRIMARIA) NIVEL_TENSION_PRIMARIA, 
	                TC1.PORC_PROP_ACTIV,
	                TC1.CONEXION_RED, 
	                TC1.ID_COMERCIALIZADOR,
	                TC1.ID_MERCADO, 
	                AL.GRUPO_CALIDAD,
	                TC1.COD_FRONTERA_COMERCIAL, 
	                AL.COD_CIRCUITO_LINEA,
	                AL.CODIGO_CONEXION COD_TRANSFORMADOR, 
	                TC1.COD_DANE,
	                TC1.CEDULA_CATASTRAL, 
	                TC1.UBICACION,
	                TC1.DIRECCION, 
	                TC1.COND_ESPEC,
	                TC1.COD_AREA_ESPEC, 
	                TC1.TIPO_AREA_ESPEC,
	                TC1.ESTRATO, 
	                AL.ALTITUD,
	                AL.LONGITUD, 
	                AL.LATITUD,
	                TC1.AUTOGENERADOR, 
	                TC1.EXP_ENERGIA,
	                TC1.CAPAC_AUTOGEN, 
	                TC1.TIP_GENERACION,
	                TC1.COD_FRONTERA_EXPORT, 
	                TC1.F_ENTRADA_GENERAR,
	                TC1.CONTR_RESPALDO, 
	                TC1.CAPAC_CONTR_RES,
	                TC1.EMPRESA
                FROM  
	                AdminTransAlum AL
	                LEFT JOIN TB_TC1_CELSIA_EXTR_TC1 TC1 ON TC1.NIU = AL.NIU
	                LEFT JOIN TransformacionTensionPrimaria TP ON AL.NIVEL_TENSION_PRIMARIA  =  TP.DATO_ORIGEN
                WHERE TC1.PERIODO_SIR = @periodo
                GROUP BY 
	                COD_DANE, AL.CODIGO_CONEXION, AL.CODIGO_CONEXION,
	                TC1.TIPO_CONEXION, TC1.NIVEL_TENSION,
	                TP.NIV_TENSION_SUI,
	                AL.NIVEL_TENSION_PRIMARIA, 
	                TC1.PORC_PROP_ACTIV,
	                TC1.CONEXION_RED, TC1.ID_COMERCIALIZADOR,
	                TC1.ID_MERCADO, AL.GRUPO_CALIDAD,
	                TC1.COD_FRONTERA_COMERCIAL, AL.COD_CIRCUITO_LINEA,
	                AL.COD_TRANSFORMADOR, TC1.COD_DANE,
	                TC1.CEDULA_CATASTRAL, TC1.UBICACION,
	                TC1.DIRECCION, TC1.COND_ESPEC,
	                TC1.COD_AREA_ESPEC, TC1.TIPO_AREA_ESPEC,
	                TC1.ESTRATO, AL.ALTITUD,
	                AL.LONGITUD, AL.LATITUD,
	                TC1.AUTOGENERADOR, TC1.EXP_ENERGIA,
	                TC1.CAPAC_AUTOGEN, TC1.TIP_GENERACION,
	                TC1.COD_FRONTERA_EXPORT, TC1.F_ENTRADA_GENERAR,
	                TC1.CONTR_RESPALDO, TC1.CAPAC_CONTR_RES,
	                TC1.EMPRESA

                
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 13,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "SIR2",
                    Sid = "SIR2",
                    NombreTabla = "TRSF_TC1",//colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_TRSF_TC1_2
                },
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            //Archivos
            var QUERY_TC1_561_20437 = @"
                
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '20437'
                
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 14,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_20437,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 15,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_20437,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_2438 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '2438'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 16,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_2438,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 17,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_2438,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_637 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '637'


            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 18,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_637,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 19,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_637,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_480 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '480'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 20,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_480,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 21,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_480,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_25984 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '25984'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 22,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_25984,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 23,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_25984,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_31191 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '31191'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 24,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_31191,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 25,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_31191,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_23442 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '23442'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 26,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_23442,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 27,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_23442,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_564 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '564'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 28,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_564,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 29,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_564,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_597 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '597'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 30,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_597,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 31,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_597,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_27631 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '27631'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 33,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_27631,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 33,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_27631,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_2322 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '2322'    
            
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 32,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_2322,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 33,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_2322,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_694 = @"

                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '694'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 34,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_694,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 35,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_694,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_2020 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '2020'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 36,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_2020,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 37,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_2020,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_2249 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '2249'                

            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 38,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_2249,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 39,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_2249,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_536 = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                AND C.ID_COMERCIALIZADOR = '536'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 40,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_536,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 41,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_536,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_TOTAL = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 42,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_TOTAL,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 43,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_TOTAL,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_31191 = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            AND C.ID_COMERCIALIZADOR = '31191'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 44,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_31191,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 45,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_31191,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_564 = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            AND C.ID_COMERCIALIZADOR = '564'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 46,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_564,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 47,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_564,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_2020 = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            AND C.ID_COMERCIALIZADOR = '2020'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 48,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_2020,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 49,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_2020,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_20469 = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            AND C.ID_COMERCIALIZADOR = '20469'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 50,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_20469,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 51,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_20469,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_637 = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            AND C.ID_COMERCIALIZADOR = '637'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 52,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_637,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 53,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_637,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_2438 = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            AND C.ID_COMERCIALIZADOR = '2438'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 54,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_2438,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 55,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_2438,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_2322 = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            AND C.ID_COMERCIALIZADOR = '2322'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 56,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_2322,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 57,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_2322,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_2249 = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            AND C.ID_COMERCIALIZADOR = '2249'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 58,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_2249,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 59,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_2249,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_TOTAL = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 60,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_TOTAL,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 61,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_TOTAL,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_561_CONTROL = @"
                SELECT 
	                LTRIM(RTRIM(NIU)) NIU
	                ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	                ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	                ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	                ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	                ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	                ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	                ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	                ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	                ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	                ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	                ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	                ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	                ,COD_DANE
	                ,UBICACION
	                ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	                ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	                ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	                ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	                ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	                ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	                ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	                ,LTRIM(RTRIM(LATITUD)) LATITUD
	                ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	                ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	                ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	                ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	                ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	                ,F_ENTRADA_GENERAR
	                ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	                ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES
	                ,LTRIM(RTRIM(CODIGO_CONEXION_UNICO)) CODIGO_CONEXION_UNICO
	                ,LTRIM(RTRIM(CODIGO_CONEXION_ORIGINAL)) CODIGO_CONEXION_ORIGINAL
	                ,NIS_RAD
	                ,NIC
	                ,NIF
	                ,LTRIM(RTRIM(COD_TAR)) COD_TAR
	                ,LTRIM(RTRIM(EST_SUM)) EST_SUM
                FROM TB_TC1_CELSIA_TRSF_TC1 C
                WHERE
                C.PERIODO_SIR = @periodo
                AND C.ID_MERCADO = '561'  
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 62,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_561_CONTROL,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 63,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_561_CONTROL,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_TC1_166_CONTROL = @"SELECT 
	            LTRIM(RTRIM(NIU)) NIU
	            ,LTRIM(RTRIM(CODIGO_CONEXION)) CODIGO_CONEXION
	            ,LTRIM(RTRIM(TIPO_CONEXION)) TIPO_CONEXION
	            ,LTRIM(RTRIM(NIVEL_TENSION)) NIVEL_TENSION
	            ,LTRIM(RTRIM(NIVEL_TENSION_PRIMARIA)) NIVEL_TENSION_PRIMARIA
	            ,LTRIM(RTRIM(PORC_PROP_ACTIV)) PORC_PROP_ACTIV
	            ,LTRIM(RTRIM(CONEXION_RED))  CONEXION_RED
	            ,LTRIM(RTRIM(ID_COMERCIALIZADOR)) ID_COMERCIALIZADOR
	            ,LTRIM(RTRIM(ID_MERCADO)) ID_MERCADO
	            ,LTRIM(RTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,LTRIM(RTRIM(COD_FRONTERA_COMERCIAL)) COD_FRONTERA_COMERCIAL
	            ,LTRIM(RTRIM(COD_CIRCUITO_LINEA)) COD_CIRCUITO_LINEA
	            ,LTRIM(RTRIM(COD_TRANSFORMADOR)) COD_TRANSFORMADOR
	            ,COD_DANE
	            ,UBICACION
	            ,LTRIM(RTRIM(DIRECCION)) DIRECCION
	            ,LTRIM(RTRIM(COND_ESPEC)) COND_ESPEC
	            ,LTRIM(RTRIM(TIPO_AREA_ESPEC)) TIPO_AREA_ESPEC
	            ,LTRIM(RTRIM(COD_AREA_ESPEC)) COD_AREA_ESPEC
	            ,LTRIM(RTRIM(ESTRATO)) ESTRATO
	            ,LTRIM(RTRIM(ALTITUD))  ALTITUD
	            ,LTRIM(RTRIM(LONGITUD)) LONGITUD
	            ,LTRIM(RTRIM(LATITUD)) LATITUD
	            ,LTRIM(RTRIM(AUTOGENERADOR)) AUTOGENERADOR
	            ,LTRIM(RTRIM(EXP_ENERGIA)) EXP_ENERGIA
	            ,LTRIM(RTRIM(CAPAC_AUTOGEN)) CAPAC_AUTOGEN
	            ,LTRIM(RTRIM(TIP_GENERACION)) TIP_GENERACION
	            ,LTRIM(RTRIM(COD_FRONTERA_EXPORT)) COD_FRONTERA_EXPORT
	            ,F_ENTRADA_GENERAR
	            ,LTRIM(RTRIM(CONTR_RESPALDO)) CONTR_RESPALDO
	            ,LTRIM(RTRIM(CAPAC_CONTR_RES)) CAPAC_CONTR_RES
	            ,LTRIM(RTRIM(CODIGO_CONEXION_UNICO)) CODIGO_CONEXION_UNICO
	            ,LTRIM(RTRIM(CODIGO_CONEXION_ORIGINAL)) CODIGO_CONEXION_ORIGINAL
	            ,NIS_RAD
	            ,NIC
	            ,NIF
	            ,LTRIM(RTRIM(COD_TAR)) COD_TAR
	            ,LTRIM(RTRIM(EST_SUM)) EST_SUM
            FROM TB_TC1_CELSIA_TRSF_TC1 C
            WHERE
            C.PERIODO_SIR = @periodo
            AND C.ID_MERCADO = '166'  ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 64,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                ConsultaFinal = QUERY_TC1_166_CONTROL,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 65,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TC1,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_TC1_166_CONTROL,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;
            registro.nuevo();
            context.Registrar(registro);
        }

        static void EjecutarFlujoReporteTC1()
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 104;

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2021, 2, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 8// revisar y cambiar
            });

            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2021, 2, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 9// revisar y cambiar
            });
        }
        #endregion

        #region TT1
        static void CrearEstructurasReporteTT1()
        {
            int idEmpresa = 1;
            int idServicio = 1;

            MODReporte EXTR_TT1_SIR = new MODReporte()
            {
                Descripcion = "EXTR_TT1_SIR",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_TT1_SIR",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "VOLTAJE_NOMINAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GRUPO_CALIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "RELE_TELECONT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CANTIDAD_RELE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALIMENTADOR_RADIAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NORMAL_ABIERTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LONGITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "PROPIEDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 10,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 11,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_TT1 = new MODReporte()
            {
                Descripcion = "EXTRACCION_TT1",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TT1",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "VOLTAJE_NOMINAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GRUPO_CALIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "RELE_TELECONT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CANTIDAD_RELE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALIMENTADOR_RADIAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NORMAL_ABIERTO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LONGITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "PROPIEDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 10,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CIRCUITO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 11,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 12,
                        esVersionable = false
                    }
                }
            };

            var reporteContext = FabricaNegocio.CrearReporteNegocio;
            reporteContext.Registrar(EXTR_TT1_SIR);
            reporteContext.Registrar(EXTR_TT1);
        }

        static void CrearEstructurasArchivoTT1() 
        {
            int idReporte = 131;

            int[] arrayCampos = new int[] {
                733
                ,722
                ,723
                ,724
                ,725
                ,726
                ,727
                ,728
                ,729
                ,730
                ,731
                ,732
            };

            MODArchivo TT1_EPSA = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TT1_EPSA",
                Descripcion = "Archivo TT1_EPSA",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2 //Coma
            };

            MODArchivo TT1_CETSA = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TT1_CETSA",
                Descripcion = "Archivo TT1_CETSA",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2 //Coma
            };

            int cont = 0;

            foreach (var item in arrayCampos)
            {
                TT1_EPSA.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TT1_CETSA.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });

                cont++;
            }

            var archivoNegocio = FabricaNegocio.CrearArchivoNegocio;
            archivoNegocio.CrearArchivo(TT1_EPSA);
            archivoNegocio.CrearArchivo(TT1_CETSA);
        }

        static void CrearFlujoReporteTT1()
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 131;

            int idGrupoEjecucion = 12;
            int idGrupoEjecucionTrsf = 13;
            int ID_REPORTE_EXTR_TT1_SIR = 130;
            int ID_REPORTE_EXTR_TT1 = 131;

            int ID_ARCHIVO_TT1_EPSA = 100;
            int ID_ARCHIVO_TT1_CETSA = 101;

            MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = idServicio, //LUZ ARTIFICIAL
                IdEmpresa = idEmpresa, //Celsia
                NombreEmpresa = "Celsia",
                IdElemento = idElemento, // REPORTE PRUEBA SIGMA
                Elemento = "TT1",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual, //Campo nuevo del reporte
                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>(),
                Ip = "::1",
                IdUsuario = 1,
                IdCategoria = 2
            };

            var QUERY_EXTR_TT1_SIR = @"
                
                 select distinct 
                    rtrim(b.descripcion, ' KV') voltaje_nominal,
                    a.GC_015 grupo_calidad,
                    decode(nvl(sd2.TELECONTROL,2),1,1,2) rele_telecont, 
	                    nvl(si.TELECONTROL,0) cantidad_rele,
                    decode(a.alimentador_radial,'S',1,2) alimentador_radial,
                    decode(a.normal_abierto,'S',1,2) normal_abierto,
                    TRIM(to_char(nvl(a.longitud,d.longitud), '999999999D999999')) as longitud,
                    TRIM(to_char(nvl(a.latitud,d.latitud), '999999999D999999')) as latitud,
                    TRIM(to_char(nvl(a.altitud,d.altitud), '99999999999990')) as altitud,
                    decode(a.propiedad,1,0,1) propiedad,
                    a.codigo
                    from bdiv10_sgd_salmt a,
                    (SELECT codigo, descripcion FROM sgd_maestro WHERE tip_tabla = 'TNOM') b,
                    (select codigo, nombre,longitud,latitud,altitud from bdiv10_sgd_subestac where onis_stat = '0' and onis_ver not like '%.%') d,
                    (                
		                    select mt.codigo,TELECONTROL
		                    from bdiv10_sgd_salmt mt, bdiv10_sgd_disyun sd
		                    where mt.onis_stat = '0'
		                    and mt.onis_ver not like '%.%' and mt.virtual <> '1'
		                    and substr(sd.posicion_v10, instr(sd.posicion_v10, ':',1)+1)=mt.codigo
		                    and sd.onis_stat = '0'
		                    and sd.onis_ver not like '%.%'
		                    )sd2, 
		                    (select SUM(TELECONTROL) TELECONTROL,mt.codigo
		                    from bdiv10_sgd_salmt mt, bdiv10_sgd_interr sd
		                    where mt.onis_stat = '0'
		                    and mt.onis_ver not like '%.%' and mt.virtual <> '1'
		                    and substr(sd.instalacion_origen_v10, instr(sd.instalacion_origen_v10, ':',1)+1)=mt.codigo
		                    and sd.onis_stat = '0'
		                    and sd.onis_ver not like '%.%'
		                    AND TELECONTROL = 1
		                    GROUP BY mt.codigo                
		                    )si
                    where a.onis_stat = '0'
	                    AND a.onis_ver not like '%.%'
	                    and a.ten_nom = b.codigo
	                    and sd2.CODIGO(+) = a.codigo
	                    and a.instalacion_origen = d.codigo(+)
	                    and si.CODIGO(+) = a.codigo

                
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TT1_SIR, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_TT1_SIR",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_TT1_SIR
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            var QUERY_EXTR_TT1 = @"SELECT DISTINCT
	            TC1.COD_CIRCUITO_LINEA AS CODIGO_CIRCUITO
                ,ISNULL(TT1S.VOLTAJE_NOMINAL, 'KV') VOLTAJE_NOMINAL
                ,TT1S.GRUPO_CALIDAD
                ,TT1S.ID_MERCADO
	            ,TT1S.RELE_TELECONT
	            ,ISNULL(TT1S.CANTIDAD_RELE,0) CANTIDAD_RELE
                ,TT1S.ALIMENTADOR_RADIAL
                ,TT1S.NORMAL_ABIERTO
                ,TT1S.LONGITUD
                ,TT1S.LATITUD
                ,TT1S.ALTITUD
                ,TT1S.PROPIEDAD
                ,TC1.EMPRESA
            FROM [dbo].[TB_TT1_CELSIA_EXTR_TT1_SIR] TT1S
            INNER JOIN [dbo].[DefTransformacionTC1] TC1 ON TC1.COD_CIRCUITO_LINEA = CAST(TT1S.CODIGO AS VARCHAR(25))
            WHERE TC1.PERIODO_SIR = @periodo";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TT1, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "SIR2",
                    Sid = "SIR2",
                    NombreTabla = "EXTR_TT1",//colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_EXTR_TT1
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            var QUERY_ARCHIVO_TT1_CETSA= @"SELECT 
	            CODIGO_CIRCUITO
	            ,RTRIM(LTRIM(VOLTAJE_NOMINAL)) VOLTAJE_NOMINAL
	            ,RTRIM(LTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,ID_MERCADO 
	            ,RELE_TELECONT
	            ,CANTIDAD_RELE
	            ,ALIMENTADOR_RADIAL
	            ,NORMAL_ABIERTO
	            ,REPLACE(CAST(LONGITUD AS VARCHAR(100)),',','.') LONGITUD
	            ,REPLACE(CAST(LATITUD AS VARCHAR(100)),',','.') LATITUD
	            ,REPLACE(CAST(ALTITUD AS VARCHAR(100)),',','.') ALTITUD
	            ,PROPIEDAD
            FROM TB_TT1_CELSIA_EXTR_TT1 C
            WHERE C.PERIODO_SIR = @periodo AND C.EMPRESA = 'CETSA'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TT1, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConsultaFinal = QUERY_ARCHIVO_TT1_CETSA,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TT1,
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdArchivo = ID_ARCHIVO_TT1_CETSA,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_ARCHIVO_TT1_EPSA = @"SELECT 
	            CODIGO_CIRCUITO
	            ,RTRIM(LTRIM(VOLTAJE_NOMINAL)) VOLTAJE_NOMINAL
	            ,RTRIM(LTRIM(GRUPO_CALIDAD)) GRUPO_CALIDAD
	            ,ID_MERCADO 
	            ,RELE_TELECONT
	            ,CANTIDAD_RELE
	            ,ALIMENTADOR_RADIAL
	            ,NORMAL_ABIERTO
	            ,REPLACE(CAST(LONGITUD AS VARCHAR(100)),',','.') LONGITUD
	            ,REPLACE(CAST(LATITUD AS VARCHAR(100)),',','.') LATITUD
	            ,REPLACE(CAST(ALTITUD AS VARCHAR(100)),',','.') ALTITUD
	            ,PROPIEDAD
            FROM TB_TT1_CELSIA_EXTR_TT1 C
            WHERE C.PERIODO_SIR = @periodo AND C.EMPRESA = 'EPSA'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TT1, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConsultaFinal = QUERY_ARCHIVO_TT1_EPSA,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TT1,
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdArchivo = ID_ARCHIVO_TT1_EPSA,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            //Finalizar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 6,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //Finalizar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 7,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;
            registro.nuevo();
            context.Registrar(registro);
        }

        static void EjecutarFlujoReporteTT1() 
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 131;

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2021, 2, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 20// revisar y cambiar
            });

            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2021, 2, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 21// revisar y cambiar
            });
        }

        #endregion

        #region TT2
        static void CrearEstructurasReporteTT2() 
        {
            int idEmpresa = 1;
            int idServicio = 1;

            MODReporte EXTR_TT2_SIR = new MODReporte()
            {
                Descripcion = "EXTR_TT2_SIR",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "EXTR_TT2_SIR",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>() {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CAPACIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "PROPIEDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_SUBESTACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ESTADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FECHA_ESTADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "RESOL_METODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 6,
                        esVersionable = false
                    }
                }
            };

            MODReporte EXTR_TT2 = new MODReporte()
            {
                Descripcion = "EXTRACCION_TT2",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TT2",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>() {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GRUPO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CAPACIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LONGITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "PROPIEDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_SUBESTACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "VOLTAJE_NOMINAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "PERIODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "6",
                        Ordinal = 10,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_CONEXION_UNICO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "40",
                        Ordinal = 11,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ESTADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 12,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FECHA_ESTADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 13,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "RESOL_METODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 14,
                        esVersionable = false
                    }
                }
            };

            MODReporte TRSF_TT2 = new MODReporte()
            {
                Descripcion = "TRSF_TT2",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TRSF_TT2",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = false,
                campos = new List<MODCampos>() {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_TRANSFORMADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "GRUPO_CALIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 1,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 2,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CAPACIDAD_TRANSFORMADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 3,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "PROPIEDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_SUBESTACION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 4,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LONGITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
                        Ordinal = 5,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "LATITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
                        Ordinal = 6,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ALTITUD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "120",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ESTADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 7,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FECHA_ESTADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "0",
                        Ordinal = 8,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "RESOL_METODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 10,
                        esVersionable = false
                    }
                }
            };

            var reporteContext = FabricaNegocio.CrearReporteNegocio;
            reporteContext.Registrar(EXTR_TT2_SIR);
            reporteContext.Registrar(EXTR_TT2);
            reporteContext.Registrar(TRSF_TT2);
        }

        static void CrearArchivosReporteTT2() 
        {
            int idReporte = 155;

            int[] arrayCampos = new int[] {
                1018
                ,1019
                ,1020
                ,1021
                ,1022
                ,1023
                ,1024
                ,1025
                ,1026
                ,1027
                ,1028
                ,1029
            };

            MODArchivo TT2_EPSA = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TT2_EPSA",
                Descripcion = "Archivo TT2_EPSA",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2 //Coma
            };

            MODArchivo TT2_CETSA = new MODArchivo()
            {
                IdReporte = idReporte,
                Nombre = "TT2_CETSA",
                Descripcion = "Archivo TT2_CETSA",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2 //Coma
            };

            int cont = 0;

            foreach (var item in arrayCampos)
            {
                TT2_EPSA.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });
                TT2_CETSA.Campos.Add(new MODCamposArchivo() { IdCampo = item, Orden = cont });

                cont++;
            }

            var archivoNegocio = FabricaNegocio.CrearArchivoNegocio;
            archivoNegocio.CrearArchivo(TT2_EPSA);
            archivoNegocio.CrearArchivo(TT2_CETSA);
            
        }

        static void CrearFlujoReporteTT2() 
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 154;

            int idGrupoEjecucion = 12;
            int idGrupoEjecucionTrsf = 13;
            int ID_REPORTE_EXTR_TT2_SIR = 153;
            int ID_REPORTE_EXTR_TT2 = 154;
            int ID_REPORTE_TRSF_TT2 = 155;

            int ID_ARCHIVO_TT2_EPSA = 104;
            int ID_ARCHIVO_TT2_CETSA = 105;

            MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = idServicio, //LUZ ARTIFICIAL
                IdEmpresa = idEmpresa, //Celsia
                NombreEmpresa = "Celsia",
                IdElemento = idElemento, // REPORTE PRUEBA SIGMA
                Elemento = "TT2",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual, //Campo nuevo del reporte
                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>(),
                Ip = "::1",
                IdUsuario = 1,
                IdCategoria = 2
            };

            var QUERY_EXTR_TT2_SIR = @"
                
                 SELECT DISTINCT 
	                MB.sspdid CODIGO, 
                    MB.potencia_nominal CAPACIDAD, 
                    DECODE(MB.propiedad, '5', 1, '6', 1, 2) propiedad, 
                    CASE
                    WHEN MB.tipo_subestacion IS NULL THEN
                        '9'
                    WHEN MB.tipo_subestacion IS NOT NULL THEN
                        MB.tipo_subestacion
                    END tipo_subestacion,          
                    2 estado,
                    decode(nvl(mb.fecha_instal,NULL),NULL,NULL,decode(sign(length(mb.fecha_instal)-8),-1,NULL,mb.fecha_instal)) FECHA_ESTADO,
                    1 RESOL_METODO
                  FROM bdiv10_sgd_trafo_mb MB
                  WHERE MB.onis_stat = '0' 
                  and instr(MB.onis_ver,'.')=0

            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TT2_SIR, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_TT2_SIR",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_TT2_SIR
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            var QUERY_EXTR_TT2 = @"SELECT
	            TT2.CODIGO AS CODIGO_CONEXION_UNICO
	            , TC1.GRUPO_CALIDAD GRUPO
	            , TC1.ID_MERCADO
	            , TT2.CAPACIDAD
	            , TT2.PROPIEDAD
	            , TT2.TIPO_SUBESTACION
	            , ISNULL(TC1.LATITUD,'0') LATITUD
	            , ISNULL(TC1.LONGITUD,'0') LONGITUD
	            , ISNULL(TC1.ALTITUD,'0') ALTITUD
	            , TT2.ESTADO
	            , CAST(TT2.FECHA_ESTADO AS DATE) FECHA_ESTADO
	            , TT2.RESOL_METODO
	            , TC1.EMPRESA
            FROM [dbo].[DefTransformacionTC1] TC1 WITH(NOLOCK)
            INNER JOIN [dbo].[TB_TT2_CELSIA_EXTR_TT2_SIR] TT2 ON CAST(TT2.CODIGO AS VARCHAR(20)) = TC1.CODIGO_CONEXION
            WHERE
	            TC1.PERIODO_SIR = @periodo";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_EXTR_TT2, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "SIR2",
                    Sid = "SIR2",
                    NombreTabla = "EXTR_TT2",//colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_EXTR_TT2
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            var QUERY_TRSF_TT2 = @"SELECT
	                TT2.CODIGO_CONEXION_UNICO CODIGO_TRANSFORMADOR, 
                    TT2.GRUPO GRUPO_CALIDAD,
                    TT2.ID_MERCADO,
                    TT2.CAPACIDAD CAPACIDAD_TRANSFORMADOR,
                    TT2.PROPIEDAD,
                    TS.CODIGO TIPO_SUBESTACION,
                    TT2.LONGITUD,
                    TT2.LATITUD,
                    ROUND(TT2.ALTITUD, 0) ALTITUD,
                    TT2.ESTADO,
                    TT2.FECHA_ESTADO,
                    RM.DESCRIPCION RESOL_METODO,
                    TT2.EMPRESA
                FROM [dbo].[TB_TT2_CELSIA_EXTR_TT2] TT2
                INNER JOIN TransformacionTipoSubestacion TS ON TS.SISTEMAS_FUENTE = TT2.TIPO_SUBESTACION
                INNER JOIN ParidadResolMetodo RM ON RM.ID_RESOL = TT2.RESOL_METODO
                WHERE TT2.PERIODO_SIR = @periodo
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TT2, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "SIR2",
                    Sid = "SIR2",
                    NombreTabla = "TRSF_TT2",//colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "SIRUserSA",
                    ClaveBD = "h2*gJpZTnS$",
                    Servidor = "MAIA",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer,
                    consulta = QUERY_TRSF_TT2
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            var QUERY_ARCHIVO_TT2_CETSA = @"SELECT 
	                CODIGO_TRANSFORMADOR
	                ,GRUPO_CALIDAD
	                ,ID_MERCADO
	                ,CAPACIDAD_TRANSFORMADOR
	                ,PROPIEDAD
	                ,TIPO_SUBESTACION
	                ,LONGITUD
	                ,LATITUD
	                ,ALTITUD
	                ,ESTADO
	                ,FECHA_ESTADO
	                ,RESOL_METODO
                FROM [SIR2].[dbo].[TB_TT2_CELSIA_TRSF_TT2] TT2 WITH(NOLOCK)
                WHERE
	                TT2.PERIODO_SIR = @periodo
	                AND TT2.EMPRESA = 'CETSA'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TT2, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConsultaFinal = QUERY_ARCHIVO_TT2_CETSA,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TT2,
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdArchivo = ID_ARCHIVO_TT2_CETSA,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            var QUERY_ARCHIVO_TT2_EPSA = @"SELECT 
	                CODIGO_TRANSFORMADOR
	                ,GRUPO_CALIDAD
	                ,ID_MERCADO
	                ,CAPACIDAD_TRANSFORMADOR
	                ,PROPIEDAD
	                ,TIPO_SUBESTACION
	                ,LONGITUD
	                ,LATITUD
	                ,ALTITUD
	                ,ESTADO
	                ,FECHA_ESTADO
	                ,RESOL_METODO
                FROM [SIR2].[dbo].[TB_TT2_CELSIA_TRSF_TT2] TT2 WITH(NOLOCK)
                WHERE
	                TT2.PERIODO_SIR = @periodo
	                AND TT2.EMPRESA = 'EPSA'";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TT2, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConsultaFinal = QUERY_ARCHIVO_TT2_EPSA,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 6,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_TRSF_TT2,
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdArchivo = ID_ARCHIVO_TT2_EPSA,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            //Finalizar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 7,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //Finalizar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 8,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;
            registro.nuevo();
            context.Registrar(registro);
        }

        static void EjecutarFlujoReporteTT2() 
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 154;

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            //Ejecutar el flujo creado, compruebe el identificador
            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2021, 2, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 22//Extracción
            });

            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2021, 2, 1),
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 23//Extracción
            });
        }
        #endregion

        #region S6
        static void CrearEstructurasReporteS6()
        {
            int idEmpresa = 1;
            int idServicio = 1;

            MODReporte EXTR_S6 = new MODReporte()
            {
                Descripcion = "EXTR_S6",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "S6",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIU",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DV",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_ACTIVIDAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "MES_TRIMESTRE",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 0,
                        esVersionable = false
                    },
                    new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 0,
                        esVersionable = false
                    }
                }
            };

            var reporteContext = FabricaNegocio.CrearReporteNegocio;
            reporteContext.Registrar(EXTR_S6);
        }

        static void CrearArchivosReporteS6() 
        {
            int idReporteEPSA = 80;
            int idReporteCETSA = 80;
            
            MODArchivo ARCH_S6_CETSA = new MODArchivo()
            {
                IdReporte = idReporteCETSA,
                Nombre = "S6_CETSA",
                Descripcion = "Archivo de salida S6 CETSA",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
                Campos = new List<MODCamposArchivo>()
                {
                    new MODCamposArchivo()
                    {
                        IdCampo = 44,
                        Orden = 0
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 45,
                        Orden = 1
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 46,
                        Orden = 2
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 47,
                        Orden = 3
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 48,
                        Orden = 4
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 52,
                        Orden = 5
                    }
                }
            };

            MODArchivo ARCH_S6_ETSA = new MODArchivo()
            {
                IdReporte = idReporteEPSA,
                Nombre = "S6_EPSA",
                Descripcion = "Archivo de salida S6 EPSA",
                IdTipoArchivo = 2, //CSV
                Activo = true,
                IdSeparador = 2, //Coma
                Campos = new List<MODCamposArchivo>()
                {
                    new MODCamposArchivo()
                    {
                        IdCampo = 44,
                        Orden = 0
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 45,
                        Orden = 1
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 46,
                        Orden = 2
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 47,
                        Orden = 3
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 48,
                        Orden = 4
                    },
                    new MODCamposArchivo()
                    {
                        IdCampo = 52,
                        Orden = 5
                    }
                }
            };

            var archivoNegocio = FabricaNegocio.CrearArchivoNegocio;
            archivoNegocio.CrearArchivo(ARCH_S6_CETSA);
            archivoNegocio.CrearArchivo(ARCH_S6_ETSA);
        }

        static void CrearFlujoReporteS6() 
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 80;

            int idGrupoEjecucion = 12;
            int idGrupoEjecucionTrsf = 13;
            int ID_REPORTE_QUERY_EXTR_S6 = 80;

            int ID_ARCHIVO_CETSA = 108;
            int ID_ARCHIVO_EPSA = 109;

            MODFlujo registro = new MODFlujo()
            {
                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdServicio = idServicio, //LUZ ARTIFICIAL
                IdEmpresa = idEmpresa, //Celsia Colombia
                NombreEmpresa = "Celsia",
                IdElemento = idElemento, // REPORTE PRUEBA SIGMA
                Elemento = "S6",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.trimestral, //Campo nuevo del reporte
                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new System.Collections.Generic.LinkedList<MODTarea>(),
                Ip = "::1",
                IdUsuario = 1,
                IdCategoria = 2
            };

            //EXTR_S6
            var QUERY_EXTR_S6 = @"
                SELECT DISTINCT
                  NU.NIU NIU,
                  C.DOC_ID NIT,
                  DECODE(C.TIP_DOC,'TD001',DECODE(SIGN(LENGTH(C.DOC_ID)-10),-1,'0',SUBSTR(C.DOC_ID,10,1)),0) DV,
                  S.COD_CNAE COD_ACTIVIDAD,
                  S.NIC NIC,
                  S.NIF NIF,
                  S.NIS_RAD NIS_RAD,
                  (SELECT TO_CHAR(M.EQUIVALENCIA) FROM TRSF_MERCADO M where M.CODIGO = SUBSTR(LPAD(BOCAS_CONTRAINCENDIO,9,'0'),4,2)) AS ID_MERCADO,
                  (SELECT EM.EMPRESA FROM TRSF_F2_3_EMPRESA EM WHERE EM.SISTEMA_FUENTE = SUBSTR(LPAD(BOCAS_CONTRAINCENDIO,9,'0'),1,3)) AS EMPRESA,
                  DECODE(TO_CHAR(TR.F_IRE, 'MM'),'04','01','05','02','06','03','07','01','08','02','09','03','10','01','11','02','12','03',TO_CHAR(TR.F_IRE, 'MM')) MES_TRIMESTRE
              FROM NIU_REL_SUMCON NU, SUMCON S, CLIENTES C, TRABPEND_RE TR
              WHERE NU.NIS_RAD = S.NIS_RAD
              AND S.COD_CLI = C.COD_CLI
              AND TR.NIS_RAD = S.NIS_RAD
              --AND TR.TIP_RCM = 'ZO113'
              --AND UPPER(TR.MOT_RCM) LIKE '%PRIMERA VEZ%'
              AND TR.F_IRE BETWEEN trunc(TO_DATE(@periodo, 'YYYYMM'), 'MM') AND LAST_DAY(ADD_MONTHS(TO_DATE(@periodo, 'YYYYMM'), 2))
  
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_QUERY_EXTR_S6, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    Nombre = "EXTR_S6",   //colocar control no mas 20 caracteres
                    IdTarea = 0,
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = QUERY_EXTR_S6
                },
                Agrupador = "Extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_QUERY_EXTR_S6,
                IdGrupoEjecucion = 13,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            //TRSF_S6
            var QUERY_TRSF_S6_EPSA = @"
                                
                SELECT 
                    [NIU]
                    ,[NIT]
                    ,CAST([DV] AS INT) DV
                    ,CAST([COD_ACTIVIDAD] AS INT) COD_ACTIVIDAD
                    ,CAST([MES_TRIMESTRE] AS INT) MES_TRIMESTRE
                    ,[ID_MERCADO]
                FROM [dbo].[TB_S6_CELSIA_EXTR_S6]
                WHERE [PERIODO_SIR] = @periodo
                AND UPPER(RTRIM(LTRIM([EMPRESA]))) = 'EPSA'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_QUERY_EXTR_S6, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConsultaFinal = QUERY_TRSF_S6_EPSA,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_QUERY_EXTR_S6,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_EPSA,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            //TRSF_S6
            var QUERY_TRSF_S6_CETSA = @"
                                
                SELECT 
                     [NIU]
                    ,[NIT]
                    ,CAST([DV] AS INT) DV
                    ,CAST([COD_ACTIVIDAD] AS INT) COD_ACTIVIDAD
                    ,CAST([MES_TRIMESTRE] AS INT) MES_TRIMESTRE
                    ,[ID_MERCADO]
                FROM [dbo].[TB_S6_CELSIA_EXTR_S6]
                WHERE [PERIODO_SIR] = @periodo
                AND UPPER(RTRIM(LTRIM([EMPRESA]))) = 'CETSA'
            ";

            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_QUERY_EXTR_S6, //No tiene que ser el mismo que el flujo - el del flujo es solo indicativo este es para obtener la estructura
                ConsultaFinal = QUERY_TRSF_S6_CETSA,
                Agrupador = "Transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
            });

            //Archivo CETSA
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = ID_REPORTE_QUERY_EXTR_S6,
                IdGrupoEjecucion = 13,
                IdArchivo = ID_ARCHIVO_CETSA,
                Agrupador = "Transformación",
                IdCategoria = 2
            });

            //Finalizar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 6,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar extracción",
                IdGrupoEjecucion = idGrupoEjecucion,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            //Finalizar
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 7,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Finalizar,
                IdFlujo = 0,
                IdPadre = 0,
                Agrupador = "Finalizar transformación",
                IdGrupoEjecucion = idGrupoEjecucionTrsf,
                IdCategoria = 2
                //Si coloco mas de un obtener y requiero listar ellos con este campo podria
            });

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;
            registro.nuevo();
            context.Registrar(registro);
        }

        static void EjecutarFlujoReporteS6() 
        {
            int idEmpresa = 1;
            int idServicio = 1;
            int idElemento = 80;

            var context = FabricaNegocio.CrearFlujoTrabajoNegocio;

            //Ejecutar el flujo creado, compruebe el identificador
            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2020, 3, 1),
                DatoPeriodo = 4,
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.trimestral,
                IdGrupoEjecucion = 6//Extracción
            });

            context.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = idElemento,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Automatico,
                Periodo = new DateTime(2020, 3, 1),
                DatoPeriodo = 4,
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.trimestral,
                IdGrupoEjecucion = 7//Transformación
            }) ;
        }
        #endregion

        #region TC2
        static void CrearEstructurasReporteTC2_P_EXTR_PROCEDIMIENTO_TC2()
        {
            int idEmpresa = 1036;
            int idServicio = 2;

            MODReporte c_extr_TC2_recibos = new MODReporte()
            {
                Descripcion = "EXTRACCION_TC2_RECIBOS",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TC2_RECIBOS",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>() {
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "IND_REAL_EST",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "SIMBOLO_VAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 2,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_PUESTA_COBRO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 3,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_FACT_ANT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 4,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NUM_MESES_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 5,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIP_REC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_FACT_ANUL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 7,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NUM_APA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "30",
                        Ordinal = 8,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 9,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DIF_LECT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 10,
                        esVersionable = false
                    }
                }
            };

            MODReporte c_extr_TC2_campos_1_12 = new MODReporte() //Aqui vamos
            {
                Descripcion = "EXTRACCION_TC2_1_12",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TC2_1_12",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>() {
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "IDFACTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "16",
                        Ordinal = 0,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EXPEDICION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 1,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FECHA_INICIO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 2,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DIAS_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 3,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ESTRATO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 4,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_LECTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 5,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONSUMO_SUBSISTENCIA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 6,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_FACTURA_FOES",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "100",
                        Ordinal = 7,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIPO_FACTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "1",
                        Ordinal = 8,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_TRAFO_MB",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 9,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NUMERO_FAMILIAS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 10,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CSMO_REACT_MED",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 11,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CODIGO_MEDIDOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "30",
                        Ordinal = 12,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 13,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 14,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 15,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 16,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 17,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "MERCADO_ID",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 18,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TIP_TARIFA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 19,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "POT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 20,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "F_FACT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 21,
                        esVersionable = false
                    }
                }
            };

            MODReporte c_extr_TC2_3_dias_mora = new MODReporte()
            {
                Descripcion = "EXTRACCION_TC2_3_DIAS_MORA",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TC2_3_DIAS_MORA",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>() {
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "DIAS_MORA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    }
                }
            };

            MODReporte c_extr_TC2_3_total_facturado = new MODReporte()//Aqui vamos
            {
                Descripcion = "EXTRACCION_TC2_3_TOTAL_FACTURADO",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TC2_3_TOTAL_FACTURADO",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>() {
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TOTAL_FACTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_FACTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 1,
                        esVersionable = false
                    }
                }
            };

            MODReporte c_extr_TC2_3_otros_concep = new MODReporte()
            {
                Descripcion = "EXTRACCION_TC2_3_OTROS_CONCEPTOS",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TC2_3_OTROS_CONCEPTOS",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>() {
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FACTURACION_POR_CONSUMOS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 0,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONSUMOS",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "VALOR_REACTIVA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 2,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FACT_REACTIVA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 3,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CSMO_REACTIVA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 4,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "SUBSIDIO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 5,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CONTRIBUCION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 6,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "VALOR_FOES",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 7,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FACT_MORA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 8,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_FACTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 9,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 10,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 11,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "COD_TAR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "3",
                        Ordinal = 12,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FACT_CARTERA_CSMO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 13,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FACT_CARTERA_CONTRIB",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 14,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CSMO_ACTIVA_EXP",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 15,
                        esVersionable = false
                    }
                }
            };

            MODReporte C_extr_TC2_tarifa_consumo = new MODReporte()
            {
                Descripcion = "EXTRAER_TC2_TARIFA_CONSUMO",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TC2_TARIFA_CONSUMO",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>() {
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "ID_FACTURA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "TARIFA_APLICADA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 2,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "FECHA_PUB_TARIFA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._datetime,
                        Largo = "20",
                        Ordinal = 3,
                        esVersionable = false
                    }
                }
            };

            MODReporte c_extr_TC2_csmos_prom = new MODReporte()
            {
                Descripcion = "EXTRAER_TC2_CSMOS_PROM",
                Activo = true,
                ActivoEmpresa = true,
                Nombre = "TC2_CSMOS_PROM",
                IdCategoria = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                EsReporte = true,
                campos = new List<MODCampos>() {
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "7",
                        Ordinal = 0,
                        esVersionable = false
                    },
                 new MODCampos()
                    {
                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio,
                        Nombre = "CSMO_PROM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "0",
                        Ordinal = 1,
                        esVersionable = false
                    }
                }
            };

            var reporteContext = FabricaNegocio.CrearReporteNegocio;
            reporteContext.Registrar(c_extr_TC2_recibos);
            reporteContext.Registrar(c_extr_TC2_campos_1_12);
            reporteContext.Registrar(c_extr_TC2_3_dias_mora);
            reporteContext.Registrar(c_extr_TC2_3_total_facturado);
            reporteContext.Registrar(c_extr_TC2_3_otros_concep);
            reporteContext.Registrar(C_extr_TC2_tarifa_consumo);
            reporteContext.Registrar(c_extr_TC2_csmos_prom);
        }
        #endregion

        #endregion
    }
}