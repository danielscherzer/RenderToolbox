using PluginBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Zenseless.Patterns;

namespace RenderToolbox
{
	internal class MainViewModel : NotifyPropertyChanged
	{
		public MainViewModel()
		{
		}

		public IPlugin? Plugin { get => _plugin; private set => Set(ref _plugin, value); }

		public string PluginPath
		{
			get => _pluginPath; set
			{
				IEnumerable<IPlugin> plugins = PluginLoader.LoadPlugins(value);
				foreach (var plugin in plugins)
				{
					Set(ref _pluginPath, value);
					Plugin = plugin;
					return; // load first
				}
			}
		}

		internal void Render(float frameTime)
		{
			Plugin?.Render(frameTime);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void Unload()
		{
			if (Plugin is null) return;
			WeakReference<AssemblyLoadContext>? contextRef = null;
			AssemblyLoadContext? loadContext = AssemblyLoadContext.GetLoadContext(Plugin.GetType().Assembly);
			if (loadContext != null)
			{
				contextRef = new(loadContext, trackResurrection: true);
			}

			if (Plugin is IDisposable disposable) disposable.Dispose();
			Plugin = null;

			for (int i = 0; i < 100; ++i)
			//while (contextRefs.Any(r => r.TryGetTarget(out _)))
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

		internal void Resize(int frameBufferWidth, int frameBufferHeight)
		{
			Plugin?.Resize(frameBufferWidth, frameBufferHeight);
		}

		private static IEnumerable<string> EnumeratePlugins(string searchPath)
		{
			string jsonExtension = "*.deps.json";
			var fileNames = Directory.EnumerateFiles(searchPath, jsonExtension, SearchOption.AllDirectories);
			foreach (var fileName in fileNames.Select(name => name.Substring(0, 1 + name.Length - jsonExtension.Length)))
			{
				var dll = Path.GetFullPath(fileName) + ".dll";
				if (File.Exists(dll)) yield return dll;
			}
		}

		private string _pluginPath = "";
		private IPlugin? _plugin = null;

		private static IEnumerable<IPlugin> GetPlugins() => EnumeratePlugins(@"..\..\..\Plugins").SelectMany(PluginLoader.LoadPlugins);
	}
}