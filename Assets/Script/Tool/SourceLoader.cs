using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
//using UnityEditor;
public class SourceLoader : MonoBehaviour {
    private const float WAIT_DELAY = 2.0f;

    //save everything of a sheet to file
    public void SaveSheetToFile(ref GlobalInfo.SheetInfo tempSheet) {
        CheckDirectory(GlobalInfo.DEFAULT_SHEET_PATH);
        string fullSavePath = GlobalInfo.DEFAULT_SHEET_PATH + tempSheet.sheetName + GlobalInfo.FILE_EXTENSION;
        FileStream fs;
        // Create a file or open an old one up for writing to
        if (!File.Exists(fullSavePath)) {
            fs = File.Open(fullSavePath, FileMode.Create);
        }
        else {
            fs = File.Open(fullSavePath, FileMode.Create);
        }

        XmlSerializer serializer = new XmlSerializer(typeof(GlobalInfo.SheetInfo));
        TextWriter textWriter = new StreamWriter(fs);
        serializer.Serialize(textWriter, tempSheet);
        fs.Close();
    }

    //check directory if exist
    public static void CheckDirectory(string path) {
        // Check if directory exists, if not create it
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    //get sheet file and return it due to path
    public GlobalInfo.SheetInfo GetSheetFile(string fullFilePath) {
        if (File.Exists(fullFilePath)&&Path.GetExtension(fullFilePath) == GlobalInfo.FILE_EXTENSION) {
            FileStream fs = File.Open(fullFilePath, FileMode.Open);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GlobalInfo.SheetInfo));
            XmlReader reader = XmlReader.Create(fs);
            GlobalInfo.SheetInfo data = xmlSerializer.Deserialize(reader) as GlobalInfo.SheetInfo;
            fs.Close();
            return data;
        }
        else {
            return null;
        }
    }

    //Get setting file from default setting file path and set it to GlobalInfo
    public static void GetSettingFileAndSet() {
        if (File.Exists(GlobalInfo.DEFAULT_SETTING_PATH+GlobalInfo.DEFAULT_SETTING_FILE_NAME)) {
            FileStream fs = File.Open(GlobalInfo.DEFAULT_SETTING_PATH + GlobalInfo.DEFAULT_SETTING_FILE_NAME, FileMode.Open);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GlobalInfo.Setting));
            XmlReader reader = XmlReader.Create(fs);
            GlobalInfo.Setting data = xmlSerializer.Deserialize(reader) as GlobalInfo.Setting;
            fs.Close();

            GlobalInfo.G_necessaryTime = data.necessaryTime;
            GlobalInfo.G_speed = data.speed;
            GlobalInfo.G_playerDelay = data.delay;
        }
        else {
            GlobalInfo.Setting tempSetting = new GlobalInfo.Setting();
            tempSetting.delay = 0.0f;
            tempSetting.necessaryTime = 2.0f;
            tempSetting.speed = GlobalInfo.DEFAULT_DISTANCE / tempSetting.necessaryTime;
            GlobalInfo.G_necessaryTime = tempSetting.necessaryTime;
            GlobalInfo.G_speed = tempSetting.speed;
            GlobalInfo.G_playerDelay = tempSetting.delay;
            SaveSettingToFile(ref tempSetting);
        }
    }

    //call when setting file is changed
    public static void SaveSettingToFile(ref GlobalInfo.Setting tempSetting) {
        CheckDirectory(GlobalInfo.DEFAULT_SETTING_PATH);
        string fullSavePath = GlobalInfo.DEFAULT_SETTING_PATH + GlobalInfo.DEFAULT_SETTING_FILE_NAME;

        FileStream fs;

        // Create a file or open an old one up for writing to
        fs = File.Open(fullSavePath, FileMode.OpenOrCreate);

        XmlSerializer serializer = new XmlSerializer(typeof(GlobalInfo.Setting));
        TextWriter textWriter = new StreamWriter(fs);
        serializer.Serialize(textWriter, tempSetting);
        fs.Close();
    }

    public void LoadSong(string path,ref AudioSource result) {
        StartCoroutine(LoadSongCoroutine(path,result));
    }

    public IEnumerator LoadSongCoroutine(string path, AudioSource result) {
        string url = string.Format("file://{0}", path);
        WWW www = new WWW(url);
        yield return www;
        yield return new WaitForSeconds(WAIT_DELAY);
        result.clip = www.GetAudioClip(false, false);
        //////////////////////////
        //Action after Song is Loaded
        /////////////////////////
    }

    public void LoadImage(string path, ref Image result ) {
        StartCoroutine(LoadImageCoroutine(path,result));
    }

    public IEnumerator LoadImageCoroutine(string path, Image result) {
        Texture2D tex;
        tex = new Texture2D(500, 500, TextureFormat.DXT1, false);
        WWW www = new WWW(path);
        yield return www;
        yield return new WaitForSeconds(WAIT_DELAY);
        www.LoadImageIntoTexture(tex);
        result.sprite = Sprite.Create(tex, new Rect(0, 0, 500, 500), Vector2.zero);
        //////////////////////////
        //Action after Image is Loaded
        /////////////////////////
    }
}

