using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RadialMovement : MonoBehaviour, ISaveable{
    public Vector3 normal;
    public Vector3 center;
    private Vector3 actualCenter;
    private Quaternion originalRotation;
    public float speed;
    private Rigidbody rb;
    private void Start() {
        actualCenter = center + this.transform.position;
        rb = GetComponent<Rigidbody>();
        originalState = State;
        originalRotation = this.transform.rotation;
    }
    public float timeShift;
    private void FixedUpdate() {
        var relativePosition = transform.position - actualCenter;
        timeShift += Time.fixedDeltaTime;
        var rotation = Quaternion.AngleAxis(speed * timeShift / 3, normal);
        rb.MovePosition(actualCenter + rotation * (-center));
        rb.MoveRotation(rotation * originalRotation);
    }

    public void Load(SaveItemReader reader)
    {
        State = reader.Load<MovementState>("State");
    }

    public void Save(SaveItemWriter writer)
    {
        writer.Save("State", State);
    }

    public void Restore()
    {
        State = originalState;
    }

    MovementState originalState;
    private MovementState State
    {
        get => new MovementState
            {
                velocity = rb.velocity,
                angularVelocity = rb.angularVelocity,
                position = rb.position,
                rotation = rb.rotation,
            };
        set {
            rb.velocity = value.velocity;
            rb.angularVelocity = value.angularVelocity;
            rb.position = rb.position;
            rb.rotation = rb.rotation;
        }
    }

    public string type => "RadialMovement";
    [field: SerializeField]
    public int id{ get; set; }
    [System.Serializable]
    struct MovementState
    {
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public Vector3 position;
        public Quaternion rotation;

    }
}