using backend.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace backend.Shared.Http;

public static class ApplicationResultExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, ApplicationResult result)
    {
        if (result.Success)
        {
            return controller.StatusCode(
                result.StatusCode,
                new ApiResponse<object?>(true, result.Message));
        }

        return controller.StatusCode(
            result.StatusCode,
            new ApiErrorResponse(false, result.Message, result.Errors));
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, ApplicationResult<T> result)
    {
        if (result.Success)
        {
            return controller.StatusCode(
                result.StatusCode,
                new ApiResponse<T>(true, result.Message, result.Data));
        }

        return controller.StatusCode(
            result.StatusCode,
            new ApiErrorResponse(false, result.Message, result.Errors));
    }
}
