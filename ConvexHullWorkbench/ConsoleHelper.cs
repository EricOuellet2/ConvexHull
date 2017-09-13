using System;
using System.Runtime.InteropServices;

namespace ConvexHullWorkbench
{
	public class ConsoleHelper
	{
		/// <summary>
		/// Allocates a new console for current process.
		/// </summary>
		[DllImport("kernel32.dll")]
		public static extern Boolean AllocConsole();

		/// <summary>
		/// Frees the console.
		/// </summary>
		[DllImport("kernel32.dll")]
		public static extern Boolean FreeConsole();
	}
}
