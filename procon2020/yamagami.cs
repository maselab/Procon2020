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
  public class Yamagami : AbstPlayer {
    //ランダムに移動
    public Yamagami() : base() { }
    public Yamagami(string name) : base() { this.name = name; }
        //初期配置の設定　1 <= X <= 3, 0 <= Y <= 1
        public override Init_pos Init_pos()
        {
            Init_pos init = new Init_pos();
            init.G0 = new Point(3, 0); // 青
            init.G1 = new Point(2, 0); // 青
            init.G2 = new Point(3, 1); // 青
            init.G3 = new Point(1, 1); // 赤
            init.G4 = new Point(2, 1); // 赤
            init.G5 = new Point(1, 0); // 赤
            return init;
        }
        //行動の定義
        public override Dicision Dicision()
        {
            Dicision outdic = new Dicision();
            Random cRandom = new System.Random();
            int[] operate_array = new int[6] { 3, 4, 2, 5, 0, 1 };

            while (true)
            {
                int flg = 0;

                for (int i = 0; i < 6; i++)
                {
                    if (Function.MyGhostState(operate_array[i]).GetDoA())
                    {
                        if (Function.CanDR(Function.MyGhostState(operate_array[i]).GetPos(), Direction.Down))
                        {
                            outdic.SI = operate_array[i];
                            outdic.AL = Action_Label.Move;
                            outdic.DR = Direction.Down;
                            flg = 1;
                            break;
                        }

                        switch (operate_array[i])
                        {
                            case 2:
                                if (Function.CanDR(Function.MyGhostState(operate_array[i]).GetPos(), Direction.Right))
                                {
                                    outdic.SI = operate_array[i];
                                    outdic.AL = Action_Label.Move;
                                    outdic.DR = Direction.Right;
                                    flg = 1;
                                }
                                break;
                            case 0:
                                if (Function.CanDR(Function.MyGhostState(operate_array[i]).GetPos(), Direction.Right))
                                {
                                    outdic.SI = operate_array[i];
                                    outdic.AL = Action_Label.Move;
                                    outdic.DR = Direction.Right;
                                    flg = 1;
                                }
                                break;
                            default:
                                if (Function.CanDR(Function.MyGhostState(operate_array[i]).GetPos(), Direction.Left))
                                {
                                    outdic.SI = operate_array[i];
                                    outdic.AL = Action_Label.Move;
                                    outdic.DR = Direction.Left;
                                    flg = 1;
                                }
                                break;
                        }
                        if (flg == 1){break;}
                    }
                    else
                    {
                        continue;
                    }
                }

                if (flg == 1) { break; }

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

}
