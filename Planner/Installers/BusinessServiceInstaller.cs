using System;
using Microsoft.EntityFrameworkCore;
using Planner.Persistence;
using Planner.Repository.Contracts;
using Planner.Service.Contracts;

namespace Planner.Api.Installers
{
	public static class BusinessServiceInstaller
	{
		public static void AddBusinessServices(this IServiceCollection services/*, IConfigurationProvider configuration*/)
		{
			services.RegisterAllDirectImplementations<IService>(ServiceLifetime.Scoped);
			services.RegisterAllDirectImplementations<IRepository>(ServiceLifetime.Scoped);

			services.AddDbContext<PlannerDbContext>(options => options.UseSqlServer("connectionString"));
        }
	}
}

