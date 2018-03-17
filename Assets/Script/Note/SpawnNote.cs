using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNote : BasicNote {
    private GameObject instatiate;
    private AudioSource music;
    public bool isSelect;
    public int index;
    private const float DELETE_DELAY = 0.5f;

    void Start () {
        instatiate = GameObject.Find("Music");
        music = instatiate.GetComponent<AudioSource>();
	}
	
	void Update () {
        Move();
        // if note is being select and specific button is press call function to change note correspond value
        if (isSelect) {
            if (Input.GetButtonDown(GlobalInfo.B_DELETE_NOTE)) {
                GlobalInfo.G_deleteNoteIndex = index;
                GlobalInfo.G_isNoteDelete = true;
                StartCoroutine(NoteDeleteing());
            }
            if (Input.GetButtonDown(GlobalInfo.B_TIME_PLUS)) {
                noteTime += GlobalInfo.NOTE_TIME_OFFSET;
                SetPos();
            }
            else if (Input.GetButtonDown(GlobalInfo.B_TIME_MINUS)) {
                noteTime -= GlobalInfo.NOTE_TIME_OFFSET;
                SetPos();
            }
            if (Input.GetButtonDown(GlobalInfo.B_POS_UP)) {
                if ((int)(enotePos) + 1 > (int)GlobalInfo.EnotePos.R)
                    return;
                else {
                    enotePos = enotePos + 1;
                    SetPos();
                }
            }
            else if (Input.GetButtonDown(GlobalInfo.B_POS_DOWN)) {
                if ((int)(enotePos) - 1 < (int)GlobalInfo.EnotePos.L)
                    return;
                else {
                    enotePos = enotePos - 1;
                    SetPos();
                }
            }
        }
	}

    //Set Pos
    void SetPos() {
        float spawnZ = (noteTime - music.time) * GlobalInfo.G_speed;
        Vector3 newPos = new Vector3(GlobalInfo.NOTE_SPAWN_POS[(int)enotePos].x, GlobalInfo.NOTE_SPAWN_POS[(int)enotePos].y, spawnZ);
        GetComponent<Transform>().position = newPos;
    }

    //Set Pos Before Move
    public override void Move() {
        if (GlobalInfo.G_state == GlobalInfo.STATE_MUSIC_PLAYING) {
            Rigidbody noteRb = gameObject.GetComponent<Rigidbody>();
            SetPos();
            noteRb.velocity = Vector3.zero;
            noteRb.angularVelocity = Vector3.zero;
            noteRb.MovePosition(noteRb.position - GetComponent<Transform>().forward * Time.deltaTime * GlobalInfo.G_speed);
        }
    }

    //Delete Note
    IEnumerator NoteDeleteing() {
        yield return new WaitForSeconds(DELETE_DELAY);
        GlobalInfo.G_isNoteDelete = false;
        Destroy(gameObject);
    }
}
