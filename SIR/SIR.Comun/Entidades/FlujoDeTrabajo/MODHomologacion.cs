using SIR.Comun.Enumeradores;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Comun.Funcionalidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.FlujoDeTrabajo
{
    [DataContract]
    [Serializable]
    public class MODHomologacion
    {
        [DataMember]
        [Column(Order = 1, TypeName = "ID")]
        public int Id { get; set; }
        [DataMember]
        [Column(Order = 7, TypeName = "codigo_Externo")]
        public Guid codigo_Externo { get; set; }
        [DataMember]
        [Column(Order = 2, TypeName = "IdTarea")]
        public int IdTarea { get; set; }
        [DataMember]
        [Column(Order = 8, TypeName = "codigo_Externo_tarea")]
        public Guid codigo_Externo_tarea { get; set; }
        [DataMember]
        [Column(Order = 3, TypeName = "IdCampo")]
        public int IdCampo { get; set; }
        [DataMember]
        public string NombreCampo { get; set; }

        private string lambda = null;
        
        public string Ecuacion { 
            get 
            {
                if (string.IsNullOrEmpty(lambda))
                {
                    var resultado = string.Format("(string {0}) => (",
                    string.Join(", ", Condiciones.Select(y => y.Campo).Distinct()));
                    var _subgrupo = string.Empty;
                    var esPrimerConector = 0;
                    foreach (var _niveles in Condiciones.GroupBy(y => y.Nivel).OrderBy(y => y.Key))
                    {
                        if (_niveles.Key != 0)
                        {
                            resultado += string.Format("{0}(", Conector(_niveles.First().Conector));
                            esPrimerConector = _niveles.First().Id;
                        }
                        foreach (var _campo in _niveles)
                        {
                            _subgrupo += string.Format("{0}{1}{2}",
                           esPrimerConector != _campo.Id ? Conector(_campo.Conector) : "",
                           Convertir(_campo.TipoCampo, _campo.Campo),
                           Criterio(_campo.Condicion, _campo.Valor, _campo.TipoCampo));
                        }
                        if (_niveles.Key != 0)
                        {
                            resultado += string.Format("{0})", _subgrupo);
                            _subgrupo = string.Empty;
                        }
                        else
                        {
                            resultado += string.Format("{0}", _subgrupo);
                            _subgrupo = string.Empty;
                        }
                    }
                    resultado += ")";
                    lambda = resultado;
                }
                
                return lambda;
            } 
        }
        [DataMember]
        [Column(Order = 5, TypeName = "ValorSi")]
        public string ValorSi { get; set; }
        [DataMember]
        [Column(Order = 4, TypeName = "ValorNo")]
        public string ValorNo { get; set; }
        [DataMember]
        public EnumTipoDato TipoCampo { get; set; }

        public List<MODCondiciones> Condiciones { get; set; }
        [Column(Order = 6, TypeName = "TipoReemplazo")]
        public EnumTipoReemplazo TipoReemplazo { get; set; }

        public void nuevo()
        {
            Condiciones.ForEach(x => {
                x.codigo_Externo_Homologacion = this.codigo_Externo;
            });
        }

        private string Conector (EnumConectores? enumConectores)
        {
            switch (enumConectores)
            {
                case EnumConectores._And:
                    return " && ";
                case EnumConectores._Or:
                    return " || ";
                default:
                    return "";
            }
        }

        private string Criterio(EnumCondiciones condiciones, object valor, EnumTipoDato enumConectores)
        {
            switch (condiciones)
            {
                case EnumCondiciones.Igualque:
                    return string.Format(" == {0} ", Convertir(enumConectores, valor, false));
                case EnumCondiciones.Contiene:
                    return string.Format(".Contains(\"{0}\")", valor.ToString());
                case EnumCondiciones.Mayorque:
                    return string.Format(" > {0}", Convertir(enumConectores, valor, false));
                case EnumCondiciones.Menorque:
                    return string.Format(" < {0}", Convertir(enumConectores, valor, false));
                case EnumCondiciones.Diferente:
                    return string.Format("!= {0}", Convertir(enumConectores, valor, false));
                default:
                    return "";
            }
        }

        private string Convertir(EnumTipoDato enumConectores, object Campo, bool constate = true)
        {
            switch (enumConectores)
            {
                case EnumTipoDato._datetime:
                    Campo = Campo.Equals("@fechaActual") ? DateTime.Now : Campo;
                    return constate? string.Format("Convert.ToDateTime({0})", Campo.ToString()) : 
                        string.Format("Convert.ToDateTime(\"{0}\")", Campo.ToString());
                case EnumTipoDato._int:
                    return constate ? string.Format("Convert.ToInt32({0})", Campo.ToString()) :
                        string.Format("Convert.ToInt32(\"{0}\")", Campo.ToString());
                case EnumTipoDato._bool:
                    return constate ? string.Format("Convert.ToBoolean({0})", Campo.ToString()) :
                        string.Format("Convert.ToBoolean(\"{0}\")", Campo.ToString());
                case EnumTipoDato._string:
                    return constate ? string.Format("{0}", Campo) :
                        string.Format("\"{0}\"", Campo.ToString());
                default:
                    return "";
            }
        }
    }
}
