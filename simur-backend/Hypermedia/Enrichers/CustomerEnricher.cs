using Microsoft.AspNetCore.Mvc;
using simur_backend.Hypermedia.Constants;
using simur_backend.Models.DTO.V1;

namespace simur_backend.Hypermedia.Enrichers
{
    public class CustomerEnricher : ContentResponseEnricher<CustomerDto>
    {
        protected override Task EnrichModel(CustomerDto content, IUrlHelper urlHelper)
        {
            var request = urlHelper.ActionContext.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host.ToUriComponent()}{request.PathBase.ToUriComponent()}/api/v1/customer";
            content.Links.AddRange(GenerateLinks(content, baseUrl));
            return Task.CompletedTask;
        }

        private IEnumerable<HypermediaLinks> GenerateLinks(CustomerDto content, string baseUrl)
        {
            return new List<HypermediaLinks>()
            {
                new ()
                {
                    Rel = RelationType.SELF,
                    Href = $"{baseUrl}/{content.Id}",
                    Type = ResponseTypeFormat.DEFAULT_GET,
                    Action = HttpActionVerb.GET,
                },
                new ()
                {
                    Rel = RelationType.SELF,
                    Href = $"{baseUrl}/document/{content.Document}",
                    Type = ResponseTypeFormat.DEFAULT_GET,
                    Action = HttpActionVerb.GET,
                },
                new ()
                {
                    Rel = RelationType.CREATE,
                    Href = $"{baseUrl}",
                    Type = ResponseTypeFormat.DEFAULT_POST,
                    Action = HttpActionVerb.POST,
                },
                new ()
                {
                    Rel = RelationType.UPDATE,
                    Href = $"{baseUrl}",
                    Type = ResponseTypeFormat.DEFAULT_PUT,
                    Action = HttpActionVerb.PUT,
                },
                new ()
                {
                    Rel = RelationType.DELETE,
                    Href = $"{baseUrl}/document/{content.Document}",
                    Type = ResponseTypeFormat.DEFAULT_DELETE,
                    Action = HttpActionVerb.DELETE,
                }
            };
        }
    }
}
