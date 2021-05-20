using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Reportes;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.FlujoDeTrabajo;
using SIR.Negocio.Interfaces.Reporte;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectoPruebas.Preparacion_de_ambiente
{
    public static class ReportesSUI
    {
        private const int idUsuario = 27;
        private const string ip = "::1";
        private const string usuario = "jatencia";

        private static int idEmpresa;
        private static int idServicio;
        private static int idCategoriaSUI = 2;//Esto se obtine de la tabla [dbo].[CategoriasFlujos]
        private static int grupoEjecucionEstraccion = 12;//Esto se obtine de la tabla [dbo].[GruposDeEjecucion] -- Es configuracion interna.
        private static int grupoEjecuciontranformacion = 13;//Esto se obtine de la tabla [dbo].[GruposDeEjecucion] -- Es configuracion interna.

        private static IReporteNegocio contextReporte = FabricaNegocio.CrearReporteNegocio;
        private static IFlujoTrabajoNegocio contextFlujo = FabricaNegocio.CrearFlujoTrabajoNegocio;

        static void Inicializar()
        {
            var empresas = ConfiguracionGeneral.ConsultarEmpresa();
            idEmpresa = empresas.FirstOrDefault().IdEmpresa;
            idServicio = empresas.FirstOrDefault().Servicios.FirstOrDefault().IdServicio;
        }
        static IEnumerable<MODReporte> ObtenerEstructuras()
        {
            return contextReporte.Obtener(new MODReporteFiltro());
        }

        static void TC3_Estructuras()
        {
            MODReporte estructura = new MODReporte()
            {
                Descripcion = "Extraer consumos peajes nr ap",
                Nombre = "TC3 CONSUMOS PEAJE",
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
                        Nombre = "NIC",
                        nombreVisible = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "CONSUMO",
                        nombreVisible = "CONSUMO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "NIT",
                        nombreVisible = "NIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "SUMI_CODIGO",
                        nombreVisible = "SUMI_CODIGO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "ID_COMERCIALIZADOR",
                        nombreVisible = "ID_COMERCIALIZADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 5,
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
                Descripcion = "Trae todos los clientes No Regulados independiente de su estado",
                Nombre = "TC3 AUX SUMCON NR",
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
                        Nombre = "NIC",
                        nombreVisible = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "COD_TAR",
                        nombreVisible = "COD_TAR",
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
                        Nombre = "TIP_TENSION",
                        nombreVisible = "TIP_TENSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "CODIGO_SIC",
                        nombreVisible = "CODIGO_SIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "BOCAS_CONTRAINCENDIO",
                        nombreVisible = "BOCAS_CONTRAINCENDIO",
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
                        Nombre = "NIF",
                        nombreVisible = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "EST_SUM",
                        nombreVisible = "EST_SUM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 7,
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
                Descripcion = "Extraer regulados reporte TC3 y F1",
                Nombre = "TC3 REGULADOS",
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
                        Nombre = "NIC",
                        nombreVisible = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "NIF",
                        nombreVisible = "NIF",
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
                        Nombre = "ID_COMERCIALIZADOR",
                        nombreVisible = "ID_COMERCIALIZADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "NIV_TENSION",
                        nombreVisible = "NIV_TENSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "CARGO_INVERSION",
                        nombreVisible = "CARGO_INVERSION",
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
                        Nombre = "CONEXION_RED",
                        nombreVisible = "CONEXION_RED",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "SIC",
                        nombreVisible = "SIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
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
                        Nombre = "COD_TAR",
                        nombreVisible = "COD_TAR",
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
                        Nombre = "EST_SUM",
                        nombreVisible = "EST_SUM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
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
                Descripcion = "Extraer peajes nr ap",
                Nombre = "TC3 PEAJES NR AP",
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
                        Nombre = "NIC",
                        nombreVisible = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "NIV_TENSION",
                        nombreVisible = "NIV_TENSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "NIF",
                        nombreVisible = "NIF",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "CODIGO_SIC",
                        nombreVisible = "CODIGO_SIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "ID_COMERCIALIZADOR",
                        nombreVisible = "ID_COMERCIALIZADOR",
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
                        Nombre = "CARGO_INVERSION",
                        nombreVisible = "CARGO_INVERSION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "CONEXION_RED",
                        nombreVisible = "CONEXION_RED",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "EMPRESA",
                        nombreVisible = "EMPRESA",
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
                        Nombre = "COD_TAR",
                        nombreVisible = "COD_TAR",
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
                        Nombre = "EST_SUM",
                        nombreVisible = "EST_SUM",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 10,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    } ,
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "SIC",
                        nombreVisible = "SIC",
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
                Descripcion = "Extraer tension primaria",
                Nombre = "TC3 TEN PRIMARIA",
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
                        Nombre = "NIS_RAD",
                        nombreVisible = "NIS_RAD",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "CODIGO_CONEXION",
                        nombreVisible = "CODIGO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "18,8",
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
                        Nombre = "CODIGO_CONEXION_UNICO",
                        nombreVisible = "CODIGO_CONEXION_UNICO",
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
                        Nombre = "TIPO_CONEXION",
                        nombreVisible = "TIPO_CONEXION",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "NIVEL_TENSION_PRI",
                        nombreVisible = "NIVEL_TENSION_PRI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "GRUPO_CALIDAD",
                        nombreVisible = "GRUPO_CALIDAD",
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
                        Nombre = "CODIGO_CONEXION_ORIGINAL",
                        nombreVisible = "CODIGO_CONEXION_ORIGINAL",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 6,
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
                Descripcion = "Reporte TC3 - DECRETO ## - YYYY",
                Nombre = "TC3",
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
                        Nombre = "NIC",
                        nombreVisible = "NIC",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "NIT",
                        nombreVisible = "NIT",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
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
                        Nombre = "CONSUMO",
                        nombreVisible = "CONSUMO",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "18,8",
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
                        Nombre = "ID_COMERCIALIZADOR",
                        nombreVisible = "ID_COMERCIALIZADOR",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "50",
                        Ordinal = 3,
                        esVersionable = false,
                        esEditable = false,

                        IdEmpresa = idEmpresa,
                        IdServicio = idServicio
                    }
                }
            };
            contextReporte.Registrar(estructura);
        }
        static void TC3_Flujo()
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
                IdElemento = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3")).Id,
                Elemento = "TC3",
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                Tipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
                SubTipo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumSubTipoFlujo.SUI,
                Tareas = new LinkedList<MODTarea>(),
                Prerequisitos = new List<MODFlujoPrerequisito>(), 
                Categoria = "SUI"
            };

            //000. Crea tabla:TB_TC3_CELSIA_AUX_SUMCON_NR
            //C_EXTR_AUX_SUMCON_NR_TC3
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3 AUX SUMCON NR")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Ejecutar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos(),
                ConsultaFinal = @"TRUNCATE TABLE TB_TC3_CELSIA_AUX_SUMCON_NR"
            }); 
            string query = @"select nic, nis_rad,cod_Tar,tip_tension,codigo_sic,bocas_contraincendio,nif,est_sum
                           from sumcon
                           where tip_Serv='SV120' and est_sum<>'EC000' AND cod_tar <>'ST4' AND cgv_sum!='-' and
                           (substr(lpad(bocas_contraincendio,9,0),4,2)='00' or substr(lpad(bocas_contraincendio,9,0),4,2)='10') 
                           and cod_tar not in('PC2','PE2')
                           UNION
                           select  h.nic, sc.nis_rad, sc.cod_Tar, sc.tip_tension, sc.codigo_sic, sc.bocas_contraincendio, sc.nif, sc.est_sum
                           from sumcon sc, (select nic, max(nis_Rad) nis
                                from hsumcon
                                where tip_Serv='SV120' AND cod_tar <>'ST4' AND cgv_sum!='-' and
                                (substr(lpad(bocas_contraincendio,9,0),4,2)='00' or substr(lpad(bocas_contraincendio,9,0),4,2)='10')
                                and f_baja<to_date('29991231','yyyymmdd') and cod_tar not in('PC2','PE2')
                                group by nic) h
                                where sc.nis_Rad=h.nis and sc.est_sum='EC000' and sc.tip_Serv='SV120'";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 0,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3 AUX SUMCON NR")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "AUX SUMCON NR",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },


            });

            //001. Crea tabla:Temp_Combinacion_TC3_CONSUMOS_PEAJE   
            query = @"SELECT M.CODIGO_NIC NIC, '' NIS_RAD, M.CONSUMO, '0' AS NIT, '0' AS SUMI_CODIGO, M.ID_COMERCIALIZADOR AS ID_COMERCIALIZADOR
                            FROM  M06V_IOR_SIR M
                            WHERE M.PERIODO = @periodo";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 1,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3 CONSUMOS PEAJE")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "CONSUMOS PEAJE",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                 ConsultaFinal = @"SELECT T0.NIC, T0.NIS_RAD, M.CONSUMO, '0' AS NIT, '0' AS SUMI_CODIGO, M.ID_COMERCIALIZADOR AS ID_COMERCIALIZADOR
                                FROM TB_TC3_CELSIA_AUX_SUMCON_NR T0
                                JOIN Temp_Combinacion_TC3_CONSUMOS_PEAJE M ON T0.NIC = M.NIC"
            });
            //001. Crea tabla:TB_TC3_CELSIA_TC3_CONSUMOS_PEAJE
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3 CONSUMOS PEAJE")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });

            //003. Crea tabla:TB_TC3_CELSIA_REGULADOS
            //C_EXTR_TC3_F1_REGULADOS
            query = @"SELECT sc.nic nic, sc.nis_rad nis_rad, sc.nif nif, to_char(decode(substr(lpad(sc.bocas_contraincendio,9,0),1,3),'000','536','100','637')) Id_comercializador,
                     sc.tip_tension niv_tension, substr(lpad(sc.bocas_contraincendio,9,0),9,1) cargo_inversion, substr(lpad(sc.bocas_contraincendio,9,0),8,1) conexion_red,
                     sc.codigo_sic sic, substr(lpad(SC.bocas_contraincendio,9,'0'),1,3) empresa, sc.cod_Tar, sc.est_sum
                     FROM   sumcon sc
                     WHERE sc.tip_Serv='SV100' AND sc.est_sum<>'EC000' AND (substr(lpad(sc.bocas_contraincendio,9,0),4,2)='00' or
                     substr(lpad(sc.bocas_contraincendio,9,0),4,2)='10')
                     union
                     select distinct nic,NIS_RAD, nif, ID_COMERCIALIZADOR, NIV_TENSION,CARGO_INVERSION,CONEXION_RED,SIC,EMPRESA,cod_tar,est_sum
                     from
                     (select sc.nic as nic, sc.nif as nif, to_char(decode(substr(lpad(sc.bocas_contraincendio,9,0),1,3),'000','536','100','637')) Id_comercializador,
                      sc.tip_tension niv_tension, substr(lpad(sc.bocas_contraincendio,9,0),9,1) cargo_inversion, substr(lpad(sc.bocas_contraincendio,9,0),8,1) conexion_red,
                      sc.nis_rad nis_rad, trim(sc.codigo_sic) sic, decode(substr(lpad(sc.bocas_contraincendio,9,0),4,2),'00','000','10','100') empresa,
                      sc.cod_Tar, sc.est_sum
                      from  sumcon sc,recibos rc
                      where sc.nis_RAD=rc.nis_rad and sc.tip_serv='SV100' AND sc.est_sum='EC000' AND (substr(lpad(sc.bocas_contraincendio,9,0),4,2)='00' or 
                      substr(lpad(sc.bocas_contraincendio,9,0),4,2)='10') AND TO_CHAR(rc.f_puesta_cobro, 'YYYYMM') = @periodo)";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3 REGULADOS")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "REGULADOS",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },


            });

            //004. Crea tabla:Temp_Combinacion_TC3_PEAJES_NR_AP
            query = @"SELECT '' AS NIF, M.CODIGO_NIC AS NIC, M.ID_COMERCIALIZADOR AS ID_COMERCIALIZADOR, '' NIV_TENSION,
                    '' CARGO_INVERSION, '' CONEXION_RED,
                    '' NIS_RAD, M.FRONTERA SIC, '' EMPRESA,
                    '' COD_TAR, '' EST_SUM, '' CODIGO_SIC
                    FROM  M06V_IOR_SIR M
                    WHERE M.PERIODO = @periodo";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 5,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3 PEAJES NR AP")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Combinacion,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "PEAJES NR AP",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                },
                ConsultaFinal = @"SELECT SC.NIF, SC.NIC, M.ID_COMERCIALIZADOR, SC.TIP_TENSION, SUBSTRING(RIGHT('000000000' + SC.BOCAS_CONTRAINCENDIO, 9), 9, 1) CARGO_INVERSION,
                                SUBSTRING(RIGHT('000000000' + SC.BOCAS_CONTRAINCENDIO, 9), 8, 1) CONEXION_RED, SC.NIS_RAD, M.SIC,
                                CASE WHEN SUBSTRING(RIGHT('000000000' + SC.BOCAS_CONTRAINCENDIO, 9), 4, 2) = '00' THEN '000' WHEN SUBSTRING(RIGHT('000000000' + SC.BOCAS_CONTRAINCENDIO, 9), 4, 2) = '10' THEN '100' ELSE '' END EMPRESA,
                                SC.COD_TAR, SC.EST_SUM, SC.CODIGO_SIC
                                FROM TB_TC3_CELSIA_AUX_SUMCON_NR SC
                                JOIN Temp_Combinacion_TC3_PEAJES_NR_AP  M ON SC.NIC = M.NIC"
            });
            //004. Crea tabla:TB_TC3_CELSIA_TC3_PEAJES_NR_AP
            //C_EXTR_CONSUMOS_PEAJE_NR_AP
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 2,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3 PEAJES NR AP")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Registrar,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
            });

            //005. Crea tabla:TB_TC3_CELSIA_TEN_PRIMARIA
            query = @"select sc.nis_rad nis_rad, case when sc.cod_tar in ('AA1','AP1','APC','SA1','SA2','SA5','SA6') then 'ALPM0001'
                    when sc.cod_tar in ('SO1','SO5') and f.tip_fin ='TF045' then 'ALPM0001' else to_char(trafo_ct.codigo_trafo) end as  codigo_conexion,
                    case when sc.cod_tar in ('AA1','AP1','APC','SA1','SA2','SA5','SA6') then 'ALPM0001' when sc.cod_tar in ('SO1','SO5') and f.tip_fin ='TF045' then 'ALPM0001' else trafo_ct.sspdid end as codigo_conexion_unico,
                    'T' tipo_conexion,
                    CASE WHEN sc.tip_tension = 'TT001' OR sc.tip_tension = 'TT002' THEN trafo_ct.niv_tension_pri_trafo else 'Default' end as niv_tension_pri,
                    trafo_ct.grupo_calidad_trafo as grupo_calidad, trafo_ct.codigo_trafo as codigo_conexion_original
                    FROM (select a.codigo as codigo_trafo, a.sspdid, a.tens_pri niv_tension_pri_trafo, a.grupo grupo_calidad_trafo, SUBSTR(A.INSTALACION_ORIGEN_V10, INSTR(A.INSTALACION_ORIGEN_V10, ':', 1)+1) ins_origen_trafo,
                        b.codigo as codigo_ct, SUBSTR(b.INSTALACION_ORIGEN_V10, INSTR(b.INSTALACION_ORIGEN_V10, ':', 1)+1) ins_origen_ct
                        from   bdiv10_sgd_ct b, bdiv10_sgd_trafo_mb a
                        where substr(A.INSTALACION_ORIGEN_V10, INSTR(A.INSTALACION_ORIGEN_V10, ':', 1)+1) = b.codigo
                        and   a.onis_stat = '0' and instr(a.onis_ver,'.')=0 and b.onis_stat = '0' and instr(b.onis_ver,'.') = 0 ) trafo_ct,
                        fincas f, gi_cliente_instalacion ci,
                        (select nis_rad, nif, cod_tar, tip_tension from sumcon A where (tip_serv='SV100' or tip_serv='SV120') and cod_tar!='ST4' and cgv_sum!='-') sc
                    where sc.nif=f.nif(+) and sc.nis_Rad=ci.nis_rad(+) and ci.codigo_trafo_mb=trafo_ct.codigo_trafo(+)";
            registro.Tareas.AddLast(new MODTarea
            {
                Id = 0,
                Orden = 4,
                IdFlujo = 0,
                IdPadre = 0,
                IdGrupoEjecucion = grupoEjecucionEstraccion,
                IdCategoria = idCategoriaSUI,
                IdReporte = estructuras.FirstOrDefault(rep => rep.Nombre.Equals("TC3 TEN PRIMARIA")).Id,
                Proceso = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumProceso.Obtener,
                Agrupador = "Extracción",
                ConfiguracionBD = new MODOrigenDatos()
                {
                    id = 0,
                    IdTarea = 0,
                    Nombre = "TEN PRIMARIA",
                    UsuarioBD = "sir_adminis",
                    ClaveBD = "sir_adminis",
                    Sid = "SIRDEV",
                    Servidor = "PV30065",
                    Puerto = "1521",
                    tipoMando = System.Data.CommandType.Text,
                    TipoOrigen = SIR.Comun.Enumeradores.EnumBaseDatosOrigen.Oracle,
                    consulta = query
                }
            });

            registro.nuevo();
            contextFlujo.Registrar(registro);
        }

        static void TC3_Ejecutar()
        {
            var reporte = contextReporte.Obtener(new MODReporteFiltro { Nombre = "TC3", IdEmpresa = idEmpresa, IdServicio = idServicio });
            var flujo = contextFlujo.Obtener(new MODFlujoFiltro());
            contextFlujo.Ejecutar(new MODFlujoFiltro
            {
                Id = 2,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = reporte.FirstOrDefault(rep => rep.EsReporte).Id,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
                Periodo = new DateTime(2020, 3, 1),
                DatoPeriodo = 0,
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = flujo.FirstOrDefault(ele => ele.Elemento.Equals("TC3")).Tareas.FirstOrDefault().IdGrupoEjecucion
            });
        }
        public static void GenerarTC3(bool crear = false)
        {
            Inicializar();
            if (crear)
            {
                TC3_Estructuras();
                TC3_Flujo();
            }
            
            TC3_Ejecutar();
        }
    }
}
