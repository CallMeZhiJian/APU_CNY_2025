using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

enum Direction
{
    UP,
    DOWN,
    LEFT, 
    RIGHT,
    RIGHT_UP, 
    RIGHT_DOWN, 
    LEFT_UP, 
    LEFT_DOWN
}

public class SnakeController : MonoBehaviour
{
    [SerializeField] private TileMapData _tileMapData;
    public TileBase _TailTileBase;
    public TileBase _StraightBodyTileBase;
    public TileBase _TurningBodyTileBase;

    private SpriteRenderer _SpriteRenderer;
    public Sprite UpSprite;
    public Sprite DownSprite;
    public Sprite LeftSprite;
    public Sprite RightSprite;

    [Header("Controls Properties")]
    public KeyCode _MovementKey;
    public float _MovementSpeed;
    private Vector3 targetPosition;
    [SerializeField] private int currentPoint;
    private bool checkSpam;

    private Direction _CurrentDirection;
    private Direction _LastDirection;
    public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();

        currentPoint = 0;

        var pos = _tileMapData.tileMap.WorldToCell(transform.position) + _tileMapData.tileMap.tileAnchor;
        transform.position = pos;

        targetPosition = transform.position;

        _CurrentDirection = Direction.LEFT;
        _LastDirection = _CurrentDirection;
        checkSpam = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkSpam)
        {
            if (Input.GetKeyDown(_MovementKey))
            {
                Vector3 lastPointPos;

                if (currentPoint == 0)
                {
                    lastPointPos = _tileMapData._MovementPoints[currentPoint].initPos;
                }
                else
                {
                    lastPointPos = _tileMapData._MovementPoints[currentPoint - 1].initPos;
                }

                // Get and assign target position
                var nextPointPos = _tileMapData._MovementPoints[currentPoint].initPos;
                targetPosition = nextPointPos;

                direction = nextPointPos - lastPointPos;

                FindDirection();

                ChangeAnimation();

                Vector3Int cellPos = Vector3Int.FloorToInt(transform.position);

                if (currentPoint == 0)
                {
                    _tileMapData.tileMap.SetTile(cellPos, _TailTileBase);
                }
                else
                {
                    Matrix4x4 tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 0.0f));
                    TileBase tile = _StraightBodyTileBase;

                    switch (_CurrentDirection)
                    {
                        case Direction.UP:
                        case Direction.DOWN:
                            tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 90.0f));
                            tile = _StraightBodyTileBase;
                            break;
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            tile = _StraightBodyTileBase;
                            break;
                        case Direction.RIGHT_UP:
                            tile = _TurningBodyTileBase;
                            tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 0.0f));
                            break;
                        case Direction.RIGHT_DOWN:
                            tile = _TurningBodyTileBase;
                            tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 270.0f));
                            break;
                        case Direction.LEFT_UP:
                            tile = _TurningBodyTileBase;
                            tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 90.0f));
                            break;
                        case Direction.LEFT_DOWN:
                            tile = _TurningBodyTileBase;
                            tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 180.0f));
                            break;
                    }

                    var tileChangeData = new TileChangeData()
                    {
                        position = cellPos,
                        tile = tile,
                        color = Color.white,
                        transform = tileTransform
                    };

                    _tileMapData.tileMap.SetTile(tileChangeData, false);  
                }

                currentPoint++;
                checkSpam = true;
                StartCoroutine(CheckSpam());
            }
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, _MovementSpeed * Time.deltaTime);
    }

    private void FindDirection()
    {
       // Store last Direction
        var tempDirection = _CurrentDirection;

        switch (_CurrentDirection)
        {
            case Direction.UP:
                if (direction.x > 0.0)
                {
                    _CurrentDirection = Direction.RIGHT_DOWN;
                }
                else if (direction.x < 0.0)
                {
                    _CurrentDirection = Direction.LEFT_DOWN;
                }
                break;

            case Direction.DOWN:
                if (direction.x > 0.0)
                {
                    _CurrentDirection = Direction.RIGHT_UP;
                }
                else if (direction.x < 0.0)
                {
                    _CurrentDirection = Direction.LEFT_UP;
                }
                break;

            case Direction.LEFT:
                if (direction.y > 0.0)
                {
                    _CurrentDirection = Direction.RIGHT_UP;
                }
                else if (direction.y < 0.0)
                {
                    _CurrentDirection = Direction.RIGHT_DOWN;
                }
                break;

            case Direction.RIGHT:
                if (direction.y > 0.0)
                {
                    _CurrentDirection = Direction.LEFT_UP;
                }
                else if (direction.y < 0.0)
                {
                    _CurrentDirection = Direction.LEFT_DOWN;
                }
                break;

            case Direction.RIGHT_UP:
                if (_LastDirection == Direction.DOWN)
                {
                    _CurrentDirection = Direction.RIGHT;
                }
                else if (_LastDirection == Direction.LEFT)
                {
                    _CurrentDirection = Direction.UP;
                }
                break;

            case Direction.RIGHT_DOWN:
                if (_LastDirection == Direction.UP)
                {
                    _CurrentDirection = Direction.RIGHT;
                }
                else if (_LastDirection == Direction.LEFT)
                {
                    _CurrentDirection = Direction.DOWN;
                }
                break;

            case Direction.LEFT_UP:
                if (_LastDirection == Direction.DOWN)
                {
                    _CurrentDirection = Direction.LEFT;
                }
                else if (_LastDirection == Direction.RIGHT)
                {
                    _CurrentDirection = Direction.UP;
                }
                break;
            case Direction.LEFT_DOWN:
                if (_LastDirection == Direction.UP)
                {
                    _CurrentDirection = Direction.LEFT;
                }
                else if (_LastDirection == Direction.RIGHT)
                {
                    _CurrentDirection = Direction.DOWN;
                }
                break;
        }

        _LastDirection = tempDirection;
    }

    private void ChangeAnimation()
    {
        if (direction.x > 0)
        {
            // Face Right
            _SpriteRenderer.sprite = RightSprite;
        }
        else if (direction.y > 0)
        {
            // Face Up
            _SpriteRenderer.sprite = UpSprite;
        }
        else if (direction.x < 0)
        {
            // Face Left
            _SpriteRenderer.sprite = LeftSprite;
        }
        else if (direction.y < 0)
        {
            // Face Down
            _SpriteRenderer.sprite = DownSprite;
        }

    }
    IEnumerator CheckSpam()
    {
        yield return new WaitForSeconds(0.5f);
        checkSpam = false;
    }
}
