using UnityEngine;

public class Destroyable : MonoBehaviour, ISaveable {
    public string type => "Destroyable";
    [field: SerializeField]
    public int id{ get; set; }
    private void Awake() {
        SaveState.Register(this);
    }
    private void OnDestroy() {
        SaveState.Deregister(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
        {
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Projectile")
        {
            gameObject.SetActive(false);
        }
    }

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                public void Load(SaveItemReader reader)
    {
        gameObject.SetActive(!reader.Load<bool>("Destroyed"));
    }

    public void Save(SaveItemWriter writer)
    {
        writer.Save("Destroyed", !gameObject.activeSelf);
    }

    public void Restore()
    {
        gameObject.SetActive(true);
    }
}