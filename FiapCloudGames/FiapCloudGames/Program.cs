using FiapCloudGames.Api.Auth;
using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Utils;
using FiapCloudGames.Infrastructure.Repository;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

#region Swagger Doc
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FiapCloudGames", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Faça o login para receber o Token. (Sem o 'Bearer')"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
#endregion

var connection = configuration.GetConnectionString("ConnectionString");

#region Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connection);
    options.UseLazyLoadingProxies();
}, ServiceLifetime.Scoped);
#endregion

#region Injection de Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IJogoRepository, JogoRepository>();
builder.Services.AddScoped<IUsuarioJogoPropriedadeRepository, UsuarioJogoRepository>();
builder.Services.AddScoped<IPromocaoRepository, PromocaoRepository>();
builder.Services.AddScoped<IJogosPromocoesRepository, JogosPromocoesRepository>();
#endregion

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<JogosPromocoesInputValidator>();

#region Auth 
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings!.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.ChaveSecreta))
        };
    });
builder.Services.AddAuthorization();
#endregion

#region Logging
var columnOptions = new ColumnOptions
{
    Store = new Collection<StandardColumn>
    {
        StandardColumn.Id,
        StandardColumn.Message,
        StandardColumn.MessageTemplate,
        StandardColumn.Level,
        StandardColumn.TimeStamp,
        StandardColumn.Exception,
        StandardColumn.Properties
    }
};

builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    loggerConfig
        .WriteTo.MSSqlServer(
            connectionString: context.Configuration.GetConnectionString("ConnectionString"),
            sinkOptions: new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlTable = false
            },
            columnOptions: columnOptions
        );
});
#endregion

var app = builder.Build();

#region Migration Setup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var adminEmail = config["SeedAdmin:Email"];
    var adminSenha = Encoding.UTF8.GetString(Convert.FromBase64String(config["SeedAdmin:Senha"]!));
    var adminNome = config["SeedAdmin:Nome"];

    if (!dbContext.Usuario.Any(u => u.Email == adminEmail))
    {
        var admin = new Usuario
        {
            Nome = adminNome!,
            Email = adminEmail!,
            Senha = PasswordHelper.HashSenha(adminSenha!),
            NivelAcesso = "Admin",
            Saldo = 0
        };

        dbContext.Usuario.Add(admin);
        dbContext.SaveChanges();
    }
}
#endregion

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
