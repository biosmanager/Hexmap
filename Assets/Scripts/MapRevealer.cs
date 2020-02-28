using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(Tilemap))]

public class MapRevealer : MonoBehaviour
{
    public Tile coveredTile;
    public Tile uncoveredTile;

    private Tilemap tilemap;


    public Vector3[] GetCoveredTileCellPositions()
    {
        var tilePositions = new List<Vector3>();
        var tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        foreach (var tilePosition in tilemap.cellBounds.allPositionsWithin)
        {
            Tile tile = (Tile) tilemap.GetTile(tilePosition);
            if (tile == coveredTile)
            {
                tilePositions.Add(tilePosition);
            }
        }

        return tilePositions.ToArray();
    }

    public void PlaceCoveredTile(Vector3Int position)
    {
        tilemap.SetTile(position, coveredTile);
    }

    public void PlaceUncoveredTile(Vector3Int position)
    {
        tilemap.SetTile(position, uncoveredTile);
    }

    [ContextMenu("Cover All")]
    public void CoverAll()
    {
        SetAllTiles(coveredTile);
    }

    [ContextMenu("Uncover All")]
    public void UncoverAll()
    {
        SetAllTiles(uncoveredTile);
    }

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
            tilemap.SetTile(tilePos, uncoveredTile);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
            tilemap.SetTile(tilePos, coveredTile);
        }

        // Keyboard
        if (Input.GetKeyDown(KeyCode.C))
        {
            CoverAll();
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            UncoverAll();
        }
    }

    private void SetAllTiles(Tile tile)
    {
        foreach (var tilePosition in tilemap.cellBounds.allPositionsWithin)
        {
            tilemap.SetTile(tilePosition, tile);
        }
    }
}
