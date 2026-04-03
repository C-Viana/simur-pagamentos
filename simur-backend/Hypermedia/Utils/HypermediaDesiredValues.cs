using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace simur_backend.Hypermedia.Utils
{
    public static class HypermediaDesiredValues
    {
        public static ObjectResult GetTypeFromResult(ResultExecutingContext context)
        {
            switch (context.Result)
            {
                case OkObjectResult:
                    return (OkObjectResult)context.Result;
                case CreatedResult:
                    return (CreatedResult)context.Result;
                case CreatedAtActionResult:
                    return (CreatedAtActionResult)context.Result;
                default:
                    return null;
            }
        }

        public static bool IsHypermediaExpected(ResultExecutingContext context)
        {
            return context.Result is OkObjectResult or CreatedResult or CreatedAtActionResult;
        }
    }
}
