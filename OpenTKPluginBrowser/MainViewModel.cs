using PluginBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace OpenTKPluginBrowser
{
	internal class MainViewModel
	{
		private List<IPlugin> _plugins;

		public MainViewModel()
		{
			_plugins = GetPlugins().ToList();
		}

		public IPlugin? Plugin => _plugins.FirstOrDefault();
		
		internal void Render(float frameTime)
		{
			Plugin.Render(frameTime);
			//foreach (var plugin in _plugins)
			//{
			//	plugin.Render(frameTime);
			//}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void Reload()
		{
			HashSet<WeakReference<AssemblyLoadContext>> contextRefs = new();
			foreach (var plugin in _plugins)
			{
				var context = AssemblyLoadContext.GetLoadContext(plugin.GetType().Assembly);
				if (context != null)
				{
					WeakReference<AssemblyLoadContext> r = new(context, trackResurrection: true);
					_ = contextRefs.Add(r);
				}
			}
			foreach (var plugin in _plugins)
			{
				if (plugin is IDisposable disposable) disposable.Dispose();
			}
			_plugins.Clear();
			foreach (var contextRef in contextRefs)
			{
				if (contextRef.TryGetTarget(out var context))
				{
					context.Unload();
				}
			}
			while (contextRefs.Any(r => r.TryGetTarget(out _)))
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			_plugins = GetPlugins().ToList();
		}

		internal void Resize(int frameBufferWidth, int frameBufferHeight)
		{
			foreach (var plugin in _plugins)
			{
				plugin.Resize(frameBufferWidth, frameBufferHeight);
			}
		}

		private static IEnumerable<string> EnumeratePlugins(string searchPath)
		{
			string jsonExtension = "*.deps.json";
			var fileNames = Directory.EnumerateFiles(searchPath, jsonExtension, SearchOption.AllDirectories);
			foreach(var fileName in fileNames.Select(name => name.Substring(0, 1 + name.Length - jsonExtension.Length)))
			{
				var dll = Path.GetFullPath(fileName) + ".dll";
				if (File.Exists(dll)) yield return dll;
			}
		}
		private static IEnumerable<IPlugin> GetPlugins() => EnumeratePlugins(@"..\..\..\Plugins").SelectMany(PluginLoader.LoadPlugins);
	}
}