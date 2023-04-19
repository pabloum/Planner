using System;
using System.Net;
using System.Reflection;
using System.Text.Json;
using Planner.Api.Models;

namespace Planner.Api.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly Dictionary<Type, HttpStatusCode> _statusCode;

        public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
			_statusCode = new Dictionary<Type, HttpStatusCode>
			{
				{ typeof(Exception), HttpStatusCode.InternalServerError }
			};

        }

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsyn(context,ex);
			}
		}

		private Task HandleExceptionAsyn(HttpContext context, Exception ex)
		{
			if (ex is TargetInvocationException && ex.InnerException != null)
			{
				ex = ex.InnerException;
			}

			if (!_statusCode.TryGetValue(ex.GetType(), out var code))
			{
				code = HttpStatusCode.InternalServerError;
			}

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;

			var error = CreateError(ex);
			var response = JsonSerializer.Serialize(error, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

			return context.Response.WriteAsync(response);
		}

		private object CreateError(Exception ex)
		{
			if (ex is AggregateException aggEx)
			{
				return new ApiError(ex.Message, aggEx.InnerExceptions.Select(x => x.Message));
			}

            //if (ex is AnotherTypeOfException valEx)
            //{
            //	return new AnotherTypeOfException();
            //}

            return new ApiError(ex.Message);
		}
    }
}

