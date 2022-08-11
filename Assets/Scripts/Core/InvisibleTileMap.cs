using UnityEngine;
using UnityEngine.Tilemaps;

public class InvisibleTileMap : MonoBehaviour
{
    public void Start()
    {
        GetComponent<TilemapRenderer>().enabled = false;
    }
}
