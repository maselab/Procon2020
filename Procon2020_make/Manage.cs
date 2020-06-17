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
  public enum Player { P1, P2, Dummy }
  //public enum Direction { Up, Upper_right, Right, Lower_right, Down, Lower_left, Left, Upper_left, Dummy }
  public enum Direction { Up, Right, Down, Left, Dummy }
  //public enum Action_Label { Move, Fire, Dummy }
  public enum Action_Label { Move, Dummy }
  //public enum Action_Result { Miss, Atk_Hit, P1Death, P2Death, BothDeath, Dummy }
  public enum Action_Result { Miss, P1Death, P2Death, Dummy }

    public class global {
    // マスの数を5*6に変更
    public const int Range = 7;
    public const int Range_X = 5;
    public const int Range_Y = 6;
        // 船の数を6変更
        //public const int GhostNum = 4;
        public const int GhostNum = 6;
    public const int FinalTurn = 200;
    internal static int Turn;
        internal static List<int> Eflag_p1;
        internal static List<int> Eflag_p2;
        internal static Player currentplayer;
    internal static Log[] log;
  }

  public partial class Form1 {
    internal static GhostState[] S1, S2;
    internal static AbstPlayer P1, P2;
    private static Point Bpos;
  }

  public class Dicision {
    public Dicision() { this.SI = 0; this.AL = Action_Label.Dummy; this.DR = Direction.Dummy; }
    public int SI;
    public Action_Label AL;
    public Direction DR;
  }

public class Init_pos
    {
        public Init_pos() { this.G0 = new Point (0,1); this.G1 = new Point(0, 2); this.G2 = new Point(0, 3); this.G3 = new Point(1, 1); this.G4 = new Point(1, 2); this.G5 = new Point(1, 3); }
        public Point G0;
        public Point G1;
        public Point G2;
        public Point G3;
        public Point G4;
        public Point G5;
    }

  public class Log {
    public Log() { this.player = Player.Dummy; this.al = Action_Label.Dummy; this.ar = Action_Result.Dummy; }
    public Player player;
    public Action_Label al;
    public Action_Result ar;
  }

  // ライフと攻撃を1に変更, 
  //初期位置は引数取れるようにする
  // 座標ローカル
  // 船の数6に
  public class GhostState {
    Random rnd = new System.Random();
    public GhostState() { }
    public GhostState(Player cp, int ghostid, Point Gpos) {
      flag = true;
      id = ghostid;
      life = 1;
      atk = 1;
      pos = Gpos;
            if(pos.X <= 0){pos.X = 1;}
            if (pos.X >= 4) { pos.X = 3; }
            if (pos.Y < 0) { pos.X = 0; }
            if (pos.Y >= 2) { pos.X = 1; }
            /*
      switch (ghostid) {
        case 1:
          life = 1; atk = 1;
          pos = Gpos;
          break;
        case 2:
          life = 1; atk = 1;
          pos = new Point(rnd.Next(2)+5, rnd.Next(3));
          break;
        case 3:
          life = 1; atk = 1;
          pos = new Point(rnd.Next(2), rnd.Next(3));
          break;
        case 4:
          life = 1; atk = 1;
          pos = new Point(rnd.Next(3)+2, 2);
          break;
        case 5:
        　life = 1; atk = 1;
        　pos = new Point(rnd.Next(3) + 2, rnd.Next(2));
        　break;
        case 6:
        　life = 1; atk = 1;
        　pos = new Point(rnd.Next(2) + 5, rnd.Next(3));
        　break;
            }*/
        }
    internal int id;
    internal Point pos;
    internal bool flag; //生存フラグ,1:生存,0:脂肪
    internal int life;
    internal int atk;
    //internal int Roundenemy;
    public int GetId() { return id; }
    public Point GetPos() { return pos; }
    public bool GetDoA() { return flag; }
    public int GetLife() { return life; }
    public int GetAtk() { return atk; }
    //public int GetRoundEnemy() { return Roundenemy; }
  }

}
