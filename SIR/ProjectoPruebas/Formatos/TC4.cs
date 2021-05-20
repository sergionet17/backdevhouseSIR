using ProjectoPruebas.Interfaces;
using ProjectoPruebas.Preparacion_de_ambiente;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectoPruebas.Formatos
{
    public class TC4 : FormatoSUI
    {
        #region campos

        #endregion

        //public override void Archivos()
        //{
        //    var estructuras = ObtenerEstructuras();
        //    var reporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte);
            
        //    MODArchivo ARCH = new MODArchivo()
        //    {
        //        IdReporte = reporte.Id,
        //        Nombre = "TC4_CETSA",
        //        Descripcion = "Archivo de salida S6 CETSA",
        //        IdTipoArchivo = 2, //CSV
        //        Activo = true,
        //        IdSeparador = 1, //Punto y coma
        //        Campos = new List<MODCamposArchivo>()
        //        {
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("NUI")).Id,
        //                Orden = 0
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("CODIGO_DATE")).Id,
        //                Orden = 1
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("RADICADO_RECIBIDO")).Id,
        //                Orden = 2
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("FECHA_PETICION")).Id,
        //                Orden = 3
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("CLASE_PETICION")).Id,
        //                Orden = 4
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("TIPO_RESPUESTA")).Id,
        //                Orden = 5
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("FECHA_RESPUESTA")).Id,
        //                Orden = 6
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("RADICADO_RESPUESTA")).Id,
        //                Orden = 7
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("FECHA_EJECUCION")).Id,
        //                Orden = 8
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("ID_MERCADO")).Id,
        //                Orden = 9
        //            }
        //        }
        //    };
        //    ContextArchivo.CrearArchivo(ARCH);

        //    ARCH = new MODArchivo()
        //    {
        //        IdReporte = reporte.Id,
        //        Nombre = "TC4_EPSA",
        //        Descripcion = "Archivo de salida S6 CETSA",
        //        IdTipoArchivo = 2, //CSV
        //        Activo = true,
        //        IdSeparador = 1, //Punto y coma
        //        Campos = new List<MODCamposArchivo>()
        //        {
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("NUI")).Id,
        //                Orden = 0
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("CODIGO_DATE")).Id,
        //                Orden = 1
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("RADICADO_RECIBIDO")).Id,
        //                Orden = 2
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("FECHA_PETICION")).Id,
        //                Orden = 3
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("CLASE_PETICION")).Id,
        //                Orden = 4
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("TIPO_RESPUESTA")).Id,
        //                Orden = 5
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("FECHA_RESPUESTA")).Id,
        //                Orden = 6
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("RADICADO_RESPUESTA")).Id,
        //                Orden = 7
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("FECHA_EJECUCION")).Id,
        //                Orden = 8
        //            },
        //            new MODCamposArchivo()
        //            {
        //                IdCampo = reporte.campos.FirstOrDefault(cap => cap.Nombre.Equals("ID_MERCADO")).Id,
        //                Orden = 9
        //            }
        //        }
        //    };
        //    ContextArchivo.CrearArchivo(ARCH);
        //}

        public override void Ejecutar()
        {
            var reporte = contextReporte.Obtener(new MODReporteFiltro { Nombre = "TC4", IdEmpresa = idEmpresa, IdServicio = idServicio });
            var flujo = contextFlujo.Obtener(new MODFlujoFiltro());
            contextFlujo.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = reporte.FirstOrDefault(rep => rep.EsReporte).Id,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
                Periodo = new DateTime(2020, 1, 1),
                DatoPeriodo = 0,
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.anual,
                IdGrupoEjecucion = flujo.FirstOrDefault(ele => ele.Elemento.Equals("TC4")).Tareas.FirstOrDefault().IdGrupoEjecucion
            });
        }

        public override void Estructuras()
        {
            MODReporte estructura = new MODReporte()
            {
                Descripcion = "Extraer EXTR SOLICITADAS",
                Nombre = "EXTR SOLICITADAS",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_reclamacion",
                        nombreVisible = "tipo_reclamacion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_EMPRESA",
                        nombreVisible = "COD_EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 1,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50", 
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tip_solic",
                        nombreVisible = "tip_solic",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Extraer PENDIENTES",
                Nombre = "EXTR PENDIENTES",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_reclamacion",
                        nombreVisible = "tipo_reclamacion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_EMPRESA",
                        nombreVisible = "COD_EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 1,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50", 
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Extraer CERRADAS",
                Nombre = "EXTR CERRADAS",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_reclamacion",
                        nombreVisible = "tipo_reclamacion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_EMPRESA",
                        nombreVisible = "COD_EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 1,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50", 
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Extraer EXTR REINSTALACION",
                Nombre = "EXTR REINSTALACION",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_reclamacion",
                        nombreVisible = "tipo_reclamacion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_EMPRESA",
                        nombreVisible = "COD_EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 1,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50", 
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Extraer EXTR RECONEXION",
                Nombre = "EXTR RECONEXION",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_reclamacion",
                        nombreVisible = "tipo_reclamacion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_EMPRESA",
                        nombreVisible = "COD_EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 1,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50", 
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Extraer EXTR EVENTUALES",
                Nombre = "EXTR EVENTUALES",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_reclamacion",
                        nombreVisible = "tipo_reclamacion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "COD_EMPRESA",
                        nombreVisible = "COD_EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "20",
                        Ordinal = 1,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50", 
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIU",
                        nombreVisible = "NIU",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 12,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Extraer ETL",
                Nombre = "ETL EXTR",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_reclamacion",
                        nombreVisible = "tipo_reclamacion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50", 
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },

                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "PERIODO",
                        nombreVisible = "PERIODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NUI",
                        nombreVisible = "NUI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },

                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Reporte TC4 - DECRETO ## - YYYY",
                Nombre = "EXTRACCION",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_reclamacion",
                        nombreVisible = "tipo_reclamacion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "10",
                        Ordinal = 0,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 2,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50", 
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },

                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "PERIODO",
                        nombreVisible = "PERIODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NUI",
                        nombreVisible = "NUI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },

                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);

            estructura = new MODReporte()
            {
                Descripcion = "Reporte TC4 - DECRETO ## - YYYY",
                Nombre = "TC4",
                IdCategoria = idCategoriaSUI,
                EsReporte = true,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,

                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "codigo_dane",
                        nombreVisible = "codigo_dane",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_recibido",
                        nombreVisible = "radicado_recibido",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 4,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_peticion",
                        nombreVisible = "f_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 5,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "clase_peticion",
                        nombreVisible = "clase_peticion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 6,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "tipo_respuesta",
                        nombreVisible = "tipo_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 7,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "f_respuesta",
                        nombreVisible = "f_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 8,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "radicado_respuesta",
                        nombreVisible = "radicado_respuesta",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 9,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "fecha_ejecucion",
                        nombreVisible = "fecha_ejecucion",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },

                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "PERIODO",
                        nombreVisible = "PERIODO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "15",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "NUI",
                        nombreVisible = "NUI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    },

                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "ID_MERCADO",
                        nombreVisible = "ID_MERCADO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._int,
                        Largo = "4",
                        Ordinal = 11,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);
        }

        public override void Flujo()
        {
            var estructuras = ObtenerEstructuras();

            MODFlujo registro = new MODFlujo()
            {
                Ip = ip,
                IdUsuario = idUsuario,
                IdCategoria = idCategoriaSUI,
                Usuario = usuario,

                IdServicio = idServicio,
                IdEmpresa = idEmpresa,
                NombreEmpresa = "Celsia",

                Id = 0,
                Accion = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumAccion.ProcesarReporte,
                IdElemento = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte).Id,
                Elemento = "TC4",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.anual,
                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new LinkedList<MODTarea>(),
                Prerequisitos = new List<MODFlujoPrerequisito>(),
                Categoria = "SUI"
            };

            #region TRUNCATE TABLAS
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR SOLICITADAS")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = "TRUNCATE TABLE TB_TC4_CELSIA_EXTR_SOLICITADAS"
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR PENDIENTES")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = "TRUNCATE TABLE TB_TC4_CELSIA_EXTR_PENDIENTES"
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR CERRADAS")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = "TRUNCATE TABLE TB_TC4_CELSIA_EXTR_CERRADAS "
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 3,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR REINSTALACION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = "TRUNCATE TABLE TB_TC4_CELSIA_EXTR_REINSTALACION "
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR RECONEXION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = "TRUNCATE TABLE TB_TC4_CELSIA_EXTR_RECONEXION "
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTRACCION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = "TRUNCATE TABLE TB_TC4_CELSIA_EXTRACCION "
            });
            #endregion

            #region EXTR SOLICITADAS
            string query = @"
                            SELECT 
                                'S' tipo_reclamacion, 
                                u.COD_EMPRESA COD_EMPRESA, 
                                e.nis_rad nis_rad, 
                                d.cod_divipola codigo_dane, 
                                to_char(e.num_exp) radicado_recibido, 
                                to_char(f_sol,'dd-mm-yyyy') f_peticion,
                                decode(F.tip_tension,'FI002','FI002','FI001','FI001','FI003','FI003','FI004','FI004',0) clase_peticion,
                                e.estado tipo_respuesta,
                                to_char(e.f_uce,'dd-mm-yyyy') f_respuesta, 
                                to_char(e.num_exp) radicado_respuesta, 
                                to_char(e.f_uce,'dd-mm-yyyy') fecha_ejecucion,
                                op.cod_operador as ID_MERCADO, 
                                tip_solic
                            FROM   expedientes e,fincas_exp f, unicom u,municipios m,divipola d,localidades g,callejero c,OPERADOR_RED_DEPTOS op
                            WHERE 
                                e.num_exp = f.num_exp 
                                and op.cod_local=g.cod_local 
                                and u.cod_unicom = cod_unicom_compet 
                                and u.cod_calle=c.cod_calle 
                                and c.cod_local = g.cod_local 
                                and c.cod_munic = m.cod_munic 
                                and m.cod_munic=d.cod_munic and
                                (e.nis_rad is not null or e.nis_rad <> 0) 
                                and (e.f_sol >= TO_DATE(:fecha_ini, 'yyyymmdd hh24:mi:ss') 
                                and e.f_sol <= TO_DATE(:fecha_fin, 'yyyymmdd hh24:mi:ss'))
                                and  (tip_serv = 'SV100' or tip_serv = 'SV120')
                                and e.estado NOT IN  ('AC050','AC055')
