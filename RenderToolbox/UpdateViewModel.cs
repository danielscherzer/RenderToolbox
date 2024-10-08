﻿using AutoUpdateViaGitHubRelease;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Zenseless.Patterns.Property;

namespace RenderToolbox
{
	internal class UpdateViewModel : NotifyPropertyChanged
	{
		public UpdateViewModel()
		{
			Update update = new();
			update.PropertyChanged += (s, a) => Available = update.Available;
			Assembly assembly = Assembly.GetExecutingAssembly();
			_ = update.CheckDownloadNewVersionAsync("danielScherzer", "RenderToolbox", assembly.GetName().Version, Path.GetTempPath());


			void UpdateAndClose()
			{
				_ = update.StartInstall(Path.GetDirectoryName(assembly.Location));
				Application app = Application.Current;
				app.Shutdown();
			}

			_command = new DelegateCommand(_ => UpdateAndClose(), _ => Available);
		}

		public bool Available { get => _available; private set => Set(ref _available, value, _ => CommandManager.InvalidateRequerySuggested()); }
		public ICommand Command => _command;

		private bool _available;
		private readonly DelegateCommand _command;
	}
}
