using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingManager : MonoBehaviour
{
    [SerializeField]
    private bool _enableStateLog;

    public bool enableStateLog
    {
        get { return _enableStateLog; }
        set {
            _enableStateLog = value;    
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        enableStateLog = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if (_enableStateLog == true)
        {
            Debug.Log("Logging enabled");
            Logging.EnableStateLog();
        }
        if (_enableStateLog == false)
        {
            Debug.Log("Logging disabled");
            Logging.DisableStateLog();
        }
    }
}
