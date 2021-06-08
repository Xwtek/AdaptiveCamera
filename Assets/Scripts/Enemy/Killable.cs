using UnityEngine;
[RequireComponent(typeof(EnemyEvent))]
public class Killable : MonoBehaviour, ISaveable {
    public string[] killableBy;
    EnemyEvent evt;
    public bool died;

    public string type => "Killable";
    [field: SerializeField]
    public int id{ get; set; }
    private void Awake() {
        SaveState.Register(this);
    }
    private void OnDestroy() {
        SaveState.Deregister(this);
    }
    private void Start() {
        evt = GetComponent<EnemyEvent>();
    }
    private void CheckDeath(Collider other){
        for (var i = 0; i < killableBy.Length;i++){
            if (other.tag == killableBy[i]){
                if(other.GetComponent<EnemyKiller>().enabled){
                    evt.OnDie.Invoke();
                    died = true;
                }
                return;
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        CheckDeath(other.collider);
    }
    private void OnTriggerEnter(Collider other){
        CheckDeath(other);
    }

    public void Load(SaveItemReader reader)
    {
        var oldDied = died;
        died = reader.Load<bool>("Died");
        gameObject.SetActive(!died);
        if(oldDied && !died) evt.OnRevive.Invoke();
    }

    public void Save(SaveItemWriter writer)
    {
        writer.Save("Died", died);
    }

    public void Restore()
    {
        if(died){
            died = false;
            evt.OnRevive.Invoke();
        }
    }
}