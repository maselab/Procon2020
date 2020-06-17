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
  public class Timai : AbstPlayer {
    List<GhostState> lg;
    int chasel = 0, chaser = 2;
    int nt;
    public Timai() : base() { }
    public Timai(string name) : base() { this.name = name; }
    //初期配置の設定　1 <= X <= 3, 0 <= Y <= 1
    public override Init_pos Init_pos() {
      Init_pos init = new Init_pos();
      init.G0 = new Point(1, 0);
      init.G1 = new Point(2, 1);
      init.G2 = new Point(3, 0);
      init.G3 = new Point(1, 1);
      init.G4 = new Point(2, 0);
      init.G5 = new Point(3, 1);
      return init;
    }

    private Dicision retDic() {
      Dicision retD = new Dicision();
      retD.AL = Action_Label.Move;
      List<Point> lp = Function.EnemyGhostState();
      for(int i = 0; i < lp.Count();i++) {
        if(lp[i].Y == 0) {
          if(lp[i].X == 1) {
            retD.SI = 0;
            retD.DR = Direction.Right;
            return retD;
          }
          if(lp[i].X == 3) {
            retD.SI = 2;
            retD.DR = Direction.Left;
            return retD;
          }
        } else if(lp[i].Y == 1) {
          if(lp[i].X == 0) {
            retD.SI = 0;
            retD.DR = Direction.Down;
            return retD;
          }
          if(lp[i].X == 4) {
            retD.SI = 2;
            retD.DR = Direction.Down;
            return retD;
          }
        }
      }
      if(lg[chasel].GetDoA()) {
        if(lg[chasel].GetPos() == new Point(1, 0)) {
          retD.SI = chasel;
          retD.DR = Direction.Left;
          return retD;
        }
        if(lg[chasel].GetPos() == new Point(0, 1)) {
          retD.SI = chasel;
          retD.DR = Direction.Up;
          return retD;
        }
      }
      if(lg[chaser].GetDoA()) {
        if(lg[2].GetPos() == new Point(3, 0)) {
          retD.SI = chaser;
          retD.DR = Direction.Right;
          return retD;
        }
        if(lg[chaser].GetPos() == new Point(4, 1)) {
          retD.SI = chaser;
          retD.DR = Direction.Up;
          return retD;
        }
      }

      if(lg[4].GetDoA()) {
        if(!lg[0].GetDoA()) {
          retD.SI = 4;
          retD.DR = Direction.Left;
          if(lg[4].GetPos() == new Point(1, 0)) {
            chasel = 4;
          }
          return retD;
        }
        if(!lg[2].GetDoA()) {
          retD.SI = 4;
          retD.DR = Direction.Right;
          if(lg[4].GetPos() == new Point(3, 0)) {
            chaser = 4;
          }
          return retD;
        }
      }

      if(lg[3].GetDoA()) {
        retD.SI = 3;
        retD.DR = Direction.Down;
        return retD;
      } else if(lg[5].GetDoA()) {
        retD.SI = 5;
        retD.DR = Direction.Down;
        return retD;
      }

      retD.SI = 1;
      retD.DR = Direction.Left;
      return retD;
    }

    //行動の定義
    public override Dicision Dicision() {
      Dicision outdic = new Dicision();
      outdic.AL = Action_Label.Move;
      lg = new List<GhostState>{
        Function.MyGhostState(0),Function.MyGhostState(1),Function.MyGhostState(2),
        Function.MyGhostState(3),Function.MyGhostState(4),Function.MyGhostState(5)
      };
      nt = Function.NowTurn();
      if(nt < 8) {
        if(nt < 2) {
          outdic.SI = 0;
          outdic.DR = Direction.Left;
        } else if(nt < 4) {
          outdic.SI = 2;
          outdic.DR = Direction.Right;
        } else if(nt < 6) {
          if(lg[3].GetDoA() == true) {
            outdic.SI = 3;
            outdic.DR = Direction.Down;
          } else {
            outdic.SI = 5;
            outdic.DR = Direction.Down;
          }
        } else {
          outdic = retDic();
          if(lg[5].GetDoA() && lg[5].GetPos().Y == 1) {
            outdic.SI = 5;
            outdic.DR = Direction.Down;
          }
        }
      } else if(nt > 193) {
        if(nt < 196) {
          outdic.SI = 1;
        }else if(nt < 198) {
          outdic.SI = chasel;
        } else {
          outdic.SI = chaser;
        }
        outdic.DR = Direction.Down;
      } else {
        outdic = retDic();
      }

      return outdic;
    }
  }

}
