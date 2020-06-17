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
  public class Function {

    public static int NowTurn() { return global.Turn; }

    public static Player MyPlayer() { return global.currentplayer; }

    public static Log Log(int turn) {
      if (turn >= 0 && turn < global.FinalTurn) { return global.log[turn]; }
      else { return new Log(); }
    }

    public static GhostState MyGhostState(int id) {
      if (id >= 0 && id < global.GhostNum) {
        if (global.currentplayer == Player.P1) { return Form1.S1[id]; }
        else { return Form1.S2[id]; }
      }
      else { return new GhostState(); }
    }

    // 敵の船座標値リストを返す
    public static List<Point> EnemyGhostState(){
      var enemy_pos_list = new List<Point>();
      Point tmp_pos = new Point();
      if (global.currentplayer == Player.P1) {
        // プレイヤー1から見る場合は、グローバル→ローカルの変換が必要
        for (int i = 0; i < global.GhostNum; i++)
        {
          if(Form1.S2[i].flag){
            // プレイヤー2から見たローカル→プレイヤー1から見たローカル
            tmp_pos.X = (global.Range_X - 1) + (-1)*Form1.S2[i].pos.X;
            tmp_pos.Y = (global.Range_Y - 1) + (-1)*Form1.S2[i].pos.Y;
            enemy_pos_list.Add(tmp_pos);
          }
        }
      }
            else
            {
                // プレイヤー1から見る場合は、グローバル→ローカルの変換が必要
                for (int i = 0; i < global.GhostNum; i++)
                {
                    if (Form1.S1[i].flag)
                    {
                        // プレイヤー2から見たローカル→プレイヤー1から見たローカル
                        tmp_pos.X = (global.Range_X - 1) + (-1) * Form1.S1[i].pos.X;
                        tmp_pos.Y = (global.Range_Y - 1) + (-1) * Form1.S1[i].pos.Y;
                        enemy_pos_list.Add(tmp_pos);
                    }
                }
            }
            /*
            else {
              for (int i = 0; i < global.GhostNum; i++)
              {
                if (Form1.S1[i].flag) {
                  tmp_pos.X = Form1.S1[i].pos.X;
                  tmp_pos.Y = Form1.S1[i].pos.Y;
                  enemy_pos_list.Add(tmp_pos);
                }
              }
            }*/
            return enemy_pos_list;
    }

    // 取ったコマの数を返す
    public static List<int> GetDeleteEnemyNum(){
      int S1_b=0, S1_r=0, S2_b=0, S2_r=0;
      List<int> count_list = new List<int>();
      // ここでバグる可能性
      for (int i = 0; i < (int)global.GhostNum/2; i++)
      {
        if (!Form1.S1[i].flag) { S1_b += 1; }
        if (!Form1.S2[i].flag) { S2_b += 1; }
      }
      for (int i = (int)global.GhostNum/2; i < global.GhostNum; i++)
      {
        if (!Form1.S1[i].flag) { S1_r += 1; }
        if (!Form1.S2[i].flag) { S2_r += 1; }
      }
      if (global.currentplayer == Player.P1){
        count_list.Add(S2_b);
        count_list.Add(S2_r);
        return count_list;
      }
      else
      {
        count_list.Add(S1_b);
        count_list.Add(S1_r);
        return count_list;
      }
    }

    public static bool CanDR(Point pos, Direction dir) {
      Player cp = global.currentplayer;
      int x = pos.X, y = pos.Y;
      bool canflag = true;
      switch (dir) {
        case Direction.Up:
          y -= 1; break;
        //case Direction.Upper_right:
        //  y -= 1; x += 1; break;
        case Direction.Right:
          x += 1; break;
        //case Direction.Lower_right:
        //  y += 1; x += 1; break;
        case Direction.Down:
          y += 1; break;
        //case Direction.Lower_left:
        //  y += 1; x -= 1; break;
        case Direction.Left:
          x -= 1; break;
        //case Direction.Upper_left:
        //  y -= 1; x -= 1; break;
        case Direction.Dummy:
          return false;
      }
      if (x >= global.Range_X || x < 0 || y >= global.Range_Y || y < 0) { canflag = false; }
      else {
        for (int i = 0; i < global.GhostNum; i++) {
          if (MyGhostState(i).flag && Form1.YourPoint(cp, MyGhostState(i).pos) == Form1.YourPoint(cp, new Point(x, y))) { canflag = false; }
        }
      }
      return canflag;
    }

  }
}
