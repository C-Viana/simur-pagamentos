using Scalar.AspNetCore;
using simur_backend.Configurations;
using simur_backend.Hypermedia.Filters;
using simur_backend.Messaging;
using simur_backend.Models.Constants;
using simur_backend.Models.Deserealizers;
using simur_backend.Repositories.CustomerRepository;
using simur_backend.Repositories.MerchantRepository;
using simur_backend.Repositories.PaymentRepository;
using simur_backend.Services;
using simur_backend.Services.Customers;
using simur_backend.Services.Merchants;
using simur_backend.Services.Payments;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(
    options => options.Filters.Add<HypermediaFilter>()
    )
    .AddJsonOptions( options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new StringEnumSerializer<PaymentBrand>());
        options.JsonSerializerOptions.Converters.Add(new StringEnumSerializer<PaymentStatus>());
        options.JsonSerializerOptions.Converters.Add(new StringEnumSerializer<PaymentType>());
    });

builder.Services.AddOpenApi();

builder.Services.AddDatabaseConfiguration(builder.Configuration);

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();
builder.Services.AddScoped<IMerchantService, MerchantService>();

builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPaymentStatusHistoryRepository, PaymentStatusHistoryRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentServices, PaymentServices>();

builder.Services.AddScoped<IMessageBusService, RabbitMqPublisherService>(); //Creating service for RabbitMQ publisher
builder.Services.AddHostedService<RabbitMqConsumerService>(); //Creating service for RabbitMQ consumer

builder.Services.AddHateoasConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.Map("/", () => Results.Redirect("/scalar"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHateoasRoutes();

app.Run();
