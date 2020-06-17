using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace geister.taiyo
{

    public enum GhostType
    {
        Good = 0,
        Evil = 1,
        Unknown = 2
    }

    public abstract class IGhost
    {
        abstract public GhostType getType();
        abstract public int getTeamId();

        public bool isEvil()
        {
            return getType() == GhostType.Evil;
        }

        public bool isGood()
        {
            return getType() == GhostType.Good;
        }

    }

    public class Ghost : IGhost
    {
        private static Ghost[,] sGhosts = new Ghost[2,3]{
            {new Ghost(0, GhostType.Good),new Ghost(0, GhostType.Evil),new Ghost(0, GhostType.Unknown)},
            {new Ghost(1, GhostType.Good),new Ghost(1, GhostType.Evil),new Ghost(1, GhostType.Unknown)}
        };

        public static Ghost Create(IGhost g)
        {
            return Create(g.getTeamId(), g.getType());
        }

        public static Ghost Create(int teamId, GhostType type)
        {
            return sGhosts[teamId, (int)type];
        }

        public Ghost(IGhost other)
        {
            this.mTeamNo = other.getTeamId();
            this.mType = other.getType();
        }

        public Ghost(int teamNo, GhostType type)
        {
            this.mTeamNo = teamNo;
            this.mType = type;
        }

        override public GhostType getType()
        {
            return mType;
        }

        override public int getTeamId()
        {
            return mTeamNo;
        }

        private int mTeamNo;
        private GhostType mType;
    }

    public class PlayersGhost : IGhost
    {
        private int mViewerPlayerId = -1;
        private IGhost mMasterGhost = null;

        public PlayersGhost(IGhost ghost, int viewerPlayerId)
        {
            Debug.Assert(ghost != null);
            this.mViewerPlayerId = viewerPlayerId;
            this.mMasterGhost = ghost;
        }

        override public GhostType getType()
        {
            if (mViewerPlayerId != getTeamId())
            {
                return GhostType.Unknown;
            }
            else
            {
                return mMasterGhost.getType();
            }
        }

        override public int getTeamId()
        {
            return mMasterGhost.getTeamId();
        }

    }
}
