using UnityEngine;

public class WinBlock : MonoBehaviour {
    private void Win() {
        GameEvents.OnWin.Invoke();
    }
}