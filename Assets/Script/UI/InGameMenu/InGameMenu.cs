using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour {
    void Awake() {
        GameManager.instance.inGameMenu = this.gameObject;
        gameObject.SetActive(false);
    }
	void Start () {
        GameManager.instance.inGameMenu = this.gameObject;
        gameObject.SetActive(false);
    }
	
}
