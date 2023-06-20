﻿using CodivaServiceWS.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Service.Interface
{
    public interface IPessoaAutuadaService
    {
        bool VerificarExistenciaPessoaAutuada(string cpf_cnpj);
        bool IncluirPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string nmCidade, int codCidade);
        bool AtualizarPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string nmCidade, int codCidade);
        int ObterCodigoPessoaAutuada(string cpf_cnpj);
        bool ValidaPessoaAutuada(string cpf_cnpj, string nome_razaoSocial, string endereco, string cep, string municipio);
        PessoaAutuadaDto ObterDadosPessoaFisicaBaseDbCorporativo(string cpf_cnpj);
        PessoaAutuadaDto ObterDadosPessoaJuridicaBaseDbCorporativo(string cpf_cnpj);
    }
}
