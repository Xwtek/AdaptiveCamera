using UnityEngine;

using UnityEngine;

public class WinPoint : MonoBehaviour {
    public void Win(){
        GameEvents.OnWin.Invoke();
    }
}