using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using ConvexHullHelper;
using OxyPlot;
using OxyPlot.Series;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Timers;
using System.Windows.Controls;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Loyc.Collections;
using Mathematic;
using OuelletConvexHullAvl3;
using MarkerType = OxyPlot.MarkerType;

namespace ConvexHullWorkbench
{
	public class MainWindowModel : NotifyPropertyChangeBase
	{
		// ******************************************************************
		private string _quadrant;
		private int _iteration;
		private PlotModel _plotModel;
		private bool _isCountOfPointSpecific = true;
		private bool _isCountOfPointRandom = false;
		private int _countOfPoint = 1000;
		private int _countOfPointMin = 1000;
		private int _countOfPointMax = 1000000;
		private PointGenerator _pointGeneratorSelected = ConvexHullHelper.PointGeneratorManager.Instance.Generators[0];
		private Random _rnd = new Random();

		// ******************************************************************
		public AlgorithmManager AlgorithmManager
		{
			get { return AlgorithmManager.Instance; }
		}

		// ******************************************************************
		public PointGeneratorManager PointGeneratorManager
		{
			get { return PointGeneratorManager.Instance; }
		}

		// ******************************************************************
		public string Quadrant
		{
			get { return _quadrant; }
			set
			{
				if (value == _quadrant) return;
				_quadrant = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public int Iteration
		{
			get { return _iteration; }
			set
			{
				if (value == _iteration) return;
				_iteration = value;
				RaisePropertyChanged();
			}
		}

		public Global Global => Global.Instance;

		// ******************************************************************
		/// <summary>
		/// Gets the plot model.
		/// </summary>
		public PlotModel PlotModel
		{
			get { return _plotModel; }
			private set
			{
				if (Equals(value, _plotModel)) return;
				_plotModel = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public MainWindowModel()
		{
			GeneratePoints();
			ShowPoints();
			GenerateConvexHulls(new List<Algorithm> { AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullSingleThread] });

			// Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				RaisePropertyChanged(nameof(PlotModel));
				// this.PlotModel = tmp;
			}), DispatcherPriority.ContextIdle);

			Global.Instance.PropertyChanged += InstanceOnPropertyChanged;

			IsCountOfPointRandom = true;

			AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexChan].IsSelected = true;
			//AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexLiuAndChen].IsSelected = true;

			//AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullSingleThread].IsSelected = true;
			//AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullSingleThreadArray].IsSelected = true;
			//AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullSingleThreadArrayMemCpy].IsSelected = true;
			//AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullSingleThreadArrayImmu].IsSelected = true;
			////AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullSingleThreadArrayMemCpyNoIndirect].IsSelected = true;
			//AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullAvl].IsSelected = true;
			AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullAvl2].IsSelected = true;

			AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullAvl2Online].IsSelected = true;
			AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullAvl2OnlineWithOnlineUse].IsSelected = true;

			AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHull4Threads].IsSelected = true;
			AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullMultiThreads].IsSelected = true;

			AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullCpp].IsSelected = true;
		}

		// ******************************************************************
		public string SelectedGeneratorDescription
		{
			get
			{
				int min = IsCountOfPointSpecific ? CountOfPoint : CountOfPointMin;
				int max = IsCountOfPointSpecific ? CountOfPoint : CountOfPointMax;

				return $"{PointGeneratorSelected} : {min} - {max}";
			}
		}

		// ******************************************************************
		private void InstanceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Iteration")
			{
				Iteration = Global.Iteration;
			}
		}

		// ******************************************************************
		public bool IsCountOfPointSpecific
		{
			get { return _isCountOfPointSpecific; }
			set
			{
				if (value == _isCountOfPointSpecific) return;
				_isCountOfPointSpecific = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public bool IsCountOfPointRandom
		{
			get { return _isCountOfPointRandom; }
			set
			{
				if (value == _isCountOfPointRandom) return;
				_isCountOfPointRandom = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public int CountOfPoint
		{
			get { return _countOfPoint; }
			set
			{
				if (value == _countOfPoint) return;
				_countOfPoint = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public int CountOfPointMin
		{
			get { return _countOfPointMin; }
			set
			{
				if (value == _countOfPointMin) return;
				_countOfPointMin = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public int CountOfPointMax
		{
			get { return _countOfPointMax; }
			set
			{
				if (value == _countOfPointMax) return;
				_countOfPointMax = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public int CountOfTest
		{
			get { return _countOfTest; }
			set
			{
				if (value == _countOfTest) return;
				_countOfTest = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public int CountOfSourcePoints
		{
			get { return Enumerable.Count(_points); }
		}

		// ******************************************************************
		public PointGenerator PointGeneratorSelected
		{
			get { return _pointGeneratorSelected; }
			set
			{
				if (Equals(value, _pointGeneratorSelected)) return;
				_pointGeneratorSelected = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		private Point[] _points;
		private int _countOfTest = 100;

		public Point[] Points => _points;

		// ******************************************************************
		public void GeneratePoints(int countOfPts = -1)
		{
			int qty;

			if (countOfPts != -1)
			{
				qty = countOfPts;
			}
			else
			{
				if (IsCountOfPointRandom)
				{
					qty = (int)((_rnd.NextDouble() * (CountOfPointMax - CountOfPointMin)) + CountOfPointMin);
				}
				else
				{
					qty = CountOfPoint;
				}
			}

			_points = PointGeneratorSelected.GeneratorFunc(qty);
		}

		// ******************************************************************
		public void ShowPoints()
		{
			AddMessage($"Adding {_points.Length} points in OxyPlot started.");

			var series = new ScatterSeries()
			{
				Title = PointGeneratorSelected.Name + " point generator",
				MarkerType = MarkerType.Circle,
				MarkerSize = 2
			};

			for (int ptIndex = 0; ptIndex < _points.Length; ptIndex++)
			{
				series.Points.Add(new ScatterPoint(_points[ptIndex].X, _points[ptIndex].Y)); // new DataPoint(0, 0));
			}

			var tmp = new PlotModel { Title = "ConvexHull Workbench", Subtitle = $"'Sample test' with {series.Points.Count} points " };

			SetOxyPlotDefaultColorPalette(tmp);

			tmp.Series.Add(series);
			this.PlotModel = tmp;

			AddMessage($"Adding {_points.Length} points in OxyPlot ended.");
		}

		// ******************************************************************
		public void GenerateConvexHulls(List<Algorithm> algorithms)
		{
			while (PlotModel.Series.Count > 1)
			{
				PlotModel.Series.RemoveAt(0);
			}

			foreach (Algorithm algo in algorithms)
			{
				var series = new LineSeries { Title = algo.Name, MarkerType = MarkerType.Square, MarkerFill = algo.Color };

				var algoStd = algo as AlgorithmStandard;
				if (algoStd == null)
				{
					throw new System.ArgumentException("The algorithm is not a Standard Convex Hull algorithm.");
				}

				var points = algoStd.Calc(_points, null);

				Debug.Assert(points != null);

				AddMessage($"Adding {points.Length} convex hull points in OxyPlot started.");

				foreach (Point pt in points)
				{
					series.Points.Add(new DataPoint(pt.X, pt.Y));
				}

				this.PlotModel.Series.Insert(0, series);
				this.PlotModel.PlotView?.InvalidatePlot();

				AddMessage($"Adding {points.Length} convex hull points in OxyPlot ended.");
			}
		}

		// ******************************************************************
		private void SetOxyPlotDefaultColorPalette(PlotModel plotModel)
		{
			plotModel.DefaultColors = new List<OxyColor>
			{
				OxyColors.Red,
				OxyColors.Green,
				OxyColors.Blue,
				OxyColors.LightSkyBlue,
				OxyColors.Purple,
				OxyColors.HotPink,
				OxyColors.Olive,
				OxyColors.SaddleBrown,
				OxyColors.Indigo,
				OxyColors.YellowGreen,
				OxyColors.Thistle,
				OxyColors.SlateBlue,
				OxyColors.Orange,
				OxyColors.Khaki,
			};
		}

		//// ******************************************************************
		//private int _countOfPointToAddSequentially = 1000;

		//public int CountOfPointToAddSequentially
		//{
		//	get { return _countOfPointToAddSequentially; }
		//	set
		//	{
		//		_countOfPointToAddSequentially = value;
		//		RaisePropertyChanged();
		//	}
		//}

		// ******************************************************************
		//private bool _isUseGetPreviousInsteadOfFullArrayCopy = false;

		//public bool IsUseGetPreviousInsteadOfFullArrayCopy
		//{
		//	get => _isUseGetPreviousInsteadOfFullArrayCopy;
		//	set
		//	{
		//		_isUseGetPreviousInsteadOfFullArrayCopy = value;
		//		RaisePropertyChanged();
		//	}
		//}
		private EnumAddPointPostBehavior _enumAddPointPostBehavior = EnumAddPointPostBehavior.GetResultAsArrayOfPointAlways;

		public EnumAddPointPostBehavior EnumAddPointPostBehavior
		{
			get { return _enumAddPointPostBehavior; }
			set
			{
				_enumAddPointPostBehavior = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		public void SpeedTest(List<Algorithm> algorithms, bool isOnlineTest)
		{
#if DEBUG
			if (
				MessageBox.Show("You are running in debug mode. Results would not reflect the reality. Do you want to continue?",
					"Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
			{
				return;
			}
#endif

			Global.Instance.ResetCancel();

			Dictionary<Algorithm, ScatterSeries> algoToSeries = new Dictionary<Algorithm, ScatterSeries>();

			string onlineTest = null;
			if (isOnlineTest)
			{
				onlineTest = $" ({EnumAddPointPostBehavior})";
			}
			else
			{
				onlineTest = "";
			}

			var tmp = new PlotModel { Title = "ConvexHull Workbench", Subtitle = $"Speed Test for {SelectedGeneratorDescription} {onlineTest}" };

			SetOxyPlotDefaultColorPalette(tmp);

			tmp.LegendPosition = LegendPosition.LeftTop;

			foreach (var algo in algorithms)
			{
				var series = new ScatterSeries() { Title = algo.Name, MarkerType = MarkerType.Circle, MarkerSize = 2, MarkerFill = algo.Color };

				algoToSeries.Add(algo, series);
				tmp.Series.Add(series);
			}

			this.PlotModel = tmp;

			tmp.DefaultXAxis.Title = "Points";
			tmp.DefaultYAxis.Title = "Millisecs";

			Task.Run(() =>
			{
				AddMessage("Speed test started. You can look the iteration in the status bar to know the status of the test.");

				for (Iteration = 0; Iteration < CountOfTest; Iteration++)
				{
					//if (isOnlineTest)
					//{
					//	GeneratePoints(_countOfPointToAddSequentially);
					//	pointsToAdd = _points;
					//}

					GeneratePoints();

					foreach (var algo in algorithms)
					{
						if (Global.IsCancel)
						{
							AddMessage("Speed test canceled");
							Global.ResetCancel();
							return;
						}

						AlgorithmStat stat;
						Stopwatch stopwatch = new Stopwatch();

						Point[] resultPoints = new Point[2];
						resultPoints[0] = _points[0];
						resultPoints[1] = _points[1];
						IReadOnlyList<Point> result = resultPoints;

						TimeSpan timeSpanTotal = new TimeSpan();

						if (isOnlineTest)
						{
							var algoStd = algo as AlgorithmStandard;
							var algoOnline = algo as AlgorithmOnline;

							stat = new AlgorithmStat();
							//if (algoStd != null)
							//{
							//	stopwatch.Reset();
							//	stopwatch.Start();
							//	result = algoStd.Calc(_points, stat);
							//	stopwatch.Stop();
							//	stat.TimeSpanCSharp = stopwatch.Elapsed;
							//	timeSpanTotal += stat.BestTimeSpan;
							//}
							//else
							//{
							//	stopwatch.Reset();
							//	stopwatch.Start();
							//	algoOnline.Init();
							//	var cho = algoOnline.Algo as ConvexHull;
							//	foreach (Point pt in _points)
							//	{
							//		cho.TryAddOnePoint(pt);
							//	}
							//	result = algoOnline.GetResult();
							//	stopwatch.Stop();

							//	timeSpanTotal += stopwatch.Elapsed;
							//}

							if (algoOnline != null)
							{
								if (algoOnline.Algo == null)
								{
									algoOnline.Init();
								}

								//var cho = algoOnline.Algo as OuelletConvexHullAvl3.ConvexHull;

								result = null;

								stopwatch.Reset();
								stopwatch.Start();
								foreach (Point point in _points)
								{
									// cho.TryAddOnePoint(point);
									bool isAdded = algoOnline.AddPoint(point);

									switch (EnumAddPointPostBehavior)
									{
										case EnumAddPointPostBehavior.QueryOnlyNeighborsWhenPointIsConvexHullAdded:
											{
												if (isAdded)
												{
													var cho = algoOnline.Algo as OuelletConvexHullAvl3.ConvexHull;
													cho.GetNeighbors(point);
												}
												break;
											}
										case EnumAddPointPostBehavior.GetResultAsArrayOfPointAlways:
											{
												result = algoOnline.GetResult();
												break;
											}
										case EnumAddPointPostBehavior.GetResultAsArrayOfPointOnlyWhenPointIsConvexHullAdded:
											{
												if (isAdded)
												{
													result = algoOnline.GetResult();

												}
												break;
											}
										case EnumAddPointPostBehavior.DoNothing:
											{
												break;
											}
									}
								}
								stopwatch.Stop();

								if (result == null)
								{
									result = algoOnline.GetResult();
								}

								timeSpanTotal += stopwatch.Elapsed;
							}
							else
							{
								foreach (Point point in _points)
								{
									stopwatch.Reset();
									stopwatch.Start();

									Point[] hullpoints = result as Point[];
									Array.Resize(ref hullpoints, hullpoints.Length + 1);

									stopwatch.Stop();

									timeSpanTotal += stopwatch.Elapsed;

									stopwatch.Reset();
									stopwatch.Start();

									hullpoints[hullpoints.Length - 1] = point;
									result = algoStd.Calc(hullpoints, stat);

									stopwatch.Stop();
									stat.TimeSpanCSharp = stopwatch.Elapsed;

									timeSpanTotal += stat.BestTimeSpan;
								}
							}

							stat.TimeSpanOriginal = timeSpanTotal;
							stat.TimeSpanCSharp = timeSpanTotal;
							stat.PointCount = _points.Length;
						}
						else // Not online
						{
							stat = new AlgorithmStat();
							var algoStd = algo as AlgorithmStandard;
							if (algoStd == null)
							{
								MessageBox.Show($"Stats can only be acheived on '{nameof(AlgorithmStandard)}' (Not online).");
								return;
							}

							stopwatch.Reset();
							stopwatch.Start();
							result = algoStd?.Calc(_points, stat);
							stopwatch.Stop();

							stat.TimeSpanCSharp = stopwatch.Elapsed; //  TimeSpan.FromTicks(stopwatch.ElapsedTicks);
							stat.PointCount = _points.Length;
						}

						stat.ResultCount = result?.Count ?? -1;

						if (Global.IsCancel)
						{
							AddMessage("Speed test canceled");
							Global.ResetCancel();
							return;
						}


						Application.Current.Dispatcher.BeginInvoke(new Action(() =>
						{
							ScatterPoint sp = new ScatterPoint(_points.Length, stat.BestTimeSpan.TotalMilliseconds);
							algoToSeries[algo].Points.Add(sp);
						}));
					}

					Application.Current.Dispatcher.BeginInvoke(new Action(() =>
					{
						this.PlotModel.PlotView.InvalidatePlot();
					}));
				}

				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					AddLinearRegression();
				}));

				AddMessage("Speed test ended.");
			});
		}

		// ******************************************************************
		private Point[] MergeArrays(Point[] array1, Point[] array2)
		{
			int count = array1.Length + array2.Length;
			Point[] points = new Point[count];
			Array.Copy(array1, points, array1.Length);
			Array.Copy(array2, 0, points, array1.Length, array2.Length);
			return points;
		}

		// ******************************************************************
		private void AddLinearRegression()
		{
			if (this.IsCountOfPointRandom)
			{
				List<ScatterSeries> allSeries = new List<ScatterSeries>(PlotModel.Series.Cast<ScatterSeries>());

				foreach (ScatterSeries series in allSeries)
				{
					List<Point> points = new List<Point>(series.Points.Count);

					series.Points.ForEach(scatterPoint => points.Add(new Point(scatterPoint.X, scatterPoint.Y)));

					double rsSquared;
					double yIntercept;
					double slope;

					LinearRegression.Calc(points, 0, points.Count, out rsSquared, out yIntercept, out slope);

					Point[] pts = new Point[2];
					pts[0].X = this.CountOfPointMin;
					pts[0].Y = slope * pts[0].X + yIntercept;

					pts[1].X = this.CountOfPointMax;
					pts[1].Y = slope * pts[1].X + yIntercept;

					// AddSeriesLines(pts, PlotModel, series.Title + " (Linear Regression)", MarkerType.Circle, 1, 1, series.ActualMarkerFillColor);
					AddSeriesLines(pts, PlotModel, null, MarkerType.Circle, 1, 1, series.ActualMarkerFillColor);
				}

				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					this.PlotModel?.PlotView.InvalidatePlot();
				}));
			}
		}

		// ******************************************************************
		private void AddSeriesLines(IReadOnlyList<Point> points, PlotModel plotModel, string title, MarkerType markerType = MarkerType.Circle, int markerSize = 2, int strokeTickness = 2, OxyColor color = default(OxyColor))
		{
			if (points != null && points.Count > 0)
			{
				LineSeries series;
				if (title == null)
				{
					series = new LineSeries {MarkerType = markerType, MarkerSize = markerSize, StrokeThickness = strokeTickness};
				}
				else
				{
					series = new LineSeries { Title = title, MarkerType = markerType, MarkerSize = markerSize, StrokeThickness = strokeTickness };
				}

				for (int ptIndex = 0; ptIndex < points.Count; ptIndex++)
				{
					series.Points.Add(new DataPoint(points[ptIndex].X, points[ptIndex].Y));
				}

				if (color != default(OxyColor))
				{
					series.Color = color;
				}

				plotModel.Series.Add(series);
			}
		}

		// ******************************************************************
		public ObservableCollection<LogEntry> Messages { get; } = new ObservableCollection<LogEntry>();

		// ******************************************************************
		public void AddMessage(string message)
		{
			if (Application.Current.Dispatcher.CheckAccess())
			{
				Messages.Add(new LogEntry(Messages.Count, message));
			}
			else
			{
				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					Messages.Add(new LogEntry(Messages.Count, message));
				}));
			}
		}

		// ******************************************************************
		public void AlgorithmTests(List<Algorithm> algorithms)
		{
			Global.Instance.ResetCancel();

			Func<DifferencesInPath, ExecutionState> funcShouldStopTesting = (diffs) =>
			{
				if (diffs.HasErrors)
				{
					AddMessage(diffs.Description + "\r\n");

					if (diffs.Exception == null)
					{
						var tmp = new PlotModel { Title = "ConvexHull Workbench", Subtitle = "Tests results" };

						var series = new LineSeries()
						{
							Title = "Ref",
							MarkerType = MarkerType.Circle,
							MarkerSize = 3
						};

						for (int ptIndex = 0; ptIndex < diffs.PointsRef.Count; ptIndex++)
						{
							series.Points.Add(new DataPoint(diffs.PointsRef[ptIndex].X, diffs.PointsRef[ptIndex].Y));
							// new DataPoint(0, 0));
						}

						tmp.Series.Add(series);

						series = new LineSeries()
						{
							Title = "Points",
							MarkerType = MarkerType.Circle,
							MarkerSize = 3
						};

						for (int ptIndex = 0; ptIndex < diffs.Points.Count; ptIndex++)
						{
							series.Points.Add(new DataPoint(diffs.Points[ptIndex].X, diffs.Points[ptIndex].Y)); // new DataPoint(0, 0));
						}

						tmp.Series.Add(series);

						ScatterSeries scatterSeries = new ScatterSeries()
						{
							Title = "Unwanted points",
							MarkerType = MarkerType.Square,
							MarkerSize = 5
						};

						for (int ptIndex = 0; ptIndex < diffs.UnwantedPoints.Count; ptIndex++)
						{
							scatterSeries.Points.Add(new ScatterPoint(diffs.UnwantedPoints[ptIndex].X, diffs.UnwantedPoints[ptIndex].Y));
							// new DataPoint(0, 0));
						}

						tmp.Series.Add(scatterSeries);

						scatterSeries = new ScatterSeries()
						{
							Title = "Missing points",
							MarkerType = MarkerType.Square,
							MarkerSize = 5
						};

						for (int ptIndex = 0; ptIndex < diffs.MissingPoints.Count; ptIndex++)
						{
							scatterSeries.Points.Add(new ScatterPoint(diffs.MissingPoints[ptIndex].X, diffs.MissingPoints[ptIndex].Y));
							// new DataPoint(0, 0));
						}

						tmp.Series.Add(scatterSeries);

						this.PlotModel = tmp;
					}

					if (MessageBox.Show("Error Found! Continue ?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
					{
						return ExecutionState.Continue;
					}

					return ExecutionState.Stop;
				}

				return ExecutionState.Continue;
			};

			Task.Run(new Action(() =>
			{
				AddMessage("Testing started: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

				try
				{
					foreach (var algo in algorithms.Where(a => a.AlgorithmType == AlgorithmType.ConvexHull && a.IsSelected).ToList())
					{
						AddMessage("Testing algorithm: " + algo.Name);

						Func<Point[], IReadOnlyList<Point>> funcConvexHull = null;
						var algoStd = algo as AlgorithmStandard;
						if (algo != null)
						{
							funcConvexHull = (points) => algoStd.Calc(points, null);
						}
						else
						{
							var algoOnline = algo as AlgorithmOnline;
							if (algo != null)
							{
								funcConvexHull = (Points) =>
								{
									algoOnline.Init();
									foreach (var pt in Points)
									{
										algoOnline.AddPoint(pt);
									}

									return algoOnline.GetResult();
								};
							}
						}

						if (funcConvexHull == null)
						{
							throw new System.InvalidOperationException("Unable to get an appropriate 'Convex Hull Func'.");
						}

						ConvexHullTests tests = new ConvexHullTests(algo.Name, funcConvexHull, funcShouldStopTesting);
						tests.TestSpecialCases();
						tests.ExtensiveTests();
					}
				}
				catch (Exception)
				{
				}

				AddMessage("Testing ended: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			}));
		}

		// ******************************************************************
		public void ValidateAgainstOuelletSharpSingleThread(List<Algorithm> algorithms)
		{
			Global.Instance.Iteration = 0;

			Algorithm algoRef = AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullSingleThread];
			var algoRefStd = algoRef as AlgorithmStandard;

			if (algorithms.Count == 0)
			{
				MessageBox.Show("You should at least select on algorithm that is not 'Ouellet Single Thead'.");
				return;
			}

			algorithms.RemoveAll(algo => algo == algoRef);

			Task.Run(new Action(() =>
			{
				for (Iteration = 0; Iteration < CountOfTest; Iteration++)
				{
					Global.Instance.Iteration++;

					GeneratePoints();

					var refPoints = algoRefStd?.Calc(_points, null);

					foreach (var algo in algorithms.Where(a => a.AlgorithmType == AlgorithmType.ConvexHull && a.IsSelected).ToList())
					{
						if (Global.Instance.IsCancel)
						{
							MessageBox.Show("Side by side test cancelled"); // GUI in model... i'm lazy, sorry.
							Global.Instance.ResetCancel();
							return;
						}

						var algoStd = algo as AlgorithmStandard;
						DifferencesInPath diffs = ConvexHullUtil.GetPathDifferences(algo.Name, _points, refPoints, algoStd.Calc(_points, null));

						if (diffs.HasErrors)
						{
							EnumerableExtensions.ForEach(diffs.PointsRef, pt => Debug.Print($"Pt: {pt}"));

							var tmp = new PlotModel { Title = "Convex Hull differences", Subtitle = "using OxyPlot" };

							AddSeriesPoints(diffs.UnwantedPoints, tmp, "Unwanted points", MarkerType.Square, 9);
							AddSeriesPoints(diffs.MissingPoints, tmp, "Missing points", MarkerType.Square, 7);

							AddSeriesLines(refPoints, tmp, "Ref lines", MarkerType.Circle, 4, 3);
							AddSeriesLines(diffs.Points, tmp, algo.Name, MarkerType.Circle, 2, 1);

							this.PlotModel = tmp;

							Application.Current.Dispatcher.BeginInvoke(new Action(() =>
							{
								this.PlotModel.PlotView.InvalidatePlot();
							}));

							var result = MessageBox.Show("Diffs found, do you want to continue to search for diffs?", "Continue?", MessageBoxButton.YesNo);
							if (result == MessageBoxResult.No)
							{
								return;
							}
						}
					}
				}

				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					MessageBox.Show("Algo diffs ended.");
				}));
			}));
		}

		// ******************************************************************
		private void AddSeriesPoints(IReadOnlyList<Point> points, PlotModel plotModel, string title, MarkerType markerType = MarkerType.Circle, int markerSize = 2)
		{
			if (points != null && points.Count > 0)
			{
				var series = new ScatterSeries { Title = title, MarkerType = markerType, MarkerSize = markerSize };
				for (int ptIndex = 0; ptIndex < points.Count; ptIndex++)
				{
					series.Points.Add(new ScatterPoint(points[ptIndex].X, points[ptIndex].Y));
				}
				plotModel.Series.Add(series);
			}
		}

		// ******************************************************************
		private void AddSeriesLines(IReadOnlyList<Point> points, PlotModel plotModel, string title, MarkerType markerType = MarkerType.Circle, int markerSize = 2, int strokeTickness = 2)
		{
			if (points != null && points.Count > 0)
			{

				var series = new LineSeries() { Title = title, MarkerType = markerType, MarkerSize = markerSize, StrokeThickness = strokeTickness };
				for (int ptIndex = 0; ptIndex < points.Count; ptIndex++)
				{
					series.Points.Add(new DataPoint(points[ptIndex].X, points[ptIndex].Y));
				}
				plotModel.Series.Add(series);
			}
		}

		// ******************************************************************
		public int[] CountOfInputPoints { get; set; } = new int[] { 10, 100, 1000, 10000, 100000, 1000000, 10000000, 50000000 };

		private int _indexOfLastInputPointsCount = 5;
		public int IndexOfLastInputPointsCount
		{
			get { return _indexOfLastInputPointsCount; }
			set
			{
				if (_indexOfLastInputPointsCount == value) return;

				_indexOfLastInputPointsCount = value;
				RaisePropertyChanged();
			}
		}

		// ******************************************************************
		/// <summary>
		/// Performance test. Test selected algorithms with the same set of random points.
		/// Each of these test for a count of points is performed many time in order to get a good normalization (good average) of time taken.
		/// Results are shown in excel
		/// </summary>
		/// <param name="path"></param>
		public void TestForArticle(string path)
		{
			var wb = new XLWorkbook();
			var ws = wb.Worksheets.Add("Algo stats");

			// int[] countOfInputPoints = new int[] { 10, 100, 1000, 10000, 100000 };
			// int[] countOfInputPoints = new int[] { 10, 100, 1000, 10000, 100000, 1000000, 10000000, 50000000 };

			int countOfSimulationWhereTheAverageIsDone = 10; // more representative average result

			var pointGenerators = PointGeneratorManager.Generators.Where(g => g.IsSelected).ToList();

			//pointGenerators.Clear();
			//pointGenerators.Add(PointGeneratorManager.Generators[4]);

			var algos = GetSelectedAlgorithms();

			//algos.Clear();
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexHeap]);
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexMiConvexHull]);

			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexChan]);
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexLiuAndChen]);
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullSingleThread]);
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullAvl]);
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHull4Threads]);
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullMultiThreads]);
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullCpp]);

			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexChan]);
			//algos.Add(AlgorithmManager.Algorithms[AlgorithmManager.AlgoIndexOuelletConvexHullAvl]);

			int dataStartCol = 2;

			int algoIndex = 0;
			int pointGeneratorIndex = 0;

			double[,,] stats = new double[algos.Count, pointGenerators.Count, CountOfInputPoints.Length];

			// Data
			for (int countOfInputPointsIndex = 0; countOfInputPointsIndex <= _indexOfLastInputPointsCount; countOfInputPointsIndex++)
			{
				for (int countOfSimulationWhereTheAverageIsDoneIndex = 0; countOfSimulationWhereTheAverageIsDoneIndex < countOfSimulationWhereTheAverageIsDone; countOfSimulationWhereTheAverageIsDoneIndex++)
				{
					pointGeneratorIndex = 0;

					foreach (var pointGenerator in pointGenerators)
					{
						Point[] points = pointGenerator.GeneratorFunc(CountOfInputPoints[countOfInputPointsIndex]);

						algoIndex = 0;

						foreach (var algo in algos)
						{
							var algoStd = algo as AlgorithmStandard;

							AddMessage($"Input points: {CountOfInputPoints[countOfInputPointsIndex]}, Test: {countOfSimulationWhereTheAverageIsDoneIndex + 1}, Algo: {algo.Name}, Generator: {pointGenerator.Name}");

							var algoStat = new AlgorithmStat();
							Stopwatch stopWatch = new Stopwatch();
							stopWatch.Start();

							Point[] result = algoStd?.Calc(points, algoStat);

							stopWatch.Stop();
							algoStat.TimeSpanCSharp = stopWatch.Elapsed;

							stats[algoIndex, pointGeneratorIndex, countOfInputPointsIndex] += algoStat.BestTimeSpan.TotalMilliseconds;

							algoIndex++;
						}

						pointGeneratorIndex++;
					}
				}
			}

			int row = 3;

			var listOfMinTimePerLine = new double[CountOfInputPoints.Length];

			for (pointGeneratorIndex = 0; pointGeneratorIndex < pointGenerators.Count; pointGeneratorIndex++)
			{
				ws.Cell(row++, dataStartCol - 1).Value = $"{pointGenerators[pointGeneratorIndex].Name} generator";

				// Start: Header and also Ensure everything is properly loaded in memory
				ws.Cell(row, dataStartCol - 1).Value = "Points";
				algoIndex = 0;
				foreach (var algo in algos)
				{
					ws.Cell(row, dataStartCol + algoIndex).Value = algo.Name;
					algoIndex++;
				}
				row++;
				// End: Header and also Ensure everything is properly loaded in memory


				for (int countOfInputPointsIndex = 0; countOfInputPointsIndex <= _indexOfLastInputPointsCount; countOfInputPointsIndex++)
				{
					double minTimePerLine = 0;

					ws.Cell(row, dataStartCol - 1).Value = CountOfInputPoints[countOfInputPointsIndex];

					for (algoIndex = 0; algoIndex < algos.Count; algoIndex++)
					{
						double time = stats[algoIndex, pointGeneratorIndex, countOfInputPointsIndex] / countOfSimulationWhereTheAverageIsDone;

						ws.Cell(row, dataStartCol + algoIndex).Value = time;

						if (minTimePerLine <= 0)
						{
							minTimePerLine = time;
						}
						else
						{
							minTimePerLine = Math.Min(minTimePerLine, time);
						}
					}

					listOfMinTimePerLine[countOfInputPointsIndex] = minTimePerLine;
					row++;
				}

				ws.Cell(row, dataStartCol - 1).Value = $"{pointGenerators[pointGeneratorIndex].Name} generator ratio";
				row++;

				// Start: Header and also Ensure everything is properly loaded in memory
				ws.Cell(row, dataStartCol - 1).Value = "Points";
				algoIndex = 0;
				foreach (var algo in algos)
				{
					ws.Cell(row, dataStartCol + algoIndex).Value = algo.Name;
					algoIndex++;
				}
				row++;
				// End: Header and also Ensure everything is properly loaded in memory

				for (int countOfInputPointsIndex = 0; countOfInputPointsIndex <= _indexOfLastInputPointsCount; countOfInputPointsIndex++)
				{
					ws.Cell(row, dataStartCol - 1).Value = CountOfInputPoints[countOfInputPointsIndex];

					for (algoIndex = 0; algoIndex < algos.Count; algoIndex++)
					{
						ws.Cell(row, dataStartCol + algoIndex).Value = stats[algoIndex, pointGeneratorIndex, countOfInputPointsIndex] /
												  countOfSimulationWhereTheAverageIsDone / listOfMinTimePerLine[countOfInputPointsIndex];
					}

					row++;
				}

				row++;
			}

			// End: Print the ratio in regards to the best one


			wb.SaveAs(path);
			Process.Start(path);
		}

		// ******************************************************************
		public List<Algorithm> GetSelectedAlgorithms()
		{
			List<Algorithm> algorithms = new List<Algorithm>();

			foreach (Algorithm algo in AlgorithmManager.Algorithms)
			{
				if (algo.IsSelected)
				{
					algorithms.Add(algo);
				}
			}

			return algorithms;
		}

		// ******************************************************************
		public void CallOnlineConvexHullDifferentWays()
		{
			GeneratePoints(); // Points are generated in "Point[] _points";

			ConvexHull convexHull = new ConvexHull();

			// First way to call: Standard way to call (standard/batch)
			convexHull.CalcConvexHull(_points);

			// Usage of IEnumerable wrapper to loop over each convex hull points 
			// that are node in avl tree of each quadrant.
			foreach (Point pt in convexHull) { Debug.Print(pt.ToString()); }

			//Or can get an array copy of the convex hull points
			Point[] pointsStandardCall = convexHull.GetResultsAsArrayOfPoint();

			convexHull = new ConvexHull();

			// Second way to call: Adding one point at a time (online).
			foreach (Point pt in _points)
			{
				convexHull.TryAddOnePoint(pt);
			}

			Point[] pointsOnlineCall = convexHull.GetResultsAsArrayOfPoint();

			DifferencesInPath diffs = ConvexHullUtil.GetPathDifferences(nameof(ConvexHull), _points, pointsStandardCall, pointsOnlineCall);
			Debug.Assert(diffs.HasErrors == false);

			convexHull = new ConvexHull();
			Point[] allPoints = new Point[_points.Length * 2];
			Array.Copy(_points, allPoints, _points.Length);

			// Third way to call: Standard/Batch then Online
			convexHull.CalcConvexHull(_points);

			GeneratePoints(_points.Length); // Points are generated in "Point[] _points";
			Array.Copy(_points, 0, allPoints, _points.Length, _points.Length);

			foreach (Point pt in _points)
			{
				if (convexHull.TryAddOnePoint(pt) == EnumConvexHullPoint.ConvexHullPoint)
				{
					// Do nothing if only knowing if Hull point or not is enough

					// Get Only neighbors if enough
					var neighbors = convexHull.GetNeighbors(pt);

					// Query full result as an array
					var convexHullPoints = convexHull.GetResultsAsArrayOfPoint();
				}
			}

			// HERE: Verifying previous result
			ConvexHull convexHullOnline2 = new ConvexHull();
			convexHullOnline2.CalcConvexHull(allPoints);

			diffs = ConvexHullUtil.GetPathDifferences(nameof(ConvexHull), _points,
				convexHull.GetResultsAsArrayOfPoint(), convexHullOnline2.GetResultsAsArrayOfPoint());

			Debug.Assert(diffs.HasErrors == false);
		}

		// ******************************************************************

	}
}
