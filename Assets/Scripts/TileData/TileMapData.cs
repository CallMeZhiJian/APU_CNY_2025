using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapData : MonoBehaviour
{
    public Tilemap tileMap;
    public Grid grid;

    public List<GameObject> _MovementPoints;

    // Start is called before the first frame update
    void Start()
    {
        //var obj = FindObjectsOfType<MovementPoint>();

        //_MovementPoints = new List<GameObject>();

        //for (int i = 0; i < obj.Length; i++)
        //{
        //    _MovementPoints.Add(obj[i].gameObject);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
