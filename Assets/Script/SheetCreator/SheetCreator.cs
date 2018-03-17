using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class SheetCreator : MonoBehaviour {
    //ref for gameObject
    public SourceLoader sourceLoader;
    public GameObject spawnNotePrefab;
    public List<Material> noteColor;
    public GameObject noteEditModeUI;
    public GameObject informationModeUI;
    public GameObject noteEditGameObject;
    public GameObject spawnNoteParent;

    //ref for UI
    public AudioSource music;
    public Image cover;
    public Text infoText;
    public Text titleText;
    public Text bpmText;
    public Text authorText;
    public Text sheetNameText;
    public Slider slider;
    public Text bpmPreText;
    public Text authorPreText;
    public Text sheetNamePreText;
    public Text coverPathText;
    public Text musicPathText;
    public Text sheetPathText;
    public Text coverPathPreText;
    public Text musicPathPreText;
    public Text sheetPathPreText;

    //const
    private const string INFO_MODE = "information";
    private const string NE_MODE = "noteEdit";
    private const float LOAD_DELAY = 2.0f;
    
    //private var
    private GlobalInfo.SheetInfo sheet = new GlobalInfo.SheetInfo();
    private string sheetPath;  //store sheet path
    private bool isSongLoad;    //check is music load correctly
    private bool isSheetLoad;   //check is Sheet Load external and make note start to spawn turn to false when all note already Spawn
    private bool isSheetLoadExternal;   //if Sheet Loaded External change the way to save Button click event
    private string mode = "";   //store current mode
    private SpawnNote currentSelect=null;   //store which note current select by mouse
    private List<GameObject> note = new List<GameObject>(); //add all note to list
    private Ray ray;
    private RaycastHit hit;

    ////////////
    private float noteDur;

    // Use this for initialization
    void Start () {
        isSheetLoad = false;
        isSongLoad = false;
        isSheetLoadExternal = false;
        mode = NE_MODE;
        SwitchMode(mode);
    }
	
	// Update is called once per frame
	void Update () {

        if (isSongLoad&&mode==NE_MODE)
            UpdateSlider();

        if (isSheetLoad) {
            SpawnAllNote();
        }

        //detect what we hit
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.gameObject.tag == "Note") {
                if (currentSelect != null)
                    currentSelect.isSelect = false;
                currentSelect = hit.collider.gameObject.GetComponent<SpawnNote>();
                hit.collider.gameObject.GetComponent<SpawnNote>().isSelect = true;
            }
        }

        //detect to spawn note
        if (Input.GetButtonDown(GlobalInfo.B_SPAWN)) {
            note.Add(SpawnNote(spawnNotePrefab, GlobalInfo.EnotePos.M, GlobalInfo.EnoteType.instant, music.time, noteColor[(int)GlobalInfo.EnotePos.M],music.time));
            note[note.Count - 1].name = "note" + (note.Count).ToString();
            note[note.Count - 1].GetComponent<SpawnNote>().index = note.Count ;
            noteDur = music.time;
        }
        //if (Input.GetButtonUp(GlobalInfo.B_SPAWN)) {
        //    if (music.time - noteDur > GlobalInfo.NOTE_TIME_OFFSET*2) {
        //        note[note.Count - 1].GetComponent<SpawnNote>().enoteType = GlobalInfo.EnoteType.continuousStart;
        //        note.Add(SpawnNote(spawnNotePrefab,GlobalInfo))
        //    }
                
        //}

        //detect to delete note ref from note list
        if (GlobalInfo.G_isNoteDelete&&GlobalInfo.G_deleteNoteIndex!=0) {
            note.RemoveAt(GlobalInfo.G_deleteNoteIndex-1);
            GlobalInfo.G_deleteNoteIndex = 0;
        }


    }

    void SpawnAllNote() {
        note.Clear();
        for (int i = 0; i < sheet.note.Count; i++) {
            note.Add(NoteSpawner.SpawnNote(spawnNotePrefab, sheet.note[i].enotePos, sheet.note[i].enoteType, sheet.note[i].noteTime, noteColor[(int)sheet.note[i].enotePos]));
            note[note.Count - 1].GetComponent<SpawnNote>().index = note.Count;
            note[note.Count - 1].name = "note" + note.Count;
            note[note.Count - 1].transform.parent = spawnNoteParent.transform;
        }
        isSheetLoad = false;
    }

    //////////////////////////////////////
    //UI click function
    //////////////////////////////////////
    //call when play Click/
    public void PlayButtonClick() {
        music.Play();
        GlobalInfo.G_state = GlobalInfo.STATE_MUSIC_PLAYING;
    }

    //call when pause Click/
    public void PauseButtonClick() {
        music.Pause();
        GlobalInfo.G_state = GlobalInfo.STATE_MUSIC_PAUSING;
    }

    //call when pause Click/
    public void StopButtonClick() {
        music.Pause();
        GlobalInfo.G_state = GlobalInfo.STATE_MUSIC_STOPPING;
    }

    //call when CoverLoadClick/
    public void LoadCoverButtonClick() {
        sheet.sheetPhotoPath = coverPathText.text;
        AddInfo("Try to open " + Path.GetFileName(sheet.sheetPhotoPath),true);
        sourceLoader.LoadImage(sheet.sheetPhotoPath, ref cover);
    }

    //call when LoadSongClick/
    public void LoadSongButtonClick() {
        sheet.song.songPath = musicPathText.text;
        AddInfo("Try to open " + Path.GetFileName(sheet.song.songPath),true);
        if (Path.GetExtension(sheet.song.songPath) != ".ogg")
            AddInfo("Wrong music Type please choose which extension is ogg");
        else {
            sourceLoader.LoadSong(sheet.song.songPath, ref music);
            isSongLoad = true;
            sheet.song.songName = sheet.song.songPath;
            titleText.text = sheet.song.songName;
        }
    }

    //call when save Click/
    public void SaveButtonClick() {
        SetSheetFileForSave();
        sourceLoader.SaveSheetToFile(ref sheet);
        AddInfo(GlobalInfo.DEFAULT_SHEET_PATH + sheet.sheetName + GlobalInfo.FILE_EXTENSION,true);
    }

    //call when Load button click
    //set sheet to Edit and sheetPath musicPath isSheetLoad/
    public void LoadSheetButtonClick() {
        sheetPath = sheetPathText.text;
        isSheetLoadExternal = true;
        AddInfo("Try to open " + Path.GetFileName(sheetPath),true);
        if (Path.GetExtension(sheetPath) != ".sheet")
            AddInfo("Wrong Type Please choose which extension is sheet");
        else {
            GlobalInfo.SheetInfo tempSheet = new GlobalInfo.SheetInfo();
            tempSheet = sourceLoader.GetSheetFile(sheetPath);
            if (tempSheet == null) {
                AddInfo("Incorrect File Type Please select another");
                return;
            }
            else {
                sheet = tempSheet;
                SetSheetFileToUI();
                isSongLoad = true;
                isSheetLoad = true;
                UpdateSlider();
            }
        }
    }

    //switch between NoteEditMode and InfoMode
    public void SwitchButoonClick() {
        SwitchMode(mode);
    }

    //if Slider Value is being chnage Update it and set Current music Time due to slider
    public void TimeSlideVaueChnage(Slider slider) {
        if (isSongLoad) {
            music.time = slider.value;
            UpdateSlider();
        }
    }

    //////////////////////////////////
    //UI set Function
    /////////////////////////////////
    void SwitchMode(string value) {
        if (value == NE_MODE) {
            noteEditGameObject.SetActive(false);
            noteEditModeUI.SetActive(false);
            spawnNoteParent.SetActive(false);
            informationModeUI.SetActive(true);
            mode = INFO_MODE;
        }
        else if (value == INFO_MODE) {
            informationModeUI.SetActive(false);
            noteEditGameObject.SetActive(true);
            noteEditModeUI.SetActive(true);
            spawnNoteParent.SetActive(true);
            mode = NE_MODE;
        }
    }

    //Update Slider Value to UI
    void UpdateSlider() {
        slider.minValue = 0.0f;
        slider.maxValue = music.clip.length;
        slider.value = music.time;
    }

    //Add Text to Info Text
    void AddInfo(string value,bool toClean=false) {
        if (toClean)
            infoText.text = "" + value;
        else
            infoText.text = value+"\n"+infoText.text;
    }

    //call when sheet is loaded external and set Info to UI
    void SetSheetFileToUI() {
        sourceLoader.LoadSong(sheet.song.songPath, ref music);
        sourceLoader.LoadImage(sheet.sheetPhotoPath, ref cover);
        StartCoroutine(SetSheetFileToUICoroutine());
    }

    //Coroutine of SetSheetFileToUI
    IEnumerator SetSheetFileToUICoroutine() {
        yield return new WaitForSeconds(LOAD_DELAY);
        bpmPreText.text = sheet.song.bpm.ToString();
        sheetNamePreText.text = sheet.sheetName;
        authorPreText.text = sheet.author;
        musicPathPreText.text = sheet.song.songPath;
        coverPathPreText.text = sheet.sheetPhotoPath;
        titleText.text = Path.GetFileNameWithoutExtension(sheet.song.songPath);
        AddInfo("Song: " + Path.GetFileNameWithoutExtension(sheet.song.songPath) + "\n" + "Author: " + sheet.author + "\n SheetName: " + sheet.sheetName + "\n BPM: " + sheet.song.bpm);
    }

     // check info before save
    void SetSheetFileForSave() {
        if (isSheetLoadExternal) {
            if (sheet.sheetPhotoPath.Length <= 1)
                AddInfo("No cover select", true);
            if (sheet.song.songPath.Length <= 1)
                AddInfo("No song select", true);
        }
        else {
            sheet.sheetName = sheetNameText.text;
            sheet.author = authorText.text;
            if (sheet.song.bpm != 0)
                sheet.song.bpm = sheet.song.bpm;
            else
                sheet.song.bpm = int.Parse(bpmText.text);
            sheet.song.length = music.clip.length;
            sheet.song.songName = sheet.song.songPath;
            if (sheet.sheetPhotoPath.Length <= 1)
                AddInfo("No cover select", true);
            if (sheet.song.songPath.Length <= 1)
                AddInfo("No song select", true);
        }
        //note info
        sheet.note.Clear();
        for (int i = 0; i < note.Count; i++) {
            GlobalInfo.NoteInfo temp = new GlobalInfo.NoteInfo();
            temp.enotePos = note[i].GetComponent<BasicNote>().enotePos;
            temp.enoteType = note[i].GetComponent<BasicNote>().enoteType;
            temp.noteTime = note[i].GetComponent<BasicNote>().noteTime;
            sheet.note.Add(temp);
        }
    }

    //Spawn Note And set Pos due to current Music time
    public GameObject SpawnNote(GameObject notePrefab, GlobalInfo.EnotePos enotePos, GlobalInfo.EnoteType enoteType, float noteTime, Material noteColor,float currentTime) {
        GameObject note;
        float spawnZ = 0;
        Vector3 spawnPos =  new Vector3(GlobalInfo.NOTE_SPAWN_POS[(int)enotePos].x, GlobalInfo.NOTE_SPAWN_POS[(int)enotePos].y, spawnZ);
        note = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        note.transform.parent = spawnNoteParent.transform;
        note.GetComponent<MeshRenderer>().material = noteColor;
        note.GetComponent<BasicNote>().enotePos = enotePos;
        note.GetComponent<BasicNote>().enoteType = enoteType;
        note.GetComponent<BasicNote>().noteTime = noteTime;
        return note;
    }
}
