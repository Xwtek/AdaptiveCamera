using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System;
using AdaptiveCamera.Util;
using AdaptiveCamera.Data;
using UnityEngine.SceneManagement;
public class PathManager : MonoBehaviour, IConstraint{
    [SerializeField]
    private NativeArray<Line> lines;
    [NonSerialized]
    private NativeArray<bool> _sc;
    [NonSerialized]
    private NativeArray<Line> _rl;
    [NonSerialized]
    private NativeArray<OptimalCameraParam> _cp;
    [NonSerialized]
    private NativeArray<float> _van;
    [SerializeField]
    private Line relevantLine;
    [SerializeField]
    private bool lineExists;
    public Line? RelevantLine {
        get => lineExists ? relevantLine : null as Line?; private set
        {
            if (value.HasValue) relevantLine = value.Value;
            lineExists = value.HasValue;
        }
    }
    [NonSerialized]
    private NativeArray<float2> _br;
    [SerializeField]
    public float2 bestRotation;
    public float2? BestRotation {
        get => lineExists ? bestRotation : null as float2?; private set
        {
            lineExists = value.HasValue;
            if (value.HasValue) bestRotation = value.Value;
        }
    }
    public OptimalCameraParam cameraParam;
    public float vantage;
    public float3 currentRotation;
    private LineJob? job;
    private JobHandle? jobHandle;
    public float minSpeed;
    public float distanceCost;
    public float vantageCost;
    public void Awake(){
        _sc = new NativeArray<bool>(1, Allocator.Persistent);
        _rl = new NativeArray<Line>(1, Allocator.Persistent);
        _br = new NativeArray<float2>(1, Allocator.Persistent);
        _cp = new NativeArray<OptimalCameraParam>(1, Allocator.Persistent);
        _van = new NativeArray<float>(1, Allocator.Persistent);
    }
    public void Start(){
        var tempList = new List<Line>();
        var scene = SceneManager.GetActiveScene();
        foreach (var go in scene.GetRootGameObjects())
        {
            foreach (var segment in go.GetComponentsInChildren<PathSegmentSimple>()){
                tempList.Add(segment.Value);
                Destroy(segment);
            }
            foreach (var segment in go.GetComponentsInChildren<PathSegment>()){
                tempList.Add(segment.Value);
                Destroy(segment);
            }
        }
        lines = tempList.ToNativeArray(Allocator.Persistent);
    }
    float3 prevGroundVelocity;

    public void Update(){
        var groundVelocity = NoahController.Instance.Move;
        if (!jobHandle.HasValue && math.lengthsq(groundVelocity) > minSpeed * minSpeed)
        {
            job = new LineJob
            {
                lines = this.lines,
                relevantLine = _rl,
                bestRotation = _br,
                success = _sc,
                vantage = _van,
                cameraParam = _cp,
                playerPosition = NoahController.Instance.transform.position,
                playerSpeed = groundVelocity,
            };
            jobHandle = job.Value.Schedule();
        }
    }
    public void LateUpdate(){
        if (jobHandle.HasValue && jobHandle.Value.IsCompleted)
        {
            jobHandle.Value.Complete();
            if (_sc[0] )
            {
                this.RelevantLine = _rl[0];
                this.BestRotation = _br[0];
                this.cameraParam = _cp[0];
                this.vantage = _van[0];
            }else{
                this.RelevantLine = null;
                this.BestRotation = null;
            }
            jobHandle = null;
            _sc[0] = false;
        }
    }
    public void OnEnable(){
        AdaptiveCameraBrain.Instance?.constraints.Register(this);
    }
    public void OnDestroy(){
        _br.Dispose();
        _sc.Dispose();
        _rl.Dispose();
        _cp.Dispose();
        _van.Dispose();
        lines.Dispose();
    }
    public void OnDisable(){
        AdaptiveCameraBrain.Instance?.constraints.Deregister(this);
    }
    public float cost;
    public float dot;
    public void UpdateConstraints(){
        Constraints.Clear();
        if(RelevantLine.HasValue && CameraControl.Instance.state == CameraState.Default){
            var rotation = BestRotation.Value;
            dot = math.dot(NoahController.Instance.Move, rotation);
            var constraintData = ConstraintUtil.MakeCameraAngleConstraint(cost, BestRotation.Value, math.min(math.radians(30),NoahController.Instance.MaxRotationPerFrame), ((float3)AdaptiveCameraBrain.Instance.transform.forward).xz);
            currentRotation = math.forward(constraintData.rotation);
            Constraints.Add(constraintData);
            Constraints.Add(NoahController.Instance.reference.MakeDistanceConstraint(cameraParam.distance, distanceCost, 1f));
            switch(cameraParam.followVantage){
                case VantageType.Free:
                    break;
                case VantageType.Follow:
                    Constraints.Add(NoahController.Instance.reference.MakeVantageConstraint(vantage, vantageCost));
                    break;
                case VantageType.Fixed:
                    Constraints.Add(NoahController.Instance.reference.MakeVantageConstraint(math.radians(cameraParam.idealVantage), vantageCost));
                    break;
            }
        }
    }
    public List<ConstraintData> Constraints { get; private set; } = new List<ConstraintData>();
}