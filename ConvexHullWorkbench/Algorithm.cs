using System;
using System.Windows;
using ConvexHullHelper;

namespace ConvexHullWorkbench
{
	public class Algorithm : NotifyPropertyChangeBase
	{
		private bool _isSelected;
		// ************************************************************************
		public AlgorithmType AlgorithmType { get; set; }
		public string Name { get; private set; }
		public string Author { get; private set; }
		public string Comment { get; private set; }
		public OxyPlot.OxyColor Color { get; private set; }

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (value == _isSelected) return;
				_isSelected = value;
				RaisePropertyChanged();
			}
		}

		// ************************************************************************
		/// <summary>
		/// 
		/// </summary>
		/// <param name="algorithmType"></param>
		/// <param name="name"></param>
		/// <param name="author"></param>
		/// <param name="comment"></param>
		/// <param name="getHull">The func receive the points and an optional AlgorithmStat which could be null.</param>
		public Algorithm(AlgorithmType algorithmType, string name, string author, string comment, OxyPlot.OxyColor color)
		{
			AlgorithmType = algorithmType;
			Name = name;
			Author = author;
			Comment = comment;
			Color = color;
		}
		// ************************************************************************
		public override string ToString()
		{
			return Name;
		}

		// ************************************************************************
	}
}
