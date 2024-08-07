using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
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
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage && prefabConnected) {
            
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
}
