using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PathAndFileManager
{
    // savegame is located inside the application folder
    private static string _root = Application.dataPath;
    private static string _saveGameFolder = "_SaveGames";

    public static string DefaultSaveGameName = "Autosave";
    private static string _saveGameType = ".save";

    /// <summary>
    ///     Returns the savegame folder path
    /// </summary>
    /// <returns></returns>
    public static string SaveGamePath()
    {
        return Path.Combine(_root, _saveGameFolder);
    }
    /// <summary>
    ///  Returns the complete savegame path with type
    /// </summary>
    /// <param name="saveGameName"></param>
    /// <returns></returns>
    public static string SaveGamePathComplete(string saveGameName)
    {
        saveGameName = $"{saveGameName}{_saveGameType}";
        return Path.Combine(SaveGamePath(),saveGameName);
    }
}
