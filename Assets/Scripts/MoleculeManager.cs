using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class MoleculeManager : MonoBehaviour {

    public double EletrosphereMultiplier = 2.25;
    public double Threshold;
    public Vector3 AxisBetweenMolecules;
    public GameObject MainMolecule, OtherMolecule;
    public List<GameObject> MainAtoms, OtherAtoms;

    // Start is called before the first frame update
    void Start() {
        MainMolecule = GameObject.FindGameObjectsWithTag("Player")[0];
        OtherMolecule = GameObject.FindGameObjectsWithTag("Player")[1];
        AxisBetweenMolecules = (OtherMolecule.transform.position - MainMolecule.transform.position).normalized;
        MainAtoms = MainMolecule.GetComponent<Molecule>().Atoms;
        OtherAtoms = OtherMolecule.GetComponent<Molecule>().Atoms;
    }

    // Update is called once per frame
    void Update() {
        UpdateAxis(MainMolecule.transform.position, OtherMolecule.transform.position, MainAtoms, OtherAtoms);

        for(int i = 0; i < MainAtoms.Count; i++){
            for(int j = 0; j < OtherAtoms.Count; j++){
                if(CalculateDistance(MainAtoms[i].transform.position, OtherAtoms[j].transform.position) <= Threshold &&
                    !MainAtoms[i].GetComponent<Atom>().IsStable && !OtherAtoms[j].GetComponent<Atom>().IsStable){
                    BindAtoms(MainAtoms[i], OtherAtoms[j], MainMolecule, OtherMolecule, MainAtoms, OtherAtoms);
                }
            }
        }
    }

    public void UpdateAxis(Vector3 mainPosition, Vector3 otherPosition, List<GameObject> mainAtoms, List<GameObject> otherAtoms){
        this.AxisBetweenMolecules = otherPosition - mainPosition;
        foreach(GameObject atom in mainAtoms){
            atom.GetComponent<Atom>().Axis = this.AxisBetweenMolecules;
        }
        foreach(GameObject atom in otherAtoms){
            atom.GetComponent<Atom>().Axis = -this.AxisBetweenMolecules;
        }
    }

    public double CalculateDistance(Vector3 AtomPos, Vector3 OtherAtomPos){
        return Mathf.Sqrt((OtherAtomPos.x-AtomPos.x)*(OtherAtomPos.x-AtomPos.x) + (OtherAtomPos.y-AtomPos.y)*(OtherAtomPos.y-AtomPos.y) + (OtherAtomPos.z-AtomPos.z)*(OtherAtomPos.z-AtomPos.z));
    }

    void BindAtoms(GameObject MainAtom, GameObject OtherAtom, GameObject MainMolecule, GameObject OtherMolecule, List<GameObject> MainAtomList, List<GameObject> OtherAtomList){
        MainAtomList.AddRange(OtherAtomList);
        OtherAtom.transform.parent = MainMolecule.transform;

        if((MainAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Nonmetal && OtherAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Nonmetal)
            || (MainAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Hydrogen && OtherAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Hydrogen)
            || (MainAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Hydrogen && OtherAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Nonmetal)
            || (MainAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Nonmetal && OtherAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Hydrogen)){
            ShareEletrons(MainAtom, OtherAtom);
            Debug.Log("ELETRONS HAVE BEEN SHARED.");
        } else {
            if(MainAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Nonmetal){
                DonateEletrons(OtherAtom, MainAtom);
                Debug.Log(OtherAtom.name+" HAS DONATED ELETRONS");
            } else if (OtherAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Nonmetal){
                DonateEletrons(MainAtom, OtherAtom);
                Debug.Log(MainAtom.name+" HAS DONATED ELETRONS");
            }
        }

        OtherAtomList.RemoveRange(0, OtherAtomList.Count);
        foreach(Transform Atom in OtherMolecule.transform){
            GameObject.Destroy(Atom);
        }
    }

    void ShareEletrons(GameObject MainAtom, GameObject OtherAtom){
        if(MainAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Hydrogen && OtherAtom.GetComponent<Atom>().AtomicType == Molecule.atomicType.Hydrogen)
        {
            MainAtom.GetComponent<Atom>().SpawnEletrosphere(2,
                (Material) AssetDatabase.LoadAssetAtPath("Assets/Materials/SharedEletron.mat", typeof(Material)), true);
            OtherAtom.GetComponent<Atom>().SpawnEletrosphere(2,
                (Material) AssetDatabase.LoadAssetAtPath("Assets/Materials/SharedEletron.mat", typeof(Material)), false);
        }
        else
        {
            int share = Mathf.Min(
                8 - MainAtom.GetComponent<Atom>().ValenceLayer,
                8 - OtherAtom.GetComponent<Atom>().ValenceLayer);
            MainAtom.GetComponent<Atom>().SpawnEletrosphere(
                MainAtom.GetComponent<Atom>().Eletrons.Count + share,
                (Material) AssetDatabase.LoadAssetAtPath("Assets/Materials/SharedEletron.mat", typeof(Material)), true);
            OtherAtom.GetComponent<Atom>().SpawnEletrosphere(
                OtherAtom.GetComponent<Atom>().Eletrons.Count + share,
                (Material) AssetDatabase.LoadAssetAtPath("Assets/Materials/SharedEletron.mat", typeof(Material)), false);
            MainAtom.GetComponent<Atom>().ValenceLayer += share;
            OtherAtom.GetComponent<Atom>().ValenceLayer += share;
        }
        MainAtom.GetComponent<Atom>().IsStableNow();
        OtherAtom.GetComponent<Atom>().IsStableNow();
    }

    void DonateEletrons(GameObject OneWhoDonates, GameObject OneWhoReceives){
        int donate = Mathf.Min(OneWhoReceives.GetComponent<Atom>().Eletrons.Count, OneWhoDonates.GetComponent<Atom>().Eletrons.Count);
        OneWhoDonates.GetComponent<Atom>().SpawnEletrosphere(OneWhoDonates.GetComponent<Atom>().ValenceLayer - donate, (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/DonatedEletron.mat", typeof(Material)), true);
        OneWhoDonates.GetComponent<Atom>().ValenceLayer -= donate;
        OneWhoReceives.GetComponent<Atom>().SpawnEletrosphere(OneWhoReceives.GetComponent<Atom>().ValenceLayer + donate, (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/DonatedEletron.mat", typeof(Material)), true);
        OneWhoReceives.GetComponent<Atom>().ValenceLayer += donate;
        OneWhoReceives.GetComponent<Atom>().IsStableNow(); OneWhoDonates.GetComponent<Atom>().IsStableNow();
        OneWhoReceives.GetComponent<Atom>().Subtitle.GetComponent<TextMeshPro>().text = OneWhoReceives.name + "-"+(OneWhoReceives.GetComponent<Atom>().ValenceLayer - OneWhoReceives.GetComponent<Atom>().originalEletronsToStability).ToString();
        OneWhoDonates.GetComponent<Atom>().Subtitle.GetComponent<TextMeshPro>().text = OneWhoDonates.name + "+"+( -
            (OneWhoDonates.GetComponent<Atom>().ValenceLayer -
             OneWhoDonates.GetComponent<Atom>().originalEletronsToStability)).ToString();
    }
}
