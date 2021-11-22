using PluginBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Zenseless.Patterns;

namespace RenderToolbox
{
	internal class MainViewModel : NotifyPropertyChanged
	{
		public MainViewModel()
		{
			LoadCommand = new TypedDelegateCommand<string>(path => PluginPath = path);
		}

		public ICommand LoadCommand { private set; get; }

		public IPlugin? Plugin { get => _plugin; private set => Set(ref _plugin, value); }

		public string PluginPath
		{
			get => _pluginPath; set
			{
				if (value == _pluginPath) return; // no change
				// TODO: Present trace output
				IEnumerable<IPlugin> plugins = PluginLoader.LoadPlugins(value);
				foreach (var plugin in plugins)
				{
					Set(ref _pluginPath, value);
					Plugin = plugin;
					//TODO: remove Application.Current.Dispatcher in VM
					Application.Current.Dispatcher.Invoke(() => RecentlyUsed.Insert(0, value));
					IEnumerable<string> distinct = RecentlyUsed.Distinct();
					RecentlyUsed = new ObservableCollection<string>(distinct);
					_fileChangeSubscription?.Dispose();
					_fileChangeSubscription = TrackedFileObservable.DelayedLoad(value).Subscribe(fileName => PluginPath = value);
					return; // load only first
				}
			}
		}

		public ObservableCollection<string> RecentlyUsed { get => _recentlyUsed; set => Set(ref _recentlyUsed, value/*, coll => BindingOperations.EnableCollectionSynchronization(coll, _lockObj)*/); }

		internal void Render(float frameTime) => Plugin?.Render(frameTime);

		internal void Resize(int frameBufferWidth, int frameBufferHeight) => Plugin?.Resize(frameBufferWidth, frameBufferHeight);

		private IDisposable? _fileChangeSubscription;
		//private readonly object _lockObj = new();
		private IPlugin? _plugin;
		private string _pluginPath = "";
		private ObservableCollection<string> _recentlyUsed = new();

		//private static IPlugin Load(string filePath)
		//{
		//	IEnumerable<IPlugin> plugins = PluginLoader.LoadPlugins(filePath);
		//	foreach (var plugin in plugins)
		//	{
		//		_fileChangeSubscription?.Dispose();
		//		_fileChangeSubscription = TrackedFileObservable.DelayedLoad(value).Subscribe(fileName => PluginPath = value);
		//		return plugin; // load only first
		//	}

		//}
	}
}