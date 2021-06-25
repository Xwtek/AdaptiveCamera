using UnityEngine;
using Unity.Mathematics;
using AdaptiveCamera.Util;
public class ShootCamera : CameraController
{    
    public float2 cameraSpeed = new float2(0.1f, 0.1f);
    [Range(-90, 90)]
    public float maxVerticalAngle = 45;
    [Range(-90, 90)]
    public float minVerticalAngle = -45;
    public Quaternion originalRotation;
protected override void OnCameraDisabled()
{
    cameraUsed.transform.rotation = originalRotation;
}
protected override void OnCameraEnabled()
{
    originalRotation = cameraUsed.transform.rotation;
}
public override void OnCameraUpdate()
{
    var input = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    var angle = MathUtil.GetCameraAngle(cameraUsed.transform.forward) + input * cameraSpeed;
    angle.y = Mathf.Clamp(angle.y, math.radians(minVerticalAngle), math.radians(maxVerticalAngle));
    if(angle.x>math.PI) angle.x -= math.PI * 2;
    if(angle.x<-math.PI) angle.x += math.PI * 2;
    cameraUsed.transform.forward = MathUtil.FromCameraAngle(angle);
}
}