using OpenTK.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace OpenTKPluginBrowser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel;

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

			_viewModel = new MainViewModel();
			DataContext = _viewModel;
		}

		private void OpenTkControl_OnRender(TimeSpan delta)
		{
			_viewModel.Render((float)delta.TotalSeconds);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.Reload();
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
			_viewModel.Resize(OpenTkControl.FrameBufferWidth, OpenTkControl.FrameBufferHeight);
		}

		private void OpenTkControl_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) window.Close();
		}
	}
}