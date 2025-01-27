using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
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

    [Header("Controls Properties")]
    public KeyCode _MovementKey;
    public float _MovementSpeed;
    private Vector3 targetPosition;
    [SerializeField] private int currentPoint;
    private bool checkSpam;
    private bool isPlaying;

    [SerializeField]private Direction _CurrentDirection;
    [SerializeField]private Direction _LastDirection;
    public Vector3 direction;

    [Header("UI Properties")]
    public Animator _PopUpAnim;
    public AnimationClip _NextLevelAnimationClip;
    public SpriteRenderer[] _WordSpriteRenderer;
    public LevelColorData[] _LevelColorData;
    private int _CurrentLevel;

    // Start is called before the first frame update
    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();

        currentPoint = 0;

        var pos = _tileMapData._MaptileMap.WorldToCell(transform.position) + _tileMapData._MaptileMap.tileAnchor;
        transform.position = pos;

        targetPosition = transform.position;

        _CurrentDirection = Direction.DOWN;
        _LastDirection = _CurrentDirection;
        checkSpam = false;
        isPlaying = true;
        _CurrentLevel = 0;

        ChangeLevelColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (!checkSpam)
            {
                Vector3 lastPointPos;

                if (currentPoint == 0)
                {
                    lastPointPos = transform.position;
                }
                else
                {
                    lastPointPos = _tileMapData._MovementPoints[currentPoint - 1].initPos;
                }

                // Get and assign target position
                var nextPointPos = _tileMapData._MovementPoints[currentPoint].initPos;

                direction = nextPointPos - lastPointPos;

                if (Input.GetKeyDown(_MovementKey))
                {
                    targetPosition = nextPointPos;

                    FindDirection();

                    Vector3Int cellPos = Vector3Int.FloorToInt(transform.position);

                    if (currentPoint == 0)
                    {
                        _tileMapData._SnakeTileMap.SetTile(cellPos, _TailTileBase);
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

                        _tileMapData._SnakeTileMap.SetTile(tileChangeData, false);
                    }

                    currentPoint++;
                    checkSpam = true;
                    StartCoroutine(CheckSpam());
                }
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, _MovementSpeed * Time.deltaTime);

            ChangeAnimation();
        }
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
                    if (direction.y > 0.0)
                    {
                        _CurrentDirection = Direction.LEFT_UP;
                    }
                    else if (direction.y < 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_DOWN;
                    }
                    else
                    {
                        _CurrentDirection = Direction.RIGHT;
                    }
                }
                else if (_LastDirection == Direction.LEFT)
                {
                    if (direction.x > 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_DOWN;
                    }
                    else if (direction.x < 0.0)
                    {
                        _CurrentDirection = Direction.LEFT_DOWN;
                    }
                    else
                    {
                        _CurrentDirection = Direction.UP;
                    }
                }
                else if (_LastDirection == Direction.LEFT_UP)
                {
                    _CurrentDirection = Direction.UP;
                }
                else if(_LastDirection == Direction.RIGHT_DOWN)
                {
                    _CurrentDirection = Direction.RIGHT;
                }
                break;

            case Direction.RIGHT_DOWN:
                if (_LastDirection == Direction.UP)
                {
                    if (direction.y > 0.0)
                    {
                        _CurrentDirection = Direction.LEFT_UP;
                    }
                    else if (direction.y < 0.0)
                    {
                        _CurrentDirection = Direction.LEFT_DOWN;
                    }
                    else
                    {
                        _CurrentDirection = Direction.RIGHT;
                    }
                }
                else if (_LastDirection == Direction.LEFT)
                {
                    if (direction.x > 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_UP;
                    }
                    else if (direction.x < 0.0)
                    {
                        _CurrentDirection = Direction.LEFT_UP;
                    }
                    else
                    {
                        _CurrentDirection = Direction.DOWN;
                    }
                }
                else if (_LastDirection == Direction.RIGHT_UP)
                {
                    _CurrentDirection = Direction.RIGHT;
                }
                else if (_LastDirection == Direction.LEFT_DOWN)
                {
                    _CurrentDirection = Direction.DOWN;
                }
                else if (_LastDirection == Direction.LEFT_UP)
                {
                    _CurrentDirection = Direction.DOWN;
                }
                break;

            case Direction.LEFT_UP:

                if (_LastDirection == Direction.DOWN)
                {
                    if (direction.y > 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_UP;
                    }
                    else if (direction.y < 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_DOWN;
                    }
                    else
                    {
                        _CurrentDirection = Direction.LEFT;
                    }
                }
                else if (_LastDirection == Direction.RIGHT)
                {
                    if (direction.x > 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_DOWN;
                    }
                    else if (direction.x < 0.0)
                    {
                        _CurrentDirection = Direction.LEFT_DOWN;
                    }
                    else
                    {
                        _CurrentDirection = Direction.UP;
                    }
                }
                else if(_LastDirection == Direction.RIGHT_UP)
                {
                    _CurrentDirection= Direction.UP;
                }
                else if(_LastDirection== Direction.LEFT_DOWN)
                {
                    _CurrentDirection = Direction.LEFT;
                }

                break;

            case Direction.LEFT_DOWN:
                if (_LastDirection == Direction.UP)
                {
                    if (direction.y > 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_UP;
                    }
                    else if (direction.y < 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_DOWN;
                    }
                    else
                    {
                        _CurrentDirection = Direction.LEFT;
                    }
                }
                else if (_LastDirection == Direction.RIGHT)
                {
                    if (direction.x > 0.0)
                    {
                        _CurrentDirection = Direction.RIGHT_UP;
                    }
                    else if (direction.x < 0.0)
                    {
                        _CurrentDirection = Direction.LEFT_UP;
                    }
                    else
                    {
                        _CurrentDirection = Direction.DOWN;
                    }
                }
                else if( _LastDirection == Direction.LEFT_UP)
                {
                    _CurrentDirection = Direction.LEFT;
                }
                else if(_LastDirection == Direction.RIGHT_DOWN)
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
            //_SpriteRenderer.sprite = RightSprite;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
        }
        else if (direction.y > 0)
        {
            // Face Up
            //_SpriteRenderer.sprite = UpSprite;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else if (direction.x < 0)
        {
            // Face Left
            //_SpriteRenderer.sprite = LeftSprite;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        }
        else if (direction.y < 0)
        {
            // Face Down
            //_SpriteRenderer.sprite = DownSprite;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
        }

    }

    IEnumerator CheckSpam()
    {
        yield return new WaitForSeconds(0.5f);
        checkSpam = false;
    }

    public void TriggerPopUp()
    {
        if (_CurrentLevel == 4)
        {
            _PopUpAnim.SetTrigger("isLevelComplete");
        }
        else
        {
            _PopUpAnim.SetTrigger("isLevelUp");
        }

        _CurrentLevel++;
        isPlaying = false;
    }

    public void TriggerNextLevel()
    {
        _PopUpAnim.SetTrigger("isNextLevel");

        StartCoroutine(WaitAnimationEnd(_NextLevelAnimationClip.length, true));
    }

    IEnumerator WaitAnimationEnd(float length, bool bIsPlaying)
    {
        yield return new WaitForSeconds(length);

        ChangeLevelColor();

        isPlaying = bIsPlaying;
    }

    private void ChangeLevelColor()
    {
        Color mapColor = _LevelColorData[_CurrentLevel]._MapColor;
        Color wordColor = _LevelColorData[_CurrentLevel]._WordColor;

        for (int i = 0; i < _WordSpriteRenderer.Length; i++)
        {
            _WordSpriteRenderer[i].color = wordColor;
        }

        _tileMapData._MaptileMap.color = mapColor;
    }
}
