using System;
using System.Linq;
using General.AvlTreeSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneralUnitTest
{
	[TestClass]
	public class UnitTestAvlTreeSet
	{
		[TestMethod]
		public void TestInsert()
		{
			AvlTreeSet<int> tree = SetSimpleSet();
			Assert.AreEqual(tree, new int[] { 1, 2, 3, 4 });
		}

		private AvlTreeSet<int> SetSimpleSet()
		{
			AvlTreeSet<int> tree = new AvlTreeSet<int>();

			tree.Add(1);
			tree.Add(2);
			tree.Add(3);
			tree.Add(4);

			return tree;
		}
		
		[TestMethod]
		public void TestDelete()
		{
			// ***********************************************************************
			AvlTreeSet<int> tree = null;

			// ***********************************************************************
			tree = SetSimpleSet();
			tree.Remove(1);
			CollectionAssert.AreEqual(tree, new int[] { 2, 3, 4 });
			tree.DumpVisual();
			tree.DebugEnsureTreeIsValid();

			tree = SetSimpleSet();
			tree.Remove(2);
			CollectionAssert.AreEqual(tree, new int[] { 1, 3, 4 });
			tree.DebugEnsureTreeIsValid();

			tree = SetSimpleSet();
			tree.Remove(3);
			CollectionAssert.AreEqual(tree, new int[] { 1, 2, 4 });
			tree.DebugEnsureTreeIsValid();

			tree = SetSimpleSet();
			tree.Remove(4);
			CollectionAssert.AreEqual(tree, new int[] { 1, 2, 3 });
			tree.DebugEnsureTreeIsValid();

			// ***********************************************************************

			tree = SetSimpleSet();
			tree.Remove(1);
			tree.Remove(2);
			Assert.AreEqual(tree, new int[] { 3, 4 });

			tree = SetSimpleSet();
			tree.Remove(2);
			tree.Remove(1);
			Assert.AreEqual(tree, new int[] { 3, 4 });

			tree = SetSimpleSet();
			tree.Remove(2);
			tree.Remove(3);
			Assert.AreEqual(tree, new int[] { 1, 4 });

			tree = SetSimpleSet();
			tree.Remove(3);
			tree.Remove(2);
			Assert.AreEqual(tree, new int[] { 1, 4 });





		}



	}
}
