using Unity.Burst;
using Unity.Mathematics;

namespace TriBurst
{
    struct Unmanaged
    {
        internal static readonly SharedStatic<Unmanaged> Instance = SharedStatic<Unmanaged>.GetOrCreate<Unmanaged>();

        internal Unit TriBufferAllocations;
        internal TriBuffer TriBuffer;
        internal bool Initialized;

        internal void Initialize(int maxTris)
        {
            if (Initialized == false)
            {
                TriBuffer = new TriBuffer(maxTris);
                TriBufferAllocations = TriBuffer.AllocateAll();
                Initialized = true;
            }
        }

        internal void Clear()
        {
            TriBufferAllocations = TriBuffer.AllocateAll(); // clear out all the lines
        }

        internal void Dispose()
        {
            if (Initialized)
            {
                TriBuffer.Dispose();
                Initialized = false;
            }
        }
    }
}
