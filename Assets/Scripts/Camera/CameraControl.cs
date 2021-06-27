using System;
using UnityEngine;

public class CameraControl : MonoBehaviour, ISaveable
{
    public CameraState state;
    private AdaptiveCameraBrain brain;
    private FixedCamera fixedCamera;
    private FollowCamera followCamera;
    private ShootCamera shootCamera;
    private Transform player;
    private CameraController _activeCam;
    public CameraController ActiveCamera
    {
        get => _activeCam;
        set
        {
            if (_activeCam == value) return;
            if(_activeCam != null) _activeCam.CameraEnabled = false;
            value.CameraEnabled = true;
            if (_activeCam != null && _activeCam.cameraUsed != value.cameraUsed)
            {
                _activeCam.cameraUsed.enabled = false;
                value.cameraUsed.enabled = true;
            }
            _activeCam = value;

        }
    }
    private void Awake(){
        Instance = this;
    }
    private void Start()
    {
        brain = GetComponent<AdaptiveCameraBrain>();
        fixedCamera = GetComponent<FixedCamera>();
        followCamera = GetComponent<FollowCamera>();
        shootCamera = GetComponent<ShootCamera>();
        player = NoahController.Instance.transform;
    }
    public void Update()
    {
        followCamera.CameraUpdate();
        switch (state)
        {
            case CameraState.Default:
                if (Input.GetButton("CameraHorizontal") || Input.GetButton("CameraVertical") || Input.GetButton("CameraFOV"))
                {
                    ToManual();
                }
                else if (Input.GetButtonDown("Focus"))
                {
                    ToFocused();
                }
                else{
                    ToDefault();
                }
                break;
            case CameraState.Manual:
                if (Input.GetButtonDown("Focus"))
                {
                    ToFocused();
                }else
                {
                    ToManual();
                }
                break;
            case CameraState.Focused:
                if (Input.GetButtonDown("Focus"))
                {
                    ToDefault();
                    break;
                }
                else if (Input.GetButtonDown("CameraVertical"))
                {
                    if (Input.GetAxisRaw("CameraVertical") < -0.1)
                    {
                        brain.focusables.Prev(player.position);
                    }
                    else
                    {
                        brain.focusables.Next(player.position);
                    }
                    break;
                }
                brain.focusables.Maintain(player.position);
                if(brain.focusables.Current == null) ToDefault();
                break;
            case CameraState.Shoot:
                if (Input.GetButtonDown("Switch"))
                {
                    ToDefault();
                    break;
                }
                else if (Input.GetButtonDown("Focus"))
                {
                    ToFocused();
                    break;
                }
                ToShoot();
                break;
        }
        ActiveCamera.OnCameraUpdate();
    }
    public void ToDefault(){
        brain.focusables.Defocus();
        state = CameraState.Default;
        ActiveCamera = brain;
    }
    public void ToFocused(){
        brain.focusables.Focus(player.position);
        if(brain.focusables.Current ==null)ToDefault();
        else
        {
            state = CameraState.Focused;
            ActiveCamera = brain;
        }
    }
    public void ToManual(){
        state = CameraState.Manual;
        ActiveCamera = fixedCamera;
    }
    public void ToShoot(){
        state = CameraState.Shoot;
        ActiveCamera = shootCamera;
    }
    void ISaveable.Load(SaveItemReader reader)
    {
        state = reader.Load<CameraState>("State");
        transform.SetPositionAndRotation(reader.Load<Vector3>("Position"), reader.Load<Quaternion>("Rotation"));
    }
    void ISaveable.Save(SaveItemWriter writer)
    {
        writer.Save("State", state);
        writer.Save("Position", transform.position);
        writer.Save("Rotation", transform.rotation);
    }
    void ISaveable.Restore()
    {
        throw new System.NotImplementedException();
    }
    public static CameraControl Instance{ get; private set; }
    public string type => "CameraControl";
    [field: SerializeField]
    public int id { get; set; }
}
[Serializable]
public enum CameraState
{
    Default,
    Focused,
    Manual,
    Shoot,
}