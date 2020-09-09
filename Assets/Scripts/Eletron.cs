using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eletron : MonoBehaviour {

    public RuntimeAnimatorController Anim;

    public int EletronCreationNumber;

    void Start() {
        Anim = this.GetComponent<Animator>().runtimeAnimatorController;
    }

    // Update is called once per frame
    void Update() {
        this.GetComponent<Animator>().speed = 0.5f;
    }
}
