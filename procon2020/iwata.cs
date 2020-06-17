using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Procon2020
{
    public class Iwata : AbstPlayer
    {
        //ランダムに移動
        public Iwata() : base() { }
        public Iwata(string name) : base() { this.name = name; }
        //初期配置の設定　1 <= X <= 3, 0 <= Y <= 1
        public override Init_pos Init_pos()
        {
            Init_pos init = new Init_pos();
            init.G0 = new Point(1, 1);
            init.G1 = new Point(2, 0);
            init.G2 = new Point(3, 1);
            init.G3 = new Point(1, 0);
            init.G4 = new Point(2, 1);
            init.G5 = new Point(3, 0);
            return init;
        }
        //行動の定義
        public override Dicision Dicision()
        {
            int playnum = 300;
            Dicision outdic = new Dicision();
            var func = new Function();
            GameBoard gameBoard = new GameBoard();
            gameBoard.make_GameBoard(func);
            int max_n = 0;
            float max_val=0;
            Point ghostpoint = new Point();
            int direction = 0;
            int id_ghost = 0;
            
            int division_count = (int)(playnum/gameBoard.gameboard_overall.Count);
            MonteCarloTree root_nodes = new MonteCarloTree();
            for (int i = 0; i < gameBoard.gameboard_overall.Count; i++)
            {
                /*
                if(i==4){
                break;}
                */
                MonteCarloTree root_node = new MonteCarloTree();
                root_node.gameboard = gameBoard.gameboard_overall[i];
                if(i%2!=0){
                root_node.gameboard = gameBoard.gameboard_overall[gameBoard.gameboard_overall.Count-i];
                    }
                root_node.is_arrived = root_node.judge_is_arrive(root_node.gameboard,false);
                root_node.expand();
                for (int u = 0; u <= division_count; u++)
                {
                    root_node.evaluate();
                }
                foreach (MonteCarloTree child_node in root_node.child)
                {  
                    if (child_node.n > max_n)
                    {   
                        direction = child_node.dir_node;
                        ghostpoint = child_node.action;
                        max_n = child_node.n;
                        max_val=child_node.w;
                    }
                    else if(child_node.n==max_n && child_node.w>=max_val){
                        direction = child_node.dir_node;
                        ghostpoint = child_node.action;
                        max_n = child_node.n;
                        max_val=child_node.w;
                    }
                }
                root_nodes=root_node;
            }
            
            for (int t = 0; t < 6; t++)
            {
                
                Point ghost_pos = Function.MyGhostState(t).GetPos();
                if (ghost_pos.X == ghostpoint.X && ghost_pos.Y == ghostpoint.Y)
                {
                    id_ghost = t;
                    
                }
                foreach(int dir_k in root_nodes.directions)
                {
                Point ghost_pos_=root_nodes.move(ghost_pos,dir_k);
                if(t<3){
                if((((ghost_pos_.X==0 && ghost_pos_.Y==5) && (root_nodes.gameboard[1,5]==0 || root_nodes.gameboard[0,4]==0))|| ((ghost_pos_.X==4 && ghost_pos_.Y==5) && (root_nodes.gameboard[4,4]==0 || root_nodes.gameboard[3,5]==0)))&& Function.GetDeleteEnemyNum()[1]<2){
                var dir_ = (Direction)Enum.ToObject(typeof(Direction), dir_k);
                outdic.SI = id_ghost;
                outdic.AL = Action_Label.Move;
                outdic.DR = dir_;
                
                return outdic;
                        }
                        }
                if(root_nodes.is_arrived)
                        if((((ghost_pos_.X==0 && ghost_pos_.Y==0) && (root_nodes.gameboard[0,0]==1 || root_nodes.gameboard[0,0]==2))|| ((ghost_pos_.X==4 && ghost_pos_.Y==0) && (root_nodes.gameboard[4,0]==1 || root_nodes.gameboard[4,0]==2)))&& Function.GetDeleteEnemyNum()[0]<2){
                        var dir_ = (Direction)Enum.ToObject(typeof(Direction), dir_k);
                        outdic.SI = id_ghost;
                        outdic.AL = Action_Label.Move;
                        outdic.DR = dir_;
                
                return outdic;

                        }
                }
                
            }
            
            var dir = (Direction)Enum.ToObject(typeof(Direction), direction);
            outdic.SI = id_ghost;
            outdic.AL = Action_Label.Move;
            outdic.DR = dir;
            return outdic;
        }

        enum BoardState
        {
            None,
            Ally_blue,
            Ally_red,
            Enemy_blue,
            Enemy_red,
        }

        public class GameBoard
        {

            public List<int[,]> gameboard_overall = new List<int[,]>();
            void add_GameBoard(Function func, List<int> index, List<int> deadnum)
            {
                int[,] gameboard = new int[global.Range_X, global.Range_Y];
                var EnemyPoint = Function.EnemyGhostState();

                //味方(blue)->1 味方(red)->2 敵->3,4 何も無いところ->0で作る
                for (int i = 0; i <= 6; i++)
                {
                    GhostState ghost = Function.MyGhostState(i);
                    if (ghost.GetDoA())
                    {
                        var ghost_pos = ghost.GetPos();
                        
                        if (i < 3)
                        {
                            gameboard[ghost_pos.X, ghost_pos.Y] = (int)BoardState.Ally_blue;
                        }
                        else
                        {
                            gameboard[ghost_pos.X, ghost_pos.Y] = (int)BoardState.Ally_red;
                        }
                    }
                }

                for (int i = 0; i < 6-(deadnum[0] + deadnum[1]); i++)
                {
                    if (index.Contains(i))
                    {
                        gameboard[EnemyPoint[i].X, EnemyPoint[i].Y] = (int)BoardState.Enemy_red;
                    }
                    else
                    {
                        gameboard[EnemyPoint[i].X, EnemyPoint[i].Y] = (int)BoardState.Enemy_blue;
                    }
                }
                gameboard_overall.Add(gameboard);
            }

            //赤、青の全ての場合についての盤面を形成する
            public void make_GameBoard(Function func)
            {
                var deadnum = Function.GetDeleteEnemyNum();
                int sum_dead_num=deadnum[0] + deadnum[1];
                if(deadnum[1]==0){
                for (int i = 0; i < 6-sum_dead_num; i++)
                {
                    for (int u = (i + 1); u < 6-sum_dead_num; u++)
                    {
                        for (int s = (u + 1); s < 6-sum_dead_num; s++)
                        {
                            List<int> index = new List<int>();
                            index.Add(i);
                            index.Add(u);
                            index.Add(s);
                            add_GameBoard(func, index, deadnum);
                        }
                    }
                }
                }
                else if(deadnum[1]==1){
                for (int u = 0; u < 6-sum_dead_num; u++)
                    {
                        for (int s = (u + 1); s < 6-sum_dead_num; s++)
                        {
                            List<int> index = new List<int>();
                            index.Add(u);
                            index.Add(s);
                            add_GameBoard(func, index, deadnum);
                        }
                    }}
                else{
                    for (int s = 0; s < 6-sum_dead_num; s++)
                        {
                            List<int> index = new List<int>();
                            index.Add(s);
                            add_GameBoard(func, index, deadnum);
                        }

                }

            }
        }
    }

    public class MonteCarloTree
    {
        public enum BoardState
        {
            None,
            Ally_blue,
            Ally_red,
            Enemy_blue,
            Enemy_red,
        }
        public float w = 0;
        public int n = 0;
        public int[,] gameboard = new int[global.Range_X, global.Range_Y];
        public List<MonteCarloTree> child = new List<MonteCarloTree>();
        bool make_child = false;
        bool maked_child=false;
        //自分のやつを捜査しているかどうか
        //相手の場合は動かせる駒、評価値が変化する
        public bool is_ownghost = true;
        Random cRandom = new System.Random();
        public Point action=new Point();
        public int dir_node=0;
        public Array directions= Enum.GetValues(typeof(Direction));
        int deep_tree=0;
        public bool is_arrived = false;
        
        public List<Tuple<List<int>,List<Point>>> calced_legal_action_enemy =  new List<Tuple<List<int>,List<Point>>>();
        public List<Tuple<List<int>,List<Point>>> calced_legal_action_ally =  new List<Tuple<List<int>,List<Point>>>();
        public List<int[,]> calced_state_enemy=new List<int[,]>();
        public List<int[,]> calced_state_ally=new List<int[,]>();


        public Point move(Point point, int dir)
        {
            if (dir == (int)Direction.Up)
            {
                return new Point { X = point.X, Y = point.Y - 1 };
            }
            else if (dir == (int)Direction.Right)
            {
                return new Point { X = point.X + 1, Y = point.Y };
            }
            else if (dir == (int)Direction.Left)
            {
                return new Point { X = point.X - 1, Y = point.Y };
            }
            else if (dir == (int)Direction.Down)
            {
                return new Point { X = point.X, Y = point.Y + 1 };
            }
            
            return new Point { X = -1, Y = -1 };
        }

        //行動後の次の行動を返す関数
        public Tuple<List<int>, List<Point>> legal_action(int[,] state,bool is_ownghost)
        {
            List<Point> movechar_point = new List<Point>();
            List<int> movedir = new List<int>();
            if (is_ownghost)
            {
                calced_state_ally.Add(state);
                for (int i = 0; i < global.Range_X; i++)
                {
                    for (int u = 0; u < global.Range_Y; u++)
                    {
                        if (state[i, u] == (int)BoardState.Ally_blue || state[i, u] == (int)BoardState.Ally_red)
                        {
                            Point point = new Point { X = i, Y = u };
                            foreach (int dir in directions)
                            {
                                var dirval = (Direction)Enum.ToObject(typeof(Direction), dir);
                                if (Function.CanDR(point, dirval)){
                                    Point moved_ptr=move(point,dir);
                                    int moved_obj=state[moved_ptr.X,moved_ptr.Y];
                                    if(moved_obj !=(int)BoardState.Ally_blue && moved_obj!=(int)BoardState.Ally_red){
                                    int[,] state_img=next_action(this.gameboard,point,dir);
                                    //if(!islose(state_img,true,this.is_arrived)){
                                    movedir.Add((int)dir);
                                    movechar_point.Add(point);
                                    //}
                                }
                                        }
                                
                            }
                        }
                    }
                }
            calced_legal_action_ally.Add(new Tuple<List<int>, List<Point>>(movedir,movechar_point));
            }

            else
            {
                calced_state_enemy.Add(state);
                for (int i = 0; i < global.Range_X; i++)
                {
                    for (int u = 0; u < global.Range_Y; u++)
                    {
                        if (state[i, u] == (int)BoardState.Enemy_blue || state[i, u] == (int)BoardState.Enemy_red)
                        {
                            Point point = new Point { X = i, Y = u };
                            foreach (int dir in directions)
                            {
                                var dirval = (Direction)Enum.ToObject(typeof(Direction), dir);
                                
                                if (Function.CanDR(point, dirval))
                                {
                                    Point moved_ptr=move(point,dir);
                                    int moved_obj=state[moved_ptr.X,moved_ptr.Y];
                                    if(moved_obj !=(int)BoardState.Enemy_blue && moved_obj!=(int)BoardState.Enemy_red){
                                    movedir.Add(dir);
                                    movechar_point.Add(point);
                                }
                                }
                            }
                        }
                    }
                }
                calced_legal_action_enemy.Add(new Tuple<List<int>, List<Point>>(movedir,movechar_point));
            }

            return new Tuple<List<int>, List<Point>>(movedir, movechar_point);
        }

        //次の状態を返す関数
        public int[,] next_action(int[,] state, Point moveobj, int action)
        {
            Point moved_point = move(moveobj, action);
            int[,] actioned_state= new int[global.Range_X,global.Range_Y];
            Array.Copy(state,actioned_state,state.Length);
            actioned_state[moved_point.X, moved_point.Y] = actioned_state[moveobj.X, moveobj.Y];
            actioned_state[moveobj.X, moveobj.Y] *=(int)BoardState.None;
            return actioned_state;
        }


        public bool judge_is_arrive(int[,] state, bool is_ownghost)
        {
            int blueghost = 0;
            int redghost=0;
            int y=0;
            if (!is_ownghost)
            {
                blueghost = (int)BoardState.Enemy_blue;
                redghost = (int)BoardState.Enemy_red;
            }
            else
            {
                blueghost = (int)BoardState.Ally_blue;
                redghost = (int)BoardState.Ally_red;
                y=global.Range_Y-1;
            }

            if(state[0, y] == blueghost || state[global.Range_X-1, y] == blueghost)
            {
                return true;
            }
            return false;
        }

        //functionは状態遷移後使えないので、基本的に自作(list.count)でやる
        public bool islose(int[,] state, bool is_ownghost, bool is_arrived)
        {
            int redghost = 0;
            int blueghost = 0;
            int myblue=0;
            int y=0;
            if (is_ownghost)
            {
                blueghost = (int)BoardState.Enemy_blue;
                myblue=(int)BoardState.Ally_blue;
                redghost = (int)BoardState.Enemy_red;
            }
            else
            {
                blueghost = (int)BoardState.Ally_blue;
                myblue=(int)BoardState.Enemy_blue;
                redghost = (int)BoardState.Ally_red;
                y=global.Range_Y-1;
            }
            //条件１：赤を3体倒している
            //条件３：自分の青がすべてとられている
            int[] array1dim=state.Cast<int>().ToArray();

            if (!array1dim.Contains(redghost) || !array1dim.Contains(myblue))
            {
                return true;
             }
            //条件２：脱出
            //if (((state[0, y] == blueghost || state[4, y] == blueghost ||(( state[0, y] == redghost || state[4, y] == redghost ) && !is_ownghost)))&& is_arrived)
            if ((state[0, y] == blueghost || state[global.Range_X-1, y] == blueghost )&& is_arrived)
            {
                return true;
            }
            return false;
        }

        bool isend()
        {
            if (Function.NowTurn()+this.deep_tree>=200)
            {
                return true;
            }
            return false;
        }


        //探索の評価の際、子ノードがないとき
        //ランダムにプレイし、勝敗を決定する
        //負けそうな時だと評価値は低くなるし、そうでないときは高くなると
        //勝ち負けに直結するので実装は必要。
        int random_action(int[,] state, bool is_ownghost,int deep_count,bool is_arrived)
        {
            if (islose(state, is_ownghost, is_arrived)  || deep_count+Function.NowTurn()>=200)
            {
                return -1;
            }
            
            if(deep_count > 10){
            return 0;
            }
            
            Tuple<List<int>, List<Point>> legal_actions_r=null;
            //決着がついていなければランダムに動く
            if(is_ownghost && calced_state_ally.Contains(state)){
                legal_actions_r=calced_legal_action_ally[calced_state_ally.IndexOf(state)];
            }
            else if(!is_ownghost && calced_state_enemy.Contains(state)){
                legal_actions_r=calced_legal_action_enemy[calced_state_enemy.IndexOf(state)];
            }
            else{
            legal_actions_r = legal_action(state,is_ownghost);
            }
            int count_actions=legal_actions_r.Item1.Count;
            if(count_actions==0){
            return 0;
            }
            int r = cRandom.Next(count_actions);
            int[,] state_next=next_action(state, legal_actions_r.Item2[r], legal_actions_r.Item1[r]);
            bool is_arrived_r = judge_is_arrive(state_next, is_ownghost);
            bool is_ownghost_r = !is_ownghost;
            return -random_action(state_next, is_ownghost_r,deep_count+1,is_arrived_r);
        }

        public void expand()
        {
            Tuple<List<int>, List<Point>> legal_actions=null;
            //決着がついていなければランダムに動く
            if(this.is_ownghost && calced_state_ally.Contains(this.gameboard)){

                legal_actions=calced_legal_action_ally[calced_state_ally.IndexOf(this.gameboard)];
            }
            else if(!this.is_ownghost && calced_state_enemy.Contains(this.gameboard)){
                legal_actions=calced_legal_action_enemy[calced_state_enemy.IndexOf(this.gameboard)];
            }
            else{
            legal_actions = legal_action(this.gameboard,this.is_ownghost);
            }
            for (int i = 0; i < legal_actions.Item1.Count; i++)
            {
                MonteCarloTree child_node = new MonteCarloTree();
                child_node.gameboard = next_action(this.gameboard, legal_actions.Item2[i], legal_actions.Item1[i]);
                child_node.is_ownghost=!this.is_ownghost;
                child_node.action=legal_actions.Item2[i];
                child_node.dir_node=legal_actions.Item1[i];
                child_node.is_arrived=judge_is_arrive(child_node.gameboard, this.is_ownghost);
                child_node.deep_tree=this.deep_tree+1;
                this.child.Add(child_node);
            }
            this.maked_child=true;
        }

        List<float> ucb1_value(List<MonteCarloTree> child)
        {
            int sum_t_n = 0;
            foreach (MonteCarloTree child_node in child)
            {
                sum_t_n += child_node.n;
            }
            List<float> value_ucb = new List<float>();
            float sum_t_n_log = (float)System.Math.Log(sum_t_n);
            foreach (MonteCarloTree child_node in child)
            {
                value_ucb.Add((child_node.w / child_node.n) + (float)Math.Pow((double)(2 * sum_t_n_log / child_node.n), 0.5));
            }
            return value_ucb;
        }

        MonteCarloTree next_child_node()
        { 
            foreach (MonteCarloTree child_node in child)
            {
                if (child_node.n == 0)
                {
                    return child_node;
                }
            }
            List<float> value_ucb = ucb1_value(this.child);
            if(value_ucb.Count > 0){
            return this.child[value_ucb.IndexOf(value_ucb.Max())];
            }
            else{
            return this.child[0];
            }
        }

        public int evaluate()
        {
            int value = 0;
            bool lose_game=islose(this.gameboard, this.is_ownghost, this.is_arrived);
            if (isend() || lose_game)
            {
                if(lose_game){
                value = -2;}
                this.w += value;
                this.n += 1;
                return value;
            }
            if (!this.maked_child)
            {
                value = random_action(this.gameboard, this.is_ownghost,0,this.is_arrived);
                this.w += value;
                this.n += 1;

                if (this.n == 5)
                {
                    expand();
                }
                return value;
            }
            else
            {
                value = -next_child_node().evaluate();
                this.w += value;
                this.n += 1;
                return value;
            }

        }
    }

}