using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodivaServiceWS.Model
{
    public class TBPessoaJuridica
    {
        [Key]
        public string ID_PESSOA_JURIDICA { get; set; }
        public string CO_CATEGORIA { get; set; }
        public string CO_CNAE_FISCAL { get; set; }
        public string CO_CNES { get; set; }
        public string CO_PORTE { get; set; }
        public string DS_AUDITORIA { get; set; }
        public string DS_HOMEPAGE { get; set; }
        public string DS_IDENTIFICACAO_CNVS { get; set; }
        public string DS_NATUREZA_JURIDICA { get; set; }
        public string DS_OBSERVACAO { get; set; }
        public string DT_ATUALIZACAO_PORTE { get; set; }        
        public string ID_PESSOA_JURIDICA_LOCALIZACAO { get; set; }
        public string ID_PESSOA_JURIDICA_PRINCIPAL { get; set; }
        public string ID_PESSOA_JURIDICAVIRTUAL { get; set; }
        public string NO_FANTASIA { get; set; }
        public string NO_RAZAO_SOCIAL { get; set; }
        public string NU_ANO_BASE_PORTE { get; set; }
        public string NU_CNPJ { get; set; }
        public string NU_CNVS { get; set; }
        public string NU_SAC { get; set; }
        public string TP_CLASSIFICACAO { get; set; }
        public string TP_COMPROVANTE { get; set; }
        public string TP_PESSOA_JURIDICA { get; set; }
    }
}