#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AdaptiveCamera.Util;
using Unity.Mathematics;
[CustomEditor(typeof(PathSegmentSimple))]
public class PathSegmentEditor : Editor {
    bool EditStart;
    bool EditEnd;
    bool EditCurved;
    public override void OnInspectorGUI() {
        var segment = target as PathSegmentSimple;
        float3 oldStart = segment.startPoint;
        float3 oldEnd = segment.endPoint;
        float3 oldCurve = segment.curvePoint;
        float3 newStart = EditorGUILayout.Vector3Field("Start Point", oldStart);
        EditStart = EditorGUILayout.Toggle(EditStart);
        float3 newEnd = EditorGUILayout.Vector3Field("End Point", oldEnd);
        EditEnd = EditorGUILayout.Toggle(EditEnd);
        float3 newCurve = EditorGUILayout.Vector3Field("Curve Point", oldCurve);
        EditCurved = EditorGUILayout.Toggle(EditCurved);
        UpdateCurve(segment, oldStart, oldEnd, oldCurve, newStart, newEnd, newCurve);
        base.OnInspectorGUI();
    }
    private void UpdateCurve(
        PathSegmentSimple target,
        float3 oldStart, float3 oldEnd, float3 oldCurve,
        float3 newStart, float3 newEnd, float3 newCurve){
        var oldDir = oldEnd - oldStart;
        var newDir = newEnd - newStart;
        if(Mathf.Approximately(math.lengthsq(oldDir),0)){
            target.curvePoint = (newStart + newEnd) / 2;
        }else{
            var angle = Quaternion.FromToRotation(oldDir, newDir);
            var ratio = math.sqrt(math.lengthsq(newDir) / math.lengthsq(oldDir));
            target.curvePoint = ratio * (angle * (newCurve - newStart)) + ((Vector3)newStart);
        }
        target.startPoint = newStart;
        target.endPoint = newEnd;
    }
    public void OnSceneGUI(){
        var segment = target as PathSegmentSimple;
        float3 oldStart = segment.startPoint;
        float3 oldEnd = segment.endPoint;
        float3 oldCurve = segment.curvePoint;
        float3 newStart = PosHandle(segment.transform, EditStart, oldStart);
        float3 newEnd = PosHandle(segment.transform, EditEnd, oldEnd);
        float3 newCurve = PosHandle(segment.transform, EditCurved, oldCurve);
        UpdateCurve(segment, oldStart, oldEnd, oldCurve, newStart, newEnd, newCurve);

    }
    private float3 PosHandle(Transform relative, bool condition, float3 initial){
        if(!condition) return initial;
        return relative.InverseTransformPoint(Handles.PositionHandle(relative.TransformPoint(initial), relative.rotation));
    }
}
#endif