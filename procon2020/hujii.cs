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
  public class Fujii : AbstPlayer {
    //ランダムに移動
    public Fujii() : base() { }
    public Fujii(string name) : base() { this.name = name; }
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
        public override Dicision Dicision()
        {
            Dicision outdic = new Dicision();
            Random cRandom = new System.Random();
            int maxlevel(int alpha, int beta, int limit, int[,] table_info)
            {
                // 自分の青コマ勝利判定
                if (table_info[0, 5] == 1 || table_info[4, 5] == 1)
                {
                    return Int32.MaxValue;
                }
                // 敗北判定
                // 自分の青コマカウント
                int my_blue_count = 0;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (table_info[i, j] == 1) my_blue_count = my_blue_count + 1;
                    }
                }
                if (my_blue_count == 0)
                {
                    return -Int32.MaxValue + 1;
                }
                if (limit == 0)
                {
                    //評価値
                    //個数の評価値
                    int num_val = my_blue_count - (3 - Function.GetDeleteEnemyNum()[0]);
                    int my_goal_score = 0;
                    int enemy_goal_score = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            if (table_info[i, j] == 1 || table_info[i, j] == -1)
                            {
                                if (i < 3)
                                {
                                    my_goal_score = my_goal_score + Math.Abs(0 - i) + Math.Abs(5 - j);//マンハッタン距離
                                }
                                else
                                {
                                    my_goal_score = my_goal_score + Math.Abs(4 - i) + Math.Abs(5 - j);//マンハッタン距離
                                }
                            }
                            if (table_info[i, j] == 5)
                            {
                                if (i < 3)
                                {
                                    enemy_goal_score = enemy_goal_score + Math.Abs(0 - i) + Math.Abs(0 - j);//マンハッタン距離
                                }
                                else
                                {
                                    enemy_goal_score = enemy_goal_score + Math.Abs(4 - i) + Math.Abs(0 - j);//マンハッタン距離
                                }
                            }
                        }
                    }
                    //位置の評価値
                    int pos_val = enemy_goal_score - my_goal_score;
                    //評価値
                    int fin_val = num_val + 50 * pos_val;
                    return fin_val;
                }
                //可能な自分の手をすべて生成
                // Up:0, Right:1, Down:2, Left:3
                List<int[]> pos_hand = new List<int[]>();
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (table_info[i, j] == 1 || table_info[i, j] == -1)// 自分のコマだったら
                        {

                            //Up可能探索
                            if (j + 1 < 6)
                            {
                                if (j + 1 < 6 && table_info[i, j + 1] != 1 && table_info[i, j + 1] != -1)
                                {
                                    pos_hand.Add(new int[4] { i, j, 0, 1 });
                                }
                            }
                            //Right可能探索
                            if (i + 1 < 5)
                            {
                                if (i + 1 < 5 && table_info[i + 1, j] != 1 && table_info[i + 1, j] != -1)
                                {
                                    pos_hand.Add(new int[4] { i, j, 1, 0 });
                                }
                            }
                            //Down可能探索
                            if (j - 1 > -1)
                            {
                                if (j - 1 < 6 && table_info[i, j - 1] != 1 && table_info[i, j - 1] != -1)
                                {
                                    pos_hand.Add(new int[4] { i, j, 0, -1 });
                                }
                            }
                            //Left可能探索
                            if (i - 1 > -1)
                            {
                                if (i - 1 < 6 && table_info[i - 1, j] != 1 && table_info[i - 1, j] != -1)
                                {
                                    pos_hand.Add(new int[4] { i, j, -1, 0 });
                                }
                            }
                            // 動かない
                            pos_hand.Add(new int[4] { i, j, 0, 0 });
                        }
                    }
                }

                int score_max = -Int32.MaxValue;
                int[,] tmp_table_info = new int[5, 6];
                foreach (int[] tmp_hand in pos_hand)
                {
                    int score;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            tmp_table_info[i, j] = table_info[i, j];
                        }
                    }
                    //手を打つ
                    int tmp_ghost_var = tmp_table_info[tmp_hand[0], tmp_hand[1]];
                    tmp_table_info[tmp_hand[0], tmp_hand[1]] = 0;
                    tmp_table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]] = tmp_ghost_var;
                    score = minlevel(alpha, beta, limit - 1, tmp_table_info);//次の相手の手
                                                                             //手を戻す
                                                                             //tmp_ghost_var = table_info[tmp_hand[0]+tmp_hand[2], tmp_hand[1]+tmp_hand[3]];
                                                                             //table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]] = 0;
                                                                             //table_info[tmp_hand[0], tmp_hand[1]] = tmp_ghost_var;
                    if (score >= beta)
                    {
                        // beta 値を上回ったら探索を中止
                        return score;
                    }

                    if (score > score_max)
                    {
                        score_max = score;
                        if (score_max >= alpha)
                        {
                            alpha = score_max;
                        }
                    }
                }
                return score_max;
            }
            int minlevel(int alpha, int beta, int limit, int[,] table_info)
            {
                // 敵の青コマ勝利判定
                if (table_info[0, 0] == 5 || table_info[4, 0] == 5)
                {
                    return -Int32.MaxValue;
                }
                // 敵の紫or赤コマの取得個数が3個以上なら敗北判定
                // 敵の赤コマカウント
                // まず盤面の紫コマ数える
                int enemy_board_count = 0;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (table_info[i, j] == 5)
                        {
                            enemy_board_count = enemy_board_count + 1;
                        }
                    }
                }
                // 元の取った敵の赤コマの数
                int pre_red_count = Function.GetDeleteEnemyNum()[1];
                // 元の盤面の紫コマの数
                int pre_board_count = 6 - Function.GetDeleteEnemyNum()[0] - Function.GetDeleteEnemyNum()[1];
                // 取った敵の赤コマの数
                int enemy_red_count = pre_red_count + pre_board_count - enemy_board_count;
                // 敗北判定
                if (enemy_red_count >= 3)
                {
                    return -Int32.MaxValue + 1;
                }

                if (limit == 0)
                {
                    //評価値
                    int my_blue_count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            if (table_info[i, j] == 1) my_blue_count = my_blue_count + 1;
                        }
                    }
                    //個数の評価値
                    int num_val = my_blue_count - (3 - Function.GetDeleteEnemyNum()[0]);
                    int my_goal_score = 0;
                    int enemy_goal_score = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            if (table_info[i, j] == 1 || table_info[i, j] == -1)
                            {
                                if (i < 3)
                                {
                                    my_goal_score = my_goal_score + Math.Abs(0 - i) + Math.Abs(5 - j);//マンハッタン距離
                                }
                                else
                                {
                                    my_goal_score = my_goal_score + Math.Abs(4 - i) + Math.Abs(5 - j);//マンハッタン距離
                                }
                            }
                            if (table_info[i, j] == 5)
                            {
                                if (i < 3)
                                {
                                    enemy_goal_score = enemy_goal_score + Math.Abs(0 - i) + Math.Abs(0 - j);//マンハッタン距離
                                }
                                else
                                {
                                    enemy_goal_score = enemy_goal_score + Math.Abs(4 - i) + Math.Abs(0 - j);//マンハッタン距離
                                }
                            }
                        }
                    }
                    //位置の評価値
                    int pos_val = enemy_goal_score - my_goal_score;
                    //評価値
                    int fin_val = num_val + 50 * pos_val;
                    return fin_val;
                }
                //可能な敵の手をすべて生成
                // Up:0, Right:1, Down:2, Left:3
                List<int[]> pos_hand = new List<int[]>();
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (table_info[i, j] == 5)// 敵のコマだったら
                        {
                            //Up可能探索
                            if (j + 1 < 6)
                            {
                                if (j + 1 < 6 && table_info[i, j + 1] != 5)
                                {
                                    pos_hand.Add(new int[4] { i, j, 0, 1 });
                                }
                            }
                            //Right可能探索
                            if (i + 1 < 5)
                            {
                                if (i + 1 < 5 && table_info[i + 1, j] != 5)
                                {
                                    pos_hand.Add(new int[4] { i, j, 1, 0 });
                                }
                            }
                            //Down可能探索
                            if (j - 1 > -1)
                            {
                                if (j - 1 < 6 && table_info[i, j - 1] != 5)
                                {
                                    pos_hand.Add(new int[4] { i, j, 0, -1 });
                                }
                            }
                            //Left可能探索
                            if (i - 1 > -1)
                            {
                                if (i - 1 < 6 && table_info[i - 1, j] != 5)
                                {
                                    pos_hand.Add(new int[4] { i, j, -1, 0 });
                                }
                            }
                            // 動かない
                            //pos_hand.Add(new int[4] { i, j, 0, 0 });
                        }
                    }
                }

                int score_min = Int32.MaxValue;
                int[,] tmp_table_info = new int[5, 6];
                foreach (int[] tmp_hand in pos_hand)
                {
                    int score;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            tmp_table_info[i, j] = table_info[i, j];
                        }
                    }
                    //手を打つ
                    int tmp_ghost_var = tmp_table_info[tmp_hand[0], tmp_hand[1]];
                    tmp_table_info[tmp_hand[0], tmp_hand[1]] = 0;
                    tmp_table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]] = tmp_ghost_var;
                    score = maxlevel(alpha, beta, limit - 1, tmp_table_info);//次の自分の手
                                                                             //手を戻す
                                                                             //tmp_ghost_var = table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]];
                                                                             //table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]] = 0;
                                                                             //table_info[tmp_hand[0], tmp_hand[1]] = tmp_ghost_var;
                    if (score <= alpha)
                    {
                        //alpha値を下回った探索を中止
                        return score;
                    }

                    if (score < score_min)
                    {
                        score_min = score;
                        if (score_min <= beta)
                        {
                            beta = score_min;
                        }
                    }
                }
                return score_min;
            }
            while (true)
            {
                //Console.Write("enego = {0}", Function.EnemyGhostState()[0]);
                // 盤面のロード
                int[,] table_info = new int[5, 6];
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        table_info[i, j] = 0;
                    }
                }
                // 盤面のロード(1が青, -1が赤, 5が敵)
                // 自身
                int tmp_x, tmp_y;
                for (int id = 0; id < 6; id++)
                {
                    if (Function.MyGhostState(id).GetDoA() == true)
                    {
                        tmp_x = Function.MyGhostState(id).GetPos().X;
                        tmp_y = Function.MyGhostState(id).GetPos().Y;
                        if (id < 3)
                        {
                            table_info[tmp_x, tmp_y] = 1;
                        }
                        else
                        {
                            table_info[tmp_x, tmp_y] = -1;
                        }
                    }
                }
                // 敵
                List<Point> tmp_enemy_points = Function.EnemyGhostState();
                foreach (Point tmp_enemy_point in tmp_enemy_points)
                {
                    table_info[tmp_enemy_point.X, tmp_enemy_point.Y] = 5;
                }
                int alpha = -Int32.MaxValue;
                int beta = Int32.MaxValue;
                int eval;
                int eval_max = -Int32.MaxValue;
                int[] fin_hand = new int[4] { -5, -5, -5, -5 };//最終的な打ち手
                //可能な自分の手をすべて生成
                // Up:0, Right:1, Down:2, Left:3
                List<int[]> pos_hand = new List<int[]>();
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (table_info[i, j] == 1 || table_info[i, j] == -1)// 自分のコマだったら
                        {
                            //Up可能探索
                            if (j + 1 < 6)
                            {
                                if (j + 1 < 6 && table_info[i, j + 1] != 1 && table_info[i, j + 1] != -1)
                                {
                                    pos_hand.Add(new int[4] { i, j, 0, 1 });
                                }
                            }
                            //Right可能探索
                            if (i + 1 < 5)
                            {
                                if (i + 1 < 5 && table_info[i + 1, j] != 1 && table_info[i + 1, j] != -1)
                                {
                                    pos_hand.Add(new int[4] { i, j, 1, 0 });
                                }
                            }
                            //Down可能探索
                            if (j - 1 > -1)
                            {
                                if (j - 1 < 6 && table_info[i, j - 1] != 1 && table_info[i, j - 1] != -1)
                                {
                                    pos_hand.Add(new int[4] { i, j, 0, -1 });
                                }
                            }
                            //Left可能探索
                            if (i - 1 > -1)
                            {
                                if (i - 1 < 6 && table_info[i - 1, j] != 1 && table_info[i - 1, j] != -1)
                                {
                                    pos_hand.Add(new int[4] { i, j, -1, 0 });
                                }
                            }
                            // 動かない
                            pos_hand.Add(new int[4] { i, j, 0, 0 });
                        }
                    }
                }

                int[,] tmp_table_info = new int[5, 6];
                // デバッグ用
                //List<int> eval_list = new List<int>();
                foreach (int[] tmp_hand in pos_hand)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            tmp_table_info[i, j] = table_info[i, j];
                        }
                    }
                    //手を打つ
                    int tmp_ghost_var = tmp_table_info[tmp_hand[0], tmp_hand[1]];
                    tmp_table_info[tmp_hand[0], tmp_hand[1]] = 0;
                    tmp_table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]] = tmp_ghost_var;
                    eval = minlevel(alpha, beta, 4, tmp_table_info);//次の相手の手
                    //eval_list.Add(eval);
                    //手を戻す
                    //tmp_ghost_var = table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]];
                    //table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]] = 0;
                    //table_info[tmp_hand[0], tmp_hand[1]] = tmp_ghost_var;
                    if (eval >= eval_max)
                    {
                        eval_max = eval;
                        fin_hand = tmp_hand;
                    }
                }

                /*
                if (eval_max == -Int32.MaxValue)
                {
                    foreach (int[] tmp_hand in pos_hand)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                tmp_table_info[i, j] = table_info[i, j];
                            }
                        }
                        //手を打つ
                        int tmp_ghost_var = tmp_table_info[tmp_hand[0], tmp_hand[1]];
                        tmp_table_info[tmp_hand[0], tmp_hand[1]] = 0;
                        tmp_table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]] = tmp_ghost_var;
                        eval = minlevel(alpha, beta, 3, tmp_table_info);//次の相手の手
                        //eval_list.Add(eval);
                        //手を戻す
                        tmp_ghost_var = table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]];
                        table_info[tmp_hand[0] + tmp_hand[2], tmp_hand[1] + tmp_hand[3]] = 0;
                        table_info[tmp_hand[0], tmp_hand[1]] = tmp_ghost_var;
                        if (eval >= eval_max)
                        {
                            eval_max = eval;
                            fin_hand = tmp_hand;
                        }
                    }
                }*/

                Point fin_point = new Point(fin_hand[0], fin_hand[1]);
                for (int id = 0; id < 6; id++)
                {
                    if (Function.MyGhostState(id).GetDoA() == true && Function.MyGhostState(id).GetPos() == fin_point)
                    {
                        outdic.SI = id;
                        outdic.AL = Action_Label.Move;
                        if (fin_hand[3] == 1) outdic.DR = Direction.Down;
                        else if (fin_hand[2] == 1) outdic.DR = Direction.Right;
                        else if (fin_hand[3] == -1) outdic.DR = Direction.Up;
                        else if (fin_hand[2] == -1) outdic.DR = Direction.Left;
                        if (fin_hand[2] == 0 && fin_hand[3] == 0)
                        {
                            outdic.AL = Action_Label.Dummy; // 動かない
                        }
                        break;
                    }
                }
                break;
            }
            //outdic.SI = 0;
            //outdic.AL = Action_Label.Move;
            //outdic.DR = Direction.Left;
            return outdic;
        }
    }

}
