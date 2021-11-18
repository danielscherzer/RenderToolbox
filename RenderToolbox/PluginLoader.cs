using PluginBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RenderToolbox
{
	public static class PluginLoader
	{
		public static string TempDir => _tempDir;

		private static readonly string _tempDir = GetTempDir();

		private static string GetTempDir()
		{
			string? location = Assembly.GetExecutingAssembly().Location;
			if (location is null) throw new ApplicationException("Assembly has no location.");
			string? dir = Path.GetDirectoryName(location);
			if (dir is null) throw new ApplicationException("Assembly has no location directory.");
			var temp = Path.Combine(dir, "temp");
			if(Directory.Exists(temp)) Directory.Delete(temp, true);
			return temp;
		}

		public static IEnumerable<IPlugin> LoadPlugins(string assemblyFilePath)
		{
			Trace.WriteLine($"Loading commands from: {assemblyFilePath}");
			if(!File.Exists(assemblyFilePath)) return Enumerable.Empty<IPlugin>();
			try
			{
				CollectibleLoadContext loadContext = new(assemblyFilePath);
				var tempPluginDir = Path.Combine(TempDir, DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
				Directory.CreateDirectory(tempPluginDir);
				var newPath = Path.Combine(tempPluginDir, Path.GetFileName(assemblyFilePath));
				File.Copy(assemblyFilePath, newPath);
				Assembly pluginAssembly = loadContext.LoadFromAssemblyPath(newPath);
				return CreateInstancesOf<IPlugin>(pluginAssembly);
			}
			catch
			{
				return Enumerable.Empty<IPlugin>();
			}
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
				Trace.WriteLine($"Can't find any type which implements {nameof(TYPE)} in {assembly} from {assembly.Location}.\n" +
					$"Available types: {availableTypes}");
			}
		}
	}
}
