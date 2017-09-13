//// MultiSim.Math Core Library
//// MultiSim.Math.NET framework
//// http://www.MultiSim.Mathnet.com/framework/
////
//// Copyright © Andrew Kirillov, 2007-2010
//// andrew.kirillov@MultiSim.Mathnet.com
////
//// Copyright © Fabio L. Caversan, 2008
//// fabio.caversan@gmail.com
////

//using System.Numerics;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using System.Xml.Serialization;

//namespace MultiSim.Geometry
//{
//	using System;
	
//	/// <summary>
//	/// Structure for representing a pair of coordinates of double type.
//	/// </summary>
//	/// 
//	/// <remarks><para>The structure is used to store a pair of floating point
//	/// coordinates with double precision.</para>
//	/// 
//	/// <para>Sample usage:</para>
//	/// <code>
//	/// // assigning coordinates in the constructor
//	/// DoublePointOld p1 = new DoublePointOld( 10, 20 );
//	/// // creating a point and assigning coordinates later
//	/// DoublePointOld p2;
//	/// p2.X = 30;
//	/// p2.Y = 40;
//	/// // calculating distance between two points
//	/// double distance = p1.DistanceTo( p2 );
//	/// </code>
//	/// </remarks>
//	/// 
//	[Serializable, StructLayout(LayoutKind.Sequential)]
//	public struct DoublePointOld :IEquatable<DoublePointOld>
//	{
//		private double _x;
//		private double _y;
//		/// <summary> 
//		/// X coordinate.
//		/// </summary> 
//		/// 
//		/// 
//		/// 
//		[XmlAttribute]
//		public double X { get { return _x; } set { _x = value; } }

//		/// <summary> 
//		/// Y coordinate.
//		/// </summary> 
//		/// 
//		[XmlAttribute]
//		public double Y { get { return _y; } set { _y = value; } }

//		/// <summary>
//		/// Initializes a new instance of the <see cref="DoublePointOld"/> structure.
//		/// </summary>
//		/// 
//		/// <param name="x">X axis coordinate.</param>
//		/// <param name="y">Y axis coordinate.</param>
//		/// 
//		public DoublePointOld( double x, double y )
//		{
//		   this._x = x;
//		   this._y = y;
//		}

//		//Copy Constructor
//		public DoublePointOld(DoublePointOld other)
//		{
//			this._x = other.X;
//			this._y = other.Y;
//		}

//		public DoublePointOld(Complex pt)
//		{
//			this._x = pt.Real;
//			this._y = pt.Imaginary;
//		}

//		/// <summary>
//		/// Calculate Euclidean distance between two points.
//		/// </summary>
//		/// 
//		/// <param name="anotherPoint">Point to calculate distance to.</param>
//		/// 
//		/// <returns>Returns Euclidean distance between this point and
//		/// <paramref name="anotherPoint"/> points.</returns>
//		/// 
//		public double Distance( DoublePointOld anotherPoint )
//		{
//			double dx = X - anotherPoint.X;
//			double dy = Y - anotherPoint.Y;

//			return System.Math.Sqrt( dx * dx + dy * dy );
//		}

//		// Calcualte the square of the distance between two points
//		public double Distance2(DoublePointOld point)
//		{
//			double dx = X - point.X;
//			double dy = Y - point.Y;
//			return (dx * dx + dy * dy);
//		}

//		// Translate a point to the specified location
//		public void Translate(DoublePointOld point)
//		{
//			Translate(point.X, point.Y);
//		}
//		// Translate a point to the specified location (newX, newY)
//		public void Translate(double newX, double newY)
//		{
//			X = newX;
//			Y = newY;
//		}
//		// Offset a point along the x and y axes by dx and dy, respectively
//		public void Offset(double dx, double dy)
//		{
//			X += dx;
//			Y += dy;
//		}

	
//		// Calculate the middle point between two points
//		public DoublePointOld MidPoint(DoublePointOld point)
//		{
//			return new DoublePointOld((X + point.X) / 2, (Y + point.Y) / 2);
//		}
		
//		/// <summary>
//		/// Addition operator - adds values of two points.
//		/// </summary>
//		/// 
//		/// <param name="p1">First point for addition.</param>
//		/// <param name="p2">Second point for addition.</param>
//		/// 
//		/// <returns>Returns new point which coordinates equal to sum of corresponding
//		/// coordinates of specified points.</returns>
//		/// 
//		public static DoublePointOld operator +( DoublePointOld p1, DoublePointOld p2 )
//		{
//			return new DoublePointOld( p1.X + p2.X, p1.Y + p2.Y );
//		}

//		/// <summary>
//		/// Subtraction operator - subtracts values of two points.
//		/// </summary>
//		/// 
//		/// <param name="p1">Point to subtract from.</param>
//		/// <param name="p2">Point to subtract.</param>
//		/// 
//		/// <returns>Returns new point which coordinates equal to difference of corresponding
//		/// coordinates of specified points.</returns>
//		///
//		public static DoublePointOld operator -( DoublePointOld p1, DoublePointOld p2 )
//		{
//			return new DoublePointOld( p1.X - p2.X, p1.Y - p2.Y );
//		}

//		/// <summary>
//		/// Addition operator - adds scalar to the specified point.
//		/// </summary>
//		/// 
//		/// <param name="p">Point to increase coordinates of.</param>
//		/// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
//		/// 
//		/// <returns>Returns new point which coordinates equal to coordinates of
//		/// the specified point increased by specified value.</returns>
//		/// 
//		public static DoublePointOld operator +( DoublePointOld p, double valueToAdd )
//		{
//			return new DoublePointOld( p.X + valueToAdd, p.Y + valueToAdd );
//		}

