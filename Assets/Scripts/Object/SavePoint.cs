using UnityEngine;

public class SavePoint : MonoBehaviour, ISaveable {
    public bool saved;
    public string type => "SavePoint";
    [field: SerializeField]
    public int id { get; set; }

    public void Load(SaveItemReader reader)
    {
        saved = reader.Load<bool>("Saved");
    }

    public void Restore()
    {
        saved = false;
    }

    public void SaveGame() {
        if(saved) return;
        saved = !saved;
        SaveState.Save();
    }


    public void Save(SaveItemWriter writer)
    {
        writer.Save("Saved", saved);
    }
}