using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Model
{
    public class DadosDebito
    {
        public string SiglaDebito { get; set; }
        public string NumProcesso { get; set; }
        public string NomePessoa { get; set; }
        public string CpfCnpj { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
    }
}
