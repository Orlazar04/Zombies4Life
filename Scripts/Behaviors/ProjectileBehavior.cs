// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for destroying the bullet on collision
// Main Contributors: Olivia Lazar
public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject contactFX;

    private void OnCollisionEnter(Collision other)
    {
        GameObject effect = Instantiate(contactFX, transform.position, contactFX.transform.rotation);
        effect.transform.SetParent(transform.parent);

        Destroy(gameObject);
    }
}
