﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dto
{
    public class PessoaAutuadaDto
    {
        public string NOME_RAZAOSOCIAL { get; set; }
        public string ENDERECO { get; set; }
        public string CEP { get; set; }
        public int COD_CIDADE { get; set; }
        public string NM_CIDADE { get; set; }
    }
}
