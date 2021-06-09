using UnityEngine;
using AdaptiveCamera.Data;
using AdaptiveCamera.Algorithm;
using Unity.Mathematics;
using AdaptiveCamera.Util;
using Unity.Collections;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class AdaptiveCameraBrain : CameraController
{
    public PID cameraPID = new PID();
    public ConstraintCollection constraints = new ConstraintCollection();
    public FocusableCollection focusables = new FocusableCollection();
    [InspectorName("AdaptiveCameraMode")]
    public AdaptiveCameraCoroutine coroutine = new AdaptiveCameraCoroutine(500, 16, 0.75f, 12, 1000 / 24, 1000 / 24);
    public FollowCamera followCamera;
    public TargetAverager averager = new TargetAverager();
    public bool updateWhileSearching;
    private Camera mainCamera;
    private Transform player;
    private TargetAverager miliPerRound = new TargetAverager{length = 20};
    public float? milisecondPerRound => miliPerRound.currentAverage?.x;
    public void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        mainCamera = GetComponent<Camera>();
        player = NoahController.Instance.reference;
        coroutine.Initialize();
        constraints.Register(focusables);
    }
    int frame = 0;
    bool processed = true;
    NativeArray<ConstraintData>? tempConstraints;
    public override void OnCameraUpdate()
    {
        var justProcessed = false;
        if (processed)
        {
            justProcessed = true;
            processed = false;
            coroutine.accumulatedTime = Time.deltaTime;
            var constraintArray = constraints.GetConstraintArray();
            if (constraintArray == null) return;
            StartCoroutine(coroutine.Run(
                mainCamera.transform.position,
                player.position,
                constraintArray.Value));
        }
        frame++;
        coroutine.accumulatedTime += Time.deltaTime;
        if (updateWhileSearching && !justProcessed)
        {
            var newConstraint = constraints.GetTempConstraintArray();
            coroutine.Update(player.position, newConstraint);
        }

        if (coroutine.finished && !coroutine.IsCancelled)
        {
            var target = coroutine.Best.Value.current.xyz;
            //target.w = coroutine.Best.Value.current.fov;
            target = averager.Update(target);
            var moved = mainCamera.transform;
            var origin = ((float3)moved.position).xyz;
            //origin.w = mainCamera.fieldOfView;
            var smooth = cameraPID.Update(target, origin, coroutine.accumulatedTime) + origin;
            //mainCamera.fieldOfView = smooth.w;
            //Debug.Log(smooth.xyz);
            mainCamera.transform.SetPositionAndRotation(smooth.xyz, Quaternion.LookRotation((float3)player.position - smooth.xyz, Vector3.up));
            frame = 0;
            if(followCamera != null) followCamera.ApplyOffset();
            coroutine.accumulatedTime = 0;
            processed = true;
        }
    }
    public void OnDestroy()
    {
        constraints.Deregister(focusables);
        coroutine.Dispose();
        constraints.Dispose();
        tempConstraints?.Dispose();
        Instance = null;
    }

    protected override void OnCameraEnabled()
    {
        if(followCamera!=null) followCamera.ResetOffset();
        processed = true;
    }

    protected override void OnCameraDisabled()
    {
        coroutine.Cancel();
        tempConstraints?.Dispose();
    }

    public static AdaptiveCameraBrain Instance { get; private set; }
}