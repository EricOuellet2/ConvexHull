using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ConvexHullHelper;
using DocumentFormat.OpenXml.Wordprocessing;
using JetBrains.Annotations;

namespace ConvexHullWorkbench
{
	public class AlgorithmOnline : Algorithm
	{
		// ******************************************************************
		Func<object> FuncInitNew { get; set; }
		Func<object, Point, AlgorithmStat, bool> FuncAddPoint { get; set; }
		Func<object, IReadOnlyList<Point>> FuncGetResult { get; set; }

		// ******************************************************************
		public AlgorithmOnline(AlgorithmType algorithmType, string name, string author, string comment, OxyPlot.OxyColor color,
			Func<object> funcInitNew,
			Func<object, Point, AlgorithmStat, bool> funcAddPoint, Func<Object, IReadOnlyList<Point>> funcGetResult)
			: base(algorithmType, name, author, comment, color)
		{
			FuncInitNew = funcInitNew;
			FuncAddPoint = funcAddPoint;
			FuncGetResult = funcGetResult;
		}

		public object Algo { get; private set; }= null;

		public AlgorithmStat Stat { get; private set; }

		// ******************************************************************
		public void Init()
		{
			Algo = FuncInitNew();
			Stat = new AlgorithmStat();
		}

		// ******************************************************************
		public bool AddPoint(Point pt)
		{
			return FuncAddPoint(Algo, pt, Stat);
		}

		// ******************************************************************
		public IReadOnlyList<Point> GetResult()
		{
			return FuncGetResult(Algo);
		}

		//// ******************************************************************
		//public IReadOnlyList<Point> Calc(Point[] points, AlgorithmStat algorithmStat)
		//{
		//	Init();
		//	foreach (var pt in points)
		//	{
		//		FuncAddPoint(_instance, pt, Stat);
		//	}

		//	return FuncGetResult(_instance);
		//}

		// ******************************************************************

	}
}
