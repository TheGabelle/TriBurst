using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace TriBurst
{
    class Managed : IDisposable
    {
        readonly Material _triMaterial;
        int _vertsTodraw;

        ComputeBuffer _vertexBuffer;
        // ComputeBuffer _colorBuffer;

        static Managed _instance;
        // static readonly int ColorBuffer = Shader.PropertyToID("colorBuffer");
        static readonly int VertexBuffer = Shader.PropertyToID("TriBurstVertex");

        internal static Managed Instance;
        bool _warned;

        internal Managed(int maxLines, Material triMaterial)
        {
            if (triMaterial == null)
                throw new Exception("Tri burst tri material not assigned");
            Unmanaged.Instance.Data.Initialize(maxLines);
            _triMaterial = triMaterial;
#if !UNITY_DOTSRUNTIME
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
#endif
        }
        static void OnDomainUnload(object sender, EventArgs e)
        {
            _instance?.Dispose();
        }

        internal void CopyFromCpuToGpu()
        {
            if (_vertexBuffer == null || _vertexBuffer.count != Unmanaged.Instance.Data.TriBuffer.Instance.Length)
            {
                if (_vertexBuffer != null)
                {
                    _vertexBuffer.Release();
                    _vertexBuffer = null;
                }

                _vertexBuffer = new ComputeBuffer(Unmanaged.Instance.Data.TriBuffer.Instance.Length, UnsafeUtility.SizeOf<float4>());
                _triMaterial.SetBuffer(VertexBuffer, _vertexBuffer);
            }

            _vertsTodraw = Unmanaged.Instance.Data.TriBufferAllocations.Filled * 3;
            if (!_warned && _vertsTodraw == _vertexBuffer.count)
            {
                _warned = true;
                Debug.Log($"### Warning - Maximum number of lines reached, additional lines will not be drawn");
            }

            _vertexBuffer.SetData(Unmanaged.Instance.Data.TriBuffer.Instance.ToNativeArray(), 0, 0, _vertsTodraw);
        }

        internal void Clear()
        {
            Unmanaged.Instance.Data.Clear();
        }

        internal void Render()
        {
            _triMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, _vertsTodraw);
        }

/*
        internal void Render(HDCamera hdCamera, CommandBuffer cmd)
        {
            if (hdCamera.camera.cameraType != CameraType.Game)
                return;
            cmd.DrawProcedural(Matrix4x4.identity, resources.textMaterial, 0, MeshTopology.Triangles, NumTextBoxesToDraw * 6, 1);
            cmd.DrawProcedural(Matrix4x4.identity, resources.graphMaterial, 0, MeshTopology.Triangles, NumGraphsToDraw * 6, 1);
        }

        internal void Render3D(HDCamera hdCamera, CommandBuffer cmd)
        {
            cmd.DrawProcedural(Matrix4x4.identity, _triMaterial, 0, MeshTopology.Lines, NumLinesToDraw, 1);
        }
*/

        public void Dispose()
        {
            _vertexBuffer?.Dispose();
            _vertexBuffer = null;
            // _colorBuffer?.Dispose();
            // _colorBuffer = null;

            Unmanaged.Instance.Data.Dispose();
            if (_instance == this)
                _instance = null;
        }
    }
}