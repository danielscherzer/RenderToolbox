using Jot;
using Jot.Storage;
using System.Windows;

namespace RenderToolbox
{
	internal static class Persist
	{
		public static Tracker Tracker => _tracker;

		internal static void Configure(Window window, MainViewModel mainViewModel)
		{
			_tracker.Configure<Window>().Id(w => "Window")
				.Property(w => w.Left, 200)
				.Property(w => w.Top, 60)
				.Property(w => w.Width, 1024)
				.Property(w => w.Height, 1024)
				.WhenPersistingProperty((wnd, property) => property.Cancel = WindowState.Normal != wnd.WindowState)
				.PersistOn(nameof(Window.Closing));
			_tracker.Track(window);

			_tracker.Configure<MainViewModel>().Id(vm => nameof(MainViewModel))
				.Property(vm => vm.PluginPath, "")
				.Property(vm => vm.RecentlyUsed)
				.PersistOn(nameof(Window.Closing), window);
			_tracker.Track(mainViewModel);
		}

		private static readonly Tracker _tracker = new(new JsonFileStore("./"));
	}
}
