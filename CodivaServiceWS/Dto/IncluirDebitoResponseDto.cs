using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dto
{
    //public class incluirDebitoResponseDto
    //{
    //    public long CodigoReceita { get; set; }
    //    public long IdContrato { get; set; }
    //    public string LinkBoleto { get; set; }
    //    public long NumeroSequencial { get; set; }
    //    public string NumeroFistel { get; set; }
    //}

    public class IncluirDebitoResponseDto
    {
        public long CodigoReceita { get; set; }
        public long IdContrato { get; set; }
        public string LinkBoleto { get; set; }
        public long NumeroSequencial { get; set; }
        public string NumeroFistel { get; set; }
    }
}
