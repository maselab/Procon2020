using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geister.taiyo
{
    public abstract class IPlayer
    {
        public abstract GhostType[] firstPosition();
        public abstract Move move(IField field, int teamId, int remainTurn);
    }

    public class Move{
        public TPoint pos;
        public Way way;

        public Move(TPoint pos, Way way)
        {
            this.pos = pos;
            this.way = way;
        }

        public TPoint toPos()
        {
            return TPoint.add(pos, TPoint.fromWay(way));
        }

        public override string ToString()
        {
            return pos.ToString() +" , "+ way.ToString();
        }
    }

    public class RandomPlayer : IPlayer
    {
        //        static private Random mRand = new Random(DateTime.Now.Millisecond);
        static private Random mRand = new Random(0);

        public RandomPlayer()
        {

        }

        public override GhostType[] firstPosition()
        {
            return new GhostType[] {
                GhostType.Good, GhostType.Evil , GhostType.Evil ,
                GhostType.Good, GhostType.Evil , GhostType.Good
            };
        }

        public override Move move(IField field, int teamId, int remainTurn)
        {
            List<Move> availableMoveList = new List<Move>();

			//可能なMoveをピックアップ
            while(availableMoveList.Count==0)
            {
                //team1なら下、team0なら上方向を優先するため、その反対を一定確率で除外する。
                Way removeWay =　mRand.NextDouble() < 0.3 ? Way.Center :
                    teamId == 0 ? Way.Down : Way.Up;

                for (int y = 0; y < IField.cFieldHeight; y++)
                {
                    for (int x = 0; x < IField.cFieldWidth; x++)
                    {
                        foreach (Way w in WayUtil.way4)
                        {
                            if (w == removeWay)
                            {
                                continue;
                            }

                            if (field.canMove(teamId, new TPoint(x, y), TPoint.add(new taiyo.TPoint(x, y), TPoint.fromWay(w))))
                            {
                                availableMoveList.Add(new Move(new TPoint(x, y), w));
                            }
                        }
                    }
                }
            }
			

            return availableMoveList[mRand.Next() % availableMoveList.Count];
        }



    }
}
