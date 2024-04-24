using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Logging
{
    public static Logger stateLogger = new Logger(Debug.unityLogger.logHandler);

    public static void LoadLoggers()
    {
        stateLogger.logEnabled = false;
    }

    public static void DisableStateLog()
    {
        stateLogger.logEnabled = false;
    }

    public static void EnableStateLog()
    {
        stateLogger.logEnabled = true;
    }

    public static void logState(string s)
    {
        if(stateLogger.logEnabled == true)
        {
            stateLogger.Log(s);
        }
    }


}
