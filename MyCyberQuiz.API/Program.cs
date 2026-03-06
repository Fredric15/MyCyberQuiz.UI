using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyCyberQuiz.BLL.Interfaces;
using MyCyberQuiz.BLL.Services;
using MyCyberQuiz.BLL.Settings;
using MyCyberQuiz.DAL.Data;
using MyCyberQuiz.DAL.Data.SeedData;
using MyCyberQuiz.DAL.Models;
using MyCyberQuiz.DAL.Repositories;
using MyCyberQuiz.DAL.Repositories.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen(options =>
{
    // Märk väl: Ordet "Models" är helt borttaget!
    options.SwaggerDoc("v1", new global::Microsoft.OpenApi.OpenApiInfo
    {
        Title = "MyCyberQuiz API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new global::Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = global::Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = global::Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Klistra in din JWT-token här."
    });

    // Swashbuckle v10 använder det nya objektet OpenApiSecuritySchemeReference
    options.AddSecurityRequirement(document => new global::Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new global::Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});




builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Berättar för .NET att fylla JwtSettings-klassen med datan från "Jwt"-blocket i appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddHttpClient<IAiChatService, AiChatService>();

// Lägg till Identity-systemet (som ger dig UserManager, SignInManager osv)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Viktigt att detta kommer EFTER AddIdentity, annars kommer det att försöka använda cookies som standard och då kommer JWT-autentiseringen inte att fungera!
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    // Dessa tre rader är det som säger åt Identity att INTE använda cookies!
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});


//Add repositories and services
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IUserProgressRepository, UserProgressRepository>();
builder.Services.AddScoped<IUserScoreRepository, UserScoreRepository>();

builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Detta gör att Swagger UI dyker upp direkt på startsidan (http://localhost:port/)
        // Istället för att du måste skriva in /swagger i URL:en.
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyCyberQuiz API v1");
        options.RoutePrefix = string.Empty;
    });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Kör seedningen
        await UserSeed.SeedDefaultUserAsync(userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ett fel uppstod när seed-datan skulle läggas in.");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
