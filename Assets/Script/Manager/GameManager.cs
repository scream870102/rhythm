using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const float LOAD_DELAY = 3.0f;
    public static GameManager instance = null;
    public GameObject inGameMenu;
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

    }
    void Start() {
        SourceLoader.GetSettingFileAndSet();
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            GamePauseButtonClick();
    }
    //Load Scene Function
    public IEnumerator LoadScene(string sceneName) {
        if (SceneManager.GetActiveScene().name != GlobalInfo.G_currentScene)
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        GlobalInfo.G_currentScene = sceneName;
        yield return new WaitForSeconds(0.3f);
    }

    //When Player Press esc will call
    public void GamePauseButtonClick() {
        GlobalInfo.G_state = GlobalInfo.STATE_MUSIC_PAUSING;
        inGameMenu.SetActive(true);
        Time.timeScale = .0f;
    }

    //Call when Player Press Resume
    public void GameResumeButtonClick() {
        Time.timeScale = 1.0f;
        inGameMenu.SetActive(false);
        GlobalInfo.G_state = GlobalInfo.STATE_MUSIC_PLAYING;
    }

    //Call when player press menu
    public void MenuButtonClick() {
        //inGameMenu = GameObject.Find("InGameMenu");
        StartCoroutine(instance.LoadScene(GlobalInfo.SCENE_MAIN));
        GlobalInfo.G_state = GlobalInfo.STATE_MUSIC_STOPPING;
        Time.timeScale = 1.0f;
        inGameMenu.SetActive(false);
    }

}
