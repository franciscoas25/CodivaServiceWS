using PagamentoDebitoService.DTO;
using PagamentoDebitoService.Email;
using PagamentoDebitoService.Service.Interface;
using SelectPdf;

namespace PagamentoDebitoService
{
    public class PagamentoService : BackgroundService
    {
        private readonly ILogger<PagamentoService> _logger;
        private readonly IConfiguration _config;
        private string _connectionString;

        public IPagamentoGuiaService _pagamentoGuiaService { get; set; }

        private string _lstDestinatarios = string.Empty;
        private string _remetente = string.Empty;
        private string _assunto = string.Empty;
        private string _nomeEmail = string.Empty;
        private string _host = string.Empty;
        private int _port = 0;

        private readonly IHostEnvironment _hostEnvironment;

        public PagamentoService(ILogger<PagamentoService> logger, IPagamentoGuiaService pagamentoGuiaService, IConfiguration config, IHostEnvironment hostEnvironment)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pagamentoGuiaService = pagamentoGuiaService ?? throw new ArgumentNullException(nameof(pagamentoGuiaService));
            _config = config;
            _connectionString = _config.GetValue<string>("PagamentoServiceConnectioString")!;
            _hostEnvironment = hostEnvironment;

            _lstDestinatarios = _config.GetValue<string>("Email:DestinatariosEmail")!;
            _remetente = _config.GetValue<string>("Email:Remetente")!;
            _assunto = _config.GetValue<string>("Email:Assunto")!;
            _nomeEmail = _config.GetValue<string>("Email:NomeEmail")!;
            _host = _config.GetValue<string>("SmtpClient:Host")!;
            _port = _config.GetValue<int>("SmtpClient:Port")!;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var lstGuiasPagas = ObterGuiasPagas(_connectionString);

                if (lstGuiasPagas.Any())
                    GerarComprovantesPagamento(lstGuiasPagas);

                EnviarEmail(_lstDestinatarios, lstGuiasPagas, _assunto, _remetente, _nomeEmail, _host, _port);

                var lstGuiasVencidas = ObterGuiasVencidas(_connectionString);

                if (lstGuiasVencidas.Any())
                    GerarComprovantesNaoQuitacao(lstGuiasVencidas);

                EnviarEmail(_lstDestinatarios, lstGuiasVencidas, _assunto, _remetente, _nomeEmail, _host, _port);

