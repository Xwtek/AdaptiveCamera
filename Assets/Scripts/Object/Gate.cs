using UnityEngine;
public class Gate : MonoBehaviour, ISaveable
{
    public string type => "Gate";
    [field:SerializeField]
    public int id { get; set; }
    public int enemyRemaining;
    private int originalCount;
    private void Awake() {
        SaveState.Register(this);
        originalCount = enemyRemaining;
    }
    private void OnDestroy() {
        SaveState.Deregister(this);
    }
    public void Load(SaveItemReader reader)
    {
        enemyRemaining = reader.Load<int>("Remaining");
        gameObject.SetActive(reader.Load<bool>("IsClosed"));
    }

    public void Restore()
    {
        enemyRemaining = originalCount;
        gameObject.SetActive(true);
    }

    public void Save(SaveItemWriter writer)
    {
        writer.Save("Remaining", enemyRemaining);
        writer.Save("IsClosed", gameObject.activeSelf);
    }
    public void Decrement(){
        enemyRemaining--;
        gameObject.SetActive(enemyRemaining > 0);
    }
}