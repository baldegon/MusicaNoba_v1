using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using System.Security.Cryptography.X509Certificates;
using MusicaNoba_v1.Services;

namespace MusicaNoba_v1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            const string RapidApiKey = "a1a6ebfce0msh68bc1d6b0af44a2p113418jsnae8ba9cb15a4";

            // Fix: Ensure AddHttpClient extension method is available by referencing Microsoft.Extensions.Http
            builder.Services.AddHttpClient("SongstatsRapidApi", client =>
            {
                client.BaseAddress = new Uri("https://songstats.p.rapidapi.com/");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "songstats.p.rapidapi.com");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", RapidApiKey);
            })
            // Para evitar respuestas “raras” si el servidor envía brotli/gzip:
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All
            });

            builder.Services.AddSingleton<ISongstatsService, SongstatsService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
