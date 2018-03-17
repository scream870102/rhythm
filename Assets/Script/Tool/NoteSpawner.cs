using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteSpawner : MonoBehaviour  {

    public static GameObject SpawnNote(GameObject notePrefab, GlobalInfo.EnotePos enotePos, GlobalInfo.EnoteType enoteType, float noteTime,Material noteColor) {
        GameObject note;
        float spawnZ = noteTime * GlobalInfo.G_speed;
        Vector3 spawnPos = GlobalInfo.NOTE_SPAWN_POS[(int)enotePos] + new Vector3(0.0f, 0.0f, spawnZ);
        note = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        note.GetComponent<MeshRenderer>().material = noteColor;
        note.GetComponent<BasicNote>().enotePos = enotePos;
        note.GetComponent<BasicNote>().enoteType = enoteType;
        note.GetComponent<BasicNote>().noteTime = noteTime;
        return note;
    }
}
