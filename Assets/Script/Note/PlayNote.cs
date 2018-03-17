using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNote : BasicNote {
    public PlayManager manager;
    public bool isCombo;

	void Update () {
        Move();
    }

    public override void Move() {
        if (GlobalInfo.G_state == GlobalInfo.STATE_MUSIC_PLAYING) {
            Rigidbody noteRb = gameObject.GetComponent<Rigidbody>();
            float spawnZ = (noteTime - manager.musicTime) * GlobalInfo.G_speed;
            Vector3 newPos = new Vector3(GlobalInfo.NOTE_SPAWN_POS[(int)enotePos].x, GlobalInfo.NOTE_SPAWN_POS[(int)enotePos].y, spawnZ);
            noteRb.position = newPos;
            noteRb.velocity = Vector3.zero;
            noteRb.angularVelocity = Vector3.zero;
            noteRb.MovePosition(noteRb.position - GetComponent<Transform>().forward * Time.deltaTime * GlobalInfo.G_speed);
        }
    }
}
