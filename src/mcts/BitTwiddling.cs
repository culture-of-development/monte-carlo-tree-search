using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace mcts
{
    public static class BitTwiddling
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(ulong value)
        {
#if NETCOREAPP
            return BitOperations.PopCount(value);
#else
            //https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Numerics/BitOperations.cs
            const ulong c1 = 0x_55555555_55555555ul;
            const ulong c2 = 0x_33333333_33333333ul;
            const ulong c3 = 0x_0F0F0F0F_0F0F0F0Ful;
            const ulong c4 = 0x_01010101_01010101ul;

            value -= (value >> 1) & c1;
            value = (value & c2) + ((value >> 2) & c2);
            value = (((value + (value >> 4)) & c3) * c4) >> 56;

            return (int)value;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(ulong value)
        {
#if NETCOREAPP
            return BitOperations.TrailingZeroCount(value);
#else
            uint lo = (uint)value;
            if (lo == 0)
            {
                return 32 + TrailingZeroCount((uint)(value >> 32));
            }
            return TrailingZeroCount(lo);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int TrailingZeroCount(uint value)
        {
            if (value == 0) return 32;
            return TrailingZeroCountDeBruijn[(int)(((value & (uint)-(int)value) * 0x077CB531u) >> 27)];
        }

        private static ReadOnlySpan<byte> TrailingZeroCountDeBruijn => new byte[32]
        {
            00, 01, 28, 02, 29, 14, 24, 03,
            30, 22, 20, 15, 25, 17, 04, 08,
            31, 27, 13, 23, 21, 19, 16, 07,
            26, 12, 18, 06, 11, 05, 10, 09
        };
    }
}