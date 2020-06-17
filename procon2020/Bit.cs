using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace geister.taiyo
{
    //Bit操作用Utility

    class Bit
    {
        static public int bitCount(ulong bits)
        {
            bits = (bits & 0x5555555555555555) + (bits >> 1 & 0x5555555555555555);    //  2bitごとに計算
            bits = (bits & 0x3333333333333333) + (bits >> 2 & 0x3333333333333333);    //  4bitごとに計算
            bits = (bits & 0x0f0f0f0f0f0f0f0f) + (bits >> 4 & 0x0f0f0f0f0f0f0f0f);    //  8bitごとに計算
            bits = (bits & 0x00ff00ff00ff00ff) + (bits >> 8 & 0x00ff00ff00ff00ff);    //  16ビットごとに計算   
            bits = (bits & 0x0000ffff0000ffff) + (bits >> 16 & 0x0000ffff0000ffff);    //  32ビット分を計算   
            bits = (bits & 0x00000000ffffffff) + (bits >> 32 & 0x00000000ffffffff);    //  64ビット分を計算   
            return (int)bits;
        }


        static private int[] sNTZTable;

        static public void CreateNTZTable()
        {
            if (sNTZTable != null) { return; }
            sNTZTable = new int[64];
            ulong hash = 0x03F566ED27179461UL;
            for (int i = 0; i < 64; i++)
            {
                sNTZTable[hash >> 58] = i;
                hash <<= 1;
            }
        }
         
        public static int GetNumberOfTrailingZeros(ulong x)
        {
            Debug.Assert(sNTZTable!=null);
            Debug.Assert(x != 0);
            //if (x == 0) return 64;

            ulong y = rightOneBits(x); 
            int i = (int)((y * 0x03F566ED27179461UL) >> 58);
            return sNTZTable[i];
        }

        //右からゼロ埋めされている個数を数える=右からみて最初に1が見つかるidxを返す。1がない場合は64が返る。
        static public int numberOfTrainingZero(ulong bits)
        {
            return bitCount((~bits) & (bits - 1));
        }

        //右端の1であるbitを残して他を0とする。
        static public ulong rightOneBits(ulong bits)
        {
            return (bits & (bits - 1)) ^ bits;
        }

        //右端の1を0にする
        static public ulong rightOneToZero(ulong bits)
        {
            return bits & (bits - 1);
        }

    }
}
