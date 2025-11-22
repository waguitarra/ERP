using System.Text;
using Logistics.Application.Interfaces;
using Logistics.Application.Services;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Logistics.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
var mvcBuilder = builder.Services.AddControllers();

// LOG: Descobrir controllers registrados
Log.Information("========== CONTROLLERS REGISTRADOS ==========");
var assembly = typeof(Program).Assembly;
var controllerTypes = assembly.GetTypes()
    .Where(t => t.Name.EndsWith("Controller") && !t.IsAbstract)
    .ToList();

foreach (var ctrl in controllerTypes)
{
    Log.Information("Controller encontrado: {ControllerName}", ctrl.Name);
}
Log.Information("Total de controllers: {Count}", controllerTypes.Count);
Log.Information("=============================================");

// Configurar DbContext com MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LogisticsDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Dependency Injection - Repositories
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IStorageLocationRepository, StorageLocationRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// WMS Repositories
builder.Services.AddScoped<IWarehouseZoneRepository, WarehouseZoneRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInboundShipmentRepository, InboundShipmentRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IPickingWaveRepository, PickingWaveRepository>();
builder.Services.AddScoped<IVehicleAppointmentRepository, VehicleAppointmentRepository>();
builder.Services.AddScoped<IDockDoorRepository, DockDoorRepository>();
builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<IPutawayTaskRepository, PutawayTaskRepository>();
builder.Services.AddScoped<IPackingTaskRepository, PackingTaskRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IOutboundShipmentRepository, OutboundShipmentRepository>();
builder.Services.AddScoped<ISerialNumberRepository, SerialNumberRepository>();
builder.Services.AddScoped<ICycleCountRepository, CycleCountRepository>();

// Dependency Injection - Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IStorageLocationService, StorageLocationService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();

// WMS Services
builder.Services.AddScoped<IWarehouseZoneService, WarehouseZoneService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInboundShipmentService, InboundShipmentService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IPickingWaveService, PickingWaveService>();
builder.Services.AddScoped<IVehicleAppointmentService, VehicleAppointmentService>();
builder.Services.AddScoped<IDockDoorService, DockDoorService>();
builder.Services.AddScoped<ILotService, LotService>();
builder.Services.AddScoped<IPutawayTaskService, PutawayTaskService>();
builder.Services.AddScoped<IPackingTaskService, PackingTaskService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IOutboundShipmentService, OutboundShipmentService>();
builder.Services.AddScoped<ISerialNumberService, SerialNumberService>();
builder.Services.AddScoped<ICycleCountService, CycleCountService>();

// Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CompanyAccess", policy => policy.RequireRole("Admin", "CompanyAdmin", "CompanyUser"));
    options.AddPolicy("CompanyAdminOnly", policy => policy.RequireRole("Admin", "CompanyAdmin"));
});

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Logistics API",
        Version = "v1",
        Description = "API de Logística com arquitetura DDD e multi-tenancy"
    });

    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Logistics API V1");
    c.RoutePrefix = string.Empty; // Swagger na raiz
});

// app.UseHttpsRedirection(); // Desabilitado para testes

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Logging de inicialização
Log.Information("Iniciando Logistics API");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}

// Tornar Program acessível para testes de integração
public partial class Program { }
