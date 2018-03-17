using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNote : MonoBehaviour {

    public GlobalInfo.EnotePos enotePos;
    public GlobalInfo.EnoteType enoteType;
    public float noteTime;

    public virtual void Move() {
        if (GlobalInfo.G_state == GlobalInfo.STATE_MUSIC_PLAYING) {
            Rigidbody noteRb = gameObject.GetComponent<Rigidbody>();
            noteRb.velocity = Vector3.zero;
            noteRb.angularVelocity = Vector3.zero;
            noteRb.MovePosition(noteRb.position - GetComponent<Transform>().forward * Time.deltaTime * GlobalInfo.G_speed);
        }
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
