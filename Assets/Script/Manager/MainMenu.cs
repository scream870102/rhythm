using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Open Play
    public void openPlay() {
        StartCoroutine(GameManager.instance.LoadScene(GlobalInfo.SCENE_SONG_SELECT));
    }
    //Open Create
    public void openCreate() {
        StartCoroutine(GameManager.instance.LoadScene(GlobalInfo.SCENE_SHEET_CREATE));
    }
    //Open Setting
    public void openSetting() {
        StartCoroutine(GameManager.instance.LoadScene(GlobalInfo.SCENE_SETTING));
    }
}
