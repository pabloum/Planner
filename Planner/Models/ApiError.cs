using System;
namespace Planner.Api.Models
{
	public class ApiError : Exception
	{
		public ApiError(string message, IEnumerable<string> details)
		{
		}

		public ApiError(string message) : this(message, new[] { message })
		{

		}
	}
}

