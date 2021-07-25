using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace TriBurst
{
    unsafe struct UnsafeArray<T> : IDisposable where T : unmanaged
    {
        readonly T* _pointer;
        internal T* GetUnsafePtr() => _pointer;

        internal UnsafeArray(int length)
        {
            var size = UnsafeUtility.SizeOf<T>() * length;
            var alignment = UnsafeUtility.AlignOf<T>();
            _pointer = (T*)UnsafeUtility.Malloc(size, alignment, Allocator.Persistent);
            Length = length;
        }

        public void Dispose()
        {
            UnsafeUtility.Free(_pointer, Allocator.Persistent);
        }

        internal int Length { get; }

        internal ref T this[int index] => ref UnsafeUtility.AsRef<T>(_pointer + index);

        internal NativeArray<T> ToNativeArray()
        {
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(_pointer, Length, Allocator.Invalid);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, AtomicSafetyHandle.GetTempUnsafePtrSliceHandle());
#endif
            return array;
        }
    }

    struct TriBuffer : IDisposable
    {
        internal UnsafeArray<float4> Instance;

        internal TriBuffer(int KMaxTris)
        {
            Instance = new UnsafeArray<float4>(KMaxTris * 3);
        }

        internal void SetTri(Tri tri, int index)
        {
            Instance[index * 2] = tri.v1;
            Instance[index * 2 + 1] = tri.v2;
            Instance[index * 2 + 2] = tri.v3;
        }

        public void Dispose()
        {
            Instance.Dispose();
        }

        internal Unit AllocateAll()
        {
            return new Unit(Instance.Length / 3);
        }

        internal unsafe void CopyFrom(void* ptr, int amount, int offset)
        {
            UnsafeUtility.MemCpy(Instance.GetUnsafePtr() + 3 * offset, ptr, amount * UnsafeUtility.SizeOf<Tri>());
        }
    }
}
