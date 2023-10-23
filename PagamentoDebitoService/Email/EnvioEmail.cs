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

                sb.AppendLine("<table>");

                sb.Append("<tr>");
                sb.Append("<th style='border: 1px solid'>Número do processo administrativo sanitário</th>");
                sb.Append("<th style='border: 1px solid'>Número de referência da GRU</th>");
                sb.Append("<th style='border: 1px solid'>Data do pagamento</th>");
                sb.Append("<th style='border: 1px solid'>Valor pago</th>");
                sb.Append("</tr>");

                foreach (var guia in lstGuiasPagas)
                {
                    sb.Append("<tr>");
                    sb.AppendLine($"<td style='border: 1px solid; text-align: right'>{guia.NumeroProcesso}</td>");
                    sb.AppendLine($"<td style='border: 1px solid; text-align: right'>{guia.NumeroReferencia}</td>");
                    sb.AppendLine($"<td style='border: 1px solid; text-align: right'>{guia.DataPagamento.ToShortDateString()}</td>");
                    sb.AppendLine($"<td style='border: 1px solid; text-align: right'>R$ {guia.ValorPago.ToString("##,###.##")}</td>");
                    sb.Append("</tr>");                    
                }

                sb.AppendLine("</table>");

                var smtpClient = new SmtpClient(host, port);                
                smtpClient.Timeout = 10000;
                smtpClient.EnableSsl = true;
                //smtpClient.Credentials = new NetworkCredential("franciscoas25@gmail.com", "ycfq ztwk gmyi vwst");
                smtpClient.Credentials = new NetworkCredential("rni@anvisa.gov.br", "");

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
                throw new Exception(ex.Message);
            }
        }
    }
}