";
            //Crea tabla:Temp_Combinacion_EXTR_SOLICITADAS
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 6,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR SOLICITADAS")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "SOLICITADAS",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                ConsultaFinal = @"
                                SELECT 
                                    tipo_reclamacion, 
                                    COD_EMPRESA, 
                                    nis_rad, 
                                    codigo_dane, 
                                    radicado_recibido, 
                                    f_peticion,
                                    clase_peticion, 
                                    tipo_respuesta, 
                                    CASE WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN '' ELSE f_respuesta END f_respuesta,
                                    CASE WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN '' ELSE radicado_respuesta END radicado_respuesta
                                    CASE WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (2)) THEN '' ELSE fecha_ejecucion END fecha_ejecucion
                                    ,ID_MERCADO 
                                FROM Temp_Combinacion_EXTR_SOLICITADAS
                                WHERE TIP_SOLIC IN (SELECT est_rec FROM GrupoEstSIR WHERE co_grupo = 'TS001')"
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTR_SOLICITADAS
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 7,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR SOLICITADAS")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });
            #endregion

            #region EXTR PENDIENTES
            //000. Crea tabla:Temp_Combinacion_EXTR_PENDIENTES
            query = @"
                            SELECT 
                                'P' tipo_reclamacion, 
                                u.COD_EMPRESA COD_EMPRESA, 
                                e.nis_rad nis_rad, 
                                d.cod_divipola codigo_dane, 
                                to_char(e.num_exp) radicado_recibido, 
                                to_char(f_sol,'dd-mm-yyyy') f_peticion,
                                decode(F.tip_tension,'FI002','FI002','FI001','FI001','FI003','FI003','FI004','FI004','FI004') clase_peticion,
                                e.estado tipo_respuesta,
                                to_char(e.f_uce,'dd-mm-yyyy') f_respuesta, 
                                to_char(e.num_exp) radicado_respuesta, 
                                to_char(e.f_uce,'dd-mm-yyyy') fecha_ejecucion,
                                op.cod_operador as ID_MERCADO, 
                                tip_solic
                            FROM   expedientes e,fincas_exp f, unicom u,municipios m,divipola d,localidades g,callejero c,OPERADOR_RED_DEPTOS op
                            WHERE 
                                e.num_exp = f.num_exp 
                                and op.cod_local=g.cod_local 
                                and u.cod_unicom = cod_unicom_compet 
                                and u.cod_calle=c.cod_calle 
                                and c.cod_local = g.cod_local 
                                and c.cod_munic = m.cod_munic 
                                and m.cod_munic=d.cod_munic and
                                (e.nis_rad is not null or e.nis_rad <> 0) 
                                and (e.f_sol <= TO_DATE(:fecha_ini, 'yyyymmdd hh24:mi:ss') 
                                and  (tip_serv = 'SV100' or tip_serv = 'SV120')
                                and e.estado NOT IN  ('AC050','AC055','AC060')
";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 8,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR PENDIENTES")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "PENDIENTES",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                ConsultaFinal = @"
                                SELECT 
                                    tipo_reclamacion, 
                                    COD_EMPRESA, 
                                    nis_rad, 
                                    codigo_dane, 
                                    radicado_recibido, 
                                    f_peticion,
                                    clase_peticion, 
                                    tipo_respuesta, 
                                    CASE WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN '' ELSE f_respuesta END f_respuesta,
                                    CASE WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN '' ELSE radicado_respuesta END radicado_respuesta
                                    CASE WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (2)) THEN '' ELSE fecha_ejecucion END fecha_ejecucion
                                    ,ID_MERCADO 
                                FROM Temp_Combinacion_EXTR_PENDIENTES
                                WHERE TIP_SOLIC IN (SELECT est_rec FROM GrupoEstSIR WHERE co_grupo = 'TS001')"
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTR_PENDIENTES
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 9,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR PENDIENTES")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });
            #endregion 

            #region EXTR CERRADAS
            //000. Crea tabla:Temp_Combinacion_EXTR_CERRADAS
            query = @"
                            SELECT 
                                'C' tipo_reclamacion, 
                                u.COD_EMPRESA COD_EMPRESA, 
                                nvl(sc.nis_rad,0) nis_rad, 
                                d.cod_divipola codigo_dane, 
                                to_char(e.num_exp) radicado_recibido, 
                                to_char(f_sol,'dd-mm-yyyy') f_peticion,
                                decode(F.tip_tension,'FI002','FI002','FI001','FI001','FI003','FI003','FI004','FI004','FI004') clase_peticion,
                                e.estado tipo_respuesta,
                                to_char(E.F_UCE,'dd-mm-yyyy') f_respuesta, 
                                to_char(E.NUM_EXP) radicado_respuesta, 
                                to_char(E.F_UCE,'dd-mm-yyyy') fecha_ejecucion,
                                SUBSTR(LPAD(BOCAS_CONTRAINCENDIO,9,'0'),4,2) ID_MERCADO, 
                                tip_solic
                            FROM   expedientes e,fincas_exp f, unicom u, municipios m,divipola d, localidades g,callejero c,contratos ct,sumcon sc
                            WHERE 
                                e.num_exp = f.num_exp 
		                        and u.cod_unicom = cod_unicom_compet 
		                        and u.cod_calle=c.cod_calle 
		                        and c.cod_local = g.cod_local 
		                        and c.cod_munic = m.cod_munic 
		                        and m.cod_munic=d.cod_munic 
		                        and (e.f_sol >= TO_DATE(:fecha_ini, 'yyyymmdd hh24:mi:ss') and e.f_sol <= TO_DATE(:fecha_fin, 'yyyymmdd hh24:mi:ss'))                               
		                        and e.estado IN ('AC050','AC055','AC060')
		                        and e.num_exp=ct.num_exp(+)
		                        and ct.nic=sc.nic
		                        and (sc.nis_rad is not null or sc.nis_rad <> 0)
		                        and (SC.tip_Serv='SV100' or SC.tip_Serv='SV120')
