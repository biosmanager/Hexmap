using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class SaveManager : MonoBehaviour
{
    public string savegameDirectoryPath = "Savegames";
    public string savegameExtension = "sav";
    public Tilemap tilemap;
    public GameObject[] players;

    private string fullSavegameDirectoryPath;
    private MapRevealer mapRevealer;
    private Vector3[] defaultCoveredTiles;
    private List<Vector3> defaultPlayerPositions = new List<Vector3>();

    private const string AUTOSAVE_NAME = "Autosave";


    public void SaveDefaults()
    {
        defaultCoveredTiles = mapRevealer.GetCoveredTileCellPositions();
        foreach (GameObject player in players)
        {
            defaultPlayerPositions.Add(player.transform.position);
        }
    }

    public void LoadAutosave()
    {
        Load(AUTOSAVE_NAME);
    }

    public void Load(string savegameName)
    {
        string saveGamePath = GetFullSavegamePath(savegameName);
        if (File.Exists(saveGamePath))
        {
            try
            {
                using (FileStream fileStream = File.Open(saveGamePath, FileMode.Open))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    SaveFile saveFile = binaryFormatter.Deserialize(fileStream) as SaveFile;
                    fileStream.Close();

                    if (saveFile != null)
                    {
                        for (int i = 0; i < saveFile.playerPositions.Count; i++)
                        {
                            players[i].transform.position = saveFile.playerPositions[i];
                        }

                        foreach (var tileCellPosition in saveFile.coveredTilePositions)
                        {
                            mapRevealer.PlaceCoveredTile(Vector3Int.RoundToInt(tileCellPosition));
                        }

                        Debug.Log("Game Loaded");
                    }
                    else
                    {
                        Debug.LogError("Savegame empty!");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        else
        {
            Debug.LogError("No Savegame found!");
        }

    }

    public void SaveAutosave()
    {
        Save(AUTOSAVE_NAME);
    }

    [ContextMenu("Save")]
    public void Save(string savegameName)
    {
        // Check if savegame directory exists
        try
        {
            string saveGameFolder = fullSavegameDirectoryPath;
            if (!Directory.Exists(saveGameFolder))
            {
                Directory.CreateDirectory(saveGameFolder);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }

        SaveFile saveFile = MarshalSavedata();

        try
        {
            using (FileStream fileStream = File.Create(GetFullSavegamePath(savegameName)))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream,saveFile);
                fileStream.Close();
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }

        Debug.Log("Game Saved");
    }

    /// <summary>
    /// Marshal savegame data to serializable format.
    /// </summary>
    /// <returns>Serializable SaveFile instance</returns>
    private SaveFile MarshalSavedata()
    {
        SaveFile saveFile = new SaveFile();

        // vector3 must be transformed into SerializableVector3
        Vector3[] coveredTileCellPositions = mapRevealer.GetCoveredTileCellPositions();
        int coveredTileCellPositionsLenght = coveredTileCellPositions.Length;
        saveFile.coveredTilePositions = new SerializableVector3[coveredTileCellPositionsLenght];

        for (int i = 0; i < coveredTileCellPositionsLenght; i++)
        {
            saveFile.coveredTilePositions[i] = coveredTileCellPositions[i];
        }

        for (int i = 0; i < players.Length; i++)
        {
            saveFile.playerPositions.Add(players[i].transform.position);
        }

        return saveFile;
    }

    [ContextMenu("CAUTION: Delete ALL saved data!")]
    public void DeleteSavedData()
    {
        // TODO: Remove savegame files

        mapRevealer.UncoverAll();
        foreach (var tileCellPosition in defaultCoveredTiles)
        {
            mapRevealer.PlaceCoveredTile(Vector3Int.RoundToInt(tileCellPosition));
        }

        foreach (var pair in players.Zip(defaultPlayerPositions, (a, b) => new { player = a, defaultPosition = b } ))
        {
            pair.player.transform.position = pair.defaultPosition;
        }
    }

    private string GetFullSavegamePath(string savegameName)
    {
        return Path.Combine(fullSavegameDirectoryPath, $"{savegameName}.{savegameExtension}");
    }

    private void Awake()
    {
        fullSavegameDirectoryPath = Path.Combine(Application.persistentDataPath, savegameDirectoryPath);

        mapRevealer = tilemap.gameObject.GetComponent<MapRevealer>();
        Debug.Log($"Savegame directory: {fullSavegameDirectoryPath}");

        // Prune if savegame extension was specified with leading dot
        if (savegameExtension.StartsWith("."))
        {
            savegameExtension = savegameExtension.Remove(0, 1);
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DeleteSavedData();
        }
    }
}
