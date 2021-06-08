#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
	
public class MassRevert : EditorWindow {
	[MenuItem("Edit/Mass Revert Transform")]
	static void MassRevertTransform() {
		foreach(var gameObject in UnityEditor.Selection.gameObjects){
            PrefabUtility.RevertObjectOverride(gameObject.transform, InteractionMode.AutomatedAction);
        }
	}
	[MenuItem("Edit/Mass Revert Children")]
	static void MassRevertTransformChildren() {
        var reverted = Selection.activeGameObject;
        PrefabUtility.RevertObjectOverride(reverted, InteractionMode.AutomatedAction);
        foreach (var child in reverted.GetComponentsInChildren<Transform>())
            PrefabUtility.RevertObjectOverride(child, InteractionMode.AutomatedAction);
	}
}

#endif