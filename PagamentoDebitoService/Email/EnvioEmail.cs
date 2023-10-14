using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using PagamentoDebitoService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PagamentoDebitoService.Email
{
    public static class EnvioEmail
    {
        public static void Send(string lstDestinatarios, IEnumerable<DadosGuiaDto> lstGuiasPagas, string assunto, string remetente, string nomeEmail, string host, int port)
        {
            StringBuilder sb = new StringBuilder();

            MailMessage mailMessage = new MailMessage();

            try
            {
                sb.AppendLine($"<span><b>Guias pagas em {DateTime.Now.AddDays(-1).ToShortDateString()}</b></span>");
                sb.Append("<br/>");
                sb.Append("<br/>");

                if (!lstGuiasPagas.Any())
                    sb.AppendLine("Não foram constatados pagamentos de multa para esta data");

                foreach (var guia in lstGuiasPagas)
                {
                    sb.AppendLine($"Número do processo administrativo sanitário: {guia.NumeroProcesso}");
                    sb.Append("<br/>");
                    sb.AppendLine($"Número de referência da GRU: {guia.NumeroReferencia}");
                    sb.Append("<br/>");
                    sb.AppendLine($"Data do pagamento: {guia.DataPagamento}");
                    sb.Append("<br/>");
                    sb.AppendLine($"<b>Valor pago: R$ {guia.ValorPago}</b>");
                    sb.Append("<br/>");
                    sb.Append("<br/>");
                }

                var smtpClient = new SmtpClient(host, port);
                smtpClient.EnableSsl = true;
                smtpClient.Timeout = 7200;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("franciscoas25@gmail.com", "ycfq ztwk gmyi vwst");

                mailMessage.From = new MailAddress(remetente, nomeEmail);
                mailMessage.Body = sb.ToString();
                mailMessage.Subject = assunto;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.Normal;
                mailMessage.To.Add(lstDestinatarios);

                smtpClient.Send(mailMessage);
            }
            catch(Exception ex)
            {
                return;
            }
        }
    }
}
