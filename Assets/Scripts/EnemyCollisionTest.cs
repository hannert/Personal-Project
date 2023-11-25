using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionTest : MonoBehaviour
{

    public Material defaultMaterial;
    public Material hurtMaterial;
    private MeshRenderer thisRenderer;

    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (collision.collider.CompareTag("Weapon"))
        {
            Debug.Log("Collision with weapon");
            thisRenderer.material = hurtMaterial;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Weapon"))
        {
            thisRenderer.material = defaultMaterial;
        }
    }
}