";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 10,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR CERRADAS")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "CERRADAS",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                ConsultaFinal = @"
                                SELECT 
                                    tipo_reclamacion, 
                                    COD_EMPRESA, 
                                    nis_rad, 
                                    codigo_dane, 
                                    radicado_recibido, 
                                    f_peticion,
                                    clase_peticion, 
                                    tipo_respuesta, 
                                    CASE 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN '' 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN '' 
                                        ELSE f_respuesta
                                    END f_respuesta,
                                    CASE 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN ''
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN ''
                                        ELSE radicado_respuesta 
                                    END radicado_respuesta
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN ''
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN ''
                                        ELSE fecha_ejecucion 
                                    END fecha_ejecucion
                                    ,ID_MERCADO 
                                FROM Temp_Combinacion_EXTR_CERRADAS
                                WHERE TIP_SOLIC IN (SELECT est_rec FROM GrupoEstSIR WHERE co_grupo = 'TS001')"
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTR_CERRADAS
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 11,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR CERRADAS")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });
            #endregion 

            #region EXTR REINSTALACION
            //000. Crea tabla:Temp_Combinacion_EXTR_REINSTALACION
            query = @"
                            SELECT 
                                'T-RINS' tipo_reclamacion, 
                                u.COD_EMPRESA COD_EMPRESA, 
                                t.nis_rad nis_rad, 
                                d.cod_divipola codigo_dane, 
                                to_char(os.num_os) radicado_recibido, 
			                    to_char(f_gen,'dd-mm-yyyy') f_peticion,
                                t.tip_rcm as clase_peticion,
                                os.est_os tipo_respuesta,
                                to_char(OS.F_UCE,'dd-mm-yyyy') f_respuesta, 
                                to_char(T.NUM_RE) radicado_respuesta, 
                                to_char(OS.F_UCE,'dd-mm-yyyy') fecha_ejecucion,
                                SUBSTR(LPAD(BOCAS_CONTRAINCENDIO,9,'0'),4,2) ID_MERCADO                                
                            FROM   unicom u,municipios m,divipola d,localidades g, ordenes os,trabpend_re t,sumcon sc,fincas f,callejero c
                            WHERE 
                                os.tip_os='TO804' and
		                        os.f_gen >=TO_DATE(:fecha_ini, 'yyyymmdd hh24:mi:ss') and os.f_gen <= TO_DATE(:fecha_fin, 'yyyymmdd hh24:mi:ss') and
		                        t.tip_rcm in ('ZO107','ZO100') AND
		                        (sc.tip_serv='SV100' or sc.tip_serv='SV120') and
		                        u.cod_unicom=os.cod_unicom and
		                        t.num_os=os.num_os and
		                        sc.nis_rad=os.nis_rad and
		                        sc.nif=f.nif and
		                        f.cod_calle=c.cod_calle and
		                        c.cod_local = g.cod_local and 
		                        c.cod_munic = m.cod_munic and
		                        (t.nis_rad is not null or t.nis_rad <> 0)  and
		                        m.cod_munic=d.cod_munic
		                        and  (os.tip_serv = 'SV100' or os.tip_serv = 'SV120')
