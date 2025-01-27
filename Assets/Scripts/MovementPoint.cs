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
    public Vector3 initPos;
    public float amplitude;
    public float frequency;

    void Awake()
    { 
        _TileMapData = FindObjectOfType<TileMapData>();

        var pos = _TileMapData._MaptileMap.WorldToCell(transform.position) + _TileMapData._MaptileMap.tileAnchor;
        transform.position = pos;

        initPos = transform.position;

        amplitude = Random.Range(0.01f, 0.025f);
    }

    private void Update()
    {
        var offset = Mathf.Sin(frequency * Time.time) * amplitude * 0.1f;

        transform.position += new Vector3(0.0f, offset, 0.0f);
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
                collision.gameObject.GetComponent<SnakeController>().TriggerPopUp();
            }
        }
    }
}
