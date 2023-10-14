using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagamentoDebitoService.Email
{
    public class EnvioEmailViewModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CallbackUrl { get; set; }
    }
}
