#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public static class EditorHelper {
    public static void DrawCircle(Vector3 center, float radius, Vector3 normal, int segmentCount){
        var current = Quaternion.FromToRotation(Vector3.up, normal)*Vector3.forward *radius;
        var rotation = Quaternion.AngleAxis(360 / segmentCount, normal);
        for (int i = 0; i < segmentCount; i++)
        {
            var next = rotation * current;
            Gizmos.DrawLine(current, next);
            current = next;
        }
    }
}
#endif