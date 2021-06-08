using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public interface ISaveable {
    void Load(SaveItemReader reader);
    void Save(SaveItemWriter writer);
    void Restore();
    string type{ get; }
    int id{ get; set; }
}