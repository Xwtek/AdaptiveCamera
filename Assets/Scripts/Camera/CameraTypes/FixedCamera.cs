using UnityEngine;
using AdaptiveCamera.Util;
using Unity.Mathematics;
[System.Serializable]
public class FixedCamera : CameraController {    
    public float2 cameraSpeed = new float2(0.1f, 0.1f);
    [Range(-90, 90)]
    public float maxVerticalAngle = 45;
    [Range(-90, 90)]
    public float minVerticalAngle = -45;
    [Range(0,180)]
    public float minFOV = 10;
    [Range(0,180)]
    public float maxFOV = 170;
    public float fovSpeed = 1;
    private Transform player;
    private void Start() {
        player = NoahController.Instance.reference;
    }

public override void OnCameraUpdate(){
    if(!CameraEnabled) return;
    // Handle freeform camera movement.
    var normalizedPosition = cameraUsed.transform.position - player.position;
    var length = normalizedPosition.magnitude;
    var rotation = MathUtil.GetCameraAngle(normalizedPosition);
    var delta = new float2(Input.GetAxis("CameraHorizontal"), Input.GetAxis("CameraVertical"));
    rotation += delta * cameraSpeed;
    if(rotation.x > 2* math.PI) rotation.x -= 2 * math.PI;
    rotation.y = math.clamp(rotation.y, math.radians(minVerticalAngle), math.radians(maxVerticalAngle));
    normalizedPosition = MathUtil.FromCameraAngle(rotation)*length;
    var lookAt = Quaternion.LookRotation(-normalizedPosition, Vector3.up);
    
    cameraUsed.transform.SetPositionAndRotation(normalizedPosition + player.position, lookAt);
    
    cameraUsed.fieldOfView = Mathf.Clamp(cameraUsed.fieldOfView + Input.GetAxis("CameraFOV") * fovSpeed, minFOV, maxFOV);
}
    protected override void OnCameraEnabled()
    {
        // No special instruction
    }
    protected override void OnCameraDisabled()
    {
        // No special instruction
    }
}