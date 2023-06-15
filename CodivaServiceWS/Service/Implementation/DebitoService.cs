using CodivaServiceWS.Dapper.Interface;
using CodivaServiceWS.Service.Interface;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Service.Implementation
{
    public class DebitoService : IDebitoService
    {
        [Inject]
        public IDebitoDapper _debitoDapper { get; set; }

        [Inject]
        public IBaseDapper _baseDapper { get; set; }

        public bool IncluirDebito(int tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, int codigoDecisao, string nomePessoa, string receita, string unidadeArrecadadora, string dataInicialAtualizacaoMonetaria, string valorInicialAtualizacaoMonetaria, string dataVencimento, string banco, string agencia, string conta)
        {
            //ObterCodigoPessoaDevedoraPorNome (TB_DEBITO -> CO_PESSOA_DEVEDORA)
            //string sql = $"SELECT CO_SEQ_PESSOA_DEVEDORA FROM DBCODIVA.TB_PESSOA_DEVEDORA WHERE NO_PESSOA = '{nomePessoaAutuada}'";

            //var codigoPessoaDevedora = _baseDapper.ObterCodigoPessoaDevedoraPorNome(sql);



            
            //sql = $"INSERT INTO DBCODIVA.TB_DEBITO () VALUES ()";
            
            //_debitoDapper.IncluirDebito(sql);

            return true;
        }

        public bool ValidaDebito(int tipoDebito, string numDocumento, string anoDocumento, string numProcesso, string gerencia, int codigoDecisao, string nomePessoa, string receita, string unidadeArrecadadora, string dataInicialAtualizacaoMonetaria, string valorInicialAtualizacaoMonetaria, string dataVencimento, string banco, string agencia, string conta)
        {
            return tipoDebito > 0 &&
                   codigoDecisao > 0 &&
                   !string.IsNullOrEmpty(numDocumento) && 
                   !string.IsNullOrEmpty(anoDocumento) && 
                   !string.IsNullOrEmpty(numProcesso) && 
                   !string.IsNullOrEmpty(gerencia) && 
                   !string.IsNullOrEmpty(nomePessoa) && 
                   !string.IsNullOrEmpty(receita) && 
                   !string.IsNullOrEmpty(unidadeArrecadadora) &&                   
                   !string.IsNullOrEmpty(valorInicialAtualizacaoMonetaria) &&
                   DateTime.TryParse(dataInicialAtualizacaoMonetaria, out DateTime result) &&
                   DateTime.TryParse(dataVencimento, out result) &&
                   tipoDebito == 14 ? 
                   (!string.IsNullOrEmpty(banco) && !string.IsNullOrEmpty(agencia) && !string.IsNullOrEmpty(conta)) : 
                   1 == 1;
        }

        public bool VerificaSeDebitoEstaCadastrado(int tipoDebito, string numDocumento, string anoDocumento, string unidadeArrecadadora)
        {
            return _debitoDapper.VerificaSeDebitoEstaCadastrado(tipoDebito, numDocumento, anoDocumento, unidadeArrecadadora);
        }
    }
}
