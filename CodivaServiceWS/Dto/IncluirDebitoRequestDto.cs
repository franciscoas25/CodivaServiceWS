using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dto
{
    public class IncluirDebitoRequestDto
    {
        public string cpf_cnpj { get; set; }
        public string sistemaOrigem { get; set; }
        public string tipoDebito { get; set; }
        public string numDocumento { get; set; }
        public string anoDocumento { get; set; }
        public string numProcesso { get; set; }
        public string gerencia { get; set; }
        public string nomePessoa { get; set; }
        public string receita { get; set; }
        public int unidadeArrecadadora { get; set; }
        public string dataMulta { get; set; }
        public string valorMulta { get; set; }
    }
}
