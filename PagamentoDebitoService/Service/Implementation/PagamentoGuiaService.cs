using PagamentoDebitoService.Dapper.Interface;
using PagamentoDebitoService.DTO;
using PagamentoDebitoService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagamentoDebitoService.Service.Implementation
{
    public class PagamentoGuiaService : IPagamentoGuiaService
    {
        public IPagamentoGuiaDapper _pagamentoGuiaDapper { get; set; }

        public PagamentoGuiaService(IPagamentoGuiaDapper pagamentoGuiaDapper)
        {
            _pagamentoGuiaDapper = pagamentoGuiaDapper ?? throw new ArgumentNullException(nameof(pagamentoGuiaDapper));
        }

        public IEnumerable<DadosGuiaDto> ObterGuiasPagas(string connectionString)
        {
            return _pagamentoGuiaDapper.ObterGuiasPagas(connectionString);
        }

        public IEnumerable<DadosGuiaDto> ObterGuiasVencidas(string connectionString)
        {
            return _pagamentoGuiaDapper.ObterGuiasVencidas(connectionString);
        }
    }
}
