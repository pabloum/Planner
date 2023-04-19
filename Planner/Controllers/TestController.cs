using System;
using Microsoft.AspNetCore.Mvc;

namespace Planner.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TestController : ControllerBase
	{
		public TestController()
		{
		}

		[HttpGet("Ping")]
		public async Task<ActionResult<string>> TestEndopoint() => Ok("Pong");
	}
}

