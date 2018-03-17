using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHideTrigger : MonoBehaviour {

    void OnTriggerExit(Collider other) {
        other.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
