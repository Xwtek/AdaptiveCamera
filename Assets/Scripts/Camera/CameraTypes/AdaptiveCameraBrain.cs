using UnityEngine;
using AdaptiveCamera.Data;
using AdaptiveCamera.Algorithm;
using Unity.Mathematics;
using AdaptiveCamera.Util;
using Unity.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

[RequireComponent(typeof(Camera))]
public class AdaptiveCameraBrain : CameraController
{
    public ConstraintCollection constraints = new ConstraintCollection();
    public FocusableCollection focusables = new FocusableCollection();
    [InspectorName("AdaptiveCameraMode")]
    public AdaptiveCameraCoroutine coroutine = new AdaptiveCameraCoroutine(500, 16, 0.75f, 12, 1000 / 24, 1000 / 24);
    public FollowCamera followCamera;
    private Transform player;
    private Averager miliPerRound = new Averager { length = 20 };
    public float? milisecondPerRound => miliPerRound.currentAverage;

    bool processed = true;
    NativeArray<ConstraintData>? tempConstraints;
    public void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        player = NoahController.Instance.reference;
        coroutine.Initialize();
        constraints.Register(focusables);
    }
    //public void Update() { coroutine.benchmarking.Toggle(); }
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
                cameraUsed.transform.position,
                player.position,
                constraintArray.Value));
            //smoothing.StartRound(NoahController.Instance.reference.position - transform.position);
        }
        coroutine.accumulatedTime += Time.deltaTime;
        if (!justProcessed)
        {
            var newConstraint = constraints.GetTempConstraintArray();
            coroutine.Update(player.position, newConstraint);
        }

        if (coroutine.finished && !coroutine.IsCancelled)
        {
            var target = coroutine.Best.Value.current.xyz;
            cameraUsed.transform.SetPositionAndRotation(target.xyz, Quaternion.LookRotation((float3)player.position - target.xyz, Vector3.up));
            if (followCamera != null) followCamera.ApplyOffset();
            miliPerRound.Update(coroutine.accumulatedTime);
            coroutine.accumulatedTime = 0;
            processed = true;
            //smoothing.StopRound(NoahController.Instance.reference.position - transform.position);
        }
    }
    public void OnDestroy()
    {
        constraints.Deregister(focusables);
        coroutine.Dispose();
        constraints.Dispose();
        Instance = null;
    }

    protected override void OnCameraEnabled()
    {
        if (followCamera != null) followCamera.ResetOffset();
        processed = true;
    }

    protected override void OnCameraDisabled()
    {
        coroutine.Cancel();
    }
    public void ResetCamera()
    {
        coroutine.Cancel();
        if (followCamera != null) followCamera.ResetOffset();
        processed = true;
    }

    public static AdaptiveCameraBrain Instance { get; private set; }
        [System.Serializable]
        public  class Smoothing
        {
            public bool Test;
            public bool Write;
            public string writeTo;
            public Stopwatch stopwatch;
            public List<float> datas = new List<float>();
            float3 origin;
            public void StopRound(float3 currSituation)
            {
                if (!Test) goto write;
                if(stopwatch == null) goto write;
                stopwatch.Stop();
                var result = math.length(currSituation - origin) / stopwatch.ElapsedMilliseconds;
                if(result == 0 || !math.isfinite(result)) goto write;
                datas.Add(result);
                stopwatch = null;
            write:
                if(Write && !string.IsNullOrEmpty(writeTo)){
                    using var file = new StreamWriter(writeTo);
                    foreach(var data in datas){
                        file.Write(data);
                        file.WriteLine();
                    }
                    file.Flush();
                    datas.Clear();
                }
                Write = false;
            }
            public void StartRound(float3 origin)
            {
                if(!Test) return;
                stopwatch = new Stopwatch();
                stopwatch.Start();
                this.origin = origin;
            }
        }
        //public Smoothing smoothing = new Smoothing();
}