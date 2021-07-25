using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace TriBurst
{
    public static class Draw
    {
        public struct Tris
        {
            Unit _unit;

            public Tris(int count)
            {
                _unit = Unmanaged.Instance.Data.TriBufferAllocations.AllocateAtomic(count);
            }

            public static unsafe void Draw(NativeArray<Tri> tris)
            {
                var l = new Tris(tris.Length);
                var linesToCopy = math.min(tris.Length, l._unit.End - l._unit.Next);
                Unmanaged.Instance.Data.TriBuffer.CopyFrom(tris.GetUnsafeReadOnlyPtr(), linesToCopy, l._unit.Next);
                l._unit.Next += linesToCopy;
            }

            public void Draw(float3 vert1, float3 vert2, float3 vert3, Color color)
            {
                if (_unit.Next < _unit.End)
                    Unmanaged.Instance.Data.TriBuffer.SetTri(new Tri(vert1, vert2, vert3, color), _unit.Next++);
            }
        }

        public static void Tri(float3 vert1, float3 vert2, float3 vert3, Color color) => new Tris(1).Draw(vert1, vert2, vert3, color);
    }

    public struct Tri
    {
        public float4 v1;
        public float4 v2;

        public float4 v3;

        internal Tri(float3 vert1, float3 vert2, float3 vert3, Color color)
        {
            var packedColor = ((int)(color.r * 63) << 18) | ((int)(color.g * 63) << 12) | ((int)(color.b * 63) << 6) | (int)(color.a * 63);
            v1 = new float4(vert1, packedColor);
            v2 = new float4(vert2, packedColor);
            v3 = new float4(vert3, packedColor);
        }
    }
}