                await Task.Delay(10000, stoppingToken);
            }
        }

        private void EnviarEmail(string lstDestinatarios, IEnumerable<DadosGuiaDto> lstGuias, string assunto, string remetente, string nomeEmail, string host, int port)
        {
            try
            {
                EnvioEmail.Send(lstDestinatarios, lstGuias, assunto, remetente, nomeEmail, _host, _port);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Serviço {GetType().Name} iniciado em {DateTime.Now.ToString("dd/MM/yyyy")}");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Serviço {GetType().Name} finalizado em {DateTime.Now.ToString("dd/MM/yyyy")}");

            return base.StopAsync(cancellationToken);
        }

        public IEnumerable<DadosGuiaDto> ObterGuiasPagas(string connectionString)
        {
            return _pagamentoGuiaService.ObterGuiasPagas(connectionString);
        }

        public IEnumerable<DadosGuiaDto> ObterGuiasVencidas(string connectionString)
        {
            return _pagamentoGuiaService.ObterGuiasVencidas(connectionString);
        }

        public void GerarComprovantesPagamento(IEnumerable<DadosGuiaDto> lstGuiasPagas)
        {
            string html = string.Empty;

            string nomeAutuado = "Francisco Araújo";
            string cpfAutuado = "058.729.052-43";
            string numeroDebito = "123456";

            string contentRootPath = _hostEnvironment.ContentRootPath;

            string pathLogoAnvisa = Path.Combine(contentRootPath, "Imagens", "logo_anvisa.png");

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            foreach (var guia in lstGuiasPagas)
            {
                html = $@"<div style='text-align: center; width: 50%; border: solid; margin: auto; padding:20px'>
                              <table style='margin: auto'>
                                 <tr><td><b>COMPROVANTE DE PAGAMENTO</b></td></tr>
                              </table>
                              <br/>
                              <table>
                                 <tr><td><b>Processo Administrativo Sanitário (PAS): 25351.xxxxxx/202x-xx</b></td></tr>
                              </table>
                              <br/>
                              <table>
                                 <tr><td><b>Autuado: </b>{nomeAutuado}</td></tr>
                                 <tr><td><b>CPF/CNPJ: </b>{cpfAutuado}</td></tr>
                              </table>
                              <br/>
                              <table>
                                 <tr><td><b>Débito: </b>{numeroDebito}</td></tr>
                              </table>
                              <br/>
                              <table>
                                 <tr><td><b>Número de Referência: </b>{guia.NumeroReferencia}</td></tr>
                                 <tr><td><b>Data de Emissão: </b>{DateTime.Now}</td></tr>
                                 <tr><td><b>Data de Pagamento: </b>{guia.DataPagamento}</td></tr>
                                 <tr><td><b>Valor da Guia: </b>R$ {guia.ValorPago}</td></tr>
                                 <tr><td><b>Valor Pago: </b>R$ {guia.ValorPago}</td></tr>
                              </table>
                              <br/>
                              <br/>
                              <table style='margin: auto'>
                                <tr><td><img src={pathLogoAnvisa} alt='Logo Anvisa'></tr>
                              </table>
                          </div>";


                htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                htmlToPdf.Options.MarginLeft = 15;
                htmlToPdf.Options.MarginRight = 15;
                PdfDocument pdfDocument = htmlToPdf.ConvertHtmlString(html);
                byte[] pdf = pdfDocument.Save();
                Stream stream = new MemoryStream(pdf);
                pdfDocument.Close();

                string diretorioComprovante = Path.Combine(contentRootPath, "Comprovantes", "Pagas");

                if (!Directory.Exists(diretorioComprovante))
                    Directory.CreateDirectory(diretorioComprovante);

                string pathComprovante = Path.Combine(diretorioComprovante, $"Comprovante_{guia.NumeroReferencia}.pdf");

                using (var fs = new FileStream(pathComprovante, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(pdf, 0, pdf.Length);
                }
            }
        }

        public void GerarComprovantesNaoQuitacao(IEnumerable<DadosGuiaDto> lstGuiasVencidas)
        {
            string html = string.Empty;

            string nomeAutuado = "Francisco Araújo";
            string cpfAutuado = "058.729.052-43";
            string numeroDebito = "123456";

            string contentRootPath = _hostEnvironment.ContentRootPath;

            string pathLogoAnvisa = Path.Combine(contentRootPath, "Imagens", "logo_anvisa.png");

            HtmlToPdf htmlToPdf = new HtmlToPdf();

            foreach (var guia in lstGuiasVencidas)
            {
                html = $@"<div style='text-align: center; width: 50%; border: solid; margin: auto; padding:20px'>
                              <table style='margin: auto'>
                                 <tr><td><b>COMPROVANTE DE NÃO QUITAÇÃO</b></td></tr>
                              </table>
                              <br/>
                              <table>
                                 <tr><td><b>Processo Administrativo Sanitário (PAS): 25351.xxxxxx/202x-xx</b></td></tr>
                              </table>
                              <br/>
                              <table>
                                 <tr><td><b>Autuado: </b>{nomeAutuado}</td></tr>
                                 <tr><td><b>CPF/CNPJ: </b>{cpfAutuado}</td></tr>
                              </table>
                              <br/>
                              <table>
                                 <tr><td><b>Débito: </b>{numeroDebito}</td></tr>
                              </table>
                              <br/>
                              <table>
                                 <tr><td><b>Número de Referência: </b>{guia.NumeroReferencia}</td></tr>
                                 <tr><td><b>Data de Vencimento: </b>{guia.DataVencimento.ToShortDateString()}</td></tr>
                              </table>
                              <br/>
                              <br/>
                              <table style='margin: auto'>
                                <tr><td><img src={pathLogoAnvisa} alt='Logo Anvisa'></tr>
                              </table>
                          </div>";

                htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                htmlToPdf.Options.MarginLeft = 15;
                htmlToPdf.Options.MarginRight = 15;
                PdfDocument pdfDocument = htmlToPdf.ConvertHtmlString(html);
                byte[] pdf = pdfDocument.Save();
                Stream stream = new MemoryStream(pdf);
                pdfDocument.Close();

                string diretorioComprovante = Path.Combine(contentRootPath, "Comprovantes", "Vencidas");

                if (!Directory.Exists(diretorioComprovante))
                    Directory.CreateDirectory(diretorioComprovante);

                string pathComprovante = Path.Combine(diretorioComprovante, $"Comprovante_{guia.NumeroReferencia}.pdf");

                using (var fs = new FileStream(pathComprovante, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(pdf, 0, pdf.Length);
                }
            }
        }
    }
}