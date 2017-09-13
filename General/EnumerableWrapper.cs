using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General
{
	public class EnumerableWrapper<T> : IEnumerable<T> 
	{
		private Func<IEnumerator<T>>  _getEnumeratorFunc;

		public EnumerableWrapper(Func<IEnumerator<T>> getEnumeratorFunc)
		{
			_getEnumeratorFunc = getEnumeratorFunc;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _getEnumeratorFunc();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
