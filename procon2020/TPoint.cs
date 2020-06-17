using System;

namespace geister
{
	namespace taiyo
	{
		public class TPoint
		{
			public int x;
			public int y;

			public TPoint (int x, int y)
			{
				this.x = x;
				this.y = y;
			}

			public TPoint (TPoint other)
				: this (other.x, other.y)
			{
			}

			public static TPoint fromWay (Way w)
			{
				switch (w) {
				case Way.Up:
					return Up;
				case Way.Right:
					return Right;
				case Way.Down:
					return Down;
				case Way.Left:
					return Left;
				case Way.Center:
					return Center;
				}
				return null;
			}

			public bool equals (TPoint p)
			{
				return p.x == x && p.y == y;
			}

			public override int GetHashCode ()
			{
				return y * 8 + x;	//今回のx,yが取り得る範囲を考えると負の値を考えてもこれで十分
			}

			public override bool Equals (object obj)
			{
				return equals ((TPoint)obj);
			}

            public override string ToString()
            {
                return "{" + x + "," + y + "}";
            }

			static readonly TPoint Up = new TPoint (0, -1);
			static readonly TPoint Right = new TPoint (1, 0);
			static readonly TPoint Down = new TPoint (0, 1);
			static readonly TPoint Left = new TPoint (-1, 0);
			static readonly TPoint Center = new TPoint (0, 0);

			static public TPoint add (TPoint p1, TPoint p2)
			{
				return new TPoint (p1.x + p2.x, p1.y + p2.y);
			}

			static public TPoint add (TPoint p1, Way w)
			{
				return add (p1, fromWay (w));
			}

			static public int manhattan (TPoint p, int x, int y)
			{
				return manhattan (p.x, p.y, x, y);
			}

			static public int manhattan (TPoint p1, TPoint p2)
			{
				return manhattan (p1.x, p1.y, p2.x, p2.y);
			}

			static public int manhattan (int x1, int y1, int x2, int y2)
			{
				return Math.Abs (x1 - x2) + Math.Abs (y1 - y2);
			}
		}
	}

}