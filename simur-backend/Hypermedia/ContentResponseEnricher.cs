using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Polly;
using simur_backend.Hypermedia.Utils;

namespace simur_backend.Hypermedia
{
    public abstract class ContentResponseEnricher<T> : IResponseEnricher where T : ISupportHypermedia
    {

        protected abstract Task EnrichModel(T content, IUrlHelper urlHelper);

        public async Task Enrich(ResultExecutingContext context)
        {
            var urlHelper = new UrlHelperFactory().GetUrlHelper(context);

            if (HypermediaDesiredValues.IsHypermediaExpected(context))
            {
                var objectResult = HypermediaDesiredValues.GetTypeFromResult(context);
                if (objectResult.Value is T model)
                {
                    await EnrichModel(model, urlHelper);
                    return;
                }
                else if(objectResult.Value is List<T> collection)
                {
                    foreach (var item in collection)
                    {
                        await EnrichModel(item, urlHelper);
                    }
                    return;
                }
            }
            await Task.CompletedTask;
        }

        public virtual bool CanEnrich(Type contentType)
        {
            return contentType == typeof(T) || contentType == typeof(List<T>);
        }

        bool IResponseEnricher.CanEnrich(ResultExecutingContext context)
        {
            if(HypermediaDesiredValues.IsHypermediaExpected(context))
            {
                var objectResult = HypermediaDesiredValues.GetTypeFromResult(context);
                if (objectResult.Value is null) return false;
                return CanEnrich(objectResult.Value.GetType());
            }
            return false;
        }
    }
}