//		/// <summary>
//		/// Subtraction operator - subtracts scalar from the specified point.
//		/// </summary>
//		/// 
//		/// <param name="p">Point to decrease coordinates of.</param>
//		/// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
//		/// 
//		/// <returns>Returns new point which coordinates equal to coordinates of
//		/// the specified point decreased by specified value.</returns>
//		/// 
//		public static DoublePointOld operator -( DoublePointOld p, double valueToSubtract )
//		{
//			return new DoublePointOld( p.X - valueToSubtract, p.Y - valueToSubtract );
//		}

//		/// <summary>
//		/// Multiplication operator - multiplies coordinates of the specified point by scalar value.
//		/// </summary>
//		/// 
//		/// <param name="p">Point to multiply coordinates of.</param>
//		/// <param name="factor">Multiplication factor.</param>
//		/// 
//		/// <returns>Returns new point which coordinates equal to coordinates of
//		/// the specified point multiplied by specified value.</returns>
//		///
//		public static DoublePointOld operator *( DoublePointOld p, double factor )
//		{
//			return new DoublePointOld( p.X * factor, p.Y * factor );
//		}

//		/// <summary>
//		/// Division operator - divides coordinates of the specified point by scalar value.
//		/// </summary>
//		/// 
//		/// <param name="p">Point to divide coordinates of.</param>
//		/// <param name="factor">Division factor.</param>
//		/// 
//		/// <returns>Returns new point which coordinates equal to coordinates of
//		/// the specified point divided by specified value.</returns>
//		/// 
//		public static DoublePointOld operator /( DoublePointOld p, double factor )
//		{
//			return new DoublePointOld( p.X / factor, p.Y / factor );
//		}

//		/// <summary>
//		/// Explicit conversion to <see cref="IntPoint"/>.
//		/// </summary>
//		/// 
//		/// <param name="p">Double precision point to convert to integer point.</param>
//		/// 
//		/// <returns>Returns new integer point which coordinates are explicitly converted
//		/// to integers from coordinates of the specified double precision point by
//		/// casting double values to integers value.</returns>
//		/// 
//		//public static explicit operator IntPoint( DoublePointOld p )
//		//{
//		//	return new IntPoint( (int) p.X, (int) p.Y );
//		//}

//		public static explicit operator Complex(DoublePointOld p)
//		{
//			return new Complex((double)p.X, (double)p.Y);
//		}
//		/// <summary>
//		/// Rounds the double precision point.
//		/// </summary>
//		/// 
//		/// <returns>Returns new integer point, which coordinates equal to whole numbers
//		/// nearest to the corresponding coordinates of the double precision point.</returns>
//		/// 
//		//public IntPoint Round( )
//		//{
//		//	return new IntPoint( (int) System.Math.Round( X ), (int) System.Math.Round( Y ) );
//		//}

//		/// <summary>
//		/// Get string representation of the class.
//		/// </summary>
//		/// 
//		/// <returns>Returns string, which contains values of the point in readable form.</returns>
//		///
//		public override string ToString( )
//		{
//			return string.Format( "{0} ; {1}", X, Y );
//		}

//		/// <summary>
//		/// Calculate Euclidean norm of the vector comprised of the point's 
//		/// coordinates - distance from (0, 0) in other words.
//		/// </summary>
//		/// 
//		/// <returns>Returns point's distance from (0, 0) point.</returns>
//		/// 
//		public double EuclideanNorm( )
//		{
//			return System.Math.Sqrt( X * X + Y * Y );
//		}

//		//Static methods and services on DoublePointOlds
//		//[MethodImpl (MethodImplOptions.AggressiveInlining)]
//		public static double Area(ref DoublePointOld a,ref DoublePointOld b,ref DoublePointOld c)
//		{
//			return (((b).X - (a).X)*((c).Y - (a).Y) - ((b).Y - (a).Y)*((c).X - (a).X));
//		}

//		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static bool RightTurn(ref DoublePointOld a ,ref DoublePointOld b ,ref DoublePointOld c )
//		{
//			return (Area(ref a,ref b, ref c) < 0);
//		}

//		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static bool LeftTurn(ref DoublePointOld a ,ref DoublePointOld b ,ref DoublePointOld c )
//		{
//			return (Area(ref a, ref b, ref c) > 0);
//		}

//		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static bool Collinear(ref DoublePointOld a ,ref DoublePointOld b ,ref DoublePointOld c )
//		{
//			return (Math.Abs(Area(ref a,ref b,ref c) - 0) < double.Epsilon);
//		}

//		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static int Sign(double x )
//		{
//			return (((x) < 0) ? -1 : (((x) > 0) ? 1 : 0));
//		}

//		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static int Comp(ref DoublePointOld a,ref DoublePointOld b)
//		{

//			//return ((Math.Abs((a).X - (b).X) < double.Epsilon) ? Sign((a).Y - (b).Y) : Sign((a).X - (b).X));
//			return (((a).X == (b).X) ? Sign((a).Y - (b).Y) : Sign((a).X - (b).X));
//		}

//		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static void Swap(ref DoublePointOld a, ref DoublePointOld b, ref DoublePointOld c)
//		{
//			c = a; 
//			a = b; 
//			b = c;
//		}



//		#region IEquatable<DoublePointOld> Members

//		public bool Equals(DoublePointOld other)
//		{
//			return (X == other.X) && (Y == other.Y);
//		}

//		#endregion
//	}
//}
