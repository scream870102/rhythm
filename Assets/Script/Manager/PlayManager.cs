using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    //public info
    [HideInInspector]   //if score is counting it won't plus score
    public bool isCounting;
    [HideInInspector]   //set current combo
    public int combo;
    [HideInInspector]   //detect is comboing from note
    public bool isComboing;
    [HideInInspector]
    public List<string> BUTTON_STRING = new List<string>();
    [HideInInspector]
    public int hitJudge;
    [HideInInspector]
    public float musicTime;
    [HideInInspector]
    public bool isMusicPlay;

    //object refference
    public GameObject notePrefab;
    public List<Material> noteColor;
    public AudioSource audioSource;
    public Text title;
    public Text scoreText;
    public Text comboText;
    public Image cover;
    public SourceLoader sourceLoader;
    public GameObject spawnNoteParent;
    public Image loadScreen;
    public CanvasGroup playUI;
    public CanvasGroup infoUI;
    public Text jugeText;
    public Text percentageText;
    public Animator anim;

    //const
    private const float FADE_VELOCITY = 0.005f;

    //private info
    private bool isReady;   //true start to play music
    private float score;    //to store score
    private List<GameObject> note = new List<GameObject>(); //To store every note
    private float totalScore;
    private bool isGameFin; //detect is Game finish

    void Start() {
        GlobalInfo.G_state = GlobalInfo.STATE_MUSIC_STOPPING;
        loadScreen.gameObject.SetActive(true);
        SetCoverAndSong();
        InitUI();
        Debug.LogWarning("A");
        StartCoroutine(InitScene());
        Debug.LogWarning("B");
        //init boolean
        isReady = false;
        isMusicPlay = false;
        isComboing = false;
        isGameFin = false;
        //init var
        combo = 0;
        totalScore = .0f;
        score = .0f;
        musicTime = -GlobalInfo.G_necessaryTime;
        //Init Button string
        BUTTON_STRING.Add("LClick");
        BUTTON_STRING.Add("MClick");
        BUTTON_STRING.Add("RClick");
        SpawnAllNote();
    }

    void Update() {
        if (musicTime > 0.0f) {
            musicTime = audioSource.time+GlobalInfo.G_playerDelay;
            if (musicTime >= audioSource.clip.length)
                isGameFin = true;
        }
        else if (GlobalInfo.G_state == GlobalInfo.STATE_MUSIC_PLAYING)
            musicTime += Time.deltaTime;
        if (GlobalInfo.G_state == GlobalInfo.STATE_MUSIC_PAUSING)
            audioSource.Pause();
        else if (GlobalInfo.G_state == GlobalInfo.STATE_MUSIC_PLAYING&&!audioSource.isPlaying&&isMusicPlay)
            audioSource.Play();
            
        CountScore();

        //Fade when song Going to play
        if (isReady) {
            anim.SetTrigger("Fade");
            StartCoroutine(PlayMusic());
            isReady = false;
        }


        //if Game is finish set result
        if (isGameFin) {
            GlobalInfo.G_result.maxScore = totalScore;
            GlobalInfo.G_result.score = score;
            GlobalInfo.G_result.sheetName = GlobalInfo.G_sheetToPlay.sheetName;
            GlobalInfo.G_result.maxCombo = GlobalInfo.G_sheetToPlay.note.Count;
            StartCoroutine(GameManager.instance.LoadScene(GlobalInfo.SCENE_GAME_RESULT));
        }
    }

    IEnumerator InitScene() {
        Debug.LogWarning("C");
        yield return new WaitForSeconds(GlobalInfo.LOAD_DELAY);
        Debug.LogWarning("D");
        loadScreen.gameObject.SetActive(false);
        yield return StartCoroutine(Ready());
        Debug.LogWarning("E");
    }

    //call after start is all finish
    IEnumerator Ready() {
        yield return null;
        title.text = GlobalInfo.G_sheetToPlay.sheetName;
        scoreText.text = "0000 ";
        comboText.text = "000";
        percentageText.text = "000%";
        jugeText.text = "";
        isReady = true;
        for (int i = 0; i < GlobalInfo.JUGE_NUM; i++)
            GlobalInfo.G_result.hitJuges[i] = 0;
    }

    //call after ready
    IEnumerator PlayMusic() {
        GlobalInfo.G_state = GlobalInfo.STATE_MUSIC_PLAYING;
        yield return new WaitForSecondsRealtime(GlobalInfo.G_necessaryTime);
        isMusicPlay = true;
        audioSource.Play();
    }

    //count score
    void CountScore() {
        if (isComboing && isCounting) {
            score += (GlobalInfo.G_basicNoteScore * (1 + ((float)combo / GlobalInfo.G_sheetToPlay.note.Count)) * GlobalInfo.G_scoreBonus*GlobalInfo.JUGE_BONUS[hitJudge]);
            if (hitJudge > (int)GlobalInfo.EhitJuge.Good) {
                isComboing = false;
                GlobalInfo.G_result.combo = combo;
                combo = 0;
            }
            GlobalInfo.G_result.combo = combo;
            isCounting = false;
            SetScoreComboUI();
        }
        else if (!isComboing) {
            GlobalInfo.G_result.combo = combo;
            combo = 0;
            SetScoreComboUI();
        }
    }

    //SpawnAllNote from G_sheetToPlay for Play
    void SpawnAllNote() {
        note.Clear();
        for (int i = 0; i < GlobalInfo.G_sheetToPlay.note.Count; i++) {
            note.Add(NoteSpawner.SpawnNote(notePrefab, GlobalInfo.G_sheetToPlay.note[i].enotePos, GlobalInfo.G_sheetToPlay.note[i].enoteType, GlobalInfo.G_sheetToPlay.note[i].noteTime, noteColor[(int)GlobalInfo.G_sheetToPlay.note[i].enotePos]));
            note[note.Count - 1].name = "note" + note.Count;
            note[note.Count - 1].transform.parent = spawnNoteParent.transform;
            note[note.Count - 1].GetComponent<PlayNote>().manager = this;
            note[note.Count - 1].GetComponent<MeshRenderer>().enabled = false;
            note[note.Count - 1].GetComponent<PlayNote>().noteTime = GlobalInfo.G_sheetToPlay.note[note.Count-1].noteTime;
            totalScore += GlobalInfo.G_basicNoteScore * (1+((i+1.0f) / GlobalInfo.G_sheetToPlay.note.Count)) * GlobalInfo.G_scoreBonus;
        }
    }

    //Load Cover And Song
    void SetCoverAndSong() {
        sourceLoader.LoadImage(GlobalInfo.G_sheetToPlay.sheetPhotoPath, ref cover);
        sourceLoader.LoadSong(GlobalInfo.G_sheetToPlay.song.songPath, ref audioSource);
    }

    /// <summary>
    /// UI setting
    /// </summary>
    void InitUI() {
        playUI.alpha = 0.0f;
        infoUI.alpha = 1.0f;
    }

    //Set Score And Combo To Screen
    void SetScoreComboUI() {
        scoreText.text = score.ToString("0000");
        comboText.text = combo.ToString("000");
        jugeText.text = GlobalInfo.JUGE_STRING[hitJudge];
        percentageText.text = (score / totalScore * 100).ToString("000.0")+"%";
    }

}
