using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public List<Transform> AttachedNodes;

    public bool searched = false;
    

    private void Update()
    {
        if (AttachedNodes == null)
            print("Error: " + gameObject.name + " does'nt have any attached nodes.");
    }
}