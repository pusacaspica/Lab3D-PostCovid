using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{

    public MoleculeManager moleculeManager;
    
    // Start is called before the first frame update
    void Start()
    {
        moleculeManager = this.GetComponent<MoleculeManager>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moleculeManager.MainMolecule.GetComponent<Molecule>().Spawn();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moleculeManager.OtherMolecule.GetComponent<Molecule>().Spawn();
        }
    }
}
