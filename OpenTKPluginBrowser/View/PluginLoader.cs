using PluginBase;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace OpenTKPluginBrowser
{
	public static class PluginLoader
	{
		public static IEnumerable<IPlugin> LoadPlugins(string assemblyFilePath)
		{
			Console.WriteLine($"Loading commands from: {assemblyFilePath}");
			Assembly pluginAssembly = Assembly.LoadFile(assemblyFilePath);

			//PluginLoadContext loadContext = new PluginLoadContext(pluginFilePath);
			//Assembly pluginAssembly = loadContext.LoadFromAssemblyPath(pluginFilePath);

			return CreateInstancesOf<IPlugin>(pluginAssembly);
		}


		public static IEnumerable<TYPE> CreateInstancesOf<TYPE>(Assembly assembly)
		{
			int count = 0;

			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(TYPE).IsAssignableFrom(type))
				{
					if (Activator.CreateInstance(type) is TYPE instance)
					{
						count++;
						yield return instance;
					}
				}
			}

			if (count == 0)
			{
				string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
				throw new ApplicationException(
					$"Can't find any type which implements {nameof(TYPE)} in {assembly} from {assembly.Location}.\n" +
					$"Available types: {availableTypes}");
			}
		}
	}
}
