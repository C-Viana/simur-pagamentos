using simur_backend.Configurations;
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

builder.Services.AddControllers()
    .AddJsonOptions( options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
