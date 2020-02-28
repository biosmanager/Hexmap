using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    // Map
    public SerializableVector3[] coveredTilePositions;
    // Player
    public List<SerializableVector3> playerPositions = new List<SerializableVector3>();
}