";
            //TB_TC4_CELSIA_EXTR_REINSTALACION
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 12,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR REINSTALACION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "REINSTALACION",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                ConsultaFinal = @"
                                SELECT 
                                    tipo_reclamacion, 
                                    COD_EMPRESA, 
                                    nis_rad, 
                                    codigo_dane, 
                                    radicado_recibido, 
                                    f_peticion,
                                    clase_peticion, 
                                    tipo_respuesta, 
                                    CASE 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN '' 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN '' 
                                        ELSE f_respuesta
                                    END f_respuesta,
                                    CASE 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN ''
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN ''
                                        ELSE radicado_respuesta 
                                    END radicado_respuesta
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN ''
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN ''
                                        ELSE fecha_ejecucion 
                                    END fecha_ejecucion
                                    ,ID_MERCADO 
                                FROM Temp_Combinacion_EXTR_REINSTALACION "
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTR_REINSTALACION
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 13,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR REINSTALACION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });
            #endregion 

            #region EXTR RECONEXION
            //000. Crea tabla:Temp_Combinacion_EXTR_RECONEXION
            query = @"
                            SELECT 
                                'RECO' tipo_reclamacion, 
                                u.COD_EMPRESA COD_EMPRESA, 
                                sc.nis_rad nis_rad, 
                                d.cod_divipola codigo_dane, 
                                to_char(os.num_os) radicado_recibido, 
			                    to_char(f_gen,'dd-mm-yyyy') f_peticion,
                                os.tip_os clase_peticion,
                                os.est_os tipo_respuesta,
                                to_char(OS.F_UCE,'dd-mm-yyyy') f_respuesta, 
                                to_char(OS.NUM_OS) radicado_respuesta, 
                                to_char(OS.F_UCE,'dd-mm-yyyy') fecha_ejecucion,
                                SUBSTR(LPAD(BOCAS_CONTRAINCENDIO,9,'0'),4,2) ID_MERCADO                                
                            FROM   unicom u,municipios m,divipola d,localidades g, ordenes os,sumcon sc,fincas f,callejero c
                            WHERE 
                            os.tip_os in ('TO502','TO505','TO583','TO586') and
		                    (sc.tip_serv='SV100' or sc.tip_serv='SV120') and
		                    u.cod_unicom=os.cod_unicom and
		                    sc.nif=f.nif and
		                    f.cod_calle=c.cod_calle and
		                    c.cod_local = g.cod_local and
		                    c.cod_munic = m.cod_munic and
		                    m.cod_munic=d.cod_munic and
		                    sc.nis_rad=os.nis_rad and
		                    (sc.nis_rad is not null or sc.nis_rad <> 0)  and
		                    (os.tip_serv = 'SV100' or os.tip_serv = 'SV120') and
		                    (os.f_gen >=TO_DATE(:fecha_ini, 'yyyymmdd hh24:mi:ss') and os.f_gen <= TO_DATE(:fecha_fin, 'yyyymmdd hh24:mi:ss') )
