using UnityEngine;
using Unity.Collections;
using TriBurst;

public class TestScript : MonoBehaviour
{

    public Vector3 vert1;
    public Vector3 vert2;
    public Vector3 vert3;
    public Vector3 vert4;

    public Color color1;
    public Color color2;

    void Update()
    {
        var triArr = new NativeArray<Tri>(2, Allocator.Temp);
        triArr[0] = new Tri(vert1, vert2, vert3, color1);
        triArr[1] = new Tri(vert1, vert3, vert4, color2);
        TriBurst.Draw.Tris.Draw(triArr);
        triArr.Dispose();
    }
}
