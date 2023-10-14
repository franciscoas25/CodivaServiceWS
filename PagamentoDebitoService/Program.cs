using PagamentoDebitoService;
using PagamentoDebitoService.Dapper.Implementation;
using PagamentoDebitoService.Dapper.Interface;
using PagamentoDebitoService.Service.Implementation;
using PagamentoDebitoService.Service.Interface;


IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .Build();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<PagamentoService>();

        services.AddSingleton<IPagamentoGuiaDapper, PagamentoGuiaDapper>();
        services.AddSingleton<IPagamentoGuiaService, PagamentoGuiaService>();
    })
    .Build();

host.Run();

