using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public SaveManager saveManager;
    public bool loadOnStart = true;
    public bool saveOnQuit = true;

    private void Start()
    {
        if (loadOnStart)
        {
            saveManager.SaveDefaults();
            saveManager.LoadAutosave();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnApplicationQuit()
    {
        if (saveOnQuit)
        {
            saveManager.SaveAutosave();
        }
    }
}
