#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AdaptiveCamera.Util;
[CustomEditor(typeof(RadialMovement))]
public class RadialMovementEditor : Editor {
    bool editMode;
    public override void OnInspectorGUI() {
        editMode = GUILayout.Toggle(editMode, "Edit in Scene");
        base.OnInspectorGUI();
    }
    public void OnSceneGUI(){
        var target = this.target as RadialMovement;
        Debug.Log(target.center + target.transform.position);
        Handles.color = Color.red;
        var center = target.center + target.transform.position;
        var normal = target.normal + center;
        Vector3 actualCenter = MathUtil.PointClosestToALine(center, normal, target.transform.position);
        Handles.DrawWireDisc(actualCenter, target.normal, Mathf.Max(HandleUtility.GetHandleSize(target.transform.position), (target.transform.position - actualCenter).magnitude), Handles.lineThickness);
        var dist = Mathf.Max((normal - actualCenter).magnitude, (center - actualCenter).magnitude, HandleUtility.GetHandleSize(actualCenter));
        Handles.DrawLine(actualCenter+target.normal*dist , actualCenter - target.normal *dist);
        var rotationCenter = target.center + target.transform.position;
        var handleSize = target.center.magnitude;
        if (editMode)
        {
            target.normal = (Handles.DoPositionHandle(target.normal * HandleUtility.GetHandleSize(center) + center, Quaternion.FromToRotation(Vector3.forward, target.normal)) - center).normalized;
            target.center = Handles.DoPositionHandle(center, target.transform.rotation) - target.transform.position;
        }
    }
}
#endif