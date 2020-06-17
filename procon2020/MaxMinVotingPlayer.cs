using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace geister.taiyo
{
    class Permutation
    {
        //Evil,Evil...Goodの順に並んでいるリストを Good,Good...Evilの順に並び替える
        //その過程ですべての順列を列挙する
        public static bool NextPermutation(List<GhostType> dst)
        {
            for (int i = dst.Count - 1; i >= 1; i--)
            {
                GhostType left = dst[i - 1];
                GhostType right = dst[i];
                if (left == GhostType.Evil && right == GhostType.Good)
                {
                    dst[i - 1] = GhostType.Good;
                    dst[i] = GhostType.Evil;
                    return true;
                }
            }
            return false;
        }
    }

    public class MaxMinBitField : IField
    {
        // 7x7の門番付きビットフィールドとして扱う
        private static List<Move> sMovesBuff = new List<Move>(6*4);    // 高速化のための魔改造的バッファ
        public static List<Move> CreateMoves(MaxMinBitField field, int  teamId)
        {
            sMovesBuff.Clear();
            ulong myEvilGhostBit = field.mMyEvilGhostBit;
            ulong myGoodGhostBit = field.mMyGoodGhostBit;
            ulong myGhostBit = myEvilGhostBit | myGoodGhostBit;

            ulong targetGhostBit = (teamId == field.mMyPlayerId) ? myGhostBit : field.mEnemyGhostBit;
            ulong unpassable = targetGhostBit | cWall;

            //
            ulong left = (targetGhostBit >> 1) & ~unpassable;
            ulong right = (targetGhostBit << 1) & ~unpassable;
            ulong up = (targetGhostBit >> 7) & ~unpassable;
            ulong down = (targetGhostBit << 7) & ~unpassable;
            //いずれかに遷移できたものを残す
            ulong movable = (left << 1) | (right >> 1) | (up << 7) | (down >> 7);

            while (targetGhostBit != 0) 
            {
                var target = Bit.rightOneBits(targetGhostBit);
                int index = Bit.GetNumberOfTrailingZeros(target);
                int x = index % 7 - 1;
                int y = index / 7 - 1;

                foreach (Way w in WayUtil.way4)
                {
                    var toBit =
                        w == Way.Left ? (target >> 1) :
                        w == Way.Right ? (target << 1) :
                        w == Way.Up ? (target >> 7) :
                        target << 7;

                    if ((toBit & unpassable) != 0)
                    {
                        continue;
                    }

                    sMovesBuff.Add(new Move(new TPoint(x, y), w));
                }
                targetGhostBit &= ~target;
            }
            return sMovesBuff;
        }

        public double[] updateByEnemyMove(Move m)
        {
            double[] ret = new double[] { -1, 0 };   //処理の都合上idx1に0を入れておく
            ulong target = Mask(m.pos);
            ulong toBit = Mask(m.toPos());

            mEnemyGhostBit &= ~target;
            mEnemyGhostBit |= toBit;

            if ((mMyEvilGhostBit & toBit) != 0)
            {
                //Evilゴーストが取られた
                mMyEvilGhostBit &= ~toBit;
                if (mMyEvilGhostBit == 0)
                {
                    ret[0] = mMyPlayerId;
                    ret[1] = 1.0;
                    return ret;
                }
            }
            else if ((mMyGoodGhostBit & toBit) != 0)
            {
                mMyGoodGhostBit &= ~toBit;
                if (mMyGoodGhostBit == 0)
                {
                    ret[0] = 1 - mMyPlayerId;
                    ret[1] = 1.0;
                    return ret;
                }
            }

            //この時点で自分のゴーストがのゴールマスになにかいるなら、決着がついた
            if ((mMyGoodGhostBit & cGoal[mMyPlayerId]) != 0)
            {
                ret[0] = mMyPlayerId;
                ret[1] = 1.0;
                return ret;
            }

            mNextPlayerId = 1 - mNextPlayerId;

            mTurn++;

            if (mTurn <= IField.cTotalTurn)
            {
                //まだ決着がついてない
                return ret;
            }

            //全ターン終了、判定勝負

            int myGhostScore = Bit.bitCount(mMyGoodGhostBit) - Bit.bitCount(mMyEvilGhostBit);
            int enemyGhostScore = mEnemyGoodNum - mEnemyEvilNum;

            if (myGhostScore > enemyGhostScore)
            {
                ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * mMyPlayerId;
                ret[1] = 1.0;
                return ret;
            }
            if (myGhostScore < enemyGhostScore)
            {
                ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * (1 - mMyPlayerId);
                ret[1] = 1.0;
                return ret;
            }
            //距離勝負、こればかりはコストが掛かる...?
            int myManScore = myGoodManhattanScore();
            double enemyManScore = enemyManhattanScore();

            //enemyManScoreとの単純な比較ではないはず...だが、よくわからないのでとりあえずこれで
            if (myManScore < enemyManScore)
            {
                ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * mMyPlayerId;
                ret[1] = 1.0;
                return ret;
            }
            if (myManScore > enemyManScore)
            {
                ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * (1 - mMyPlayerId);
                ret[1] = 1.0;
                return ret;
            }

            ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * (0.5);
            ret[1] = 1.0;
            return ret;
        }

        //idx0に勝者を意味する0~1の値を
        //idx1にその確率0~1.0を入れる
        //更新結果は決着がつかなかった場合を入れる(決着がつかないときの遷移先はたかだか1パターンのはず)
        //idx1が1.0のときは決着がつかない未来がないので更新内容は不定
        public double[] updateByMyMove(Move m)
        {
            double[] ret = new double[] { -1, 0 };   //処理の都合上idx1に0を入れておく
            ulong target = Mask(m.pos);
            ulong toBit = Mask(m.toPos());

            if ((mMyEvilGhostBit & target) != 0)
            {
                //操作対象はEvilゴーストだった
                mMyEvilGhostBit &= ~target;
                mMyEvilGhostBit |= toBit;
            }
            else
            {
                mMyGoodGhostBit &= ~target;
                mMyGoodGhostBit |= toBit;
            }

            if ((mEnemyGhostBit & toBit) != 0)
            {
                //正体不明だけど何か取った
                if (mEnemyEvilNum == 1 && mEnemyGoodNum == 1)
                {
                    //決着は絶対につく。結果は半々。
                    ret[0] = 0.5;
                    ret[1] = 1.0;
                    return ret;
                }
                else if (mEnemyEvilNum == 1)
                {
                    //それが最後のEvilなら負ける可能性がある
                    ret[0] = 1 - mMyPlayerId;
                    ret[1] = mEnemyEvilNum / (double)(mEnemyEvilNum + mEnemyGoodNum);

                    //更新内容は決着がつかなかった未来なので、今とったのはGoodだったということにする
                    mEnemyGoodNum--;
                }
                else if (mEnemyGoodNum == 1)
                {
                    //それが最後のGoodなら勝つ可能性がある
                    ret[0] = mMyPlayerId;
                    ret[1] = mEnemyGoodNum / (double)(mEnemyEvilNum + mEnemyGoodNum);

                    //更新内容は決着がつかなかった未来なので、今とったのはEvilだったということにする
                    mEnemyEvilNum--;
                }
                mEnemyGhostBit &= ~toBit;
            }

            //この時点で相手のゴールマスになにかいるなら、決着がついた可能性がある
            if ((mEnemyGhostBit & cGoal[1 - mMyPlayerId]) != 0)
            {
                //Evilが脱出マスに入る作戦が考えにくいなら確定負けにしてもいい気がするが...
                //新しい決着がつく確率は、(すでに決着がついている確率) + (1-すでに決着がついている確率) * 新たに決着がついた確率
                //その時の勝敗期待値は...
                double goodProb = mEnemyGoodNum / (double)(mEnemyEvilNum + mEnemyGoodNum);
                double v = ret[0] * ret[1] + ((1.0 - ret[1]) * goodProb) * (1 - mMyPlayerId);
                double prob = ret[1] + (1.0 - ret[1]) * goodProb;
                ret[0] = v;
                ret[1] = prob;

                //決着がつかなかった未来として、今ゴールマスにいるGhostはEvilに確定することになるが
                //それを保持する方法がない...のでその処理は一旦省略
            }
            mNextPlayerId = 1 - mNextPlayerId;

            mTurn++;

            if (mTurn <= IField.cTotalTurn)
            {
                //まだ決着がついてない
                return ret;
            }

            //前ターン終了、判定勝負

            int myGhostScore = Bit.bitCount(mMyGoodGhostBit) - Bit.bitCount(mMyEvilGhostBit);
            int enemyGhostScore = mEnemyGoodNum - mEnemyEvilNum;

            if (myGhostScore > enemyGhostScore)
            {
                ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * mMyPlayerId;
                ret[1] = 1.0;
                return ret;
            }
            if (myGhostScore < enemyGhostScore)
            {
                ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * (1 - mMyPlayerId);
                ret[1] = 1.0;
                return ret;
            }
            //距離勝負、こればかりはコストが掛かる...?
            int myManScore = myGoodManhattanScore();
            double enemyManScore = enemyManhattanScore();

            //enemyManScoreとの単純な比較ではないはず...だが、よくわからないのでとりあえずこれで
            if (myManScore < enemyManScore)
            {
                ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * mMyPlayerId;
                ret[1] = 1.0;
                return ret;
            }
            if (myManScore > enemyManScore)
            {
                ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * (1 - mMyPlayerId);
                ret[1] = 1.0;
                return ret;
            }

            ret[0] = ret[0] * ret[1] + (1.0 - ret[1]) * (0.5);
            ret[1] = 1.0;
            return ret;
        }

        static private int[,] sManhattanScore = null;
        static private void initManhattanScore()
        {
            if (sManhattanScore != null)
            {
                return;
            }
            sManhattanScore = new int[2, (IField.cFieldHeight+2)*(IField.cFieldWidth+2)];
            for (int t = 0; t < 2; t++)
            {
                for (int y = 0; y < IField.cFieldHeight; y++)
                {
                    for (int x = 0; x < IField.cFieldWidth; x++)
                    {
                        sManhattanScore[t, Index(x, y)] = Math.Min(
                            TPoint.manhattan(IField.GetGoalPositions(t)[0], x, y),
                            TPoint.manhattan(IField.GetGoalPositions(t)[1], x, y));
                    }
                }
            }
        }

        public int myGoodManhattanScore()
        {
            return manhattanScore(mMyGoodGhostBit);
        }
        public int myEvilManhattanScore()
        {
            return manhattanScore(mMyEvilGhostBit);
        }
        private int manhattanScore(ulong bit)
        {
            initManhattanScore();
            int score = 0;
            while (bit != 0)
            {
                var index = Bit.GetNumberOfTrailingZeros(bit);
                score += sManhattanScore[mMyPlayerId, index];
                bit &= (bit - 1); //一番右端の1を0にしている
            }
            return score;
        }
        public int enemyManhattanScoreAllColor()
        {
            int score = manhattanScore(mEnemyGhostBit);
            return score;
        }
        public double enemyManhattanScore()
        {
            return enemyManhattanScoreAllColor() * mEnemyGoodNum / (double)(mEnemyGoodNum + mEnemyEvilNum);
        }


        private MaxMinBitField() { }

        public MaxMinBitField(IField field, int myTeamId, int turn)
        {
            mMyPlayerId = myTeamId;
            mTurn = turn;
            mNextPlayerId = myTeamId;


            for (int y = 0; y < IField.cFieldHeight; y++)
            {
                for (int x = 0; x < IField.cFieldWidth; x++)
                {
                    IGhost g = field.getGhost(x, y);
                    if (g == null)
                    {
                        continue;
                    }
                    if (g.getTeamId() == myTeamId)
                    {
                        if (g.getType() == GhostType.Evil)
                        {
                            mMyEvilGhostBit |= Mask(x, y);
                        }
                        else
                        {
                            mMyGoodGhostBit |= Mask(x, y);
                        }
                    }
                    else
                    {
                        mEnemyGhostBit |= Mask(x, y);
                    }
                }
            }
            mEnemyEvilNum = field.getRemainEvilNum(1 - mMyPlayerId);
            mEnemyGoodNum = field.getRemainGoodNum(1 - mMyPlayerId);
        }


        public MaxMinBitField(MaxMinBitField other)
        {
            mMyGoodGhostBit = other.mMyGoodGhostBit;
            mMyEvilGhostBit = other.mMyEvilGhostBit;
            mEnemyGhostBit = other.mEnemyGhostBit;
            mEnemyEvilNum = other.mEnemyEvilNum;
            mEnemyGoodNum = other.mEnemyGoodNum;
            mMyPlayerId = other.mMyPlayerId;

            mTurn = other.mTurn;
            mNextPlayerId = other.mNextPlayerId;
        }

        public override int getFraggedEvilNum(int teamId)
        {
            return cTeamGhostNum / 2 -
                ((teamId == mMyPlayerId) ? mEnemyEvilNum
                : countMyEvil()
                );
        }

        public override int getFraggedGoodNum(int teamId)
        {
            return cTeamGhostNum / 2 -
                ((teamId == mMyPlayerId) ? mEnemyGoodNum
                : countMyGood()
                );
        }

        public int countMyGood()
        {
            return Bit.bitCount(mMyGoodGhostBit);
        }
        public int countMyEvil()
        {
            return Bit.bitCount(mMyEvilGhostBit);
        }
        public int countEnemy()
        {
            return mEnemyGoodNum + mEnemyEvilNum;
        }
        public int getEnemyGood() { return mEnemyGoodNum; }
        public int getEnemyEvil() { return mEnemyEvilNum; }
        public int getNextPlayerId() { return mNextPlayerId; }


        public override IGhost getGhost(int x, int y)
        {
            ulong msk = Mask(x, y);
            if ((mMyEvilGhostBit & msk) != 0)
            {
                return Ghost.Create(mMyPlayerId, GhostType.Evil);
            }
            if ((mMyGoodGhostBit & msk) != 0)
            {
                return Ghost.Create(mMyPlayerId, GhostType.Good);
            }
            if ((mEnemyGhostBit & msk) != 0)
            {
                return Ghost.Create(1-mMyPlayerId, GhostType.Unknown);
            }
            return null;
        }

        private static int Index(int x, int y)
        {
            return (y + 1) * (IField.cFieldWidth+2) + x + 1;
        }
        private static ulong Mask(TPoint p)
        {
            return Mask(p.x, p.y);
        }
        private static ulong Mask(int x, int y)
        {
            return Mask(Index(x, y));
        }
        private static ulong Mask(int index)
        {
            return 0x01UL << index;
        }

        private ulong mMyGoodGhostBit;
        private ulong mMyEvilGhostBit;
        private ulong mEnemyGhostBit;
        int mEnemyEvilNum = 0;
        int mEnemyGoodNum = 0;
        int mMyPlayerId = 0;

        private int mTurn = 0;
        private int mNextPlayerId = 0;
        private static ulong cWall = 0xFFFF060C183060FF;
            //= 0b1111111100000110000011000001100000110000011111111;
        private static ulong[] cGoal = new ulong[2]
        {
            Mask(IField.GetGoalPositions(0)[0]) | Mask(IField.GetGoalPositions(0)[1]),
            Mask(IField.GetGoalPositions(1)[0]) | Mask(IField.GetGoalPositions(1)[1])
        };

        public ulong TransPositionHash()
        {
            ulong hash =
            mMyGoodGhostBit;
            hash *= 31;
            hash += mMyEvilGhostBit;
            hash *= 31;
            hash += mEnemyGhostBit;
            hash *= 31;
            hash += (ulong)(mEnemyEvilNum * 6 + mEnemyGoodNum);
            hash *= 31;
            hash += (ulong)mTurn;
            return hash;
        }
    }

    public class MaxNode
    {
        public MaxNode(Move lastMove, MinNode parent)
        {
            mLastMove = lastMove;
            mParent = parent;
            mScore = Double.PositiveInfinity;
        }
        public MaxNode(MaxMinBitField field)
        {
            mField = field;
            mLastMove = null;
            mParent = null;
        }
        public double search(int depth, double alpha, double beta, Stopwatch sw, Dictionary<ulong, MaxNode> maxNodsCache, Dictionary<ulong, MinNode> minNodesCache)
        {
            if(mField == null)
            {
                if(sw.ElapsedMilliseconds > 200)
                {
                    return -1.0;
                }

                mField = new MaxMinBitField(mParent.getField());
                var result = mField.updateByEnemyMove(mLastMove);
                mValue = result[0];
                mProb = result[1];
            }

            if(depth >= 4)
            {
                ulong hash = mField.TransPositionHash();
                MaxNode node = null;
                if (maxNodsCache.TryGetValue(hash, out node))
                {
                    mScore = node.getScore();
                    return mScore;
                }
                else
                {
                    maxNodsCache.Add(hash, this);
                }
            }

            if (mProb == 1.0)
            {
                mScore = mValue;
                return mValue;
            }

            if (depth <= 0)
            {
                mScore = mProb * mValue + (1.0 - mProb) * MaxMinVotingPlayer.Score(mField);
                return mScore;
            }

            if(mChildList == null)
            {
                var moveList = MaxMinBitField.CreateMoves(mField, mField.getNextPlayerId());
                mChildList = new List<MinNode>(moveList.Count);
                foreach (Move m in moveList)
                {
                    mChildList.Add(new MinNode(m, this));
                }
            }
            else
            {
                // 降順
                mChildList.Sort((MinNode n0, MinNode n1) =>
                {
                    return n0.getScore() > n1.getScore() ? -1 :
                    n0.getScore() < n1.getScore() ? 1 : 0;
                });
            }
            mScore = Double.NegativeInfinity;
            foreach(MinNode node in mChildList)
            {
                double s = mProb * mValue;
                if(s + (1.0-mProb) <= mScore)
                {
                    continue;
                }
                double childValue = node.search(depth - 1, alpha, beta,sw,maxNodsCache,minNodesCache);
                if (childValue < 0)
                {
                    //打ち切り
                    return childValue;
                }

                s += (1.0 - mProb) * childValue;
                if(s > mScore)
                {
                    mScore = s;
                    mBestChild = node;
                    if (s > alpha)
                    {
                        alpha = s;
                        if (s >= beta)
                        {
                            break;
                        }
                    }
                }
            }
            return mScore;
        }

        private MinNode mParent;
        private Move mLastMove; // このノードたどり着くときに打たれた最後の1手
        private MaxMinBitField mField;
        private double mScore;
        private MinNode mBestChild;
        public MinNode getBestChild() { return mBestChild; }
        

        public MaxMinBitField getField() { return mField; }

        public double getScore() { return mScore; }

        private List<MinNode> mChildList;
        private double mValue = -1.0;  //この盤面にたどり着いた時点で決着がつくとしたらその期待値
        private double mProb = 0.0;   //この盤面にたどり着いた時点で決着がつくとしたらその確率


//        static ArrayPool<List<MinNode>> mMinNodeListPool = new ArrayPool<List<MinNode>>();
    }

    public class MinNode
    {
        public MinNode(Move lastMove, MaxNode parent)
        {
            mLastMove = lastMove;
            mParent = parent;
            mScore = Double.NegativeInfinity;
        }
        public double search(int depth, double alpha, double beta,Stopwatch sw, Dictionary<ulong, MaxNode> maxNodsCache, Dictionary<ulong,MinNode>minNodesCache)
        {
            if (mField == null)
            {
                if (sw.ElapsedMilliseconds > 200)
                {
                    return -1.0;
                }

                mField = new MaxMinBitField(mParent.getField());
                var result = mField.updateByMyMove(mLastMove);
                mValue = result[0];
                mProb = result[1];
            }

            if (depth >= 4)
            {
                ulong hash = mField.TransPositionHash();
                MinNode node = null;
                if (minNodesCache.TryGetValue(hash, out node))
                {
                    mScore = node.getScore();
                    return mScore;
                }
                else
                {
                    minNodesCache.Add(hash, this);
                }
            }


            if (mProb == 1.0)
            {
                mScore = mValue;
                return mValue;
            }

            if (depth <= 0)
            {
                mScore = mProb * mValue + (1.0 - mProb) * MaxMinVotingPlayer.Score(mField);
                return mScore;
            }

            if (mChildList == null)
            {
                var moveList = MaxMinBitField.CreateMoves(mField, mField.getNextPlayerId());
                mChildList = new List<MaxNode>(moveList.Count);
                foreach (Move m in moveList)
                {
                    mChildList.Add(new MaxNode(m, this));
                }
            }
            else
            {
                // 昇順
                mChildList.Sort((MaxNode n0, MaxNode n1) =>
                {
                    return n0.getScore() > n1.getScore() ? 1 :
                    n0.getScore() < n1.getScore() ? -1 : 0;
                });
            }
            mScore = Double.PositiveInfinity;
            foreach (MaxNode node in mChildList)
            {
                double s = mProb * mValue;
                if(s >= mScore)
                {
                    continue;
                }

                double childValue = node.search(depth - 1, alpha, beta, sw, maxNodsCache, minNodesCache);
                if (childValue < 0)
                {
                    //打ち切り
                    return childValue;
                }
                s += (1.0 - mProb) * childValue;
                if( s < mScore)
                {
                    mScore = s;
                    if (s < beta)
                    {
                        beta = s;
                        if (s <= alpha)
                        {
                            break;
                        }
                    }
                }
            }
            return mScore;
        }

        private MaxNode mParent;
        private Move mLastMove; // このノードたどり着くときに打たれた最後の1手
        private MaxMinBitField mField;
        private double mScore;

        public MaxMinBitField getField() { return mField; }
        public Move getLastMove() { return mLastMove; }
        public double getScore() { return mScore; }

        private List<MaxNode> mChildList;
        private double mValue = -1.0;  //この盤面にたどり着いた時点で決着がつくとしたらその期待値
        private double mProb = 0.0;   //この盤面にたどり着いた時点で決着がつくとしたらその確率

    }

    public class MaxMinVotingPlayer : IPlayer
    {
        static public double Score(MaxMinBitField f)
        {
            double myGhostTypeScore = f.countMyGood() * 0.8 + f.getEnemyEvil() * 0.2;
            double enemyGhostTypeScore = f.getEnemyGood() * 0.8 + f.getEnemyEvil() * 0.2;
            double myManhattanScore = f.myGoodManhattanScore() *0.6 + f.myEvilManhattanScore() *0.4;
            double enemyManhattanScore = f.enemyManhattanScoreAllColor();

            double myScore = myGhostTypeScore * 1000 - myManhattanScore;
            double enemyScore = enemyGhostTypeScore * 1000 - enemyManhattanScore;

            double s = myScore / (myScore + enemyScore);
            //            Debug.WriteLine(f + "Score:" + s);
            return s;
        }
         
        public MaxMinVotingPlayer()
        {
            Bit.CreateNTZTable();
        }

        public override GhostType[] firstPosition()
        {
            return new GhostType[] {
                GhostType.Good, GhostType.Good , GhostType.Good ,
                GhostType.Evil, GhostType.Evil , GhostType.Evil ,
            };
        }

        public override Move move(IField field, int teamId, int turn)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            mMyTeamId = teamId;
            MaxMinBitField f = new MaxMinBitField(field, teamId, turn);
            MaxNode root = new MaxNode(f);
            Move bestMove = null;
            double bestScore = -1;
            int d = 1;
            Dictionary<ulong, MaxNode> maxNodesCache = new Dictionary<ulong, MaxNode>(100000);
            Dictionary<ulong, MinNode> minNodesCache = new Dictionary<ulong, MinNode>(100000);
            for (;; d++)
            {
                maxNodesCache.Clear();
                minNodesCache.Clear();
                double s = root.search(d, Double.NegativeInfinity, Double.PositiveInfinity, sw,maxNodesCache,minNodesCache);
                if(s < 0) {
                    break;
                }
                bestMove = root.getBestChild().getLastMove();
                bestScore = root.getScore();
                if (s < 0.0001 || s >= 0.9999)
                {
                    break;
                }
            }
            Debug.WriteLine("s:" + bestScore+"(d="+d);
            mTotalDepth += d;
            return bestMove;
        }


        int mMyTeamId = 0;
        int mTotalDepth = 0;
    }
}

