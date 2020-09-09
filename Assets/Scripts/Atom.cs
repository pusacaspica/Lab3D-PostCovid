using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class Atom : MonoBehaviour {

    public bool IsStable = false;
    public int originalEletronsToStability = 8;
    public Vector3 Axis;
    public int ValenceLayer;
    public Molecule.atomicType AtomicType;
    public GameObject Subtitle;
    public List<GameObject> Eletrons;
    public bool isMain;

    // Start is called before the first frame update
    void Start() {
        Subtitle = new GameObject();
        Subtitle.AddComponent<TextMeshPro>();
        Subtitle.name = "Subtitle";
        Eletrons = new List<GameObject>();
        originalEletronsToStability = ValenceLayer;
        if (isMain == null) isMain = true;
        for(int i = 0; i < ValenceLayer; i++){
            SpawnEletron(i, (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Eletron.mat", typeof(Material)), isMain);
        }
        Subtitle.transform.position = this.transform.position + new Vector3(0.0f, -0.75f, 0.0f);
        Subtitle.GetComponent<TextMeshPro>().richText = true;
        Subtitle.GetComponent<TextMeshPro>().fontStyle = FontStyles.Bold;
        Subtitle.GetComponent<TextMeshPro>().text = this.name;
        Subtitle.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
        Subtitle.GetComponent<TextMeshPro>().fontSize = 3;
        Subtitle.GetComponent<TextMeshPro>().outlineWidth = 0.4f;
        Subtitle.transform.parent = this.transform;
    }

    // Update is called once per frame
    void Update() {
        IsStableNow();
    }

    public void SpawnEletrosphere(int quantity, Material surplusMaterial, bool isMain)
    {
        List<(int, Material)> oldMaterials = new List<(int, Material)>();
        int originalEletrons = this.transform.childCount - 1;
        for (int t = 0; t < this.transform.childCount-1; t++){
            oldMaterials.Add((t, this.transform.GetChild(t).GetChild(0).gameObject.GetComponent<MeshRenderer>().material));
            Destroy(this.transform.GetChild(t).gameObject);
        }
        this.Eletrons.Clear();
        for (int t = 0; t < quantity; t++) {
            if (t < originalEletrons)
            {
                SpawnEletron(t,
                    oldMaterials[t].Item2, isMain);
            }
            else SpawnEletron(t, surplusMaterial, isMain);
        }
        this.transform.Find("Subtitle").SetAsLastSibling();
    }
    
    void SpawnEletron(int eletronName, Material material, bool isMain) {
        GameObject eletronHandle = new GameObject();
        GameObject pS = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject eletron = new GameObject();
        eletron.AddComponent<MeshFilter>().GetComponent<MeshFilter>().mesh = pS.GetComponent<MeshFilter>().mesh;
        GameObject.Destroy(pS);
        eletron.AddComponent<MeshRenderer>().GetComponent<MeshRenderer>().material = material;
        eletron.name = "Eletron";
        eletron.transform.parent = eletronHandle.transform;
        eletronHandle.transform.position = this.transform.position;
        eletronHandle.transform.Rotate(Vector3.up, eletronName*45);
        eletronHandle.transform.parent = this.transform;
        eletronHandle.name = "Eletron"+eletronName.ToString();
        if(isMain) eletronHandle.transform.Rotate(Vector3.forward, 180.0f);
        eletron.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        eletron.AddComponent<Eletron>();
        eletron.GetComponent<Eletron>().EletronCreationNumber = eletronName;
        eletron.AddComponent<Animator>();
        eletron.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Animations/Eletron.controller", typeof(RuntimeAnimatorController));
        //eletron.GetComponent<Animator>().SetFloat("EletronNumber", i);
        Eletrons.Add(eletron);
    }

    public void IsStableNow(){
        if(this.Eletrons.Count == 0 || this.Eletrons.Count == 8){
            this.IsStable = true;
        } else {
            if(this.AtomicType == Molecule.atomicType.Hydrogen && this.Eletrons.Count == 2) this.IsStable = true;
            else this.IsStable = false;
        }
    }
}
