using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodivaServiceWS.Model
{
    public class TBPessoaDevedora
    {
        [Key]
        public int CO_SEQ_PESSOA_DEVEDORA { get; set; }
        public int CO_CIDADE { get; set; }
        public string DS_BAIRRO_PESSOA { get; set; }
        public string DS_ENDERECO_PESSOA { get; set; }
        public DateTime DT_ALTERACAO { get; set; }        
        public string NO_PESSOA { get; set; }
        public string NU_AGENCIA { get; set; }
        public string NU_CEP_PESSOA { get; set; }
        public string NU_CONTA_CORRENTE { get; set; }
        public string NU_CPF_CNPJ { get; set; }
        public string NU_FAX_PESSOA { get; set; }
        public string NU_FONE_PESSOA { get; set; }
        public string TP_PESSOA { get; set; }        
    }
}