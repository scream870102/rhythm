using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
//using UnityEditor;
using UnityEngine.SceneManagement;

public class SheetLoader : MonoBehaviour {
    //const
    private const float SCROLL_SPEED = 6.0f;
    private const float SCROLL_SENSTIVE = 50.0f;
    private const int DEFAULT_VISIBLE_NUM = 3;
    private const int LEFT_REF_INDEX = 3;
    private const int RIGHT_REF_INDEX = 4;
    private const int FONT_SIZE = 70;
    private const int MIDDLE_SHEET = 1;
    private Vector2 FONT_HW = new Vector2(500, 100);
    private Vector2 COVER_HW = new Vector2(500, 500);
    private Vector3 UI_SCALE = new Vector3(1, 1, 1);

    //ref object
    public GameObject titleParent;
    public GameObject coverParent;
    public Font fontUI;
    public SourceLoader sourceLoader;
    public Image loadScreen;

    //private property and fields
    private List<GlobalInfo.SheetInfo> loadSheets = new List<GlobalInfo.SheetInfo>();     //Get every sheet from DEFAULT_SHEET_PATH
    private List<SheetPicker> sheet = new List<SheetPicker>();      //Set Every sheet Get To Sheet Picker
    private List<Vector3> coverPos;     //ref Cover position
    private List<Quaternion> UIRot;     //ref UI rotation
    private List<Vector3> titlePos;     //ref title position
    private bool isMouseScroll;     //detect mouse wheel is change
    private int dir;        //determine which direction user choose

    //Include all sheet and set their position and rotation
    void Start () {
        loadScreen.gameObject.SetActive(true);
        StartCoroutine(InitScene());
        loadSheets.AddRange(GetAllSaveFiles());
        InitSheetPicker();
        InitRefRotAndPos();
        InitAllPosAndRot();
        isMouseScroll = false;
    }

    IEnumerator InitScene() {
        yield return new WaitForSeconds(GlobalInfo.LOAD_DELAY);
        loadScreen.gameObject.SetActive(false);
    }

    //Keep Get Action from user mouse wheel and set sheetPicker
    void Update()
    {
        //set dir
        if (Input.GetAxis("MouseScrollWheel") > 0f && !isMouseScroll)
        {
            isMouseScroll = true;
            dir = 1;
        }
        else if (Input.GetAxis("MouseScrollWheel") < 0f && !isMouseScroll)
        {
            isMouseScroll = true;
            dir = 2;
        }
        else
        {
            dir = 0;
        }

        //set sheetPicker
        SetNext();
        AttemptToMove();
        CkeckMouseScroll();
    }

    //call when playButton is clicked
    public void PlayButtonClick()
    {
        for(int i = 0; i < sheet.Count; i++)
        {
            if(sheet[i].pos==MIDDLE_SHEET)
            {
                GlobalInfo.G_sheetToPlay = loadSheets[sheet[i].sheetNum];
                StartCoroutine(GameManager.instance.LoadScene(GlobalInfo.SCENE_PLAY));
                break;
            }
        }
    }

    //move sheet smoothly and detect those invisible sheet position
    void AttemptToMove()
    {
        for (int i = 0; i < sheet.Count; i++)
        {
            if(sheet[i].pos< DEFAULT_VISIBLE_NUM)
                MoveSheet(i);
            else
            {
                if (dir == 1)
                    setSheetPosRot(i, LEFT_REF_INDEX);
                else if (dir == 2)
                    setSheetPosRot(i, RIGHT_REF_INDEX);
            }
        }
    }

    //check if user's mouse wheel action is legal
    void CkeckMouseScroll()
    {
        for(int i = 0; i < sheet.Count; i++)
        {
            if (sheet[i].pos == 1)
            {
                if(Mathf.Abs((sheet[i].coverO.GetComponent<RectTransform>().localPosition-coverPos[1]).x) <= SCROLL_SENSTIVE)
                {
                    isMouseScroll = false;
                    break;
                }       
            }
        }
    }

    //use lerp to move sheet smoothly to their next position
    void MoveSheet(int index)
    {
        Quaternion newCRot = sheet[index].coverO.GetComponent<RectTransform>().localRotation;
        Quaternion newTRot = sheet[index].titleO.GetComponent<RectTransform>().localRotation;
        Vector3 newCPos = sheet[index].coverO.GetComponent<RectTransform>().localPosition;
        Vector3 newTPos = sheet[index].titleO.GetComponent<RectTransform>().localPosition;
        sheet[index].coverO.GetComponent<RectTransform>().localRotation = Quaternion.Lerp(newCRot, UIRot[sheet[index].pos], Time.deltaTime * SCROLL_SPEED);
        sheet[index].titleO.GetComponent<RectTransform>().localRotation = Quaternion.Lerp(newTRot, UIRot[sheet[index].pos], Time.deltaTime * SCROLL_SPEED);
        sheet[index].coverO.GetComponent<RectTransform>().localPosition = Vector3.Lerp(newCPos, coverPos[sheet[index].pos], Time.deltaTime * SCROLL_SPEED);
        sheet[index].titleO.GetComponent<RectTransform>().localPosition = Vector3.Lerp(newTPos, titlePos[sheet[index].pos], Time.deltaTime * SCROLL_SPEED);
    }

