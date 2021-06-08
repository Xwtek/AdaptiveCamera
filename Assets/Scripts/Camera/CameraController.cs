using UnityEngine;
public abstract class CameraController : MonoBehaviour {
    public Camera cameraUsed;
    private bool _enabled;
    public bool CameraEnabled{
        get => _enabled;
        set{
            if(_enabled == value) return;
            _enabled = value;
            if(value) OnCameraEnabled();
            else OnCameraDisabled();
        }
    }
    protected abstract void OnCameraEnabled();
    protected abstract void OnCameraDisabled();
    public abstract void OnCameraUpdate();
}