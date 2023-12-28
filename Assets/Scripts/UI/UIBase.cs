using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBase : MonoBehaviour
{

    public GameObject playerGameObject;
    public Player player;
    public TextMeshProUGUI text;


    void Start()
    {
        player = playerGameObject.GetComponent<Player>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
