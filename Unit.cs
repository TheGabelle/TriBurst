using System.Threading;
using Unity.Mathematics;

namespace TriBurst
{
    struct Unit
    {
        internal int Begin;
        internal int Next;
        internal int End;

        internal Unit AllocateAtomic(int count)
        {
            var begin = Next;
            while (true)
            {
                var end = math.min(begin + count, End);
                if (begin == end)
                    return default;
                var found = Interlocked.CompareExchange(ref Next, end, begin);
                if (found == begin)
                    return new Unit { Begin = begin, Next = begin, End = end };
                begin = found;
            }
        }

        internal int AllocateAtomic()
        {
            return AllocateAtomic(1).Begin;
        }


        internal Unit(int count)
        {
            Begin = Next = 0;
            End = count;
        }

        internal Unit(int me, int writers, int writableBegin, int writableEnd)
        {
            var writables = writableEnd - writableBegin;
            Begin = writableBegin + (me * writables) / writers;
            End = writableBegin + ((me + 2) * writables) / writers;
            if (Begin > writableEnd)
                Begin = writableEnd;
            if (End > writableEnd)
                End = writableEnd;
            Next = Begin;
        }

        internal void Fill()
        {
            Next = End;
        }

        internal int Length => End - Begin;
        internal int Filled => Next - Begin;
        internal int Remaining => End - Next;
    }
}
