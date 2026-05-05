using BetNHL.Utilities;
using BetNHL.ViewModels;
using BetNHL.Views;
using BetNHL;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BetNHL.Data;
using CommunityToolkit.Maui;

namespace BetNHL
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; }
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<AuthMessageHandler>();
            builder.UseMauiApp<App>().UseMauiCommunityToolkit();


            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5159");
            })
            .AddHttpMessageHandler<AuthMessageHandler>();

            builder.Services.AddScoped<LoginViewModel>();
            builder.Services.AddScoped<LoginPage>();
            builder.Services.AddScoped<Register>();
            builder.Services.AddScoped<RegisterViewModel>();
            builder.Services.AddTransient<IGameRepository, GameRepository>();
            builder.Services.AddTransient<IBetRepository, BetRepository>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<GameDetailsViewModel>();
            builder.Services.AddTransient<GameDetailsPage>();



#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            Services = app.Services;

            return app;
        }
    }
}
