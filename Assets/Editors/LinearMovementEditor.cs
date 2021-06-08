#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AdaptiveCamera.Util;
[CustomEditor(typeof(LinearMovement))]
public class LinerMovementEditor : Editor {
    bool editMode;
    public override void OnInspectorGUI() {
        editMode = GUILayout.Toggle(editMode, "Edit in Scene");
        base.OnInspectorGUI();
    }
    public void OnSceneGUI(){
        var target = this.target as LinearMovement;
        Handles.color = Color.red;
        var targetPos = target.transform.rotation * target.target + target.transform.position;
        Handles.DrawLine(target.transform.position, targetPos);
        if (editMode)
        {
            target.target = Quaternion.Inverse(target.transform.rotation)*( Handles.DoPositionHandle(targetPos, target.transform.rotation) - target.transform.position);
        }
    }
}
#endif