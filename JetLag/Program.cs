using Microsoft.Extensions.FileProviders;

using JetLag.Components;
using JetLag.Scripts.Data;
using JetLag.Scripts.Extensions;
using JetLag.Scripts.Factory;
using JetLag.Scripts.Geomitry;
using JetLag.Scripts.Input;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;
using JetLag.Scripts;
using Community.Blazor.MapLibre;
using JetLag.Scripts.Intialize;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddLocalization();

builder.Services
    .AddSingleton<ClientSettings>()
    .RegisterConcreteFactory<QuestionCardModelFactory, IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput>()
    .AddScoped<IMapMouseObserver, MapMouseObserver>()
    .RegisterFactoryOutput<MapUIControllerFactory, MapUIController>()
    .RegisterFactoryOutput<GeomitryCombinderFactory, IGeomitryCombinder>()
    .RegisterFactoryOutput<MapRenderFactory, MapRender>()
    .AddScoped<IGameIntializer<MapLibre>, MapLibreGameIntializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(
        new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(app.Environment.ContentRootPath, "Images")
            ),
            RequestPath = "/Images"
        }
    )
    .UseStaticFiles(
        new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(app.Environment.ContentRootPath, "Fonts")
            ),
            RequestPath = "/Fonts"
        }
    );
;
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
