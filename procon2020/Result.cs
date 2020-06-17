using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geister.taiyo
{
    public class Result
    {
        public int mWinner = -1;
        public CauseOfResult mCause = CauseOfResult.None;
    }

    public enum CauseOfResult
    {
        None = 0,
        FragAllEvil,
        FragAllGood,
        Goal,
        Judge
    }

}
