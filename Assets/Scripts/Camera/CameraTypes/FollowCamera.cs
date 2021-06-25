using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform mainCamera;
    private Transform player;
    private Vector3? prevPosition;
    private Vector3 sum = Vector3.zero;
public void CameraUpdate()
{
    if (prevPosition.HasValue)
    {
        var delta = player.position - prevPosition.Value;
        sum += delta;
        mainCamera.position += delta;
    }
    prevPosition = player.position;
}
    private void Start() {
        player = NoahController.Instance.transform;
    }
    public void ApplyOffset()
    {
        mainCamera.position += sum;
        ResetOffset();
    }
    public void ResetOffset()
    {
        sum = Vector3.zero;
        prevPosition = null;
    }
}