using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyCyberQuiz.UI.Auth;
using MyCyberQuiz.UI.Components;
using MyCyberQuiz.UI.Services;
using MyCyberQuiz.UI.Services.Handlers;
using MyCyberQuiz.UI.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Registrera handlern
//builder.Services.AddTransient<JwtAuthorizationMessageHandler>();
//Registrera Auth-tjänsten och ge den en färdigkonfigurerad HttpClient
//Detta ersätter också builder.Services.AddScoped<IFrontendAuthService, FrontendAuthService>();
builder.Services.AddHttpClient<IFrontendAuthService, FrontendAuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7256");
});

builder.Services.AddHttpClient<IFrontendQuizService, FrontendQuizService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7256");
});

builder.Services.AddHttpClient<IFrontendProfileService, FrontendProfileService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7256");
});

builder.Services.AddHttpClient<IFrontendAiChatService, FrontendAiChatService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7256");
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Berätta för Blazor att använda vår CustomAuthStateProvider när den behöver veta vem som är inloggad
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Vi ger systemet "Cookies" som ett standard-namn så att dörrvakten slutar få panik
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/login"; // Om någon försöker komma åt en skyddad sida utan att vara inloggad, skicka dem hit
    });
builder.Services.AddAuthorizationCore();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseAuthentication(); // Måste komma före Authorization
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
