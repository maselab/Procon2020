using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Procon2020
{
    public class Iwamoto : AbstPlayer
    {
        
        public Iwamoto() : base() { }
        public Iwamoto(string name) : base() { this.name = name; }
        //初期配置の設定　1 <= X <= 3, 0 <= Y <= 1
        public override Init_pos Init_pos()
        {
            Init_pos init = new Init_pos();
            init.G0 = new Point(1, 1);
            init.G1 = new Point(1, 0);
            init.G2 = new Point(2, 0);
            init.G3 = new Point(2, 1);
            init.G4 = new Point(3, 0);
            init.G5 = new Point(3, 1);
            return init;
        }
        //行動の定義
        public override Dicision Dicision()
        {
            Dicision outdic = new Dicision();
            Random cRandom = new System.Random();
            int  x = 2;
            while (true)
            {
                if (Function.MyGhostState(0).GetDoA() || Function.MyGhostState(1).GetDoA() || Function.MyGhostState(4).GetDoA()
                    || Function.MyGhostState(5).GetDoA() || Function.MyGhostState(3).GetDoA())
                {
                    if (Function.MyGhostState(0).GetDoA() && Function.CanDR(Function.MyGhostState(0).GetPos(), Direction.Left))
                    {
                        outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                    }

                    else if (Function.MyGhostState(1).GetDoA() && Function.CanDR(Function.MyGhostState(1).GetPos(), Direction.Left))
                    {
                        outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Left;
                    }
                    else if (Function.MyGhostState(0).GetDoA() && Function.CanDR(Function.MyGhostState(0).GetPos(), Direction.Down) && x % 2 == 0)
                    {
                        outdic.SI = 0; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                        x++;
                        if (!(Function.MyGhostState(1).GetDoA())) x--;
                    }

                    else if (Function.MyGhostState(1).GetDoA() && Function.CanDR(Function.MyGhostState(1).GetPos(), Direction.Down) && x % 2 == 1)
                    {
                        outdic.SI = 1; outdic.AL = Action_Label.Move; outdic.DR = Direction.Down;
                        x++;
                        if (!(Function.MyGhostState(0).GetDoA())) x--;
                    }

                    else if (Function.MyGhostState(4).GetDoA() && Function.CanDR(Function.MyGhostState(4).GetPos(), Direction.Right))
                    { outdic.SI = 4; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right; }
                    else if (Function.MyGhostState(5).GetDoA() && Function.CanDR(Function.MyGhostState(5).GetPos(), Direction.Right))
                    { outdic.SI = 5; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right; }
                    else if (Function.MyGhostState(3).GetDoA() && Function.CanDR(Function.MyGhostState(3).GetPos(), Direction.Right))
                    { outdic.SI = 3; outdic.AL = Action_Label.Move; outdic.DR = Direction.Right; }



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
      