";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 14,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR RECONEXION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "RECONEXION",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                ConsultaFinal = @"
                                SELECT 
                                    tipo_reclamacion, 
                                    COD_EMPRESA, 
                                    nis_rad, 
                                    codigo_dane, 
                                    radicado_recibido, 
                                    f_peticion,
                                    clase_peticion, 
                                    tipo_respuesta, 
                                    CASE 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN '' 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN '' 
                                        ELSE f_respuesta
                                    END f_respuesta,
                                    CASE 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN ''
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN ''
                                        ELSE radicado_respuesta 
                                    END radicado_respuesta
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN ''
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN ''
                                        ELSE fecha_ejecucion 
                                    END fecha_ejecucion
                                    ,ID_MERCADO 
                                FROM Temp_Combinacion_EXTR_RECONEXION "
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTR_RECONEXION
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 15,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR RECONEXION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });
            #endregion 

            #region EXTR EVENTUALES
            //000. Crea tabla:Temp_Combinacion_EXTR_EVENTUALES
            query = @"
                            SELECT 
                                'C' tipo_reclamacion, 
                                u.COD_EMPRESA COD_EMPRESA, 
                                nvl(sc.nis_rad,0) NIU, 
                                d.cod_divipola codigo_dane, 
                                to_char(e.num_exp) radicado_recibido, 
			                    to_char(f_sol,'dd-mm-yyyy') f_peticion,
                                f.tip_tension clase_peticion,
                                e.estado tipo_respuesta,
                                to_char(e.f_uce,'dd-mm-yyyy') f_respuesta, 
                                TO_CHAR(E.NUM_EXP) radicado_respuesta, 
                                TO_CHAR(E.F_UCE,'dd-mm-yyyy') fecha_ejecucion,
                                SUBSTR(LPAD(BOCAS_CONTRAINCENDIO,9,'0'),4,2) ID_MERCADO, 
                                sc.tip_serv,
                                tip_solic
                            FROM   
                                expedientes e,fincas_exp f, unicom u, municipios m,divipola d, localidades g,callejero c,contratos ct,sumcon sc
                            WHERE 
                                e.num_exp = f.num_exp and
			                    u.cod_unicom = cod_unicom_compet and
			                    u.cod_calle=c.cod_calle and
			                    c.cod_local = g.cod_local and
			                    c.cod_munic = m.cod_munic and
			                    m.cod_munic=d.cod_munic and
			                    (e.f_uce >= TO_DATE(:fecha_ini, 'yyyymmdd hh24:mi:ss') and e.f_uce <= TO_DATE(:fecha_fin, 'yyyymmdd hh24:mi:ss') )
			                    and e.num_exp=ct.num_exp(+)
			                    and ct.nic=sc.nic(+)
			                    and E.num_exp like 'E%'
