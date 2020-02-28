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
    public Tilemap tilemap;
    public GameObject[] players;

    private MapRevealer mapRevealer;

    private Vector3[] defaultCoveredTiles;
    private List<Vector3> defaultPlayerPositions = new List<Vector3>();

    private void Awake()
    {
        mapRevealer = tilemap.gameObject.GetComponent<MapRevealer>();
    }

    public void SaveDefaults()
    {
        defaultCoveredTiles = mapRevealer.GetCoveredTileCellPositions();
        foreach (GameObject player in players)
        {
            defaultPlayerPositions.Add(player.transform.position);
        }
    }

    public void Load(string saveGameName)
    {
        /*
        if (PlayerPrefs.HasKey("coveredTilePositions"))
        {
            mapRevealer.UncoverAll();
            Vector3[] tileCellPositions = DeserializeVector3Array(PlayerPrefs.GetString("coveredTilePositions"));
            foreach (var tileCellPosition in tileCellPositions)
            {
                mapRevealer.PlaceCoveredTile(Vector3Int.RoundToInt(tileCellPosition));
            }
        }

        foreach (GameObject player in players)
        {
            if (PlayerPrefs.HasKey(player.name))
            {
                player.transform.position = StringToVector3(PlayerPrefs.GetString(player.name));
            }
        }
        */
        
        string saveGamePath = PathAndFileManager.SaveGamePathComplete(saveGameName);
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

    [ContextMenu("Save")]
    public void Save(string saveGameName)
    {
        /*Vector3[] tileCellPositions = mapRevealer.GetCoveredTileCellPositions();
        PlayerPrefs.SetString("coveredTilePositions", SerializeVector3Array(tileCellPositions));

        foreach (GameObject player in players)
        {
            PlayerPrefs.SetString(player.name, player.transform.position.ToString());
        }

        PlayerPrefs.Save();
        */

        
        
        try
        {
            string saveGameFolder = PathAndFileManager.SaveGamePath();
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
        
        SaveFile saveFile = CreateSaveGameObj();

        try
        {
            using (FileStream fileStream = File.Create(PathAndFileManager.SaveGamePathComplete(saveGameName)))
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

    // Create a GameObject that is serialized
    private SaveFile CreateSaveGameObj()
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
        PlayerPrefs.DeleteAll();

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

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DeleteSavedData();
        }
    }

    public static string SerializeVector3Array(Vector3[] aVectors)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Vector3 v in aVectors)
        {
            sb.Append(v.x).Append(" ").Append(v.y).Append(" ").Append(v.z).Append("|");
        }
        if (sb.Length > 0) // remove last "|"
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    public static Vector3[] DeserializeVector3Array(string aData)
    {
        string[] vectors = aData.Split('|');
        Vector3[] result = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            string[] values = vectors[i].Split(' ');
            if (values.Length != 3)
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
            result[i] = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        return result;
    }
}
