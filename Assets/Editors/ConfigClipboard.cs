#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class ConfigClipboard : EditorWindow
{
    [Serializable]
    public class ClipboardState : ScriptableObject
    {
        public List<Transform> copiedTransform = new List<Transform>();
        public List<Vector3> copiedPositions = new List<Vector3>();
        public List<Quaternion> copiedRotations = new List<Quaternion>();
        public Vector3 copiedPosition;
        public Quaternion copiedRotation;
    }
    static ClipboardState state = ScriptableObject.CreateInstance<ClipboardState>();
    [MenuItem("Edit/Copy Configuration")]
    static void CopyConfiguration()
    {
        Undo.RecordObject(state, "Copy Configuration");
        state.copiedTransform.Clear();
        state.copiedRotations.Clear();
        state.copiedPositions.Clear();
        foreach (var gameObject in Selection.gameObjects)
            CopyConfiguration(gameObject.transform);
    }
    static void CopyConfiguration(Transform transform)
    {
        state.copiedPositions.Add(transform.localPosition);
        state.copiedRotations.Add(transform.localRotation);
        state.copiedTransform.Add(transform);
        for (var i = 0; i < transform.childCount; i++)
        {
            CopyConfiguration(transform.GetChild(i));
        }
    }
    [MenuItem("Edit/Paste Configuration")]
    static void PasteConfiguration()
    {
        for (var i = state.copiedPositions.Count - 1; i >= 0; i--)
        {
            Undo.RecordObject(state.copiedTransform[i], "Paste Configuration #" + i);
            state.copiedTransform[i].localPosition = state.copiedPositions[i];
            state.copiedTransform[i].localRotation = state.copiedRotations[i];
        }
    }
    [MenuItem("Edit/Copy Transform")]
    static void CopyTransform()
    {
        Undo.RecordObject(state, "Copy Transform");
        state.copiedRotation = Selection.activeGameObject.transform.rotation;
        state.copiedPosition = Selection.activeGameObject.transform.position;
    }
    [MenuItem("Edit/Paste Transform")]
    static void PasteTransform()
    {
        Undo.RecordObject(Selection.activeGameObject.transform, "Paste Transform");
        Selection.activeGameObject.transform.position = state.copiedPosition;
        Selection.activeGameObject.transform.rotation = state.copiedRotation;
    }
}

#endif