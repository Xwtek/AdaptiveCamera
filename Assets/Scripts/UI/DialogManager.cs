using UnityEngine;
using System;

public class DialogManager : MonoBehaviour {
    public GameOverDialog gameOver;
    public MenuDialog menuDialog;
    private void Awake() {
        instance = this;
        gameOver.Init();
        menuDialog.Init();
    }
    private void OnDestroy() {
        instance = null;
        GameEvents.OnDie.RemoveListener(GameOver);
    }
    private void Start() {
        GameEvents.OnDie.AddListener(GameOver);
    }
    private void Update() {
        if(Input.GetButtonDown("Cancel")){
            if(Dialog == null)Dialog = menuDialog;
            else Dialog = null;
        }
    }
    private IDialog _dialog;
    public IDialog Dialog{
        get => _dialog; set
        {
            if(_dialog!=null) _dialog.IsOpen = false;
            _dialog = value;
            if(_dialog!=null) _dialog.IsOpen = true;
        }
    }
    private void GameOver(){
        Dialog = gameOver;
    }
    public static DialogManager instance { get; private set; }
}