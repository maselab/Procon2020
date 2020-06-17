using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;

namespace Procon2020
{
    public class Kato : AbstPlayer
    {
        //ランダムに移動
        public Kato() : base() { }
        public Kato(string name) : base() { this.name = name; }
        //初期配置の設定　1 <= X <= 3, 0 <= Y <= 1
        public override Init_pos Init_pos()
        {
            Init_pos init = new Init_pos();
            init.G0 = new Point(1, 1);
            init.G1 = new Point(2, 0);
            init.G2 = new Point(1, 0);
            init.G3 = new Point(2, 1);
            init.G4 = new Point(3, 1);
            init.G5 = new Point(3, 0);
            return init;
        }
        public override Dicision Dicision()
        {
            Dicision outdic = new Dicision();
            Random cRandom = new System.Random();
            int s1 ;
            while (true)
            {
                var func = new Function();
                int a = Function.NowTurn();
                
                if (a < 6)
                {
                    if (a == 1 || a == 2)
                    {
                        outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                    }
                    if (a == 3 || a == 4)
                    {
                        outdic.SI = 4; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                    }
                    if (a == 5 || a == 6)
                    {
                        outdic.SI = 3; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                    }
                    return outdic;
                }


                if (!Function.MyGhostState(4).GetDoA() && Function.MyGhostState(5).GetDoA())
                {
                    if (Function.CanDR(Function.MyGhostState(5).GetPos(), Direction.Right))
                    {
                        outdic.SI = 5; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                        return outdic;
                    }
                }
                if (!Function.MyGhostState(5).GetDoA() && Function.MyGhostState(4).GetDoA())
                {
                    if (Function.CanDR(Function.MyGhostState(4).GetPos(), Direction.Up))
                    {
                        outdic.SI = 4; outdic.AL = Action_Label.Move; outdic.DR = Direction.Up;
                        return outdic;
                    }
                }

                if (a > 170)
                {
                    int total = 0;
                    var blue_red = Function.GetDeleteEnemyNum();
                    total += blue_red[0] - blue_red[1];
                    for (int i = 0; i < 6; i++)
                    {
                        if (i < 3)
                        {
                            if (Function.MyGhostState(i).GetDoA())
                            {
                                total += 1;
                            }
                        }
                        else
                        {
                            if (Function.MyGhostState(i).GetDoA())
                            {
                                total -= 1;
                            }
                        }
                    }
                    if (total <= 0)
                    {
                        if (Function.MyGhostState(2).GetDoA())
                        {
                            if (Function.CanDR(Function.MyGhostState(2).GetPos(), Direction.Left))
                            {
                                outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                                return outdic;
                            }
                            else if (Function.CanDR(Function.MyGhostState(2).GetPos(), Direction.Down))
                            {
                                outdic.SI = 2; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                                return outdic;
                            }
                        }
                        else if (Function.MyGhostState(1).GetDoA())
                        {
                            if (Function.CanDR(Function.MyGhostState(1).GetPos(), Direction.Down))
                            {
                                outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                                return outdic;
                            }
                            else if (Function.CanDR(Function.MyGhostState(1).GetPos(), Direction.Right))
                            {
                                outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                                return outdic;
                            }
                        }
                    }
                }


                if (Function.MyGhostState(0).GetDoA() ||Function.MyGhostState(3).GetDoA() || Function.MyGhostState(4).GetDoA() || Function.MyGhostState(5).GetDoA())
                {
                    if (Function.MyGhostState(0).GetDoA() || Function.MyGhostState(3).GetDoA())
                    {
                        s1 = 3;
                        if (Function.MyGhostState(s1).GetPos() == new Point(1, 5) || Function.MyGhostState(s1).GetPos() == new Point(1, 4) || Function.MyGhostState(s1).GetPos() == new Point(0, 4) || !Function.MyGhostState(3).GetDoA())
                        {
                            if (Function.MyGhostState(s1).GetPos() == new Point(0,4))
                            {
                                if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Right))
                                {
                                    outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                                    return outdic;
                                }
                            }
                            else if (Function.MyGhostState(s1).GetPos() == new Point(1, 4))
                            {
                                outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                                return outdic;
                            }
                            s1 = 0;
                        }
                        else
                        {
                            
                            if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Left))
                            {
                                outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                            }
                            else if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Down))
                            {
                                outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                            }
                            return outdic;
                        }
                    }
                    else { s1 = 5; }
                    if (Function.MyGhostState(s1).GetDoA())
                    {
                        if (s1 == 5)
                        {
                            if (Function.MyGhostState(s1).GetPos() != new Point(3, 5))
                            {
                                if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Down))
                                {
                                    outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                                }
                                else if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Right))
                                {
                                    outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right;
                                }
                                return outdic;
                            }
                        }
                        else
                        {
                            if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Left))
                            {
                                outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                            }
                            else if (Function.CanDR(Function.MyGhostState(s1).GetPos(), Direction.Down))
                            {
                                outdic.SI = s1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                            }
                            return outdic;
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
}
