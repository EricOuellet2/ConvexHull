using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConvexHullHelper;
using DocumentFormat.OpenXml.Office2013.PowerPoint;
using OxyPlot;
using General;
using System.Runtime.InteropServices;
using OuelletConvexHullArray;
using Unsafe;

namespace ConvexHullWorkbench
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// ******************************************************************
		public MainWindow()
		{
			InitializeComponent();
            
			// ListBoxLog.Background = Brushes.LightYellow;

			Model = DataContext as MainWindowModel;
			Model.PropertyChanged += ModelOnPropertyChanged;
		}

		// ******************************************************************
		private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "PlotModel")
			{
				PlotViewMain.Model = Model.PlotModel;
			}
		}

		// ******************************************************************
		MainWindowModel Model { get; set; }

		// ******************************************************************
		private void GeneratePoints_Click(object sender, RoutedEventArgs e)
		{
			Model.AddMessage($"Generate points started.");
			Model.GeneratePoints();
			Model.AddMessage("Generate points ended.");

			if (Model.CountOfSourcePoints > 10000)
			{
				if (
					MessageBox.Show(
						$"The count of points is {Model.CountOfSourcePoints} and exceed 10000. It could take very long. Do you really want to continue?",
						"Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
				{
					return;
				}
			}

			Mouse.OverrideCursor = Cursors.Wait;

			Model.ShowPoints();

			Mouse.OverrideCursor = null;
		}

		// ******************************************************************
		private void StartAlgorithms_Click(object sender, RoutedEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Wait;

			Model.AddMessage("Generate Convex Hull started.");

			Model.GenerateConvexHulls(Model.GetSelectedAlgorithms());

			Model.AddMessage("Generate Convex Hull ended.");

			Mouse.OverrideCursor = null;
		}

		// ******************************************************************
		private void SpeedTestClick(object sender, RoutedEventArgs e)
		{
#if DEBUG
			MessageBox.Show("In order to get proper results, you should run this test in 'Release'");
#endif

			Model.SpeedTest(Model.GetSelectedAlgorithms());
		}

		// ******************************************************************
		private void AlgorithmTestsOnClick(object sender, RoutedEventArgs e)
		{
			Model.AlgorithmTests(Model.GetSelectedAlgorithms());
		}

		// ******************************************************************
		private void ValidateAgainstOuelletSharpSingleThreadClick(object sender, RoutedEventArgs e)
		{
			Model.ValidateAgainstOuelletSharpSingleThread(Model.GetSelectedAlgorithms());
		}

		//// ******************************************************************
		//private List<Algorithm> GetSelectedAlgorithms()
		//{
		//	List<Algorithm> algorithms = new List<Algorithm>();

		//	foreach (Algorithm algo in GridAlgorithms.SelectedItems)
		//	{
		//		algorithms.Add(algo as Algorithm);
		//	}

		//	return algorithms;
		//}

		// ******************************************************************
		private void ButtonCancelOnClick(object sender, RoutedEventArgs e)
		{
			Global.Instance.Cancel();
		}

		// ******************************************************************
		private void TestForArticle(object sender, RoutedEventArgs e)
		{
#if DEBUG
			if (MessageBox.Show("You are running in debug mode. Results would not reflect the reality. Do you want to continue?",
					"Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
			{
				return;
			}
#endif

			string path = System.IO.Path.GetTempFileName() + "- Milliseconds.xlsx";
			Debug.Print("Path: " + path);

			if (MessageBox.Show(this, "It could take many hours. Start now?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
			{
				return;
			}

			Model.Messages.Add(new LogEntry(0, "Path: " + path));

			Task.Run(() =>
			{
				try
				{
					Model.TestForArticle(path);
				}
				catch (Exception ex)
				{
					Debug.Print(ex.ToString());
				}

				Dispatcher.BeginInvoke(new Action(() =>
				{
					MessageBox.Show($"Test for article is done for: " + path);
				}));
			});
		}

		private double _fakeResult = 0;

		// ************************************************************************
		private void TestDivVsMult(object sender, RoutedEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Wait;

			double[] numbers = new double[256];
			Random rnd = new Random((int)DateTime.Now.Ticks);
			for (int n = 0; n < 256; n++)
			{
				numbers[n] = rnd.NextDouble() * 10000000 + 1;
			}

			Stopwatch stopWatch = new Stopwatch();
			long count;
			const long initialCount = 10000000000;
			count = initialCount;
			Byte index = 0;

			stopWatch.Start();
			while (true)
			{
				_fakeResult = numbers[index] / numbers[index];
				index++;
				count--;
				if (count == 0)
				{
					break;
				}
			}
			stopWatch.Stop();
			TimeSpan timeSpanDiv = stopWatch.Elapsed;

			count = initialCount;
			stopWatch.Start();
			while (true)
			{
				_fakeResult = numbers[index] * numbers[index];
				index++;
				count--;
				if (count == 0)
				{
					break;
				}
			}
			stopWatch.Stop();
			TimeSpan timeSpanMult = stopWatch.Elapsed;

			Mouse.OverrideCursor = Cursors.Arrow;

			MessageBox.Show($"Div: {timeSpanDiv.TotalMilliseconds}, Mult: {timeSpanMult.TotalMilliseconds}, Ratio (mult/div) = {timeSpanMult.TotalMilliseconds / timeSpanDiv.TotalMilliseconds}");
		}

		// ************************************************************************
		private void QuickTestClick(object sender, RoutedEventArgs e)
		{
			// Task.Run(new Action(() => QuickTestClickInternal()));


		}

		// ************************************************************************
		private void QuickTestClickInternal()
		{
			Console.WriteLine(DateTime.Now.ToString());

			int[] a;
			int count = 2;

			a = new int[10];
			a[0] = 1;
			a[1] = 3;

			//DebugUtil.Print(a);

			//int pointSize = Marshal.SizeOf(typeof(Point));

			//ArrayUtil.InsertItem(ref a, 2, 1, ref count);
			//Debug.Assert(a.EqualsEx(new int[] { 1, 2, 3 }, count));
			//Debug.Assert(count == 3);

			//DebugUtil.Print(a);

			//ArrayUtil.InsertItem(ref a, 4, 3, ref count);
			//Debug.Assert(a.EqualsEx(new int[] { 1, 2, 3, 4 }, count));
			//Debug.Assert(count == 4);

			//DebugUtil.Print(a);

			//ArrayUtil.InsertItem(ref a, 0, 0, ref count);
			//Debug.Assert(a.EqualsEx(new int[] { 0, 1, 2, 3, 4 }, count));
			//Debug.Assert(count == 5);

			//DebugUtil.Print(a);

			//ArrayUtil.RemoveRange(a, 3, 2, ref count);
			//Debug.Assert(a.EqualsEx(new int[] { 0, 1, 2 }, count));
			//Debug.Assert(count == 3);

			//DebugUtil.Print(a);

			//ArrayUtil.RemoveRange(a, 0, 2, ref count);
			//Debug.Assert(a.EqualsEx(new int[] { 2 }, count));
			//Debug.Assert(count == 1);

			//DebugUtil.Print(a);


			Point[] b = new Point[10];
			b[0] = new Point(1, 1);
			b[1] = new Point(3, 3);
			count = 2;

			List<Point> ptsref = new List<Point>();
			ptsref.Add(b[0]);
			ptsref.Add(b[1]);

			DebugUtil.Print(b);

			PointArrayUtilUnsafe.InsertItem(ref b, new Point(2, 2), 1, ref count);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(1, 1), new Point(2, 2), new Point(3, 3) }, count));
			Debug.Assert(count == 3);

			PointArrayUtilUnsafe.InsertItem(ref b, new Point(4, 4), 3, ref count);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4) }, count));
			Debug.Assert(count == 4);


			PointArrayUtilUnsafe.InsertItem(ref b, new Point(0, 0), 0, ref count);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4) }, count));
			Debug.Assert(count == 5);


			PointArrayUtilUnsafe.RemoveRange(ref b, 3, 2, ref count);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2)}, count));
			Debug.Assert(count == 3);


			PointArrayUtilUnsafe.RemoveRange(ref b, 0, 2, ref count);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(2, 2) }, count));
			Debug.Assert(count == 1);






			//b = new Point[10];
			//b[0] = new Point(1, 1);
			//b[1] = new Point(3, 3);
			//count = 2;

			//ptsref = new List<Point>();
			//ptsref.Add(b[0]);
			//ptsref.Add(b[1]);

			//DebugUtil.Print(b);

			//PointArrayUtil.ImmutableInsertItem(ref b, new Point(2, 2), 1, ref count);
			//DebugUtil.Print(b);
			//Debug.Assert(b.EqualsEx(new Point[] { new Point(1, 1), new Point(2, 2), new Point(3, 3) }, count));
			//Debug.Assert(count == 3);

			//PointArrayUtilUnsafe.InsertItem(ref b, new Point(4, 4), 3, ref count);
			//DebugUtil.Print(b);
			//Debug.Assert(b.EqualsEx(new Point[] { new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4) }, count));
			//Debug.Assert(count == 4);


			//PointArrayUtilUnsafe.InsertItem(ref b, new Point(0, 0), 0, ref count);
			//DebugUtil.Print(b);
			//Debug.Assert(b.EqualsEx(new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4) }, count));
			//Debug.Assert(count == 5);


			//PointArrayUtilUnsafe.RemoveRange(b, 3, 2, ref count);
			//DebugUtil.Print(b);
			//Debug.Assert(b.EqualsEx(new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2) }, count));
			//Debug.Assert(count == 3);


			//PointArrayUtilUnsafe.RemoveRange(b, 0, 2, ref count);
			//DebugUtil.Print(b);
			//Debug.Assert(b.EqualsEx(new Point[] { new Point(2, 2) }, count));
			//Debug.Assert(count == 1);






			b = new Point[0];
			b = ArrayUtil.ImmutableInsertItem(b, new Point(1, 1), 0);
			b = ArrayUtil.ImmutableInsertItem(b, new Point(3, 3), 1);
			count = 2;

			DebugUtil.Print(b);

			b = ArrayUtil.ImmutableInsertItem(b, new Point(2, 2), 1);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(1, 1), new Point(2, 2), new Point(3, 3) }));
			Debug.Assert(b.Length == 3);

			b = ArrayUtil.ImmutableInsertItem(b, new Point(4, 4), 3);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4) }));
			Debug.Assert(b.Length == 4);


			b = ArrayUtil.ImmutableInsertItem(b, new Point(0, 0), 0);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4) }));
			Debug.Assert(b.Length == 5);


			b = ArrayUtil.ImmutableRemoveRange(b, 3, 2);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2) }));
			Debug.Assert(b.Length == 3);


			b = ArrayUtil.ImmutableRemoveRange(b, 0, 2);
			DebugUtil.Print(b);
			Debug.Assert(b.EqualsEx(new Point[] { new Point(2, 2) }));
			Debug.Assert(b.Length == 1);







			a = new int[2];
			a[0] = 1;
			a[1] = 3;
			count = 2;
			DebugUtil.Print(a);

			a = ArrayUtil.ImmutableInsertItem(a, 2, 1);
			Debug.Assert(a.EqualsEx(new int[] { 1, 2, 3 }));
			Debug.Assert(a.Length == 3);

			DebugUtil.Print(a);

			a = ArrayUtil.ImmutableInsertItem(a, 4, 3);
			Debug.Assert(a.EqualsEx(new int[] { 1, 2, 3, 4 }));
			Debug.Assert(a.Length == 4);

			DebugUtil.Print(a);

			a = ArrayUtil.ImmutableInsertItem(a, 0, 0);
			Debug.Assert(a.EqualsEx(new int[] { 0, 1, 2, 3, 4 }));
			Debug.Assert(a.Length == 5);

			DebugUtil.Print(a);

			a = ArrayUtil.ImmutableRemoveRange(a, 3, 2);
			Debug.Assert(a.EqualsEx(new int[] { 0, 1, 2 }));
			Debug.Assert(a.Length == 3);

			DebugUtil.Print(a);

			a = ArrayUtil.ImmutableRemoveRange(a, 0, 2);
			Debug.Assert(a.EqualsEx(new int[] { 2 }));
			Debug.Assert(a.Length == 1);

			DebugUtil.Print(a);

			Point[] b1 = new Point[1000]; count = 0;
			Point[] b2 = new Point[0];
			bool insert = true;
			int index = 0;
			int countOfItemToRemove = 0;
			int b1Count = 0;

			Random rnd = new Random((int)DateTime.Now.Ticks);

			for (int n = 1; n < 1000000000; n++)
			{
				if(b1Count == 0)
				{
					insert = true;
				}
				else
				{
					insert = rnd.NextDouble() >= .2 ? true : false;
				}

				if (insert)
				{
					index = (int)(rnd.NextDouble() * (b1Count + 1));

					var p =new Point(rnd.NextDouble(), rnd.NextDouble());

					ArrayUtil.InsertItem(ref b1, p, index, ref b1Count);
					b2 = ArrayUtil.ImmutableInsertItem(b2, p, index);
				}
				else
				{
					countOfItemToRemove = (int)(rnd.NextDouble() * 3);

					index = (int)(rnd.NextDouble() * (b1Count - countOfItemToRemove));
					index = Math.Min(index, b1Count - 1);

					countOfItemToRemove = Math.Min(countOfItemToRemove, b1Count - index -1);

					ArrayUtil.RemoveRange(ref b1, index, countOfItemToRemove, ref b1Count);
					b2 = ArrayUtil.ImmutableRemoveRange(b2, index, countOfItemToRemove);

					Debug.Print(n.ToString());
				}

				ArrayUtil.EqualsEx(b1, b1Count, b2);


			}
		}

		// ******************************************************************
		private void ButtonClearAllClick(object sender, RoutedEventArgs e)
		{
			foreach(var algo in Model.AlgorithmManager.Algorithms)
			{
				algo.IsSelected = false;
			}
		}

		// ******************************************************************
		private void ButtonSelectAllClick(object sender, RoutedEventArgs e)
		{
			foreach (var algo in Model.AlgorithmManager.Algorithms)
			{
				if (algo.AlgorithmType == AlgorithmType.ConvexHull)
				{
					algo.IsSelected = true;
				}
				else
				{
					algo.IsSelected = false;
				}
			}
		}

		// ******************************************************************

	}
}
