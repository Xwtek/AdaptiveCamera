using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class WinDialog : MonoBehaviour, IDialog
{

    public Button next;
    public Button quit;
    public string nextLevel;
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
        quit.onClick.AddListener(ToMainMenu);
        if(string.IsNullOrEmpty(nextLevel)){
            next.gameObject.SetActive(false);
        }else{
            next.onClick.AddListener(Next);
        }
    }
    private void ToMainMenu(){
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
    private void Next(){
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
    }
    private void LateUpdate() {
        if(!IsOpen)gameObject.SetActive(false);
    }
}