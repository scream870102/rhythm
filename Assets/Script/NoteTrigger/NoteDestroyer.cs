using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDestroyer : MonoBehaviour
{
    //If note in destroy gameObject
    void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);
    }
}
