﻿using Jot;
#if !DEBUG
using Jot.Storage;
#endif
using System.Windows;

namespace RenderToolbox
{
	internal static class Persist
	{
#if DEBUG
		public static Tracker Tracker { get; } = new();
#else
		public static Tracker Tracker { get; } = new(new JsonFileStore("./"));
#endif

		internal static void Configure(Window window, MainViewModel mainViewModel)
		{
			_ = Tracker.Configure<Window>().Id(w => "Window")
				.Property(w => w.Left, 200)
				.Property(w => w.Top, 60)
				.Property(w => w.Width, 1024)
				.Property(w => w.Height, 1024)
				.WhenPersistingProperty((wnd, property) => property.Cancel = WindowState.Normal != wnd.WindowState)
				.PersistOn(nameof(Window.Closing));
			Tracker.Track(window);

			_ = Tracker.Configure<MainViewModel>().Id(vm => nameof(MainViewModel))
				.Property(vm => vm.PluginPath, "")
				.Property(vm => vm.RecentlyUsed)
				.PersistOn(nameof(Window.Closing), window);
			Tracker.Track(mainViewModel);
		}
	}
}
