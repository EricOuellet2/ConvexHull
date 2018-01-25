using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OxyPlot;

namespace ConvexHullWorkbench
{
	public class DrawInfo
	{
		// ************************************************************************
		public IReadOnlyCollection<Point> Points { get; private set; }

		public DrawStyle DrawStyle { get; private set; }
		public OxyColor Color { get; private set; }

		public string Name { get; private set;}
		
		// ************************************************************************
		public DrawInfo(IReadOnlyCollection<Point> points, DrawStyle drawStyle, OxyColor color, string name = null)
		{
			Points = points;
			DrawStyle = drawStyle;
			Color = color;
			Name = name;
		}

		// ************************************************************************

	}
}
