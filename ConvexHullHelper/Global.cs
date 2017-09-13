using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace ConvexHullHelper
{
	public class Global : INotifyPropertyChanged
	{
		// ******************************************************************
		public static Global Instance { get; } = new Global();

		// ******************************************************************
		private bool _isTimerRunning = false;
		private Timer Timer { get; } = null;


		// ******************************************************************
		private Global()
		{
			Timer = new Timer();
			Timer.AutoReset = false;
			Timer.Elapsed += TimerOnElapsed;
			Timer.Interval = 1000;
		}

		// ******************************************************************
		private int _iteration = 0;
		public int Iteration
		{
			get { return _iteration; }
			set
			{
				_iteration = value;

				if (!_isTimerRunning)
				{
					_isTimerRunning = true;
					Timer.Enabled = true;
					Timer.Start();
				}
			}
		}
		
		// ******************************************************************
		private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			_isTimerRunning = false;
			NotifyPropertyChanged(nameof(Iteration));
		}
		
		// ******************************************************************
		private string _quadrant;
		public string Quadrant
		{
			get { return _quadrant; }
			set
			{
				_quadrant = value;
				NotifyPropertyChanged(nameof(Quadrant));
			}
		}

		// ******************************************************************
		public event PropertyChangedEventHandler PropertyChanged;

		// ******************************************************************
		protected void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if (propertyChanged != null)
			{
				Application.Current.Dispatcher.BeginInvoke(new Action(() => propertyChanged(this, new PropertyChangedEventArgs(propertyName))), DispatcherPriority.ContextIdle);
			}
		}

		// ******************************************************************
		private bool _isCancel = false;

		public bool IsCancel => _isCancel;

		// ******************************************************************
		public void Cancel()
		{
			_isCancel = true;
			NotifyPropertyChanged(nameof(IsCancel));
		}

		// ******************************************************************
		public void ResetCancel()
		{
			_isCancel = false;
			NotifyPropertyChanged(nameof(IsCancel));
		}

		// ******************************************************************
	}
}
