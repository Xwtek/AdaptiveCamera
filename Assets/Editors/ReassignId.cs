#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class SaveID : EditorWindow {
	[MenuItem("Edit/Save Id/Reassign this component")]
	static void ReassignId() {
        var maxid = GetMaxId();
        foreach(var go in Selection.gameObjects){
            foreach (var saveable in go.GetComponentsInChildren<ISaveable>())
            {
                saveable.id = ++maxid;
            }
        }
    }
	[MenuItem("Edit/Save Id/Reassign all")]
	static void ReassignAllId() {
        var scene = SceneManager.GetActiveScene();
        var rgo = scene.GetRootGameObjects();
        var currId = 1;
        foreach (var go in rgo)
        {
            foreach (var saveable in go.GetComponentsInChildren<ISaveable>())
            {
                Undo.RecordObject(saveable as MonoBehaviour, "Assign ID");
                saveable.id = currId++;
                PrefabUtility.RecordPrefabInstancePropertyModifications(saveable as MonoBehaviour);
            }
        }
    }
    static int GetMaxId(){
        var scene = SceneManager.GetActiveScene();
        var gameObjects = scene.GetRootGameObjects();
        var max = 0;
        for (var i = 0; i < gameObjects.Length; i++)
        {
            max = Math.Max(max, GetMaxId(gameObjects[i]));
        }
        return max;
    }
    static int GetMaxId(GameObject go){
        return go.GetComponentsInChildren<ISaveable>().Select(x=>x.id).Max();
    }
}

#endif