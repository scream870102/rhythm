using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingManager : MonoBehaviour {
    //ref UI object
    public Text sheetPathText;
    public Text speedText;
    public Text delayText;

    private GlobalInfo.Setting tempSetting = new GlobalInfo.Setting();  //save settting file for temp

	void Start () {
        InitUI();
	}
	
    //init speed and delay to UI
    void InitUI() {
        sheetPathText.text = "Sheet Path = \n" + GlobalInfo.DEFAULT_SHEET_PATH;
        speedText.text = "Speed = " + (GlobalInfo.MAX_NECESSARY_TIME + 1.0f- GlobalInfo.G_necessaryTime);
        delayText.text = "Delay = " + GlobalInfo.G_playerDelay*1000;
    }

    //Set all setting to file and save
    public void ApplyButtonClick() {
        InitUI();
        tempSetting.necessaryTime = GlobalInfo.G_necessaryTime;
        tempSetting.speed = GlobalInfo.G_speed;
        tempSetting.delay = GlobalInfo.G_playerDelay;
        GlobalInfo.DEFAULT_SETTING_PATH = Application.persistentDataPath + @"\Setting\";
        SourceLoader.SaveSettingToFile(ref tempSetting);
    }

    //Turn scene to Main
    public void ExitButtonClick() {
        StartCoroutine(GameManager.instance.LoadScene(GlobalInfo.SCENE_MAIN));
    }

    //call every time speed slider value change
    public void SpeedValueChange(Slider value) {
        GlobalInfo.G_necessaryTime = value.maxValue+1-value.value;
        GlobalInfo.G_speed = GlobalInfo.DEFAULT_DISTANCE / GlobalInfo.G_necessaryTime;
        InitUI();
    }

    //call every time delay slider change
    public void DelayValueChange(Slider value) {
        GlobalInfo.G_playerDelay = value.value/1000;
        InitUI();
    }
}
