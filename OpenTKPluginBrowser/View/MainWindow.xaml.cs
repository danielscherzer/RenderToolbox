using OpenTK.Wpf;
using PluginBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace OpenTKPluginBrowser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly IEnumerable<IPlugin> _plugins;

		public MainWindow()
		{
			InitializeComponent();
			var settings = new GLWpfControlSettings
			{
				MajorVersion = 4,
				MinorVersion = 5,
				GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Compatability,
				RenderContinuously = false,
			};
			OpenTkControl.Start(settings);

			string[] pluginPaths = new string[]
			{
				// Paths to plugins to load.
				@"D:\Daten\Visual Studio 2019\Projects\OpenTKPluginBrowser\Triangle\bin\Debug\triangle.dll"
			};
			_plugins = pluginPaths.SelectMany(pluginPath =>
			{
				Assembly pluginAssembly = LoadPlugin(pluginPath);
				return CreateCommands(pluginAssembly);
			}).ToList();
		}

		private static IEnumerable<IPlugin> CreateCommands(Assembly pluginAssembly)
		{
			int count = 0;

			foreach (Type type in pluginAssembly.GetTypes())
			{
				if (typeof(IPlugin).IsAssignableFrom(type))
				{
					if (Activator.CreateInstance(type) is IPlugin plugin)
					{
						count++;
						yield return plugin;
					}
				}
			}

			if (count == 0)
			{
				string availableTypes = string.Join(",", pluginAssembly.GetTypes().Select(t => t.FullName));
				throw new ApplicationException(
					$"Can't find any type which implements {nameof(IPlugin)} in {pluginAssembly} from {pluginAssembly.Location}.\n" +
					$"Available types: {availableTypes}");
			}
		}

		private Assembly LoadPlugin(string pluginPath)
		{
			string pluginLocation = pluginPath;
			Console.WriteLine($"Loading commands from: {pluginLocation}");
			return Assembly.LoadFile(pluginPath);
		}

		private void OpenTkControl_OnRender(TimeSpan delta)
		{
			foreach (var plugin in _plugins)
			{
				plugin.Render(1 / 60f);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OpenTkControl.InvalidateVisual();
		}

		private void OpenTkControl_MouseDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void OpenTkControl_MouseMove(object sender, MouseEventArgs e)
		{
			OpenTkControl.InvalidateVisual();
			var position = e.GetPosition(OpenTkControl);
			Debug.WriteLine(position);
		}

		private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			foreach (var plugin in _plugins)
			{
				plugin.Resize(OpenTkControl.FrameBufferWidth, OpenTkControl.FrameBufferHeight);
			}
		}

		private void OpenTkControl_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{

		}
	}
}
