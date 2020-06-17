using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace geister.taiyo
{

    public abstract class IField
    {
        public const int cTeamNum = 2;
        public const int cFieldHeight = 6;
        public const int cFieldWidth = 5;
        public const int cTeamGhostNum = 6;
        public const int cTotalTurn = 200;


        //下チームの初期座標
        public static readonly TPoint[] cLowerTeamFirstPositions = {
            new TPoint(3,5),new TPoint(2,5),new TPoint(1,5),
            new TPoint(3,4),new TPoint(2,4),new TPoint(1,4)
        };

        //上チームの初期座標
        public static readonly TPoint[] cUpperTeamFirstPositions = {
            new TPoint(1,0),new TPoint(2,0),new TPoint(3,0),
            new TPoint(1,1),new TPoint(2,1),new TPoint(3,1)
        };

        //下側にあるゴール座標
        public static readonly TPoint[] cLowerGoalPositions = {
            new TPoint(0,5), new TPoint(4,5)
        };

        //上側にあるゴール座標
        public static readonly TPoint[] cUpperGoalPositions = {
            new TPoint(0,0), new TPoint(4,0)
        };

        public static bool isInField(int x,int y)
        {
            return x >= 0 && y >= 0 && x < cFieldWidth && y < cFieldHeight;
        }

        public static bool isInField(TPoint p)
        {
            return isInField(p.x, p.y);
        }
        public static TPoint[] GetGoalPositions(int teamId)
        {
            return teamId == 0 ? cUpperGoalPositions : cLowerGoalPositions;
        }

        abstract public int getFraggedEvilNum(int teamId);
        abstract public int getFraggedGoodNum(int teamId);

        public int getRemainEvilNum(int teamId)
        {
            return cTeamGhostNum / 2 - getFraggedEvilNum(1 - teamId);
        }
        public int getRemainGoodNum(int teamId)
        {
            return cTeamGhostNum / 2 - getFraggedGoodNum(1 - teamId);
        }
        public IGhost getGhost(TPoint p)
        {
            return getGhost(p.x, p.y);
        }

        abstract public IGhost getGhost(int x, int y);

        static public bool isInGoal(int teamId, TPoint p)
        {
            TPoint[] goals = (teamId == 0) ? cUpperGoalPositions : cLowerGoalPositions;
            return goals.Contains(p);
        }

        virtual public bool canMove(int teamId, TPoint from, TPoint to)
        {
            if (TPoint.manhattan(from, to) != 1)
            {
                return false;
            }
            IGhost tar = getGhost(from);
            if (tar == null || tar.getTeamId() != teamId)
            {
                return false;
            }
            /*
            if (isInGoal(teamId, to))
            {
                if (tar.getType() == GhostType.Good)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            */
            if (!isInField(to))
            {
                return false;
            }
            IGhost next = getGhost(to);
            if (next != null && next.getTeamId() == teamId)
            {
                return false;
            }
            return true;
        }

        /**
		* 0,1 -> 勝利チーム
		* 2 -> 引き分け
		* */
        static public int winnerByZeroGhost(int goodGhostCount0, int evilGhostCount0, int goodGhostCount1, int evilGhostCount1)
        {
            //ゲーム終了、公式ルールに従って勝ち負け判定
            if (goodGhostCount0 == 0) 
            {
                return 1;
            }
            else if (goodGhostCount1 == 0)
            {
                return 0;
            }
            if (evilGhostCount0 == 0)
            {
                return 0;
            }
            else if(evilGhostCount1==0)
            {
                return 1;
            }

            return -1;
        }
        static public int winnerByGhostCount(int goodGhostCount0, int evilGhostCount0, int goodGhostCount1, int evilGhostCount1)
        {
            //ゲーム終了、公式ルールに従って勝ち負け判定
            int livingGhostScore0 = goodGhostCount0 - evilGhostCount0;
            int livingGhostScore1 = goodGhostCount1 - evilGhostCount1;
            if (livingGhostScore0 > livingGhostScore1)
            {
                return 0;
            }
            else if (livingGhostScore0 < livingGhostScore1)
            {
                return 1;
            }
            return -1;
        }


        override public string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < cFieldHeight; y++)
            {
                for (int x = 0; x < cFieldWidth; x++)
                {
                    IGhost ghost = getGhost(x, y);
                    if (ghost == null)
                    {
                        sb.Append("_");
                    }
                    else if (ghost.isEvil())
                    {
                        sb.Append(ghost.getTeamId() == 0 ? "E" : "e");
                    }
                    else if (ghost.isGood())
                    {
                        sb.Append(ghost.getTeamId() == 0 ? "G" : "g");
                    }
                    else
                    {
                        sb.Append(ghost.getTeamId() == 0 ? "U" : "u");
                    }
                }
                sb.Append("\n");
            }
            sb.Append("team0 fragged:");
            for (int i = 0; i < getFraggedGoodNum(0); i++) sb.Append("g");
            for (int i = 0; i < getFraggedEvilNum(0); i++) sb.Append("e");
            sb.Append("\n");
            sb.Append("team1 fragged:");
            for (int i = 0; i < getFraggedGoodNum(1); i++) sb.Append("G");
            for (int i = 0; i < getFraggedEvilNum(1); i++) sb.Append("E");
            sb.Append("\n");


            return sb.ToString();
        }
    }

    /**
     * 
     */
    class PlayersField : IField
    {
        private readonly MasterField mMasterField;
        private int mPlayerId;  //この情報を受け取るプレイヤーのId

        public PlayersField(MasterField field, int playerId)
        {
            this.mMasterField = field;
            this.mPlayerId = playerId;
        }

        override public IGhost getGhost(int x, int y)
        {
            IGhost ghost = mMasterField.getGhost(x, y);
            if (ghost != null)
            {
                return new PlayersGhost(ghost, mPlayerId);
            }
            else
            {
                return null;
            }
        }

        override public int getFraggedEvilNum(int teamId)
        {
            return mMasterField.getFraggedEvilNum(teamId);
        }

        override public int getFraggedGoodNum(int teamId)
        {
            return mMasterField.getFraggedGoodNum(teamId);
        }
    }

    /**
     *  
     * */
    class MasterField : IField
    {

        private Ghost[] mGrid = new Ghost[cFieldWidth * cFieldHeight];

        //チームごとのevil/good獲得数
        private int[] fragedEvilNum = new int[cTeamNum];
        private int[] fragedGoodNum = new int[cTeamNum];

        private CauseOfResult mDetail = CauseOfResult.None;

        public CauseOfResult getDetail()
        {
            return mDetail;
        }

        override public int getFraggedEvilNum(int teamId)
        {
            return fragedEvilNum[teamId];
        }
        override public int getFraggedGoodNum(int teamId)
        {
            return fragedGoodNum[teamId];
        }

        public void setup(GhostType[] team0Posisions, GhostType[] team1Possisions)
        {
            mDetail = CauseOfResult.None;
            //MasterFieldでは上チームを1,下チームを0とする

            for (int teamId = 0; teamId < 2; teamId++)
            {
                TPoint[] firstPos = (teamId == 0) ? cLowerTeamFirstPositions : cUpperTeamFirstPositions;
                GhostType[] teamPos = (teamId == 0) ? team0Posisions : team1Possisions;

                for (int id = 0; id < cTeamGhostNum; id++)
                {
                    setGhost(firstPos[id], new Ghost(teamId, teamPos[id]));
                }
            }
        }

        private void setGhost(TPoint p, Ghost ghost)
        {
            mGrid[p.y * cFieldWidth + p.x] = ghost;
        }

        private Ghost getGhostImpl(int x, int y)
        {
            return mGrid[y * cFieldWidth + x];
        }

        private Ghost getGhostImpl(TPoint p)
        {
            return getGhostImpl(p.x, p.y);
        }


        override public IGhost getGhost(int x, int y)
        {
            return getGhostImpl(x, y);
        }

        /**
         * 勝敗が決した場合は勝ったチームのidを返す
         * そうでない場合は-1を返す
         */
        public int move(int teamId, TPoint from, TPoint to)
        {
            Debug.Assert(TPoint.manhattan(from, to) == 1);
            Ghost tar = getGhostImpl(from);
            Debug.Assert(tar != null);
            Debug.Assert(tar.getTeamId() == teamId);
            Debug.Assert(isInField(to) || (tar.getType() == GhostType.Good && isInGoal(teamId, to)));
            Ghost next = getGhostImpl(to);
            if (next != null)
            {
                Debug.Assert(next.getTeamId() != teamId);
                if (next.isEvil())
                {
                    fragedEvilNum[teamId]++;
                }
                else
                {
                    fragedGoodNum[teamId]++;
                }
            }
            setGhost(from, null);
            setGhost(to, tar);

            if (getFraggedEvilNum(teamId) >= cTeamGhostNum/2)
            {
                //悪ゴーストを全部取った。負け
                mDetail = CauseOfResult.FragAllEvil;
                return 1 - teamId;
            }
            else if (getFraggedGoodNum(teamId) >= cTeamGhostNum / 2)
            {
                //善ゴーストを全部取った。勝ち。
                mDetail = CauseOfResult.FragAllGood;
                return teamId;
            }
            //相手側の善ゴーストがゴールしてたら相手の勝ち
            foreach(TPoint p in IField.GetGoalPositions(1 - teamId))
            {
                var g = getGhost(p);
                if(g == null) { continue; }
                if(g.getType() == GhostType.Good && g.getTeamId() == 1-teamId)
                {
                    mDetail = CauseOfResult.Goal;
                    return 1 - teamId;
                }
            }

            //まだ決着がついていない
            return -1;
        }

        private int winnerByManhattan()
        {
            int score0 = 0;
            int score1 = 0;
            for (int y = 0; y < IField.cFieldHeight; y++)
            {
                for(int x=0;x<IField.cFieldWidth; x++)
                {
                    IGhost g = getGhost(x, y);
                    if (g == null) { continue; }
                    if(g.getType() == GhostType.Good)
                    {
                        int team = g.getTeamId();
                        if (team == 0)
                        {
                            score0 += Math.Min(TPoint.manhattan(IField.GetGoalPositions(0)[0], x, y),
                                TPoint.manhattan(IField.GetGoalPositions(0)[1], x, y));
                        }
                        else
                        {
                            score0 += Math.Min(TPoint.manhattan(IField.GetGoalPositions(1)[0], x, y),
                                TPoint.manhattan(IField.GetGoalPositions(1)[1], x, y));
                        }
                    }
                }
            }
            return score0 < score1 ? 0 :
                score1 < score0 ? 1 : 2;
        }

        public int calcJudgeWinner()
        {
            //判定勝ちを調べる
            mDetail = CauseOfResult.Judge;
            int winByCount = IField.winnerByGhostCount(getRemainGoodNum(0), getRemainEvilNum(0), getRemainGoodNum(1), getRemainEvilNum(1));
            if(winByCount >= 0)
            {
                return winByCount;
            }
            //マンハッタン勝負
            return winnerByManhattan();
        }
    }
}
