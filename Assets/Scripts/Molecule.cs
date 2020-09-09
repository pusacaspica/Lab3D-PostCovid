using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Molecule : MonoBehaviour
{
    public enum atomicType{
        Hydrogen, Metal, Nonmetal
    }
    public string OriginalElementName;
    public int OriginalValenceLayer;
    public atomicType OriginalElementAtomicType;

    public List<GameObject> Atoms;

    // Create root atom
    void Start() {
        Spawn();
    }

    public void Spawn() {
        GameObject root = new GameObject();
        GameObject pS = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        root.AddComponent<Atom>();
        root.GetComponent<Atom>().ValenceLayer = OriginalValenceLayer;
        root.GetComponent<Atom>().AtomicType = this.OriginalElementAtomicType;
        root.AddComponent<MeshFilter>();
        root.GetComponent<MeshFilter>().mesh = pS.GetComponent<MeshFilter>().mesh;
        GameObject.Destroy(pS);
        root.AddComponent<MeshRenderer>();
        root.GetComponent<MeshRenderer>().material = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Nucleus.mat", typeof(Material));
        root.name = OriginalElementName;
        Atoms.Add(root);
        root.gameObject.transform.parent = this.gameObject.transform;
        root.gameObject.transform.position = this.transform.position;
        
    }
    
    // Update is called once per frame
    void Update() {
    }
}
