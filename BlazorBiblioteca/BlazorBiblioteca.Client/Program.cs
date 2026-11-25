
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorBootstrap;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Registro de servicios de Blazor Bootstrap.
builder.Services.AddBlazorBootstrap();

await builder.Build().RunAsync();
