using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public int colliderNum;
    public ParticleSystem particle;
    public AudioSource SFX;
    //const 
    private const float BUTTON_Y_OFFSET = 0.56f;
    private const float BUTTON_ANIM_SPEED = 15.0f;
    private const float LOWEST_BUTTON_POS = 0.05f;
    //var
    private bool isButtonClick;     //detect button state
    private GameObject instance = null;
    private PlayManager manager = null;
    private string buttonClickName;     //store button input name
    private Vector3 originPos;  //store button originPos
    private GameObject note; //store note which is in

    void Start() {
        note = null;
        colliderNum = 0;
        instance = GameObject.Find("PlayManager");
        if (instance == null)
            Debug.LogError("Miss playManager from note");
        else {
            manager = instance.GetComponent<PlayManager>();
            if (manager == null)
                Debug.LogError("Miss playManager from note");
        }
        originPos = gameObject.transform.localPosition;
        SetButton();
    }

    void Update() {
        //if Button is pressing and reach lowest pos set bool to false
        if (transform.localPosition.y < LOWEST_BUTTON_POS)
            isButtonClick = false;

        //if click reset button position and set bool
        if (Input.GetButtonDown(buttonClickName)) {
            ResetAnim();
            isButtonClick = true;
        }

        //keep play button anim
        PlayAnim(isButtonClick);

    }

    //Set Button type and Input name
    void SetButton() {
        if (gameObject.name == "MButton") {
            buttonClickName = "MClick";
        }
        else if (gameObject.name == "LButton") {
            buttonClickName = "LClick";
        }
        else if (gameObject.name == "RButton") {
            buttonClickName = "RClick";
        }
    }

    //resest pos to init pos
    void ResetAnim() {
        transform.localPosition = originPos;
    }

    //play anim according to isButtonClick true--click false--unclick
    void PlayAnim(bool value) {
        float y = 0.0f;
        if (value)
            y = -BUTTON_Y_OFFSET;
        else if (!value)
            y = 0.0f;
        Vector3 newPos = transform.localPosition;
        transform.localPosition = Vector3.Lerp(newPos, originPos + new Vector3(0, y, 0), Time.deltaTime * BUTTON_ANIM_SPEED);
    }

    //set note to enter note make sure won't count over one note at one time
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Note" && note == null)
            note = other.gameObject;
    }

    //if note stay in check player action
    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Note" && note == null)
            note = other.gameObject;
        //Get Player Enter and send result for noteType Instant
        if (Input.GetButtonDown(buttonClickName) && other.gameObject.name==note.name && !note.GetComponent<PlayNote>().isCombo&& note.GetComponent<PlayNote>().enoteType==GlobalInfo.EnoteType.instant) {
            note.GetComponent<PlayNote>().isCombo = true;
            float tGap = Mathf.Abs(manager.musicTime - note.GetComponent<PlayNote>().noteTime );
            //play sound effect and particle system
            PlaySoundAndParticleEffect();
            SendCombo(note.GetComponent<PlayNote>().isCombo, HitJuge(tGap));
            Destroy(note);
            StartCoroutine(SetNote());
        }
        ///////
        //Get Player Enter and send result for noteType ContinuousStart
        if (Input.GetButtonDown(buttonClickName) && other.gameObject.name == note.name && !note.GetComponent<PlayNote>().isCombo && note.GetComponent<PlayNote>().enoteType == GlobalInfo.EnoteType.continuousStart) {
            note.GetComponent<PlayNote>().isCombo = true;
            float tGap = Mathf.Abs(manager.musicTime - note.GetComponent<PlayNote>().noteTime);
            //play sound effect and particle system
            PlaySoundAndParticleEffect();
            SendCombo(note.GetComponent<PlayNote>().isCombo, HitJuge(tGap));
            Destroy(note);
            StartCoroutine(SetNote());
        }
        if (Input.GetButton(buttonClickName) && other.gameObject.name == note.name && !note.GetComponent<PlayNote>().isCombo && note.GetComponent<PlayNote>().enoteType == GlobalInfo.EnoteType.continuousDur) {
            note.GetComponent<PlayNote>().isCombo = true;
            if(manager.musicTime == note.GetComponent<PlayNote>().noteTime) {
                //play sound effect and particle system
                PlaySoundAndParticleEffect();
                Destroy(note);
                StartCoroutine(SetNote());
            }
        }
        if (Input.GetButtonUp(buttonClickName) && other.gameObject.name == note.name && !note.GetComponent<PlayNote>().isCombo && (note.GetComponent<PlayNote>().enoteType == GlobalInfo.EnoteType.continuousEnd || note.GetComponent<PlayNote>().enoteType==GlobalInfo.EnoteType.continuousDur)) {
            note.GetComponent<PlayNote>().isCombo = true;
            if (note.GetComponent<PlayNote>().enoteType == GlobalInfo.EnoteType.continuousDur) {
                PlaySoundAndParticleEffect();
                SendCombo(note.GetComponent<PlayNote>().isCombo, (int)GlobalInfo.EhitJuge.Miss);
                Destroy(note);
                StartCoroutine(SetNote());
            }
            else {
                float tGap = Mathf.Abs(manager.musicTime - note.GetComponent<PlayNote>().noteTime);
                PlaySoundAndParticleEffect();
                SendCombo(note.GetComponent<PlayNote>().isCombo, HitJuge(tGap));
                Destroy(note);
                StartCoroutine(SetNote());
            }
        }
        ////////////////
        // if reach miss juge destroy note and send Combo result
        if (manager.musicTime  - note.GetComponent<PlayNote>().noteTime > GlobalInfo.JUGE_BAD) {
            SendCombo(note.GetComponent<PlayNote>().isCombo, (int)GlobalInfo.EhitJuge.Miss);
            Destroy(note);
            StartCoroutine(SetNote());
        }
    }

    // make note to null call after a delay
    IEnumerator SetNote() {
        yield return new WaitForEndOfFrame();
        note = null;
    }

    //send result to play manager
    void SendCombo(bool value, int hitJuge) {
        if (value) {
            manager.isComboing = true;
            manager.combo++;
            manager.isCounting = true;
            manager.hitJudge = hitJuge;
            GlobalInfo.G_result.hitJuges[hitJuge]++;
        }
        else if (!value) {
            manager.isComboing = false;
            manager.hitJudge = hitJuge;
            GlobalInfo.G_result.hitJuges[hitJuge]++;
        }
    }

    //turn result of hit Juge
    int HitJuge(float tGap) {
        int result;
        if (tGap <= GlobalInfo.JUGE_PREFECT)
            result = (int)GlobalInfo.EhitJuge.Perfect;
        else if (tGap < GlobalInfo.JUGE_GREAT)
            result = (int)GlobalInfo.EhitJuge.Great;
        else if (tGap < GlobalInfo.JUGE_GOOD)
            result = (int)GlobalInfo.EhitJuge.Good;
        else if (tGap < GlobalInfo.JUGE_BAD)
            result = (int)GlobalInfo.EhitJuge.Bad;
        else
            result = (int)GlobalInfo.EhitJuge.Miss;
        return result;
    }
    void PlaySoundAndParticleEffect() {
        SFX.pitch = Random.Range(1, 2);
        SFX.Play();
        particle.Play();
    }
}
