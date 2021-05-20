using ProjectoPruebas.Interfaces;
using SIR.Comun.Entidades;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectoPruebas.Formatos
{
    public class CS3 : FormatoSUI
    {
        public override void Ejecutar()
        {
            var reporte = contextReporte.Obtener(new MODReporteFiltro { Nombre = "CS3", IdEmpresa = idEmpresa, IdServicio = idServicio });
            contextFlujo.Ejecutar(new MODFlujoFiltro
            {
                Id = 4,
                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                IdElemento = reporte.FirstOrDefault(rep => rep.EsReporte).Id,
                TipoFlujo = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumTipo.Manual,
                Periodo = new DateTime(2020, 3, 1),
                DatoPeriodo = 0,
                Periodicidad = SIR.Comun.Enumeradores.FlujoDeTrabajo.EnumPeriodicidad.mensual,
                IdGrupoEjecucion = 7
            });
        }
        public override void Estructuras()
        {
            MODReporte estructura = new MODReporte()
            {
                Descripcion = "cs3 limites",
                Nombre = "extr_cs3_limites",
                IdCategoria = idCategoriaSUI,
                EsReporte = false,
                Activo = true,
                ActivoEmpresa = true,

                IdEmpresa = idEmpresa,
                IdServicio = idServicio,
                //ToDo esta tabla tiene pendiente obtner informacion de GRUPO_EST_SIR
                campos = new List<MODCampos>()
                {
                    new MODCampos()
                    {
                        Id = 0,
                        IdReporte = 0,
                        Nombre = "saidi",
                        nombreVisible = "saidi",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "12,3",
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
                        Nombre = "saifi",
                        nombreVisible = "saifi",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "12,3",
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
                        Nombre = "bnd_indif_inf_saidi",
                        nombreVisible = "bnd_indif_inf_saidi",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "bnd_indif_sup_saidi",
                        nombreVisible = "bnd_indif_sup_saidi",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "bnd_indif_inf_saifi",
                        nombreVisible = "bnd_indif_inf_saifi",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "bnd_indif_sup_saifi",
                        nombreVisible = "bnd_indif_sup_saifi",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "op_red",
                        nombreVisible = "op_red",
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
                        Nombre = "empresa",
                        nombreVisible = "empresa",
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
                Descripcion = "Reporte CS3 - DECRETO ## - YYYY",
                Nombre = "CS3",
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
                        Nombre = "INCD1",
                        nombreVisible = "INCD1",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "INCD2",
                        nombreVisible = "INCD2",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "INCD3",
                        nombreVisible = "INCD3",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "INCF1",
                        nombreVisible = "INCF1",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "INCF2",
                        nombreVisible = "INCF2",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "INCF3",
                        nombreVisible = "INCF3",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "IC_SAIDI",
                        nombreVisible = "IC_SAIDI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "IC_SAIFI",
                        nombreVisible = "IC_SAIFI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "IF_SAIFI",
                        nombreVisible = "IF_SAIFI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "IF_SAIDI",
                        nombreVisible = "IF_SAIDI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "IV_SAIFI",
                        nombreVisible = "IV_SAIFI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Nombre = "IV_SAIDI",
                        nombreVisible = "IV_SAIDI",
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._decimal,
                        Largo = "15,0",
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
                        Tipo = SIR.Comun.Enumeradores.EnumTipoDato._string,
                        Largo = "5",
                        Ordinal = 12,
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
                        Largo = "50",
                        Ordinal = 13,
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
                        Ordinal = 14,
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
            base.Flujo();
        }
        public override void Generar(bool crear = false)
        {
            base.Generar(crear);
        }
        public override void Inicializar()
        {
            base.Inicializar();
        }
        public override IEnumerable<MODReporte> ObtenerEstructuras()
        {
            return base.ObtenerEstructuras();
        }
    }
}
