using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagamentoDebitoService.DTO
{
    public class DadosGuiaDto
    {
        public string NumeroProcesso { get; set; }
        public string NumeroReferencia { get; set; }
        public string NossoNumero { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorPago { get; set; }
    }
}
