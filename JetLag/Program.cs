using Microsoft.Extensions.FileProviders;

using JetLag.Components;
using JetLag.Scripts.Data;
using JetLag.Scripts.Extensions;
using JetLag.Scripts.Factory;
using JetLag.Scripts.Geomitry;
using JetLag.Scripts.Render;
using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Models;
using Microsoft.AspNetCore.Components;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddLocalization();

builder.Services
    .AddSingleton<ClientSettings>()
    .RegisterConcreteFactory<QuestionCardModelFactory, IReadOnlyList<QuestionCardModel>, IHandleEvent>()
    .RegisterFactory<GeomitryCombinderFactory, IGeomitryCombinder>()
    .RegisterFactory<MapRenderFactory, MapRender>();

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
