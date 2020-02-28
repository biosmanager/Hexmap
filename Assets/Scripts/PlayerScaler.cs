using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScaler : MonoBehaviour
{
    new public Camera camera;

    private float initialOrthographicSize;
    private List<Transform> players = new List<Transform>();
    private List<Vector3> initialePlayerScales = new List<Vector3>();

    void Start()
    {
        initialOrthographicSize = camera.orthographicSize;

        foreach (Transform child in this.transform)
        {
            players.Add(child);
            initialePlayerScales.Add(child.localScale);
        }
    }

    void Update()
    {
        float scale = camera.orthographicSize / initialOrthographicSize;

        for (int i = 0; i < players.Count; i++)
        {
            players[i].localScale = initialePlayerScales[i] * scale;
        }
    }
}
