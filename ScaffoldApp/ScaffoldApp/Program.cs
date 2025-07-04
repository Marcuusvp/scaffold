using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ScaffoldApp.DbConfigurations;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")!;
var otlpHeaders  = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS")!;
var otlpProto = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")!;
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // console/elastic etc.
    .WriteTo.Console()
    .WriteTo.Elasticsearch(/* sua config */)
    .WriteTo.OpenTelemetry(o =>
    {
        o.Endpoint = otlpEndpoint;
        o.Headers  = otlpHeaders
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split('=', 2))
            .ToDictionary(parts => parts[0], parts => parts[1]);
        o.Protocol = otlpProto.Equals("grpc", StringComparison.OrdinalIgnoreCase)
            ? OtlpProtocol.Grpc
            : OtlpProtocol.HttpProtobuf;

        // opcional: atribue resource attrs, se quiser
        var rawAttrs = builder.Configuration["OpenTelemetry:ResourceAttributes"]!;
        o.ResourceAttributes = rawAttrs
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split('=', 2))
            .ToDictionary(parts => parts[0], parts => (object)parts[1]);
    })
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .AddService("scaffoldApp.Api", serviceNamespace: "my-application-group")
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = "production"
        })
    )
    .WithTracing(tr =>
    {
        tr.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("scaffoldApp.Api.Manual") 
            .AddOtlpExporter();
    })
    .WithMetrics(mt =>
    {
        mt.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter();
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Verificação opcional de conexão com o banco (apenas em desenvolvimento)
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.OpenConnection();
        dbContext.Database.CloseConnection();
        Console.WriteLine("Conexão com PostgreSQL estabelecida com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Falha na conexão com PostgreSQL: {ex.Message}");
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();