using DotNetEnv;
using Scalar.AspNetCore;
using Serilog;
using simur_backend.Auth;
using simur_backend.Auth.Contract;
using simur_backend.Configurations;
using simur_backend.Exceptions;
using simur_backend.Hypermedia.Filters;
using simur_backend.Messaging;
using simur_backend.Models.Constants;
using simur_backend.Models.Deserealizers;
using simur_backend.Repositories.CustomerRepository;
using simur_backend.Repositories.MerchantRepository;
using simur_backend.Repositories.PaymentRepository;
using simur_backend.Repositories.UserRepository;
using simur_backend.Services;
using simur_backend.Services.Customers;
using simur_backend.Services.Merchants;
using simur_backend.Services.Payments;
using simur_backend.Services.Users;
using System.Text.Json;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Env.Load();

    builder.Services.AddHealthConfiguration();

    //SET SERILOG LOGGER
    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

    // Add services to the container.
    builder.Services.AddControllers(
        options => {
            options.Filters.Add<HypermediaFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new StringEnumSerializer<PaymentBrand>());
            options.JsonSerializerOptions.Converters.Add(new StringEnumSerializer<PaymentStatus>());
            options.JsonSerializerOptions.Converters.Add(new StringEnumSerializer<PaymentType>());
        });

    builder.Services.AddOpenApi();

    //builder.Services.AddDatabaseConfiguration(builder.Configuration);
    builder.Services.AddDatabaseConfigurationFromEnvironmet(builder.Configuration);

    builder.Services.AddAuthConfiguration(builder.Configuration);
    builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserServices, UserServices>();

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<ICustomerServices, CustomerServices>();

    builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();
    builder.Services.AddScoped<IMerchantServices, MerchantServices>();

    builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
    builder.Services.AddScoped<IPaymentStatusHistoryRepository, PaymentStatusHistoryRepository>();
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services.AddScoped<IPaymentServices, PaymentServices>();

    builder.Services.AddScoped<RabbitMqSetupService>();
    builder.Services.AddScoped<IMessageBusService, RabbitMqPublisherService>(); //Creating service for RabbitMQ publisher
    builder.Services.AddHostedService<RabbitMqConsumerService>(); //Creating service for RabbitMQ consumer

    builder.Services.AddHateoasConfiguration();

    var app = builder.Build();
    app.UseSerilogRequestLogging();

    app.UseMiddleware<SimurExceptionHandler>();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi().AllowAnonymous();
        app.MapScalarApiReference().AllowAnonymous();
        app.Map("/", () => Results.Redirect("/scalar")).AllowAnonymous();
    }

    app.MapControllers();
    app.UseHateoasRoutes();

    app.StartHealthchecks();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated due unexpected error");
}
finally
{
    Log.Information("Application has been closed");
    Log.CloseAndFlush();
}