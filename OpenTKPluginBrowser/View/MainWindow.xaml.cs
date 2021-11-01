using OpenTK.Audio.OpenAL;
using OpenTK.Wpf;
using PluginBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Windows;
using System.Windows.Input;

namespace OpenTKPluginBrowser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<IPlugin> _plugins;
		//private readonly OpenGLContext _openGL;
		private static readonly string[] _pluginPaths = new string[]
		{
				// Paths to plugins to load.
				@"D:\Daten\Visual Studio 2019\Projects\OpenTKPluginBrowser\Triangle\bin\Debug\triangle.dll"
		};

		public MainWindow()
		{
			InitializeComponent();
			//_openGL = new OpenGLContext(4, 5, contextProfile:OpenTK.Windowing.Common.ContextProfile.Compatability);
			var settings = new GLWpfControlSettings
			{
				//ContextToUse = _openGL.Context,
				MajorVersion = 4,
				MinorVersion = 5,
				GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Compatability,
				RenderContinuously = false,
			};
			OpenTkControl.Start(settings);

			_plugins = _pluginPaths.SelectMany(PluginLoader.LoadPlugins).ToList();
		}

		private void OpenTkControl_OnRender(TimeSpan delta)
		{
			//foreach (var plugin in _plugins)
			//{
			//	plugin.Render(1 / 60f);
			//}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Button_Click(object sender, RoutedEventArgs e)
{
			HashSet<WeakReference<AssemblyLoadContext>> contextRefs = new();
			foreach (var plugin in _plugins)
			{
				var context = AssemblyLoadContext.GetLoadContext(plugin.GetType().Assembly);
				if(context != null)
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
			//foreach (var plugin in _plugins)
			//{
			//	plugin.Resize(OpenTkControl.FrameBufferWidth, OpenTkControl.FrameBufferHeight);
			//}
		}

		private void OpenTkControl_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{

		}
	}
}
