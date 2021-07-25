using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace TriBurst
{
    /// <summary>
    /// Attach to any gameobject to render in the scene view, or the camera to also render in the game view
    /// </summary>
    public class TriBurstRenderer : MonoBehaviour
    {
        [Min(1000)]
        public int MaxTris = 250 * 1000;
        public bool DrawInGameView;
        public bool AutoClear = true;
        public static JobHandle Handle;
        bool _clear;
        public Material TriMaterial;

        void Awake()
        {
            if (DrawInGameView && GetComponent<Camera>() == null)
                throw new Exception("TriBurstRenderer needs to be attached to the camera gameobject to draw in the game view");

            // Assert.IsTrue(TriBurst.Managed.Instance == null);
            Managed.Instance = new Managed(MaxTris, TriMaterial);
            RenderPipelineManager.endFrameRendering += (arg1, arg2) => GameViewRender();
        }

        void OnPostRender()
        {
            GameViewRender();
        }

        void GameViewRender()
        {
            if (Application.isPlaying && DrawInGameView)
            {
                Render();

                if (!Application.isEditor || _clear)
                {
                    if (AutoClear)
                        Clear();
                    _clear = false;
                }
                else
                {
                    _clear = true;
                }
            }
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Render();

                if (!DrawInGameView || _clear)
                {
                    if (AutoClear)
                        Clear();
                    _clear = false;
                }
                else
                {
                    _clear = true;
                }
            }
        }

        void OnDestroy()
        {
            Managed.Instance?.Dispose();
            Managed.Instance = null;
        }

        public static void Render()
        {
            Handle.Complete();
            Managed.Instance.CopyFromCpuToGpu();
            Managed.Instance.Render();
        }

        public static void Clear()
        {
            if (Managed.Instance != null)
                Managed.Instance.Clear();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (TriMaterial == null)
                TriMaterial = Resources.Load<Material>("TriBurstTriMaterial");
        }
#endif
    }
}