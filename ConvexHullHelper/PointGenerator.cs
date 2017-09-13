using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ConvexHullHelper
{
	public class PointGenerator : INotifyPropertyChanged
	{
		// ************************************************************************
		private static readonly Random _rnd = new Random((int)DateTime.Now.Ticks);

		public event PropertyChangedEventHandler PropertyChanged;

		// ************************************************************************
		public static Point[] GeneratePointsInCircle(int count)
		{
			var points = new Point[count];

			double radiusMax = _rnd.NextDouble() * 1000;
			Point centerPoint = new Point(_rnd.NextDouble() * 5000, _rnd.NextDouble() * 5000);

			for (int n = 0; n < count; n++)
			{
				double lenght = _rnd.NextDouble() * radiusMax;
				double angle = _rnd.NextDouble() * 2 * Math.PI;

				double x = centerPoint.X + (Math.Cos(angle) * lenght);
				double y = centerPoint.Y + (Math.Sin(angle) * lenght);

				points[n] = new Point(x, y);
			}

			return points;
		}

		// ************************************************************************
		public static Point[] GeneratePointsIn5Circles(int count)
		{
			var points = new Point[count];

			int pointCountLeft = count;
			int index = 0;

			for (int circle = 1; circle <= 5; circle++)
			{
				double radiusMax = _rnd.NextDouble() * 1000;
				Point centerPoint = new Point(_rnd.NextDouble() * 10000, _rnd.NextDouble() * 10000);

				int circlePointCount;

				if (circle != 5)
				{
					circlePointCount = (int)(_rnd.NextDouble() * pointCountLeft * .9);
					pointCountLeft -= circlePointCount;
				}
				else
				{
					circlePointCount = pointCountLeft;
					pointCountLeft = 0;
				}

				for (int n = 0; n < circlePointCount; n++)
				{
					double lenght = _rnd.NextDouble() * radiusMax;
					double angle = _rnd.NextDouble() * 2 * Math.PI;

					double x = centerPoint.X + (Math.Cos(angle) * lenght);
					double y = centerPoint.Y + (Math.Sin(angle) * lenght);

					points[index] = new Point(x, y);
					index++;
				}
			}
			return points;
		}

		// ************************************************************************
		public static Point[] GeneratePointsInRectangle(int count)
		{
			var points = new Point[count];

			for (int n = 0; n < count; n++)
			{
				double x = _rnd.NextDouble() * 1000;
				double y = _rnd.NextDouble() * 1000;

				points[n] = new Point(x, y);
			}

			return points;
		}

		// ************************************************************************
		public static Point[] GeneeratePointsThrowAway(int count)
		{
			var points = new Point[count];

			double x = _rnd.NextDouble() * 1000 - 500;
			double y = _rnd.NextDouble() * 1000 - 500;

			for (int n = 0; n < count; n++)
			{
				x += _rnd.NextDouble() * 1000 - 500;
				y += _rnd.NextDouble() * 1000 - 500;

				points[n] = new Point(x, y);
			}

			return points;
		}

		// ************************************************************************
		// Points are all on the border of the circle, all forming the ConvexHull
		public static Point[] GeneratePointsArcQuadrant4(int count)
		{
			var points = new Point[count];

			double radiusMax = _rnd.NextDouble() * 1000;
			Point centerPoint = new Point(_rnd.NextDouble() * 10000, _rnd.NextDouble() * 10000);

			double lenght = _rnd.NextDouble() * radiusMax;

			for (int n = 0; n < count; n++)
			{
				double angle = _rnd.NextDouble() * Math.PI / 2;

				double x = centerPoint.X + (Math.Cos(angle) * lenght);
				double y = centerPoint.Y - (Math.Sin(angle) * lenght);

				points[n] = new Point(x, y);
			}

			return points;
		}

		// ************************************************************************
		public string Name { get; private set; }
		public Func<int, Point[]> GeneratorFunc { get; private set; }

		public PointGenerator(string name, string shortName, Func<int, Point[]> generatorFunc)
		{
			Name = name;
			Description = shortName;
			GeneratorFunc = generatorFunc;
		}

		// ************************************************************************
		public override string ToString()
		{
			return Name;
		}

		// ************************************************************************
		private bool _isSelected;

		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				if (_isSelected == value) return;

				_isSelected = value;

				RaisePropertyChanged();
			}
		}

		// ************************************************************************
		private void RaisePropertyChanged([CallerMemberName] string propName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		// ************************************************************************
		public string Description { get; private set; }

		// ************************************************************************
	}
}
