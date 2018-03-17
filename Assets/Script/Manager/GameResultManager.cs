using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameResultManager : MonoBehaviour {
    public Text sheetName;
    public Text perfectText;
    public Text greatText;
    public Text goodText;
    public Text badText;
    public Text missText;
    public Text scoreText;
	// Use this for initialization
	void Start () {
        sheetName.text = GlobalInfo.G_result.sheetName;
        perfectText.text = "Perfect  :  "+GlobalInfo.G_result.hitJuges[(int)GlobalInfo.EhitJuge.Perfect].ToString();
        greatText.text = "Great  :  " + GlobalInfo.G_result.hitJuges[(int)GlobalInfo.EhitJuge.Great].ToString();
        goodText.text = "Good  :  " + GlobalInfo.G_result.hitJuges[(int)GlobalInfo.EhitJuge.Good].ToString();
        badText.text = "Bad  :  " + GlobalInfo.G_result.hitJuges[(int)GlobalInfo.EhitJuge.Bad].ToString();
        missText.text = "Miss  :  " + GlobalInfo.G_result.hitJuges[(int)GlobalInfo.EhitJuge.Miss].ToString();
        scoreText.text = "Score : " + GlobalInfo.G_result.score + " / " + GlobalInfo.G_result.maxScore + "\n" + "Combo : " + GlobalInfo.G_result.combo + " / " + GlobalInfo.G_result.maxCombo;
    }
	
	public void ExitButtonClick() {
        StartCoroutine(GameManager.instance.LoadScene(GlobalInfo.SCENE_MAIN));
    }
}
