using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using AdaptiveCamera.Util;
using AdaptiveCamera.Data;
[System.Serializable]
public class Focusable : MonoBehaviour, IConstraint
{
    private Collider mainCollider;
    public Renderer m_Renderer;
    internal float sortOrder;
    public bool CanBeFocused{ get; private set; }
    private bool potentialFocusable;
    public float minDistance;
    public bool needToBeVisible = true;
    public bool focused = false;
    // Start is called before the first frame update
    void Start()
    {
        mainCollider = GetComponent<Collider>();
        AdaptiveCameraBrain.Instance.focusables.Register(this);
        AdaptiveCameraBrain.Instance.constraints.Register(this);
    }
    // Update is called once per frame
    void Update()
    {
        var distance = transform.position - NoahController.Instance.transform.position;
        distance.y = 0;
        if(distance.sqrMagnitude < minDistance * minDistance) potentialFocusable = false;
        else if(!this.needToBeVisible) potentialFocusable = true;
        else if(m_Renderer != null && !m_Renderer.isVisible) potentialFocusable = false;
        else potentialFocusable = true;
    }
    private void FixedUpdate() {
        if(!potentialFocusable) CanBeFocused = false;
        else{
            var playerLoc = AdaptiveCameraBrain.Instance.transform.position;
            var dir = transform.position - playerLoc;
            if(Physics.Raycast(playerLoc, dir.normalized, out var hitInfo, dir.magnitude*1.1f, ~4, QueryTriggerInteraction.Ignore)){
                CanBeFocused = hitInfo.collider == mainCollider;
            }else{
                CanBeFocused = false;
            }
        }
    }
    void OnEnable(){
        AdaptiveCameraBrain.Instance?.focusables.Register(this);
        AdaptiveCameraBrain.Instance?.constraints.Register(this);
    }
    void OnDisable(){
        potentialFocusable = false;
        CanBeFocused = false;
        AdaptiveCameraBrain.Instance?.focusables.Deregister(this);
        AdaptiveCameraBrain.Instance?.constraints.Deregister(this);
    }
    public int ConstraintCount => focused ? 1 : 0;
    public List<ConstraintData> Constraints { get; private set; } = new List<ConstraintData>();
    public void UpdateConstraints(){
        Constraints.Clear();
        if(focused && enabled){
            Constraints.Add(
                ConstraintUtil.MakeCameraAngleConstraint(
                AdaptiveCameraBrain.Instance.focusables.cost,
                ((float3)transform.position - (float3)AdaptiveCameraBrain.Instance.transform.position).xz,
                math.min(30, NoahController.Instance.MaxRotationPerFrame),
                ((float3)AdaptiveCameraBrain.Instance.transform.forward).xz)
            );
        }
    }
}
