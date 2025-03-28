// Beta Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Makes the boundary walls invisible
// Main Contributors: Grace Calianese
public class InvisibleWalls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
