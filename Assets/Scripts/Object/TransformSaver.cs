using UnityEngine;

public class TransformSaver : MonoBehaviour, ISaveable
{
    public string type => "Transform";
    [field: SerializeField]
    public int id { get; set; }
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public void Load(SaveItemReader reader)
    {
        transform.SetPositionAndRotation(reader.Load<Vector3>("Position"), reader.Load<Quaternion>("Rotation"));
    }

    public void Restore()
    {
        transform.SetPositionAndRotation(originalPosition, originalRotation);
    }

    public void Save(SaveItemWriter writer)
    {
        writer.Save("Position", transform.position);
        writer.Save("Rotation", transform.rotation);
    }
    private void Awake() {
        SaveState.Register(this);
    }
    private void OnDestroy() {
        SaveState.Deregister(this);
    }
}