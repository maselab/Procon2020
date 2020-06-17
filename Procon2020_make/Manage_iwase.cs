using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Procon2019 {
  public enum Player { P1, P2, Dummy }
  public enum Direction { Up, Right, Down, Left, Dummy }//
  public enum Action_Label { Move, Dummy }//
  public enum Action_Result { Miss, Atk_Hit, P1Death, P2Death, BothDeath, Dummy }

  public class global {
        public const int Range_col = 6;//
        public const int Range_row = 5;//
        public const int ShipNum = 6;//
        public const int FinalTurn = 200;//
        internal static int Turn;
        internal static Player currentplayer;
        internal static Log[] log;
  }

  public partial class Form1 {
    internal static ShipState[] S1, S2;
    internal static AbstPlayer P1, P2;
    private static Point Bpos;
  }

  public class Dicision {
    public Dicision() { this.SI = 0; this.AL = Action_Label.Dummy; this.DR = Direction.Dummy; }
    public int SI;
    public Action_Label AL;
    public Direction DR;
  }

  public class Log {
    public Log() { this.player = Player.Dummy; this.al = Action_Label.Dummy; this.ar = Action_Result.Dummy; }
    public Player player;
    public Action_Label al;
    public Action_Result ar;
  }

  public class ShipState {
    Random rnd = new System.Random();
    public ShipState() { }
    public ShipState(Player cp, int shipid) {//初期位置設定
      flag = true;
      id = shipid;
      switch (shipid) {
        case 1:
          life = 3; atk = 3;
          pos = new Point(rnd.Next(3)+2, rnd.Next(2));
          break;
        case 2:
          life = 2; atk = 2;
          pos = new Point(rnd.Next(2)+5, rnd.Next(3));
          break;
        case 3:
          life = 1; atk = 2;
          pos = new Point(rnd.Next(2), rnd.Next(3));
          break;
        case 4:
          life = 1; atk = 1;
          pos = new Point(rnd.Next(3)+2, 2);
          break;
        case 5:
          life = 1; atk = 2;
          pos = new Point(rnd.Next(2), rnd.Next(3));
          break;
        case 6:
          life = 1; atk = 1;
          pos = new Point(rnd.Next(3) + 2, 2);
          break;
            }
    }
    internal int id;
    internal Point pos;
    internal bool flag; //生存フラグ,1:生存,0:脂肪
    internal int life;
    internal int atk;
    internal int Roundenemy;
    public int GetId() { return id; }
    public Point GetPos() { return pos; }
    public bool GetDoA() { return flag; }
    public int GetLife() { return life; }
    public int GetAtk() { return atk; }
    public int GetRoundEnemy() { return Roundenemy; }
  }

}
