using Microsoft.Extensions.FileProviders;

using JetLag.Components;
using JetLag.Scripts.Data;
using JetLag.Scripts.Extensions;
using JetLag.Scripts.Factory;
using JetLag.Scripts.Geometry;
using JetLag.Scripts.Input;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;
using JetLag.Scripts;
using Community.Blazor.MapLibre;
using JetLag.Scripts.Intialize;
using JetLag.Scripts.Mechanics;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddLocalization();

// TODO: Swap to real server when available:
// builder.Services.AddHttpClient<IHiderProxy, HiderProxy>(client =>
//     client.BaseAddress = new Uri(builder.Configuration["HiderService:BaseUrl"]!));

builder.Services
    .AddScoped<IHiderProxy, LocalHiderProxy>()
    .AddSingleton<ClientSettings>()
    .AddScoped<IMapMouseObserver, MapMouseObserver>()
    .RegisterConcreteFactory<QuestionCardModelFactory, IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput>()
    .RegisterFactoryOutput<GeomitryCombinderFactory, IGeometryCombinder>()
    .RegisterFactoryOutput<MapRenderFactory, MapRender>()
    .RegisterFactoryOutput<MapActionManagerFactory, IMapActionManager>()
    .AddScoped<IMapOrchestrator<MapLibre>, MapLibreOrchestrator>();

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
