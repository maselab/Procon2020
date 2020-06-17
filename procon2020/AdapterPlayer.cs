using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Procon2020;
using System.Drawing;

/**
 * 開発キットと自前プレイヤーのアダプタ
 * */

namespace geister.taiyo
{

    class AdapterField : IField
    {
        public AdapterField()
        {  
            mMyFraggedEvilNum = 0;
            mMyFraggedGoodNum = 0;
            mEnemyFraggedEvilNum = 0;
            mEnemyFraggedGoodNum = 0;

            for(int i=0;i<6; i++)
            {
                var myGhost = Function.MyGhostState(i);
                if(!myGhost.GetDoA())
                {
                    if(i<3)
                    {
                        mEnemyFraggedGoodNum++;
                    }
                    else
                    {
                        mEnemyFraggedEvilNum++;
                    }

                    continue;
                }
                mGhosts[myGhost.GetPos().Y, myGhost.GetPos().X] = new Ghost(1, i<3?GhostType.Good:GhostType.Evil);
            }
            foreach(var enemy in Function.EnemyGhostState())
            {
                mGhosts[enemy.Y, enemy.X] = new Ghost(0, GhostType.Unknown);
            }
            mMyFraggedGoodNum = Function.GetDeleteEnemyNum()[0];
            mMyFraggedEvilNum = Function.GetDeleteEnemyNum()[1];
        }

        public override int getFraggedEvilNum(int teamId)
        {
            if(teamId == 1)
            {
                return mMyFraggedEvilNum;
            }
            else
            {
                return mEnemyFraggedEvilNum;
            }
        }

        public override int getFraggedGoodNum(int teamId)
        {
            if (teamId == 1)
            {
                return mMyFraggedGoodNum;
            }
            else
            {
                return mEnemyFraggedGoodNum;
            }
        }

        public override IGhost getGhost(int x, int y)
        {
            return mGhosts[y, x];
        }

        private int mMyFraggedEvilNum = 0;
        private int mMyFraggedGoodNum = 0;
        private int mEnemyFraggedEvilNum = 0;
        private int mEnemyFraggedGoodNum = 0;
        private Ghost[,] mGhosts = new Ghost[IField.cFieldHeight, IField.cFieldWidth];
    }

    public class AdapterPlayer<Player> : AbstPlayer
        where Player : IPlayer, new()
    {
        public AdapterPlayer()
            :base("六鬼夜行(原)")
        {
            mPlayer = new Player();
        }

        public AdapterPlayer(string name)
        : base(name)
        {
            mPlayer = new Player();
        }


        public override Init_pos Init_pos()
        {
            //オフィシャルの座標系は左上が0,0、右下が5,5の固定になる
            //プレイヤーは上側スタートだと思って作る
            //0~2が青、3~5が赤
            Init_pos init = new Init_pos();
            //firstPositionでは
            //_,0,1,2,_
            //_,3,4,5,_
            //という位置関係でデータを入れている
            var types = mPlayer.firstPosition();
            Point[] points = new Point[6]
            {
                new Point(1,0), new Point(2,0), new Point(3,0),
                new Point(1,1), new Point(2,1), new Point(3,1)
            };
            List<Point> list = new List<Point>();
            //青青青赤赤赤の順に対応する座標が並ぶように並び替える
            for(int i = 0; i < 6; i++)
            {
                if(types[i] == GhostType.Good)
                {
                    list.Add(points[i]);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                if (types[i] == GhostType.Evil)
                {
                    list.Add(points[i]);
                }
            }
            init.G0 = list[0];
            init.G1 = list[1];
            init.G2 = list[2];
            init.G3 = list[3];
            init.G4 = list[4];
            init.G5 = list[5];
            return init;
        }

        static private Direction ConvertWayToDirection(Way way)
        {
            switch(way)
            {
                case Way.Right:
                    return Direction.Right;
                case Way.Left:
                    return Direction.Left;
                case Way.Down:
                    return Direction.Down;
                case Way.Up:
                    return Direction.Up;
            }
            return Direction.Dummy;
        }

        public override Dicision Dicision()
        {
            //常に上側のプレイヤーとして作るのでPlayerIdは1で固定
            var field = new AdapterField();
            var move = mPlayer.move(field, 1, Function.NowTurn());
            Dicision dicision = new Dicision();
            for(int i=0;i<6;i++)
            {
                var pos = Function.MyGhostState(i).GetPos();
                if(pos.X == move.pos.x && pos.Y == move.pos.y)
                {
                    dicision.SI = i;
                    break;
                }
            }
            dicision.AL = Action_Label.Move;
            dicision.DR = ConvertWayToDirection(move.way);
            return dicision;
        }

        private Player mPlayer;
    }
}
