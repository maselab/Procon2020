using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Procon2020 {

  public class aTestPlayer : AbstPlayer {
    //近くに敵がいれば適当に攻撃，いなければ適当に移動
    public aTestPlayer() : base() { }
    public aTestPlayer(string name) : base() { this.name = name; }
        public override Init_pos Init_pos()
        {
            Init_pos init = new Init_pos();
            init.G0 = new Point(1, 1);
            init.G1 = new Point(2, 1);
            init.G2 = new Point(3, 1);
            init.G3 = new Point(1, 0);
            init.G4 = new Point(2, 0);
            init.G5 = new Point(3, 0);
            return init;
        }
    public override Dicision Dicision() {
      Dicision outdic = new Dicision();
      Random cRandom = new System.Random();
      int s1;
            while (true) {
                if (Function.MyGhostState(0).GetDoA() || Function.MyGhostState(2).GetDoA())
                {
                    
                    Console.WriteLine();
                    if (Function.MyGhostState(0).GetDoA()) { s1 = 0; }
                    else { s1 = 2; }
                    if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Down))
                    {
                        outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                    }
                    else
                    {
                        switch (s1)
                        {
                            case 0:
                                if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Left))
                                {
                                    outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                                }
                                break;
                            case 2:
                                if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Right))
                                {
                                    outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                                }
                                break;
                        }
                    }
                    
                    return outdic;
                }
                else
                {
                    int tmp1 = cRandom.Next(0, global.GhostNum);
                    if (Function.MyGhostState(tmp1).GetDoA())
                    {
                        outdic.SI = tmp1; outdic.AL = Action_Label.Move;
                        while (true)
                        {
                            int tmp = cRandom.Next(0, 4);
                            if (Function.CanDR(Function.MyGhostState(tmp1).GetPos(), (Direction)tmp)) { outdic.DR = (Direction)tmp; break; }
                        }
                        break;
                    }
                }
      }
      return outdic;
    }
  }


    public class bTestPlayer : AbstPlayer
    {
        //ランダムに移動
        public bTestPlayer() : base() { }
        public bTestPlayer(string name) : base() { this.name = name; }
        public override Init_pos Init_pos()
        {
            Init_pos init = new Init_pos();
            init.G0 = new Point(1, 0);
            init.G1 = new Point(2, 0);
            init.G2 = new Point(3, 0);
            init.G3 = new Point(1, 1);
            init.G4 = new Point(2, 1);
            init.G5 = new Point(3, 1);
            return init;
        }
        public override Dicision Dicision()
        {
            Dicision outdic = new Dicision();
            Random cRandom = new System.Random();
            while (true)
            {
                int tmp1 = cRandom.Next(0, global.GhostNum);
                if (Function.MyGhostState(tmp1).GetDoA())
                {
                    outdic.SI = tmp1; outdic.AL = Action_Label.Move;
                    while (true)
                    {
                        int tmp = cRandom.Next(0, 4);
                        if (Function.CanDR(Function.MyGhostState(tmp1).GetPos(), (Direction)tmp)) { outdic.DR = (Direction)tmp; break; }
                    }
                    break;
                }
            }
            return outdic;
        }
    }

        public class cTestPlayer : AbstPlayer
        {
        //Function.EnemyGhostStateとFunction.GetDeleteEnemyNumを使った例
        //敵の赤オバケを2体とってしまったら隣接する場合には逃げる
        //あとはランダム移動
        public cTestPlayer() : base() { }
            public cTestPlayer(string name) : base() { this.name = name; }
            public override Init_pos Init_pos()
            {
                Init_pos init = new Init_pos();
                init.G0 = new Point(1, 0);
                init.G1 = new Point(2, 0);
                init.G2 = new Point(3, 0);
                init.G3 = new Point(1, 1);
                init.G4 = new Point(2, 1);
                init.G5 = new Point(3, 1);
                return init;
            }
            public override Dicision Dicision()
            {
                Dicision outdic = new Dicision();
                Random cRandom = new System.Random();
                var delete_list = Function.GetDeleteEnemyNum();

            while (true)
            {
                int tmp1 = cRandom.Next(0, global.GhostNum);
                if (Function.MyGhostState(tmp1).GetDoA())
                {
                    outdic.SI = tmp1; outdic.AL = Action_Label.Move;
                    while (true)
                    {
                        int tmp = cRandom.Next(0, 4);
                        if (Function.CanDR(Function.MyGhostState(tmp1).GetPos(), (Direction)tmp)) { outdic.DR = (Direction)tmp; break; }
                    }
                    break;
                }
            }

            if (delete_list[1] >= 2)
            {
                var epos_list = Function.EnemyGhostState();
                for (int i = 0; i < global.GhostNum; i++)
                {
                    if (Function.MyGhostState(i).GetDoA())
                    {
                        Point mypos = Function.MyGhostState(i).GetPos();
                        for (int j = 0; j < epos_list.Count(); j++)
                        {
                            if ((epos_list[j].X == mypos.X + 1) && (epos_list[j].Y == mypos.Y))
                            {
                                if (Function.CanDR(mypos, Direction.Left))
                                {
                                    outdic.SI = i; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                                }
                            }
                            else if ((epos_list[j].X == mypos.X - 1) && (epos_list[j].Y == mypos.Y))
                            {
                                if (Function.CanDR(mypos, Direction.Right))
                                {
                                    outdic.SI = i; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                                }
                            }
                            else if ((epos_list[j].X == mypos.X) && (epos_list[j].Y == mypos.Y + 1))
                            {
                                if (Function.CanDR(mypos, Direction.Up))
                                {
                                    outdic.SI = i; outdic.AL = Action_Label.Move; outdic.DR = Direction.Up;
                                }
                            }
                            else if ((epos_list[j].X == mypos.X) && (epos_list[j].Y == mypos.Y - 1))
                            {
                                if (Function.CanDR(mypos, Direction.Down))
                                {
                                    outdic.SI = i; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                                }
                            }
                        }
                    }
                }
            }
            
                
                return outdic;
            }
        }

}
