using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCharacterAnimsFREE;
using Unity.Mathematics;
using RPGCharacterAnimsFREE.Actions;
using AdaptiveCamera.Util;

public class NoahController : MonoBehaviour, ISaveable{
    public float2 Move { get; private set; }
    public float maxRotationSpeed;
    public float MaxRotationPerFrame => (AdaptiveCameraBrain.Instance.milisecondPerRound?? Time.deltaTime) * Mathf.Deg2Rad * maxRotationSpeed / 1000;
    public static NoahController Instance { get; private set; }

    public string type =>  "Character";
    [SerializeField]
    public int _id;

    public int id { get => _id; set => _id=value; }

    public Transform reference;
    public RPGCharacterController controller;
    public EnemyKiller sword;
    public GameObject shotPrefab;
    public void Awake(){
        Instance = this;
        GameEvents.OnDie.AddListener(Die);
        SaveState.Register(this);
    }
    private void Start()
    {
        controller = GetComponent<RPGCharacterController>();
        SetUpRestore();
    }
    void Update()
    {
        Moving(new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        if(Input.GetButtonDown("Switch")) Switching();
        if(Input.GetButtonDown("Jump")) Action();
        if(Input.GetButtonDown("Shoot")) Shot();
        if(transform.position.y < -5 && !dead) GameEvents.OnDie.Invoke();
    }
    void Moving(float2 input){
        if(dead||CameraControl.Instance.state == CameraState.Shoot){
            controller.SetMoveInput(Vector3.zero);
            return;
        }
        controller.SetMoveInput(input.xyy);
        var forward = (float3)AdaptiveCameraBrain.Instance.transform.forward;
        var right = (float3)AdaptiveCameraBrain.Instance.transform.right;
        Move = math.mul(new float2x2(right.xz, forward.xz), input);
    }
    public int state = 0; // 0 = empty, 1 = sword, 2 = gun
    void Switching(){
        if(dead) return;
        if (!controller.CanStartAction("SwitchWeapon")) { return; }
        SwitchWeaponContext context = new SwitchWeaponContext();
        context.leftWeapon = -1;
        context.rightWeapon = -1;
        context.type = "Switch";
        context.side = "None";
        state++; state %= 2;
        switch(state){
            case 0:
                context.rightWeapon = 0;
                break;
            case 1:
                context.rightWeapon = (int)Weapon.TwoHandSword;
                break;
            default:
                Debug.LogError("State not recognized: " + state);
                Debug.Break();
                break;
        }
        controller.StartAction("SwitchWeapon", context);
    }
    IEnumerator SwordPower(){
        yield return new WaitForSeconds(0.2f);
        sword.enabled = true;
        yield return new WaitForSeconds(0.9f);
        sword.enabled = false;
    }
    void Action(){
        if(dead) return;
        switch(state){
            case 0:
                var jmp = Input.GetAxis("Jump") * Vector3.up;
                // jump;
                controller.SetJumpInput(jmp);
                // If we pressed jump button this frame, jump.
                if (Input.GetButtonDown("Jump") && controller.CanStartAction("Jump")) {
                    controller.StartAction("Jump");
                }
                break;
            case 1:
                if (!controller.CanStartAction("Attack")) { return; }
                // attack with a sword
                controller.StartAction("Attack", new AttackContext("Attack", "Left"));
                StartCoroutine(SwordPower());
                break;
            default:
                Debug.LogError("State not recognized: " + state);
                Debug.Break();
                break;

        }
    }
    void Shot(){
        switch(CameraControl.Instance.state){
            case CameraState.Focused:
                var target = AdaptiveCameraBrain.Instance.focusables.Current.transform.position;
                var forwardDir = target - reference.position;
                Instantiate(shotPrefab, reference.position, Quaternion.LookRotation(forwardDir,math.up()));
                return;
            case CameraState.Shoot:
                Instantiate(shotPrefab, reference.position, reference.rotation);
                return;
            default:
                CameraControl.Instance.ToShoot();
                return;
        }
    }
    bool dead=false; 
    public void Die(){
        if(dead) return;
        dead = true;
        controller.Death();
    }
    private void OnDestroy() {
        GameEvents.OnDie.RemoveListener(Die);
        SaveState.Deregister(this);
    }

    public void Load(SaveItemReader reader)
    {
        transform.SetPositionAndRotation(reader.Load<Vector3>("Position"), reader.Load<Quaternion>("Rotation"));
        if (dead)
        {
            controller.Revive();
            dead = false;
        }
    }

    public void Save(SaveItemWriter writer)
    {
        writer.Save<Vector3>("Position", transform.position);
        writer.Save<Quaternion>("Rotation", transform.rotation);
        writer.Save("Weapon", state);
    }
    float3 originalPos;
    quaternion originalRotation;
    public void Restore()
    {
        dead = false;
        transform.position = originalPos;
        transform.rotation = originalRotation;
        controller.StartAction("SwitchWeapon", new SwitchWeaponContext
        {
            leftWeapon = -1,
            rightWeapon = 0,
            type = "Switch",
            side = "None"
        });
    }
    private void SetUpRestore(){
        originalPos = transform.position;
        originalRotation = transform.rotation;
    }
}
