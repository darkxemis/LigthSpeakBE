namespace LightSpeak.Backend.Infrastructure.Middleware;

using LightSpeak.Backend.Common.Results;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(
        this Result<T> result,
        HttpContext http,
        Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        return ToErrorResult(http, result.Error!);
    }

    public static IResult ToHttpResult(
        this Result result,
        HttpContext http,
        Func<IResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess();
        }

        return ToErrorResult(http, result.Error!);
    }

    private static IResult ToErrorResult(HttpContext http, Error error)
    {
        return Results.Json(
            new ApiErrorResponse(error.Code, error.Message, http.TraceIdentifier, error.Metadata),
            statusCode: (int)error.StatusCode);
    }
}
