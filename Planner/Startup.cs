using System;
using Planner.Api.Installers;
using Planner.Api.Middleware;

namespace Planner.Api
{
	public partial class Startup
	{
		public IConfiguration configRoot { get; }

		// public object MyProperty { get; set; }

		public Startup(IConfiguration configuration)
		{
			configRoot = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// services.AddAuthentication( ...

			services.AddControllers();

			services.AddEndpointsApiExplorer();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Pablos Planner", Version = "v1" });
			});

			services.AddBusinessServices();
			//services.AddSingleton(ConfigurationProvider);
		}

		public void Configure(WebApplication app, IWebHostEnvironment env)
		{
			// Configure the HTTP request pipeline
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			else
			{
				app.UseHttpsRedirection();
			}

			app.UseCors("MyAllowSpecificationOrigins");

			//app.UseAuthentication();
			//app.UseAuthorization();
            //app.UseResponseLogging();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.MapGet("/", () => "Hello World!");
			app.MapControllers();

			app.Run();
		}
	}
}

