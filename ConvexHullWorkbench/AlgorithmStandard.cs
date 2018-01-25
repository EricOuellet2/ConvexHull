using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConvexHullWorkbench
{
	class AlgorithmStandard : Algorithm
	{
		public Func<Point[], AlgorithmStat, Point[]> Calc { get; private set; }

		public AlgorithmStandard(AlgorithmType algorithmType, string name, string author, string comment, OxyPlot.OxyColor color,
			Func<Point[], AlgorithmStat, Point[]> calc)
			: base(algorithmType, name, author, comment, color)
		{
			Calc = calc;
		}
		
	}
}
