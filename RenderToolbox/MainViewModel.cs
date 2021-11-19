using PluginBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Windows.Input;
using Zenseless.Patterns;

namespace RenderToolbox
{
	internal class MainViewModel : NotifyPropertyChanged
	{
		public MainViewModel()
		{
			LoadCommand = new DelegateCommand<string>(path => PluginPath = path);
		}

		public ICommand LoadCommand { private set; get; }

		public IPlugin? Plugin { get => _plugin; private set => Set(ref _plugin, value); }

		public string PluginPath
		{
			get => _pluginPath; set
			{
				// TODO: Present trace output
				IEnumerable<IPlugin> plugins = PluginLoader.LoadPlugins(value);
				foreach (var plugin in plugins)
				{
					Set(ref _pluginPath, value);
					Plugin = plugin;
					RecentlyUsed.Add(value);
					_fileChangeSubscription?.Dispose();
					_fileChangeSubscription = TrackedFileObservable.DelayedLoad(value).Subscribe(
						fileName =>
						{
							PluginPath = value;
						});
					return; // load first
				}
			}
		}

		public ObservableCollection<string> RecentlyUsed { get => _recentlyUsed; set => Set(ref _recentlyUsed, value); }

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

		//private static IEnumerable<string> EnumeratePlugins(string searchPath)
		//{
		//	string jsonExtension = "*.deps.json";
		//	var fileNames = Directory.EnumerateFiles(searchPath, jsonExtension, SearchOption.AllDirectories);
		//	foreach (var fileName in fileNames.Select(name => name.Substring(0, 1 + name.Length - jsonExtension.Length)))
		//	{
		//		var dll = Path.GetFullPath(fileName) + ".dll";
		//		if (File.Exists(dll)) yield return dll;
		//	}
		//}

		private IDisposable? _fileChangeSubscription;
		private IPlugin? _plugin;
		private string _pluginPath = "";
		private ObservableCollection<string> _recentlyUsed = new();

		//private static IEnumerable<IPlugin> GetPlugins() => EnumeratePlugins(@"..\..\..\Plugins").SelectMany(PluginLoader.LoadPlugins);
	}
}