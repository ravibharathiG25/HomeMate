using HomeMate.Catalog.Infrastructure;
using HomeMate.Catalog.Infrastructure.Persistence;
using HomeMate.Shared;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add building blocks & shared web pipeline
builder.Services.AddSharedWeb();

// Add Catalog Infrastructure (DbContext, Repositories)
builder.Services.AddCatalogInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger & OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HomeMate Catalog API",
        Version = "v1",
        Description = "Service Catalog & Categories Microservice for HomeMate"
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Seed initial service categories & items
await CatalogDbInitializer.SeedAsync(app.Services);

app.UseExceptionHandler();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeMate Catalog API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
