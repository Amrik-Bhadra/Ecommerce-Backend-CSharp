using System.Net;
using System.Text.Json;
using EcommerceBackend.Exceptions;
using EcommerceBackend.DTOs;
namespace EcommerceBackend.Middlewares;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Message: {e.Message}");
            await HandleExceptionAsync(context, e);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception) 
    {
        context.Response.ContentType = "application/json";
        // default status code and message
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string userFriendlyMessage = "Internal Server Error";

        if(exception is NotFoundException)
        {
            statusCode = (int)HttpStatusCode.NotFound; // 404
            userFriendlyMessage = exception.Message;
        }else if(exception is BadRequestException)
        {
            statusCode = (int)HttpStatusCode.BadRequest; // 400
            userFriendlyMessage = exception.Message;
        }
        else
        {
            userFriendlyMessage = exception.Message;
        }

        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.ErrorResponse(userFriendlyMessage);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var jsonResponse = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(jsonResponse);
    }
}
