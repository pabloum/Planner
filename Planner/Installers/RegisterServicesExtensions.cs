using System;
using System.Reflection;

namespace Planner.Api.Installers
{
	public static class RegisterServicesExtensions
	{

		public static void RegisterAllDirectImplementations<T>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
		{
			if (assemblies.Length == 0)
			{
				assemblies = new[] { Assembly.GetAssembly(typeof(T)) };
			}

			services.RegisterAssemblyPublicNonGenericClasses(assemblies)
				.Where(x => typeof(T).IsAssignableFrom(x))
				.OnlyDerivedImplementations(lifetime);
		}

		/// <summary>
		/// This finds all public, non-generic, non-nested classes in an assembly in the provided assemblies.
		/// If no assemblies are provided, then it scans the assembly that called the method
		/// </summary>
		/// <param name="services"></param>
		/// <param name="assemblies"></param>
		/// <returns></returns>
        public static AutoRegisterData RegisterAssemblyPublicNonGenericClasses(this IServiceCollection services, params Assembly[] assemblies)
		{
			if (assemblies.Length == 0)
			{
				assemblies = new[] { Assembly.GetCallingAssembly() };
			}

			var allPublicTypes = assemblies.SelectMany(x =>
				x.GetExportedTypes().Where(y => y.IsClass && !y.IsAbstract && !y.IsGenericType && !y.IsNested)
			);
			return new AutoRegisterData(services, allPublicTypes);
		}

		public static AutoRegisterData Where(this AutoRegisterData autoRegisterData, Func<Type, bool> predicate)
		{
			if (autoRegisterData == null) throw new ArgumentNullException(nameof(autoRegisterData));
			autoRegisterData.TypeFilter = predicate;
			return new AutoRegisterData(autoRegisterData.Services, autoRegisterData.TypesToConsider.Where(predicate));
		}

		/// <summary>
		/// This registers classes against any public interface (other than IDisposable)
		/// </summary>
		/// <param name="autoRegisterData"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static IServiceCollection OnlyDerivedImplementations(this AutoRegisterData autoRegisterData, ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
            if (autoRegisterData == null) throw new ArgumentNullException(nameof(autoRegisterData));

			foreach ( var classType in (autoRegisterData.TypeFilter == null
				? autoRegisterData.TypesToConsider
				: autoRegisterData.TypesToConsider.Where(autoRegisterData.TypeFilter)))
			{
				var interfaces = classType.GetTypeInfo().GetInterfaces();
				foreach (var infc in interfaces.Where(i => i != typeof(IDisposable) && i.IsPublic && !i.IsNested))
				{
					if (!interfaces.Any(i => i.GetInterfaces().Contains(infc)))
					{
						autoRegisterData.Services.Add(new ServiceDescriptor(infc, classType, lifetime));
					}
				}
			}

			return autoRegisterData.Services;
        }

        /// <summary>
        /// This is currently unused and differes only in 1 line with OnlyDerivedImplementations() method
        /// </summary>
        /// <param name="autoRegisterData"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AsPublicImplementedInterfaces(this AutoRegisterData autoRegisterData, ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
            if (autoRegisterData == null) throw new ArgumentNullException(nameof(autoRegisterData));

            foreach (var classType in (autoRegisterData.TypeFilter == null
                ? autoRegisterData.TypesToConsider
                : autoRegisterData.TypesToConsider.Where(autoRegisterData.TypeFilter)))
            {
                var interfaces = classType.GetTypeInfo().ImplementedInterfaces;
                foreach (var infc in interfaces.Where(i => i != typeof(IDisposable) && i.IsPublic && !i.IsNested))
                {
                    if (!interfaces.Any(i => i.GetInterfaces().Contains(infc)))
                    {
                        autoRegisterData.Services.Add(new ServiceDescriptor(infc, classType, lifetime));
                    }
                }
            }

            return autoRegisterData.Services;
        }

        /// <summary>
        /// 
        /// </summary>
        public class AutoRegisterData
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="services"></param>
			/// <param name="typesToConsider"></param>
			/// <exception cref="ArgumentNullException"></exception>
			public AutoRegisterData(IServiceCollection services, IEnumerable<Type> typesToConsider)
			{
				Services = services ?? throw new ArgumentNullException(nameof(services));
				TypesToConsider = typesToConsider ?? throw new ArgumentNullException(nameof(typesToConsider));
			}

			/// <summary>
			/// 
			/// </summary>
			public IServiceCollection Services { get; set; }

			/// <summary>
			/// 
			/// </summary>
			public IEnumerable<Type> TypesToConsider { get; set; }

			/// <summary>
			/// 
			/// </summary>
			public Func<Type, bool> TypeFilter { get; set; }
		}
    }

}

