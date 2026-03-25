using simur_backend.Hypermedia.Enrichers;
using simur_backend.Hypermedia.Filters;
using simur_backend.Models.DTO.V1;

namespace simur_backend.Configurations
{
    public static class HateoasConfig
    {
        public static IServiceCollection AddHateoasConfiguration(this IServiceCollection services)
        {
            var filterOptions = new HypermediaFilterOptions();
            filterOptions.ContentResponseEnricherList.AddRange([new MerchantEnricher(), new CustomerEnricher()]);
            services.AddSingleton(filterOptions);
            services.AddScoped<HypermediaFilter>();
            return services;
        }

        public static void UseHateoasRoutes(this IEndpointRouteBuilder builder)
        {
            builder.MapControllerRoute("Default", "api/v1/{controller=values}/{id?}");
        }
    }
}
