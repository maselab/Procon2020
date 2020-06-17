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
  public class Iwase : AbstPlayer {
    //ランダムに移動
    public Iwase() : base() { }
    public Iwase(string name) : base() { this.name = name; }
        static int first_pos = 0;
        //初期配置の設定　1 <= X <= 3, 0 <= Y <= 1
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
        //行動の定義
        public override Dicision Dicision() {
      Dicision outdic = new Dicision();
            //配置変更
            if (first_pos == 0)
            {
                
                if (Function.MyGhostState(3).GetDoA() && Function.MyGhostState(3).GetPos().X != 0)
                {
                    outdic.SI = 3; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                }
                else if (Function.MyGhostState(5).GetDoA() && Function.MyGhostState(5).GetPos().X != 4)
                {
                    outdic.SI = 5; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                }
                else if (Function.MyGhostState(4).GetDoA() && Function.MyGhostState(4).GetPos().X != 3)
                {
                    outdic.SI = 4; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                }
                else if (Function.MyGhostState(4).GetDoA() && Function.MyGhostState(4).GetPos().Y != 2)
                {
                    outdic.SI = 4; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                }
                else if (Function.MyGhostState(1).GetDoA() && Function.MyGhostState(1).GetPos().Y != 1)
                {
                    outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                }
                else if (Function.MyGhostState(1).GetDoA() && Function.MyGhostState(1).GetPos().X != 3)
                {
                    outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                }
                else
                {
                    first_pos = 1;
                }
                var epos_list = Function.EnemyGhostState();
                for (int i = 0; i < epos_list.Count(); i++)
                {
                    if ((epos_list[i].X == 0) && (epos_list[i].Y == 0))
                    {

                        if (Function.MyGhostState(0).GetDoA() && Function.CanDR(Function.MyGhostState(0).GetPos(), Direction.Left))
                        {
                            outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                        }
                    }
                    if ((epos_list[i].X == 1) && (epos_list[i].Y == 1))
                    {

                        if (Function.MyGhostState(0).GetDoA() && Function.CanDR(Function.MyGhostState(0).GetPos(), Direction.Down))
                        {
                            outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                        }
                    }
                    if ((epos_list[i].X == 0) && (epos_list[i].Y == 2))
                    {

                        if (Function.MyGhostState(0).GetDoA() && Function.CanDR(Function.MyGhostState(0).GetPos(), Direction.Right))
                        {
                            outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                        }
                    }
                    if ((epos_list[i].X == 4) && (epos_list[i].Y == 0))
                    {
                        if (Function.MyGhostState(2).GetDoA() && Function.CanDR(Function.MyGhostState(2).GetPos(), Direction.Right))
                        {
                            outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                        }

                    }
                    if ((epos_list[i].X == 3) && (epos_list[i].Y == 1))
                    {
                        if (Function.MyGhostState(2).GetDoA() && Function.CanDR(Function.MyGhostState(2).GetPos(), Direction.Down))
                        {
                            outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                        }

                    }
                    if ((epos_list[i].X == 0) && (epos_list[i].Y == 2))
                    {
                        if (Function.MyGhostState(2).GetDoA() && Function.CanDR(Function.MyGhostState(2).GetPos(), Direction.Left))
                        {
                            outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                        }

                    }
                }
            }
            else
            {

                var epos_list = Function.EnemyGhostState();

                if (!(Function.MyGhostState(4).GetDoA()) && Function.MyGhostState(1).GetDoA())
                {
                    Point mypos2 = Function.MyGhostState(1).GetPos();
                    if (mypos2.Y == 5)
                    {
                        if (mypos2.X < 4)
                        {
                            if (Function.CanDR(mypos2, Direction.Right))
                                outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                        }
                    }
                    else
                    {
                        if (mypos2.X > 2)
                        {
                            if (Function.CanDR(mypos2, Direction.Left))
                                outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                        }
                        else if (mypos2.X < 2)
                        {
                            if (Function.CanDR(mypos2, Direction.Right))
                                outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                        }
                        else if (mypos2.Y < 5)
                        {
                            if (Function.CanDR(mypos2, Direction.Down))
                                outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                        }
                    }
                }

                //前進(3,4,5)
                for (int j = 3; j < 6; j++)
                {
                    if (Function.MyGhostState(j).GetDoA() && Function.CanDR(Function.MyGhostState(j).GetPos(), Direction.Down))
                    {
                        Point mypos = Function.MyGhostState(j).GetPos();

                        int fflag = 0;
                        for (int i = 0; i < epos_list.Count(); i++)
                        {
                            if ((epos_list[i].X == mypos.X) && (epos_list[i].Y == mypos.Y + 1))
                            {
                                fflag = 1;
                            }
                        }
                        if (fflag == 0) {

                            outdic.SI = j; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down; }
                    }
                }

                if((Function.MyGhostState(5).GetDoA() && Function.MyGhostState(5).GetPos()==new Point(4,5))&&(Function.MyGhostState(1).GetDoA() && Function.MyGhostState(1).GetPos() == new Point(3, 5)))
                {
                    if (Function.CanDR(Function.MyGhostState(5).GetPos(), Direction.Up))
                    {
                        outdic.SI = 5; outdic.AL = Action_Label.Move; outdic.DR = Direction.Up;
                    }
                }
                if ((Function.MyGhostState(5).GetDoA() && Function.MyGhostState(5).GetPos() == new Point(4, 4)) && (Function.MyGhostState(1).GetDoA() && Function.MyGhostState(1).GetPos() == new Point(3, 5)))
                {
                    if (Function.CanDR(Function.MyGhostState(1).GetPos(), Direction.Right))
                    {
                        outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                    }
                }

                //追従(1,4)
                if (Function.MyGhostState(4).GetDoA() && Function.MyGhostState(1).GetDoA())
                {
                    if (Function.MyGhostState(4).GetPos().Y != Function.MyGhostState(1).GetPos().Y + 1)
                    {
                        if (Function.CanDR(Function.MyGhostState(1).GetPos(), Direction.Down))
                        {
                            outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                        }
                    }
                }

                //protect(0,2)
                if (Function.MyGhostState(2).GetDoA())
                {
                    Point pos = Function.MyGhostState(2).GetPos();
                    if (pos.X < 3)
                    {
                        if (Function.CanDR(pos, Direction.Right))
                            outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                    }
                    else if (pos.X > 3)
                    {
                        if (Function.CanDR(pos, Direction.Left))
                            outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                    }
                    else if (pos.Y > 0)
                    {
                        if (Function.CanDR(pos, Direction.Up))
                            outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Up;
                    }
                    else
                    {
                        for (int i = 0; i < epos_list.Count(); i++)
                        {
                            if ((epos_list[i].X == 4) && (epos_list[i].Y == 0))
                            {
                                if (Function.CanDR(pos, Direction.Right))
                                    outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                            }
                            else if ((epos_list[i].X == 3) && (epos_list[i].Y == 1))
                            {
                                if (Function.CanDR(pos, Direction.Down))
                                    outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                            }
                            else if ((epos_list[i].X == 2) && (epos_list[i].Y == 0))
                            {
                                if (Function.CanDR(pos, Direction.Left))
                                    outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                            }

                        }
                    }
                }


                if (Function.MyGhostState(0).GetDoA())
                {
                    Point pos = Function.MyGhostState(0).GetPos();
                    if (pos.X < 1)
                    {
                        if (Function.CanDR(pos, Direction.Right))
                            outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                    }
                    else if (pos.X > 1)
                    {
                        if (Function.CanDR(pos, Direction.Left))
                            outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                    }
                    else if (pos.Y > 0)
                    {
                        if (Function.CanDR(pos, Direction.Up))
                            outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Up;
                    }
                    else
                    {
                        for (int i = 0; i < epos_list.Count(); i++)
                        {
                            if ((epos_list[i].X == 0) && (epos_list[i].Y == 0))
                            {
                                if (Function.CanDR(pos, Direction.Left))
                                    outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                            }
                            else if ((epos_list[i].X == 1) && (epos_list[i].Y == 1))
                            {
                                if (Function.CanDR(pos, Direction.Down))
                                    outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                            }
                            else if ((epos_list[i].X == 2) && (epos_list[i].Y == 0))
                            {
                                if (Function.CanDR(pos, Direction.Right))
                                    outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                            }

                        }
                    }
                }
            }
            return outdic;
    }

  }

}
