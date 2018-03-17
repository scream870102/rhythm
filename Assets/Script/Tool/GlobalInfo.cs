using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalInfo
{
    //const
    public static Vector3[] NOTE_SPAWN_POS = new Vector3[] { new Vector3(-6.0f, 0.56f, 100.0f), new Vector3(0.0f, 0.56f, 100.0f), new Vector3(6.0f, 0.56f, 100.0f) };
    public static Vector3 L_NOTE_SPAWN_POS = new Vector3(-6.0f, 0.56f, 100.0f);
    public static Vector3 M_NOTE_SPAWN_POS = new Vector3(0.0f, 0.56f, 100.0f);
    public static Vector3 R_NOTE_SPAWN_POS = new Vector3(6.0f, 0.56f, 100.0f);
    public static float DEFAULT_DISTANCE = 100.0f;
    public static string FILE_EXTENSION = ".sheet";
    public static string SETTING_FILE_EXTENSION = ".xml";
    public static float NOTE_TIME_OFFSET = 0.099f;
    //Button
    public const string B_POS_UP = "PosUp";
    public const string B_POS_DOWN = "PosDown";
    public const string B_TIME_PLUS = "TimePlus";
    public const string B_TIME_MINUS = "TimeMinus";
    public const string B_DELETE_NOTE = "DeleteNote";
    public const string B_SPAWN = "Spawn";

    //Setting Data
    public static string DEFAULT_SHEET_PATH = Application.persistentDataPath + @"\Sheet\";
    public static string DEFAULT_SHEET_PHOTO = Application.persistentDataPath + @"\Default Resource\nocover.jpg";
    public static string DEFAULT_SETTING_PATH = Application.persistentDataPath + @"\Setting\";
    public static string DEFAULT_SETTING_FILE_NAME = @"Setting.xml";

    public const int MAX_NECESSARY_TIME = 10;
    public static float G_speed = 50.0f;
    public static float G_basicNoteScore = 3.0f;
    public static float G_scoreBonus = 2.0f;
    public static float G_necessaryTime = 2.0f;
    public static float G_playerDelay = 0.005f;

    //SCENE 
    public static string SCENE_MAIN = "Main";
    public static string SCENE_SONG_SELECT = "SongSelecter";
    public static string SCENE_PLAY = "Play";
    public static string SCENE_SETTING = "Setting";
    public static string SCENE_SHEET_CREATE = "SheetCreate";
    public static string SCENE_GAME_RESULT = "GameResult";
    public const float LOAD_DELAY = 2.5f;

    public static string G_currentScene;
    public static string G_sceneToLoad;

    //STATE
    public const int STATE_MUSIC_PLAYING = 1;
    public const int STATE_MUSIC_PAUSING = 2;
    public const int STATE_MUSIC_STOPPING = 3;

    //Playing
    public const float JUGE_PREFECT = 0.033f;
    public const float JUGE_GREAT = 0.055f;
    public const float JUGE_GOOD = 0.077f;
    public const float JUGE_BAD = 0.099f;
    public static float[] JUGE_BONUS = new float[] { 1.0f, 0.7f, 0.5f, 0.35f, 0.0f };
    public static string[] JUGE_STRING = new string[] { "Perfect", "Great", "Good", "Bad", "Miss" };
    public const int JUGE_NUM = 5;

    //var
    public static SheetInfo G_sheetToPlay = new SheetInfo();    //store sheet which is going to play
    public static int G_state;  //store current music state ref for note
    public static int G_deleteNoteIndex;    //store index for note gonna to be delete
    public static bool G_isNoteDelete=false;    //store bool for note delete prevent over delete
    public static Result G_result= new Result();    //store game result

    //class of sheet info
    public class SheetInfo
    {

        private string SheetName;
        private string Author;
        public string sheetName {
            get { return SheetName; }
            set {
                if (value == "")
                    SheetName = "Empty";
                else
                    SheetName = value;
            }
        }
        public string author {
            get { return Author; }
            set {
                if (value == "")
                    Author = "Empty";
                else
                    Author = value;
            }
        }
        public List<NoteInfo> note = new List<NoteInfo>();
        public MusicInfo song = new MusicInfo();
        private string SheetPhotoPath = "";
        public string sheetPhotoPath {
            get { return SheetPhotoPath; }
            set {
                SheetPhotoPath = value;
                if (SheetPhotoPath == "")
                    SheetPhotoPath = DEFAULT_SHEET_PHOTO;
            }
        }
    }

    //class of musicInfo
    public class MusicInfo
    {
        private string SongPath;
        private string SongName;
        private int Bpm;
        public string songName {
            get { return SongName; }
            set {
                if (value.Contains("/")) {
                    SongName = value.Substring(value.LastIndexOf('/') + 1);
                    SongName = SongName.Remove(SongName.LastIndexOf('.'));
                }

                else
                    SongName = "Empty";
            }
        }
        public string songPath {
            get { return SongPath; }
            set { SongPath = value; }
        }
        public float length;
        public int bpm {
            get { return Bpm; }
            set { Bpm = value; }
        }
    }

    public enum EnoteType { instant, continuousStart,continuousEnd,continuousDur };
    public enum EnotePos { L, M, R };
    public class NoteInfo
    {
        public EnotePos enotePos;
        public EnoteType enoteType;
        public float noteTime;
    }
    public enum EhitJuge { Perfect,Great,Good,Bad,Miss};
    public class Setting
    {
        public float necessaryTime;
        public float speed;
        public float delay;
        //public string sheetPath;
    }
    public class Result
    {
        public float score;
        public float maxScore;
        public int maxCombo;
        public int combo;
        public int[] hitJuges = new int[] { 0, 0, 0, 0, 0, };
        public string sheetName;
    }

}
