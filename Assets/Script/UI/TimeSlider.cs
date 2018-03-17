using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeSlider : MonoBehaviour {
    private Slider slider;
    public PlayManager manager;
	// Use this for initialization
	void Start () {
        slider=GetComponent<Slider>();
        slider.maxValue = GlobalInfo.G_sheetToPlay.song.length;
        slider.minValue = 0;
	}
	
	// Update is called once per frame
	void Update () {
        slider.maxValue = GlobalInfo.G_sheetToPlay.song.length;
        slider.minValue = 0;
        slider.value = manager.audioSource.time;
	}
}
