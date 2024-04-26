using ApiCubos.Data;
using ApiCubos.Helpers;
using ApiCubos.Repositories;
using Microsoft.EntityFrameworkCore;
using NSwag.Generation.Processors.Security;
using NSwag;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Extensions.Azure;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
    (builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient =
builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret secret =
    await secretClient.GetSecretAsync("SqlAzure");
string conn = secret.Value;


#region SERVICE OAUTH

//CREAMOS UNA INSTANCIA DEL HELPER
HelperActionServicesOAuth helper =
    new HelperActionServicesOAuth(builder.Configuration);
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);

#endregion

#region TOKEN CONFIG

//HABILITAMOS LOS SERVICIOS DE AUTHENTICATION QUE HEMOS 
//CREADO EN EL HELPER CON Action<>
builder.Services.AddAuthentication
    (helper.GetAuthenticationSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());

#endregion 

#region REPO

builder.Services.AddTransient<CubosRepository>();
builder.Services.AddDbContext<CubosContext>(options =>
{
    options.UseSqlServer(conn);
});

#endregion


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region SWAGGER CONFIG

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Api Cubos";
    document.Description = "Api cubos.";
    // CONFIGURAMOS LA SEGURIDAD JWT PARA SWAGGER,
    // PERMITE AÑADIR EL TOKEN JWT A LA CABECERA.
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
    url: "/swagger/v1/swagger.json",
    name: "api vubos");
    options.RoutePrefix = "";
    options.DocExpansion(DocExpansion.None);
});

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
