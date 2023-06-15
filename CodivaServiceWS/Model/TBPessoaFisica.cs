using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodivaServiceWS.Model
{
    public class TBPessoaFisica
    {
        [Key]
        public string ID_PESSOA_FISICA { get; set; }
        public string CO_CIDADE_NATURALIDADE { get; set; }
        public string CO_PAIS_NACIONALIDADE { get; set; }
        public string DS_AUDITORIA { get; set; }
        public string DT_NASCIMENTO { get; set; }        
        public string NO_CIDADE_NATURALIDADE { get; set; }
        public string NO_CONJUGUE { get; set; }
        public string NO_MAE { get; set; }
        public string NO_PAI { get; set; }
        public string NO_PESSOA_FISICA { get; set; }
        public string NU_CNVS { get; set; }
        public string NU_CPF { get; set; }
        public string TP_CIDADANIA { get; set; }
        public string TP_ESTADO_CIVIL { get; set; }
        public string TP_NACIONALIDADE { get; set; }
        public string TP_ORIGEM_ETNICA { get; set; }
        public string TP_SANGUE_RH { get; set; }
        public string TP_SEXO { get; set; }
    }
}