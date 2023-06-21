using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Enum
{
    public class Enum
    {
        public enum TipoPessoa
        {
            PessoaFisica=11,
            PessoaJuridica=14
        }

        public enum SistemaOrigem
        {
            [Description("SEI")]
            SEI
        }

        public enum TipoDebito
        {
            [Description("Auto de Infração")]
            AutoInfracao = 1,
            [Description("Cheque devolvido")]
            ChequeDevolvido = 14
        }

        public enum Receita
        {
            [Description("0123")]
            CobrancaMulta
        }

        public enum UnidadeArrecadadora
        {
            [Description("10707 (DF)")]
            DistritoFederal = 10707
        }
    }
}
