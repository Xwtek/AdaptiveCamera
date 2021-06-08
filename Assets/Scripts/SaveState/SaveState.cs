using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;
using System;
public static class SaveState{
    private static List<ISaveable> saveables;
    public static void Register(ISaveable saveable){
        if(saveables == null) saveables = new List<ISaveable>();
        if(!saveables.Contains(saveable)){
            saveables.Add(saveable);
        }
    }
    public static void Deregister(ISaveable saveable){
        if(saveables.Contains(saveable)){
            saveables.Remove(saveable);
        }
        if(saveables.Count == 0) saveables = null;
    }
    public static string name = "test";
    public static int level = 1;
    public static void Load(){
        var reader = QuickSaveReader.Create(name, new QuickSaveSettings { CompressionMode = CompressionMode.Gzip });
        foreach(var saveable in saveables){
            saveable.Load(new SaveItemReader(saveable.id, reader));
        }
    }
    public static void Save(){
        var writer = QuickSaveWriter.Create(name, new QuickSaveSettings { CompressionMode = CompressionMode.Gzip });
        foreach (var key in writer.GetAllKeys())
        {
            writer.Delete(key);
        }
        foreach(var saveable in saveables){
            saveable.Save(new SaveItemWriter(saveable.id, writer));
        }
        writer.Write("Level", level);
        writer.Commit();
    }
    public static void Restore(){
        foreach(var saveable in saveables){
            saveable.Restore();
        }
    }
}