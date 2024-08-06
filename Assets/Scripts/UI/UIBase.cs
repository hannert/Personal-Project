using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBase : MonoBehaviour
{

    private GameObject playerGameObject;
    protected Player player;
    protected TextMeshProUGUI text;


    void Start()
    {
        playerGameObject = GameObject.Find("Player");
        player = playerGameObject.GetComponent<Player>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
