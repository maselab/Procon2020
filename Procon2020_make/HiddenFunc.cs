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
  public partial class Form1 {
    
    internal static int YourPos_X(Player player, int pos) {
      int cp = (int)player;
      return cp * (global.Range_X - 1) + (1 - 2 * cp) * pos;
    }
    internal static int YourPos_Y(Player player, int pos)
    {
      int cp = (int)player;
      return cp * (global.Range_Y - 1) + (1 - 2 * cp) * pos;
    }
      // プレイヤー, ローカル位置 → グローバル位置
    internal static Point YourPoint(Player player, Point pos) {
      Point ret = new Point();
      ret.X = YourPos_X(player, pos.X);
      ret.Y = YourPos_Y(player, pos.Y);
      return ret;
    }
    
    // ローカル位置, 進行方向 → 新しいグローバル位置
    private Point YourDR(Point pos, Direction dir) {
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
          return new Point(-1, -1);
      }
      if (x >= global.Range_X || x < 0 || y >= global.Range_Y || y < 0) { canflag = false; }
      else {
        for (int i = 0; i < global.GhostNum; i++) {
          if (global.currentplayer == Player.P1) {
           //ここで味方の被り判定
            if (S1[i].flag && YourPoint(Player.P1, S1[i].pos) == YourPoint(Player.P1, new Point(x, y))) { canflag = false; }
          }
          else {
            if (S2[i].flag && YourPoint(Player.P2, S2[i].pos) == YourPoint(Player.P2, new Point(x, y))) { canflag = false; }
          }
        }
      }
      if (canflag) {
        return new Point(x, y);
      }
      else { return new Point(-1, -1); }
    }

    // 行動の反映
    private void ActionDic(Dicision dodic) {
      global.log[global.Turn].player = global.currentplayer;
      if (dodic.SI >= 0 && dodic.SI < global.GhostNum) {

        if (global.currentplayer == Player.P1) {
          if (S1[dodic.SI].flag) {
            if (dodic.AL == Action_Label.Move) {
              SMove(S1[dodic.SI], dodic.DR);
            }
            //else {
            //  SFire(S1[dodic.SI], dodic.DR);
            //}
          }
        }
        else {
          if (S2[dodic.SI].flag) {
            if (dodic.AL == Action_Label.Move) {
              SMove(S2[dodic.SI], dodic.DR);
            }
            //else {
            //  SFire(S2[dodic.SI], dodic.DR);
            //}
          }
        }

      }
    }

    // ダメージ処理
    private bool Damage(GhostState ss, int damage) {
      ss.life -= damage;
      if (ss.life <= 0) { ss.flag = false; return true; }
      else {
        return false;
      }
    }
    
    // 移動処理
    private void SMove(GhostState ss, Direction dir) {
      Bpos = new Point(-1, -1);
      global.log[global.Turn].al = Action_Label.Move;
      Point tmp = YourDR(ss.pos, dir);
      bool deathflagP1 = false, deathflagP2 = false;
      if (tmp != new Point(-1, -1)) {
        ss.pos = tmp;
        tmp = YourPoint(global.currentplayer, tmp);
        // 衝突判定
        if (global.currentplayer == Player.P1) {
          for (int i = 0; i < global.GhostNum; i++) {
            if (S2[i].flag && YourPoint(Player.P2, S2[i].pos) == tmp) {
              //deathflagP1 = Damage(ss, S2[i].atk);
              deathflagP2 = Damage(S2[i], ss.atk);
            }
          }
        }
        else {
          for (int i = 0; i < global.GhostNum; i++) {
            if (S1[i].flag && YourPoint(Player.P1, S1[i].pos) == tmp) {
              //deathflagP2 = Damage(ss, S1[i].atk);
              deathflagP1 = Damage(S1[i], ss.atk);
            }
          }
        }
        // 死亡判定
        if (deathflagP1 == false) {
          if (deathflagP2 == false) {
            global.log[global.Turn].ar = Action_Result.Miss;
          }
          else { global.log[global.Turn].ar = Action_Result.P2Death; }
        }
        else {
          if (deathflagP2 == false) {
            global.log[global.Turn].ar = Action_Result.P1Death;
          }
          //else { global.log[global.Turn].ar = Action_Result.BothDeath; }
        }
      }
    }

    // 攻撃処理(不必要)
    /*
    private void SFire(GhostState ss, Direction dir) {
      Bpos = new Point(-1, -1);
      global.log[global.Turn].al = Action_Label.Fire;
      Point tmp = YourDR(ss.pos, dir);
      bool hitflag = false, deathflag = false;
      if (tmp != new Point(-1, -1)) {
        tmp = YourPoint(global.currentplayer, tmp);
        Bpos = tmp;
        if (global.currentplayer == Player.P1) {
          for (int i = 0; i < global.GhostNum; i++) {
            if (S2[i].flag && YourPoint(Player.P2, S2[i].pos) == tmp) { hitflag = true; deathflag = Damage(S2[i], ss.atk); }
          }
        }
        else {
          for (int i = 0; i < global.GhostNum; i++) {
            if (S1[i].flag && YourPoint(Player.P1, S1[i].pos) == tmp) { hitflag = true; deathflag = Damage(S1[i], ss.atk); }
          }
        }
        if (hitflag == false) { global.log[global.Turn].ar = Action_Result.Miss; }
        else {
          if (deathflag == false) { global.log[global.Turn].ar = Action_Result.Atk_Hit; }
          else {
            if (global.currentplayer == Player.P1) {
              global.log[global.Turn].ar = Action_Result.P2Death;
            }
            else {
              global.log[global.Turn].ar = Action_Result.P1Death;
            }
          }
        }
      }
    }*/

    // 周囲の敵の数更新処理(要らない説)
    /*
    private void Search() {
      int counter1, counter2;
      for (int i = 0; i < global.GhostNum; i++) {
        counter1 = 0; counter2 = 0;
        for (int j = 0; j < global.GhostNum; j++) {
          for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
              if (YourPoint(Player.P1, Point.Add(S1[i].pos, new Size(x, y))) == YourPoint(Player.P2, S2[j].pos) && S2[j].flag) {
                counter1 += 1;
              }
              if (YourPoint(Player.P2, Point.Add(S2[i].pos, new Size(x, y))) == YourPoint(Player.P1, S1[j].pos) && S1[j].flag) {
                counter2 += 1;
              }
            }
          }
        }
        if(S1[i].flag) { S1[i].Roundenemy = counter1; }
        else { S1[i].Roundenemy = -1; }
        if(S2[i].flag) { S2[i].Roundenemy = counter2; }
        else { S2[i].Roundenemy = -1; }
      }
    }
    */
  }
}
