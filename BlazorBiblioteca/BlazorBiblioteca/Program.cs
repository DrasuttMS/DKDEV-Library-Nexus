using BlazorBiblioteca.Context;
using BlazorBiblioteca.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// HttpClient registrado para BlazorBiblioteca (prerenderizado del servidor).
builder.Services.AddScoped<System.Net.Http.HttpClient>(sp =>
{
    var navigation = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
    return new System.Net.Http.HttpClient
    {
        BaseAddress = new System.Uri(navigation.BaseUri)
    };
});

// Servicios de la API y persistencia SQL Server.
builder.Services.AddControllers();
builder.Services.AddDbContext<LibroDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BibliotecaConnection")));

// Blazor Web App con interactividad WebAssembly global.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Endpoints REST del controlador LibroController.
app.MapControllers();

// Componentes Blazor del servidor y ensamblado cliente.
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorBiblioteca.Client._Imports).Assembly);

app.Run();

