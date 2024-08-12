using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Zenseless.RenderToolbox;

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
			if (Directory.Exists(temp)) Directory.Delete(temp, true);
			return temp;
		}

		public static IEnumerable<IPlugin> LoadPlugins(string assemblyFilePath)
		{
			Trace.WriteLine($"{nameof(LoadPlugins)}: Loading commands from: {assemblyFilePath}");
			if (!File.Exists(assemblyFilePath)) return Enumerable.Empty<IPlugin>();
			try
			{
				CollectibleLoadContext loadContext = new(assemblyFilePath);
				var tempPluginDir = Path.Combine(TempDir, DateTime.Now.ToString("yyyy-MM-dd_HHmmss", CultureInfo.InvariantCulture));
				if (!Directory.Exists(tempPluginDir)) _ = Directory.CreateDirectory(tempPluginDir);
				var newPath = Path.Combine(tempPluginDir, Path.GetFileName(assemblyFilePath));
				if (!File.Exists(newPath))
				{
					// copy all files in directory
					var sourceDir = Path.GetDirectoryName(assemblyFilePath);
					if (sourceDir is null)
					{
						Trace.WriteLine($"{nameof(LoadPlugins)}: '{assemblyFilePath}' contains no directory information.");
						return Enumerable.Empty<IPlugin>();
					}
					foreach (var file in Directory.EnumerateFiles(sourceDir, "*.*"))
					{
						File.Copy(file, Path.Combine(tempPluginDir, Path.GetFileName(file)));
					}
				}
				Assembly pluginAssembly = loadContext.LoadFromAssemblyPath(newPath);
				return CreateInstancesOf<IPlugin>(pluginAssembly);
			}
			catch (Exception e)
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
						++count;
						yield return instance;
					}
				}
			}

			if (0 == count)
			{
				string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
				Trace.WriteLine($"{nameof(CreateInstancesOf)}: Can't find any type which implements {nameof(TYPE)} in {assembly} from {assembly.Location}.\n" +
					$"Available types: {availableTypes}");
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Unload(this IPlugin plugin)
		{
			if (plugin is null) return;
			WeakReference<AssemblyLoadContext>? contextRef = null;
			AssemblyLoadContext? loadContext = AssemblyLoadContext.GetLoadContext(plugin.GetType().Assembly);
			if (loadContext != null)
			{
				contextRef = new(loadContext, trackResurrection: true);
			}

			if (plugin is IDisposable disposable) disposable.Dispose();
			//plugin = null;

			for (int i = 0; i < 10; ++i)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			if (contextRef != null)
			{
				if (contextRef.TryGetTarget(out var context))
				{
					context.Unload();
				}
			}
		}
	}
}