";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR EVENTUALES")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "EVENTUALES",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                ConsultaFinal = @"
                                SELECT 
                                    tipo_reclamacion, 
                                    0 NIS_RAD, 
                                    COD_EMPRESA,
                                    NIU,
                                    codigo_dane,
                                    radicado_recibido,
                                    f_peticion,
                                    clase_peticion,
                                    tipo_respuesta,
                                    CASE 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN '' 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN '' 
                                        ELSE f_respuesta
                                    END f_respuesta,
                                    CASE 
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN ''
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN ''
                                        ELSE radicado_respuesta 
                                    END radicado_respuesta
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (5,6)) THEN ''
                                        WHEN tipo_respuesta IN (SELECT EST_REC FROM GrupoEstSIR WHERE CO_GRUPO = 'TR001' AND ORDEN_GI IN (0,2)) THEN ''
                                        ELSE fecha_ejecucion 
                                    END fecha_ejecucion
                                    ,ID_MERCADO 
                                FROM Temp_Combinacion_EXTR_EVENTUALES
                                WHERE 
                                    tip_serv in('SV100','SV120') or tip_serv is null)
                                    AND TIP_SOLIC IN (SELECT est_rec FROM GrupoEstSIR WHERE co_grupo = 'TS001')"
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTR_EVENTUALES
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTR EVENTUALES")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });

            #endregion 

            #region TC4 ETL
            query = @"
                            SELECT 
                                TIPO_RECLAMACION,
			                    RI.NIS_RAD,
			                    CODIGO_DANE, 
			                    RADICADO_RECIBO RADICADO_RECIBIDO,
			                    F_PETICION,
			                    CLASE_PETICION,
			                    TIPO_RESPUESTA,
			                    F_RESPUESTA,
			                    RADICADO_RESPUESTA,
			                    FECHA_EJECUCION,
			                    PERIODO_SIR PERIODO,
			                    COD_EMPRESA EMPRESA,
			                    PMNIU.NIU,
                                RI.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_REINSTALACION RI, (SELECT niu, nis_rad FROM TB_CONSUMOS_NF_CELSIA_REL_NIS_NIU WHERE PERIODO  between CONVERT(DATETIME, @fecha_ini) and CONVERT(DATETIME, @fecha_fin) group by niu,nis_rad) PMNIU
		                    WHERE  
                                RI.NIS_RAD = PMNIU.NIS_RAD
		                    UNION
		                    SELECT 
                                TIPO_RECLAMACION,
			                    CR.NIS_RAD,
			                    CODIGO_DANE,
			                    RADICADO_RECIBO RADICADO_RECIBIDO,
			                    F_PETICION,
			                    CLASE_PETICION,
			                    TIPO_RESPUESTA,
			                    F_RESPUESTA,
			                    RADICADO_RESPUESTA,
			                    FECHA_EJECUCION,
			                    PERIODO_SIR PERIODO,
			                    COD_EMPRESA EMPRESA,
			                    PMNIU.NIU,
                                CR.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_CERRADAS CR, (SELECT niu, nis_rad FROM TB_CONSUMOS_NF_CELSIA_REL_NIS_NIU WHERE PERIODO  between CONVERT(DATETIME, @fecha_ini) and CONVERT(DATETIME, @fecha_fin) group by niu,nis_rad) PMNIU
		                    WHERE  
                                CR.NIS_RAD = PMNIU.NIS_RAD
		                    UNION
		                    SELECT 
                                TIPO_RECLAMACION,
			                    PD.NIS_RAD,
			                    CODIGO_DANE,
			                    RADICADO_RECIBO RADICADO_RECIBIDO,
			                    F_PETICION,
			                    CLASE_PETICION,
			                    TIPO_RESPUESTA,
			                    F_RESPUESTA,
			                    RADICADO_RESPUESTA,
			                    FECHA_EJECUCION,
			                    PERIODO_SIR PERIODO,
			                    COD_EMPRESA EMPRESA,
			                    PMNIU.NIU,
                                PD.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_PENDIENTES PD, (SELECT niu, nis_rad FROM TB_CONSUMOS_NF_CELSIA_REL_NIS_NIU WHERE PERIODO  between CONVERT(DATETIME, @fecha_ini) and CONVERT(DATETIME, @fecha_fin) group by niu,nis_rad) PMNIU
		                    WHERE  
                                PD.NIS_RAD = PMNIU.NIS_RAD
		                    UNION
		                    SELECT 
                                TIPO_RECLAMACION,
			                    RC.NIS_RAD,
			                    CODIGO_DANE,
			                    RADICADO_RECIBO RADICADO_RECIBIDO,
			                    F_PETICION,
			                    CLASE_PETICION, 
			                    TIPO_RESPUESTA,
			                    F_RESPUESTA,
			                    RADICADO_RESPUESTA,
			                    FECHA_EJECUCION,
			                    PERIODO_SIR PERIODO,
			                    COD_EMPRESA EMPRESA,
			                    PMNIU.NIU,
                                RC.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_RECONEXION RC, (SELECT niu, nis_rad FROM TB_CONSUMOS_NF_CELSIA_REL_NIS_NIU WHERE PERIODO  between CONVERT(DATETIME, @fecha_ini) and CONVERT(DATETIME, @fecha_fin) group by niu,nis_rad) PMNIU
		                    WHERE  
                                RC.NIS_RAD = PMNIU.NIS_RAD
		                    UNION
		                    SELECT 
                                TIPO_RECLAMACION,
			                    SL.NIS_RAD,
			                    CODIGO_DANE,
			                    RADICADO_RECIBO RADICADO_RECIBIDO,
			                    F_PETICION,
			                    CLASE_PETICION,
			                    TIPO_RESPUESTA,
			                    F_RESPUESTA,
			                    RADICADO_RESPUESTA,
			                    FECHA_EJECUCION,
			                    PERIODO_SIR PERIODO,
			                    COD_EMPRESA EMPRESA,
			                    PMNIU.NIU,
                                SL.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_SOLICITADAS SL, (SELECT niu, nis_rad FROM TB_CONSUMOS_NF_CELSIA_REL_NIS_NIU WHERE PERIODO  between CONVERT(DATETIME, @fecha_ini) and CONVERT(DATETIME, @fecha_fin) group by niu,nis_rad) PMNIU
		                    WHERE  
                                SL.NIS_RAD = PMNIU.NIS_RAD
