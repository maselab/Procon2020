using System;
using System.Collections;

namespace geister.taiyo
{
		public enum Way
		{
			Up = 0,
			Right,
			Down,
			Left,
			Center
		}

		public static class WayUtil
		{
			static public readonly Way[] way4 = { Way.Up, Way.Right, Way.Down, Way.Left };
			static public readonly Way[] way5 = { Way.Up, Way.Right, Way.Down, Way.Left, Way.Center };
            static public readonly Way[] revWay4 = { Way.Down, Way.Left, Way.Up, Way.Right };
		}
}