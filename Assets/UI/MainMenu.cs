using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public ButtonGroup[] buttongroups;
    void Start()
    {
    }
    public void Quit(){
        Application.Quit();
    }
    void Load(){
    }
    public void GotoLevel(string levelName){
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }
    public void ToggleGroup(int idx){
        for (int i = 0; i < buttongroups.Length; i++){
            foreach(var button in buttongroups[i].buttons){
                button.gameObject.SetActive(i == idx);
            }
        }
    }
}

[System.Serializable]
public class ButtonGroup
{
    public Button[] buttons;
}