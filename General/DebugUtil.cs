using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace General
{
    public class DebugUtil
    {
	    public static void Print(IEnumerable enumerable)
	    {
			StringBuilder sb = new StringBuilder();

		    bool firstLine = true;
		    foreach (var t in enumerable)
		    {
			    if (firstLine)
			    {
				    firstLine = false;
			    }
			    else
			    {
				    sb.Append(", ");
			    }

			    sb.Append("[");
				sb.Append(t);
				sb.Append("]");
			}

			Debug.Print(sb.ToString());
	    }
    }
}
