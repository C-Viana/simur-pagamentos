using Microsoft.AspNetCore.Mvc.Filters;

namespace simur_backend.Hypermedia
{
    public interface IResponseEnricher
    {
        bool CanEnrich(ResultExecutingContext context);
        Task Enrich(ResultExecutingContext context);
    }
}
