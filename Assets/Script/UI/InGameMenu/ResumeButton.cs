using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResumeButton : MonoBehaviour {

    private Button button;

    void Start() {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate () { this.EventOnClick(); });
    }

    void EventOnClick() {
        GameManager.instance.GameResumeButtonClick();
    }
}
