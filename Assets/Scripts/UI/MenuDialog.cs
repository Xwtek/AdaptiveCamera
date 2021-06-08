using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class MenuDialog : MonoBehaviour, IDialog
{

    public Button resume;
    public Button lastCheckpoint;
    public Button restart;
    public Button quit;
    private Animator _anim;
    private Animator animator{
        get
        {
            if (_anim == null) _anim = GetComponent<Animator>();
            return _anim;
        }
    }
    private EventSystem evt;
    public bool IsOpen {
        get => animator.GetBool("Open"); set
        {
            //evt. = value ? tryAgain.gameObject : (GameObject)null;
            if (value) gameObject.SetActive(true);
            animator.SetBool("Open", value);
        }
    }
    public void Init()
    {
        lastCheckpoint.onClick.AddListener(TryAgain);
        quit.onClick.AddListener(ToMainMenu);
        resume.onClick.AddListener(Resume);
        restart.onClick.AddListener(Restart);
    }
    private void ToMainMenu(){
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
    private void TryAgain(){
        SaveState.Load();
        DialogManager.instance.Dialog = null;
    }
    private void Restart(){
        SaveState.Restore();
        DialogManager.instance.Dialog = null;
    }
    private void Resume(){
        DialogManager.instance.Dialog = null;
    }
    private void LateUpdate() {
        if(!IsOpen)gameObject.SetActive(false);
    }
}