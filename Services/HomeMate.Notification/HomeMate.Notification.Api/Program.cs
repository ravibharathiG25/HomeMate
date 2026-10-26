using HomeMate.Notification.Application.Consumers;
using HomeMate.Notification.Infrastructure;
using HomeMate.Notification.Infrastructure.Persistence;
using HomeMate.Shared;
using HomeMate.Shared.Messaging;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add building blocks & shared web pipeline (RFC 7807 ProblemDetails, GlobalExceptionHandler)
builder.Services.AddSharedWeb();

// Add Notification Infrastructure (DbContext, Repositories, EmailService)
builder.Services.AddNotificationInfrastructure(builder.Configuration);

// Add MassTransit EventBus with consumers
builder.Services.AddEventBus(builder.Configuration, busConfig =>
{
    busConfig.AddConsumer<UserRegisteredConsumer>();
    busConfig.AddConsumer<BookingCreatedConsumer>();
});

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
        Title = "HomeMate Notification API",
        Version = "v1",
        Description = "Notification & Event Processing Microservice for HomeMate"
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Ensure database schema is created
await NotificationDbInitializer.InitializeDatabaseAsync(app.Services);

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeMate Notification API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
