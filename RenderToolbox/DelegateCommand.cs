using System;
using System.Windows.Input;

namespace RenderToolbox
{
	public class DelegateCommand<Type> : ICommand
	{
		private readonly Predicate<Type>? _canExecute;
		private readonly Action<Type> _execute;

		public event EventHandler? CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public DelegateCommand(Action<Type> execute, Predicate<Type>? canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(Type parameter)
		{
			return _canExecute == null || _canExecute(parameter);
		}

		public void Execute(Type parameter) => _execute(parameter);

		public static void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

		public bool CanExecute(object? parameter)
		{
			if (parameter is null) return false;
			return CanExecute((Type)parameter);
		}

		public void Execute(object? parameter)
		{
			if (parameter is null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}
			else
			{
				Execute((Type)parameter);
			}
		}
	}
}
