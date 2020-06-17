using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Procon2020 {
  public partial class Form1 : Form {
    public Form1() { }
    public Form1(AbstPlayer p1, AbstPlayer p2) {
      InitializeComponent();
      Text = "Geister";
      StartPosition = FormStartPosition.CenterScreen;
      ClientSize = new Size(950, 700); //エリアが500*600,上下50左右100が余白，右300でその他情報
      BackColor = Color.MediumSlateBlue;//
      DoubleBuffered = true;

      Init(p1, p2);

      System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
      timer.Interval = 1;
      timer.Tick += new EventHandler(Test_Tick);
      timer.Start();
    }

    private int winflag, stopflag, pattern;
    private void Init(AbstPlayer p1, AbstPlayer p2) {
      global.Turn = 0;
            global.Eflag_p1 = new List<int>() { -1,-1,-1};
            global.Eflag_p2 = new List<int>() { -1, -1, -1 };
            global.currentplayer = Player.P2;
      winflag = 0;
      stopflag = 0;
            pattern = 0;
      Bpos = new Point(-1, -1);

      global.log = new Log[global.FinalTurn];
      for (int i = 0; i < global.FinalTurn; i++) { global.log[i] = new Log(); }

      P1 = p1;
      P2 = p2;
            //初期位置設定する？ P1.pos1_x, P1.pos1_y
      S1 = new GhostState[global.GhostNum]{new GhostState(Player.P1, 0, P1.Init_pos().G0), new GhostState(Player.P1, 1,P1.Init_pos().G1),
         new GhostState(Player.P1, 2, P1.Init_pos().G2),new GhostState(Player.P1, 3, P1.Init_pos().G3),
          new GhostState(Player.P1, 4,P1.Init_pos().G4),new GhostState(Player.P1, 5, P1.Init_pos().G5) };//
      S2 = new GhostState[global.GhostNum]{new GhostState(Player.P2, 0, (P2.Init_pos().G0)), new GhostState(Player.P2, 1,P2.Init_pos().G1),
         new GhostState(Player.P2, 2, P2.Init_pos().G2),new GhostState(Player.P2, 3, P2.Init_pos().G3),
          new GhostState(Player.P2, 4,P2.Init_pos().G4),new GhostState(Player.P2, 5, P2.Init_pos().G5) };//
    }

    private double Score_Pos1(Point GhostPos) {
            //return 6 - Math.Abs(GhostPos.X - 3) - Math.Abs(GhostPos.Y - 3);//
            return GhostPos.Y + Math.Abs(GhostPos.X - 2);//
    }
    private double Score_Pos2(Point GhostPos)
    {
        //return 6 - Math.Abs(GhostPos.X - 3) - Math.Abs(GhostPos.Y - 3);//
        return 5-GhostPos.Y + Math.Abs(GhostPos.X - 2);//
    }

    private void Test_Tick(object sender, EventArgs e) {
            Dicision dodic = new Dicision();

            if (stopflag == 0)
            {
                var thread = new Thread(new ThreadStart(() => {
                    if (global.currentplayer == Player.P1)
                    {
                        dodic = P1.Dicision();
                    }
                    else
                    {
                        dodic = P2.Dicision();
                    }
                    ActionDic(dodic);
                }));
                thread.Start();
                Thread.Sleep(250);
                if (thread.IsAlive) { thread.Abort(); }
                //for (1~3)
                //P1[i].pos_y=5 && x= 0or4


                //ghostnum 0~3 青(善)
                //        4~6 赤(悪)
                //Search();
                //生存1 死亡0 脱出状態2 
                //青の生存
                if (!S1[0].flag && !S1[1].flag && !S1[2].flag) {
          winflag = 2;
                pattern = 1;
        }
        if (!S2[0].flag && !S2[1].flag && !S2[2].flag) {
          winflag = 1;
                pattern = 2;
        }

        if (!S1[3].flag && !S1[4].flag && !S1[5].flag)
        {
          winflag = 1;
                pattern = 3;
        }
        if (!S2[3].flag && !S2[4].flag && !S2[5].flag)
        {
          winflag = 2;
                pattern = 4;
        }
            for (int i = 0; i < 3; i++)
            {
                    if (global.currentplayer == Player.P1)
                    {
                        if (S1[i].flag)
                        {
                            if (((S1[i].pos.X == 0) || (S1[i].pos.X == 4))&& (S1[i].pos.Y == 5))
                            {
                                    if (global.Eflag_p1[i] == 1)
                                    {
                                        winflag = 1;
                                        pattern = 5;
                                    }
                                    else
                                    {
                                        global.Eflag_p1[i] = 1;
                                    }

                            }
                            else
                            {
                                global.Eflag_p1[i] = -1;
                            }
                        
                        }
                    }
                    else
                    {
                        if (S2[i].flag)
                        {
                            if (((YourPos_X(Player.P2, S2[i].pos.X) == 0) || (YourPos_X(Player.P2, S2[i].pos.X) == 4))&& (YourPos_Y(Player.P2, S2[i].pos.Y) == 0))
                            {
                                    if (global.Eflag_p2[i] == 1)
                                    {
                                        winflag = 2;
                                        pattern = 6;
                                    }
                                    else
                                    {
                                        global.Eflag_p2[i] = 1;
                                    }

                                
                            }
                            else
                            {
                                global.Eflag_p2[i] = -1;
                            }
                        }
                    }
            }
            
            global.Turn++;
        if (global.Turn >= global.FinalTurn) {
          winflag = 0;
          double[] keisuu = new double[6] { 1, 1, 1, -1, -1, -1 };
          double score1 = 0, score2 = 0, scoreP1 = 0, scoreP2 = 0;
          for (int i = 0; i < global.GhostNum; i++) {
            if (S1[i].flag) { score1 += keisuu[i]; scoreP1 += Score_Pos1(S1[i].pos); }
            if (S2[i].flag) { score2 += keisuu[i]; scoreP2 += Score_Pos1(S2[i].pos); }
          }
          if (score1 > score2) { winflag += 1; }
          else if (score1 < score2) { winflag += 2; }
          else {
            if (scoreP1 >= scoreP2) { winflag += 1; }
            if (scoreP1 <= scoreP2) { winflag += 2; }
          }
                stopflag = 1;
            }
            global.currentplayer = (Player)(1 - global.currentplayer);
                if (winflag >= 1) { stopflag = 1;
                    if (global.Turn < global.FinalTurn)
                    {
                        global.Turn -= 1;
                        global.currentplayer = (Player)(1 - global.currentplayer);
                    }
                }




                Invalidate();
      }
    }

    private Image[] IGhost1, IGhost2;
    private Image IGhost_w, IGhost_r, IGhost_b, IGhost_r2, IGhost_b2;
    protected override void OnPaint(PaintEventArgs e) {
      base.OnPaint(e);

      Pen pen = new Pen(Color.White, 1);
      pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
      Point[] pt1 = { new Point(150, 50), new Point(250, 50), new Point(250, 650), new Point(150, 650) };
      Point[] pt2 = { new Point(350, 50), new Point(450, 50), new Point(450, 650), new Point(350, 650) };
      Point[] pt3 = { new Point(50, 150), new Point(550, 150), new Point(550, 250), new Point(50, 250) };
      Point[] pt4 = { new Point(50, 350), new Point(550, 350), new Point(550, 450), new Point(50, 450) };
      Point[] pt5 = { new Point(50, 550), new Point(550, 550), new Point(550, 650), new Point(50, 650) };
      e.Graphics.DrawPolygon(pen, pt1);
      e.Graphics.DrawPolygon(pen, pt2);
      e.Graphics.DrawPolygon(pen, pt3);
      e.Graphics.DrawPolygon(pen, pt4);
      e.Graphics.DrawPolygon(pen, pt5);
            Point[] ad1 = { new Point(600, 50), new Point(900, 50), new Point(900, 150), new Point(600, 150) };
            Point[] ad2 = { new Point(700, 50), new Point(800, 50), new Point(800, 250), new Point(700, 250) };
            Point[] ad3 = { new Point(600, 450), new Point(900, 450), new Point(900, 550), new Point(600, 550) };
            Point[] ad4 = { new Point(700, 450), new Point(800, 450), new Point(800, 650), new Point(700, 650) };
            e.Graphics.DrawPolygon(pen, ad1);
            e.Graphics.DrawPolygon(pen, ad2);
            e.Graphics.DrawPolygon(pen, ad3);
            e.Graphics.DrawPolygon(pen, ad4);
            pen = new Pen(Color.Silver, 3);
      Point[] pts = { new Point(50, 50), new Point(550, 50), new Point(550, 650), new Point(50, 650) };
            Point[] ads1 = { new Point(600, 50), new Point(900, 50), new Point(900, 250), new Point(600, 250) };
            Point[] ads2 = { new Point(600, 450), new Point(900, 450), new Point(900, 650), new Point(600, 650) };
            e.Graphics.DrawPolygon(pen, pts);
            e.Graphics.DrawPolygon(pen, ads1);
            e.Graphics.DrawPolygon(pen, ads2);
            SolidBrush brush = new SolidBrush(Color.Yellow);
            e.Graphics.FillRectangle(brush, 50, 50, 100, 100);
            e.Graphics.FillRectangle(brush, 450, 50, 100, 100);
            e.Graphics.FillRectangle(brush, 50, 550, 100, 100);
            e.Graphics.FillRectangle(brush, 450, 550, 100, 100);

            brush = new SolidBrush(Color.Black);
            Font font1 = new Font("Arial", 30);
      e.Graphics.DrawString("Turn" + global.Turn + "：" + global.currentplayer, font1, brush, 600, 10);
      e.Graphics.DrawString("Player1:"+P1.name, font1, brush, 600, 250);
      e.Graphics.DrawString("Player2:"+P2.name, font1, brush, 600, 400);
      switch (winflag) {
        case 1:
          e.Graphics.DrawString(P1.name+" Win!", font1, brush, 600, 300); break;
        case 2:
          e.Graphics.DrawString(P2.name+" Win!", font1, brush, 600, 300); break;
        case 3:
          e.Graphics.DrawString("Draw!", font1, brush, 600, 300); break;
      }

      
      if (IGhost_w == null) {
        IGhost_w = Image.FromFile(@".\image\ghost_w.png");
        IGhost_b = Image.FromFile(@".\image\ghost_b.png");
        IGhost_r = Image.FromFile(@".\image\ghost_r.png");
                IGhost_r2 = Image.FromFile(@".\image\ghost_r2.png");
                IGhost_b2 = Image.FromFile(@".\image\ghost_b2.png");
                IGhost1 = new Image[global.GhostNum];
        IGhost2 = new Image[global.GhostNum];
        for(int i = 0; i<3; i++){
          IGhost1[i] = IGhost_b;
          IGhost2[i] = IGhost_w;
                    //IGhost2[i] = IGhost_b2;
                }
                for (int i = 3; i < 6; i++){
          IGhost1[i] = IGhost_r;
          IGhost2[i] = IGhost_w;
                    //IGhost2[i] = IGhost_r2;
                }
      }
      for (int i = 0; i < global.GhostNum; i++) {
        if (S1[i].flag) {
                    int tx = YourPos_X(Player.P1, S1[i].pos.X), ty = YourPos_Y(Player.P1, S1[i].pos.Y);
                    e.Graphics.DrawImage(IGhost1[i], tx * 100 + 55, ty * 100 + 55, 90, 90);
          
        }
        if (S2[i].flag) {
                    int tx = YourPos_X(Player.P2, S2[i].pos.X), ty = YourPos_Y(Player.P2, S2[i].pos.Y);
                    e.Graphics.DrawImage(IGhost2[i], tx * 100 + 55, ty * 100 + 55, 90, 90);
          
        }
      }
      int S1_b=0, S1_r=0, S2_b=0, S2_r = 0;
      for(int i = 0; i < 3; i++){
        if (!S1[i].flag){S1_b += 1;}
        if (!S2[i].flag){S2_b += 1;}
      }
      for (int i = 3; i < 6; i++){
        if (!S1[i].flag){S1_r += 1;}
        if (!S2[i].flag){S2_r += 1;}
      }
            switch (S1_b)
            {
                case 1:
                    e.Graphics.DrawImage(IGhost_b, 605, 455, 90, 90);
                    break;
                case 2:
                    e.Graphics.DrawImage(IGhost_b, 605, 455, 90, 90);
                    e.Graphics.DrawImage(IGhost_b, 705, 455, 90, 90);
                    break;
                case 3:
                    e.Graphics.DrawImage(IGhost_b, 605, 455, 90, 90);
                    e.Graphics.DrawImage(IGhost_b, 705, 455, 90, 90);
                    e.Graphics.DrawImage(IGhost_b, 805, 455, 90, 90);
                    break;
            }
            switch (S1_r){
                case 1:
                    e.Graphics.DrawImage(IGhost_r, 605, 555, 90, 90);
                    break;
                case 2:
                    e.Graphics.DrawImage(IGhost_r, 605, 555, 90, 90);
                    e.Graphics.DrawImage(IGhost_r, 705, 555, 90, 90);
                    break;
                case 3:
                    e.Graphics.DrawImage(IGhost_r, 605, 555, 90, 90);
                    e.Graphics.DrawImage(IGhost_r, 705, 555, 90, 90);
                    e.Graphics.DrawImage(IGhost_r, 805, 555, 90, 90);
                    break;
            }
            switch (S2_b){
                case 1:
                    e.Graphics.DrawImage(IGhost_b, 605, 55, 90, 90);
                    break;
                case 2:
                    e.Graphics.DrawImage(IGhost_b, 605, 55, 90, 90);
                    e.Graphics.DrawImage(IGhost_b, 705, 55, 90, 90);
                    break;
                case 3:
                    e.Graphics.DrawImage(IGhost_b, 605, 55, 90, 90);
                    e.Graphics.DrawImage(IGhost_b, 705, 55, 90, 90);
                    e.Graphics.DrawImage(IGhost_b, 805, 55, 90, 90);
                    break;
            }
            switch (S2_r){
                case 1:
                    e.Graphics.DrawImage(IGhost_r, 605, 155, 90, 90);
                    break;
                case 2:
                    e.Graphics.DrawImage(IGhost_r, 605, 155, 90, 90);
                    e.Graphics.DrawImage(IGhost_r, 705, 155, 90, 90);
                    break;
                case 3:
                    e.Graphics.DrawImage(IGhost_r, 605, 155, 90, 90);
                    e.Graphics.DrawImage(IGhost_r, 705, 155, 90, 90);
                    e.Graphics.DrawImage(IGhost_r, 805, 155, 90, 90);
                    break;
            }
        }
    protected override void OnResize(EventArgs e) { base.OnResize(e); }

  }
}
