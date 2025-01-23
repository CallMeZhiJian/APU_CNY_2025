using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
public enum FoodType
{
    GAN,
    SPECIAL
}

public class MovementPoint : MonoBehaviour
{
    private TileMapData _TileMapData;
    public FoodType _FoodType;

    void Awake()
    { 
        _TileMapData = FindObjectOfType<TileMapData>();

        var pos = _TileMapData.tileMap.WorldToCell(transform.position) + _TileMapData.tileMap.tileAnchor;
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.instance.PlayCollectCoin();

            gameObject.SetActive(false);

            if (_FoodType == FoodType.SPECIAL)
            {
                Debug.Log("Do something special");
            }
        }
    }
}
