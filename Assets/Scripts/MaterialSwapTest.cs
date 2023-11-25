using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwapTest : MonoBehaviour
{
    public Material parrying;
    public Material blocking;
    public Material baseMaterial;

    public bool isParrying = false;
    public bool isBlocking = false;

    public float parryWindow;
    public float timeSpentParrying;

    public GameObject player;
    private MeshRenderer _playerMeshRend;
    // Start is called before the first frame update
    void Start()
    {
        _playerMeshRend = player.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isParrying == true)
        {
            tickParry();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _playerMeshRend.material = parrying;
            isParrying = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            _playerMeshRend.material = baseMaterial;
            isParrying = false;
            timeSpentParrying = 0;
        }
    }

    void tickParry()
    {
        timeSpentParrying += Time.deltaTime;
        if(timeSpentParrying >= parryWindow)
        {
            timeSpentParrying = 0;
            isParrying = false;
            _playerMeshRend.material = blocking;
        }
    }
}