    //according to direction choose next position will be and save it
    void SetNext()
    {
        for (int i = 0; i < sheet.Count; i++)
        {
            int tempPos = sheet[i].pos;
            if (dir == 1)
            {
                if (tempPos + 1 > sheet.Count - 1)
                    tempPos = 0;
                else
                    tempPos = tempPos + 1;
            }
            else if (dir == 2)
            {
                if (tempPos - 1 < 0)
                    tempPos = sheet.Count - 1;
                else
                    tempPos = tempPos - 1;
            }
            else if (dir == 0)
                tempPos = sheet[i].pos;
            sheet[i].pos = tempPos;
        }
    }

    //immediately move sheet according posIndex
    void setSheetPosRot(int index,int posIndex)
    {
        sheet[index].coverO.GetComponent<RectTransform>().localPosition = coverPos[posIndex];
        sheet[index].titleO.GetComponent<RectTransform>().localPosition = titlePos[posIndex];
        sheet[index].coverO.GetComponent<RectTransform>().localRotation = UIRot[posIndex];
        sheet[index].titleO.GetComponent<RectTransform>().localRotation = UIRot[posIndex];
    }

    //init all ref rot and pos
    void InitRefRotAndPos()
    {
        coverPos = new List<Vector3>();
        coverPos.Add(new Vector3(-650, 0, 200));
        coverPos.Add(new Vector3(0, 0, 0));
        coverPos.Add(new Vector3(650, 0, 200));
        UIRot = new List<Quaternion>();
        UIRot.Add(Quaternion.Euler(0,30,0));
        UIRot.Add(Quaternion.Euler(0, 0, 0));
        UIRot.Add(Quaternion.Euler(0, -30, 0));
        titlePos = new List<Vector3>();
        titlePos.Add(new Vector3(-516, -240, -76));
        titlePos.Add(new Vector3(0, -321, 0));
        titlePos.Add(new Vector3(516, -240, -76));
        //left ref(3)
        coverPos.Add(new Vector3(-1500, 0, 700));
        UIRot.Add(Quaternion.Euler(0f, 90.0f, 0f));
        titlePos.Add(new Vector3(-1500, -240, 600));
        //right ref(4) -76``500
        coverPos.Add(new Vector3(1500, 0, 700));
        UIRot.Add(Quaternion.Euler(0f, 90.0f, 0f));
        titlePos.Add(new Vector3(1500, -240, 600));
    }

    //init sheet picker according sheet we loaded at first
    void InitSheetPicker()
    {
        for (int i = 0; i < loadSheets.Count; i++)
        {
            sheet.Add(new SheetPicker());
            sheet[i].pos = i;
            sheet[i].sheetNum = i;
            GameObject titleNew = new GameObject("songTitle"+i);
            titleNew.transform.SetParent(titleParent.transform);
            Text myText = titleNew.AddComponent<Text>();
            sheet[i].titleO = titleNew;
            GameObject coverNew = new GameObject("cover" + i);
            coverNew.transform.SetParent(coverParent.transform);
            Image myCover = coverNew.AddComponent<Image>();
            sheet[i].coverO = coverNew;
        }
        InitTitle();
        InitCover();
    }

    //init all text object
    void InitTitle()
    {
        for(int i = 0; i < sheet.Count; i++)
        {
            sheet[i].titleO.GetComponent<Text>().resizeTextMaxSize = FONT_SIZE;
            sheet[i].titleO.GetComponent<Text>().resizeTextForBestFit = true;
            sheet[i].titleO.GetComponent<Text>().fontSize = FONT_SIZE;
            sheet[i].titleO.GetComponent<Text>().font = fontUI;
            sheet[i].titleO.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            sheet[i].titleO.GetComponent<RectTransform>().sizeDelta = FONT_HW;
            sheet[i].titleO.GetComponent<RectTransform>().localScale = UI_SCALE;
            sheet[i].titleO.GetComponent<Text>().text = loadSheets[i].sheetName;
        }
            
    }

    //init all image object
    void InitCover()
    {
        for(int i = 0; i < sheet.Count; i++)
        {
            sheet[i].coverO.GetComponent<RectTransform>().sizeDelta = COVER_HW;
            sheet[i].coverO.GetComponent<RectTransform>().localScale = UI_SCALE;
            Image image = sheet[i].coverO.GetComponent<Image>();
            sourceLoader.LoadImage(loadSheets[sheet[i].sheetNum].sheetPhotoPath, ref image);
        }
    }

    //Init all sheet pos and rot
    void InitAllPosAndRot()
    {
        for (int i = 0; i < sheet.Count; i++)
        {
            if (sheet[i].pos < DEFAULT_VISIBLE_NUM)
                setSheetPosRot(i, sheet[i].pos);
            else
                setSheetPosRot(i, LEFT_REF_INDEX);
        }
    }

    //Get All Save File from Directory
    public List<GlobalInfo.SheetInfo> GetAllSaveFiles()
    {
        List<GlobalInfo.SheetInfo> allSaves = new List<GlobalInfo.SheetInfo>();
        SourceLoader.CheckDirectory(GlobalInfo.DEFAULT_SHEET_PATH);
        // Check Save Path
        foreach (string fileName in Directory.GetFiles(GlobalInfo.DEFAULT_SHEET_PATH))
        {
            // Get Player Data for Each File
            allSaves.Add(sourceLoader.GetSheetFile(fileName));
        }
        return allSaves;
    }

}

public class SheetPicker
{
    public int sheetNum;
    public GameObject coverO;
    public GameObject titleO;
    public int pos;
}