using UnityEngine;
using Unity.Collections;
using AdaptiveCamera.Data;
using AdaptiveCamera.Util;
using System.Collections.Generic;
using Unity.Mathematics;

class PlayerConstraint : MonoBehaviour, IConstraint{
    public Camera mainCamera;
    public float distanceCost;
    public float distance;
    //public float angleCost;
    //public float idealDistance;
    public List<ConstraintData> Constraints{ get;  private set; }= new List<ConstraintData>();
    private ConstraintData vantageConstraint;
    private ConstraintData? directionConstraint;
    //private ConstraintData viewpointConstraint;
    public float scale;
    private float idealVantage;
    public float vantageCost;
    public float directionCost;
    public float maxVantagePerSecond;
    public void UpdateConstraints()
    {
        Constraints.Clear();
        Constraints.Add(transform.MakeDistanceConstraint(distance, distanceCost, scale));
        Constraints.Add(transform.MakeVantageConstraint(idealVantage,/* maxVantagePerSecond * (AdaptiveCameraBrain.Instance.milisecondPerRound?? Time.deltaTime)/1000,*/ vantageCost));
        if (math.lengthsq(NoahController.Instance.Move) > 0.01)
        {
            Constraints.Add(ConstraintUtil.MakeCameraAngleConstraint(
                directionCost,
                NoahController.Instance.Move,
                Mathf.PI,
                ((float3)AdaptiveCameraBrain.Instance.transform.forward).xz));
        }
    }
    public void Start(){
        idealVantage = MathUtil.GetCameraAngle(transform.position - AdaptiveCameraBrain.Instance.transform.position).y;
        AdaptiveCameraBrain.Instance?.constraints.Register(this);
    }
    public void Update(){
        if(CameraControl.Instance.state == CameraState.Manual){
            idealVantage = MathUtil.GetCameraAngle(transform.position - AdaptiveCameraBrain.Instance.transform.position).y;
        }
    }
    public void OnEnable()
    {
        AdaptiveCameraBrain.Instance?.constraints.Register(this);
    }
    public void OnDisable()
    {
        AdaptiveCameraBrain.Instance?.constraints.Deregister(this);
    }

}