";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 16,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("ETL EXTR")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "ETLTC4",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                ConsultaFinal = ""
            });
            //000. Crea tabla:TB_TC4_CELSIA_ETL_EXTR
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 17,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("ETL EXTR")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });

            #endregion 

            #region EXTRACCCION
            query = @"SELECT 
                        REG.TIPO_RECLAMACION, 
					    REG.NIU, 
					    REG.CODIGO_DANE, 
					    REG.RADICADO_RECIBIDO, 
					    REG.F_PETICION, 
					    REG.CLASE_PETICION, 
					    REG.TIPO_RESPUESTA, 
					    REG.F_RESPUESTA, 
					    REG.RADICADO_RESPUESTA, 
					    REG.FECHA_EJECUCION, 
					    REG.PERIODO, 
					    REG.EMPRESA,
                        REG.ID_MERCADO
		            FROM   
                        TB_TC4_CELSIA_ETL_EXTR REG";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 18,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTRACCION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = query
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTRACCION
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 19,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTRACCION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });


            //SEGUNDA PARTE
            query = @"SELECT 
                                reg.TIPO_RECLAMACION, 
                                reg.NIU, 
                                reg.CODIGO_DANE, 
                                reg.RADICADO_RECIBIDO, 
                                reg.F_PETICION,
					            reg.CLASE_PETICION, 
                                reg.TIPO_RESPUESTA, 
                                reg.F_RESPUESTA, 
                                reg.RADICADO_RESPUESTA,
					            reg.FECHA_EJECUCION,
                                PERIODO_SIR PERIODO,
                                reg.COD_EMPRESA EMPRESA,
                                reg.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_EVENTUALES reg";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 20,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTRACCION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = query
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTRACCION
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 21,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("EXTRACCION")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });

            //TERCERA PARTE
            query = @"SELECT 
                                reg.TIPO_RECLAMACION, 
                                reg.NIU, 
                                reg.CODIGO_DANE, 
                                reg.RADICADO_RECIBIDO, 
                                reg.F_PETICION,
					            reg.CLASE_PETICION, 
                                reg.TIPO_RESPUESTA, 
                                reg.F_RESPUESTA, 
                                reg.RADICADO_RESPUESTA,
					            reg.FECHA_EJECUCION,
                                PERIODO_SIR PERIODO,
                                reg.COD_EMPRESA EMPRESA,
                                reg.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_EVENTUALES reg";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 22,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = query
            });
            //000. Crea tabla:TB_TC4_CELSIA_EXTRACCION
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 23,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });

            var archivos = ContextArchivo.ObtenerArchivos(new MODArchivoFiltro { 
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte).Id 
            });

            //TODO:consulta con filtro pa la empresa que necesitas  TC4_CETSA
            query = @"SELECT reg.TIPO_RECLAMACION, 
                                reg.NIU, 
                                reg.CODIGO_DANE, 
                                reg.RADICADO_RECIBIDO, 
                                reg.F_PETICION,
					            reg.CLASE_PETICION, 
                                reg.TIPO_RESPUESTA, 
                                reg.F_RESPUESTA, 
                                reg.RADICADO_RESPUESTA,
					            reg.FECHA_EJECUCION,
                                PERIODO_SIR PERIODO,
                                reg.COD_EMPRESA EMPRESA,
                                reg.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_EVENTUALES reg";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 24,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = query
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 25,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte).Id,
                IdGrupoEjecucion = grupoEjecuciontranformacion,
                IdArchivo = archivos.FirstOrDefault(arh => arh.Nombre.Equals("TC4_CETSA")).IdArchivo,
                Agrupador = "Transformación",
                IdCategoria = idCategoriaSUI
            });
            //TODO:consulta con filtro pa la empresa que necesitas   TC4_EPSA
            query = @"SELECT reg.TIPO_RECLAMACION, 
                                reg.NIU, 
                                reg.CODIGO_DANE, 
                                reg.RADICADO_RECIBIDO, 
                                reg.F_PETICION,
					            reg.CLASE_PETICION, 
                                reg.TIPO_RESPUESTA, 
                                reg.F_RESPUESTA, 
                                reg.RADICADO_RESPUESTA,
					            reg.FECHA_EJECUCION,
                                PERIODO_SIR PERIODO,
                                reg.COD_EMPRESA EMPRESA,
                                reg.ID_MERCADO
		                    FROM   
                                TB_TC4_CELSIA_EXTR_EVENTUALES reg";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 26,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = query
            });
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 27,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Archivo,
                IdFlujo = 0,
                IdPadre = 0,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC4") && rep.EsReporte).Id,
                IdGrupoEjecucion = grupoEjecuciontranformacion,
                IdArchivo = archivos.FirstOrDefault(arh => arh.Nombre.Equals("TC4_EPSA")).IdArchivo,
                Agrupador = "Transformación",
                IdCategoria = idCategoriaSUI
            });

            #endregion 



            registro.nuevo();
            contextFlujo.Registrar(registro);
        }

        public override void Generar(bool crear = false)
        {
            Inicializar();
            if (crear)
            {
                Estructuras();
                //Archivos();
                Flujo();
            }

            Ejecutar();
        }

        public override void Inicializar()
        {
            var empresas = ConfiguracionGeneral.ConsultarEmpresa();
            idEmpresa = empresas.FirstOrDefault().IdEmpresa;
            idServicio = empresas.FirstOrDefault().Servicios.FirstOrDefault().IdServicio;
        }       
    }
}
