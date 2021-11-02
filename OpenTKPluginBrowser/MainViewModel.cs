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
		//private readonly OpenGLContext _openGL;
		private readonly string[] _pluginPaths = new string[]
		{
				// Paths to plugins to load.
				Path.GetFullPath(@"..\..\..\Triangle\bin\Debug\triangle.dll")
		};

		public MainViewModel()
		{
			_plugins = _pluginPaths.SelectMany(PluginLoader.LoadPlugins).ToList();
		}

		public IPlugin? Plugin => _plugins.FirstOrDefault();
		
		internal void Render(float frameTime)
		{
			foreach (var plugin in _plugins)
			{
				plugin.Render(frameTime);
			}
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
					contextRefs.Add(r);
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
			_plugins = _pluginPaths.SelectMany(PluginLoader.LoadPlugins).ToList();
		}

		internal void Resize(int frameBufferWidth, int frameBufferHeight)
		{
			foreach (var plugin in _plugins)
			{
				plugin.Resize(frameBufferWidth, frameBufferHeight);
			}
		}
	}
}