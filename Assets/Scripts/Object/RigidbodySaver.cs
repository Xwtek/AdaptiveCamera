using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class RigidbodySaver : MonoBehaviour, ISaveable
{
    public string type => "Rigidbody";
    [field: SerializeField]
    public int id { get; set; }
    private Rigidbody _rb = null;
    private Rigidbody rb{ get{
        if(_rb == null) _rb = GetComponent<Rigidbody>();
            return _rb;
        }}
    public void Load(SaveItemReader reader)
    {
        rb.angularVelocity = reader.Load<Vector3>("AngularVelocity");
        rb.velocity = reader.Load<Vector3>("Velocity");
        transform.rotation = reader.Load<Quaternion>("Rotation");
        transform.position = reader.Load<Vector3>("Position");
    }
    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 originalVelocity;
    Vector3 originalAngularVelocity;
    public void Restore()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.velocity = originalVelocity;
        rb.angularVelocity = originalAngularVelocity;
    }
    private void Start() {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalVelocity = rb.velocity;
        originalAngularVelocity = rb.angularVelocity;
    }
    public void Save(SaveItemWriter writer)
    {
        writer.Save("Position", transform.position);
        writer.Save("Rotation", transform.rotation);
        writer.Save("Velocity", rb.velocity);
        writer.Save("AngularVelocity", rb.angularVelocity);
    }
    private void Awake() {
        SaveState.Register(this);
    }
    private void OnDestroy() {
        SaveState.Deregister(this);
    }
}