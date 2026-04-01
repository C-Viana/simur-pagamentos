using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using simur_backend.Messaging;
using System.Text.Json;

namespace simur_backend.Configurations
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthConfiguration(this IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariables();
            string RabbitConnectionString = $"amqp://{env["RABBITMQ_USERNAME"]}:{env["RABBITMQ_PASSWORD"]}@{env["RABBITMQ_HOSTNAME"]}:{env["RABBITMQ_PORT"]}";

            services.AddHealthChecks()
                .AddMongoDb()
                .AddRabbitMQ(
                    factory: sp => RabbitMqSetupService.GetConnection()
                );
            return services;
        }

        public static void StartHealthchecks(this IEndpointRouteBuilder builder)
        {
            builder.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(x => new
                        {
                            component = x.Key,
                            status = x.Value.Status.ToString(),
                            description = x.Value.Description,
                            duration = x.Value.Duration.TotalSeconds
                        }),
                        total_duration = report.TotalDuration.TotalSeconds
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });
        }
    }
}
