using Microsoft.AspNetCore.Mvc;

namespace HouseBrokerApplication.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult HttpResponse<T>(this Result<T> result)
        {
            switch (result.ResultType)
            {
                case ResultType.Success:
                    return new OkObjectResult(result);

                case ResultType.ValidationError:
                    return new BadRequestObjectResult(result);

                case ResultType.Error:
                    return new BadRequestObjectResult(result);

                case ResultType.Unathorized:
                    return new UnauthorizedObjectResult(result);

                case ResultType.NotFound:
                    return new NotFoundResult();

                default:
                    return new BadRequestResult();
            }
        }
    }
}
