using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unsafe
{
	public class Test
	{
		public static void TestFixedOnObject()
		{
			object[] objs = new object[] { new object(), new object() };

			unsafe
			{
				// Can't do that:
				// fixed (object* p = objs)
				{

				}
			}
		}

		public void TestSizeOf<T>(T toto)
		{
			// Can't do that:
			//int x = sizeof(T);
			//int y = sizeof(toto);
		}
	}
}
