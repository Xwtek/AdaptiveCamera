using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class LinearMovement : MonoBehaviour, ISaveable
{
    public Vector3 target;
    public float speed;
    public float returnSpeed;
    private Vector3 origin;
    private Vector3 actualTarget;
    private float length;
    private Vector3 targetDiff;
    public float start;
    private Transform player;
    private Rigidbody rb;

    public string type => "LinearMovement";

    [field: SerializeField]
    public int id { get; set; }

    private void Awake()
    {
        origin = transform.position;
        targetDiff = transform.rotation * target;
        actualTarget = targetDiff + transform.position;
        length = target.magnitude;
        this.rb = GetComponent<Rigidbody>();
        SaveState.Register(this);
    }
    private void OnDestroy() {
        SaveState.Deregister(this);
    }
    private void Start()
    {
        rb.position = (1 - start) * origin + start * actualTarget;
        originalState = State;
    }
    private void LateUpdate()
    {
        if (!enabled) return; // Do not move when disabled
        var currLen = (transform.position - origin).magnitude;
        currLen += speed * Time.fixedDeltaTime;
        if (currLen > length)
        {
            currLen = (2 * length - currLen) * returnSpeed / speed;
            (speed, returnSpeed) = (-returnSpeed, -speed);
        }
        else if (currLen < 0)
        {
            currLen = -currLen * speed / returnSpeed;
            (speed, returnSpeed) = (-returnSpeed, -speed);
        }
        rb.MovePosition((currLen / length) * targetDiff + origin);
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
                speed = speed,
                returnSpeed = returnSpeed
            };
        set {
            rb.velocity = value.velocity;
            rb.angularVelocity = value.angularVelocity;
            rb.position = value.position;
            rb.rotation = value.rotation;
            speed = value.speed;
            returnSpeed = value.returnSpeed;
        }
    }
    [System.Serializable]
    struct MovementState
    {
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public Vector3 position;
        public Quaternion rotation;
        public float speed;
        public float returnSpeed;

    }
}