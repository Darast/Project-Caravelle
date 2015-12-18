using UnityEngine;
using System.Collections;
using UnityEditor;

public class prefabSaver : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PrefabUtility.CreatePrefab("Assets/Base.prefab", this.gameObject);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
