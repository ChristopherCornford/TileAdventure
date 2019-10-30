using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class PlayerBehaviour : MonoBehaviour
{
    Camera cam;
    [Header("Camera Movement")]
    public Vector3 startingPos;
    public float cameraMoveSpeed = 1f;
    public const float MAX_X_MOVEMENT = 7;
    public const float MAX_Y_MOVEMENT = 3.5f;
    [Space]
    [Space]

    public Vector3 camPosition;

    public Grid currentGrid;

    public Tilemap currentTileMap;

    public TileBase tile;

    public SpriteRenderer previewSprite;

    public bool hasTile;

    public Color canPlace;
    public Color cannotPlace;

    public GameObject playerIcon;

    public GameManager gameManager;
    public GameBoardManager boardManager;
    public DeckManager deckManager;

    public GameObject currentHandChoice;

    public Slider zoomSlider;
    
    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;

        deckManager = GameObject.FindGameObjectWithTag("Deck Manager").GetComponent<DeckManager>();
        boardManager = GameObject.FindGameObjectWithTag("Game Board").GetComponent<GameBoardManager>();
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        
    }

    public void StartRound()
    {
        this.transform.position = startingPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isRoundProgressing)
        {
            cam.orthographicSize = zoomSlider.value;

            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);

            MoveCamera();

            if (gameManager.currentGamePhase == GameManager.GamePhase.TilePlacement)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.origin.z);
                Vector3Int position = currentGrid.WorldToCell(worldPoint);

                position.z = 0;


                if (Input.GetMouseButtonDown(0))
                {
                    if (currentTileMap.GetTile(position) != null && currentTileMap.GetTile(position).name == "NESW-x")
                    {
                        if (playerIcon.GetComponent<PartyBehaviour>().currentTile.name == "NESW-x")
                        {
                            gameManager.ProceedToNextGamePhase();
                            StartCoroutine(playerIcon.GetComponent<PartyBehaviour>().MoveParty(null));
                        }
                        else
                        {

                            Vector3 nextPos = currentGrid.GetCellCenterWorld(position);
                            nextPos.x += 0.0f;
                            nextPos.y += 0.5f;
                            nextPos.z = 0;

                            boardManager.aStarGrid.SetVariables(nextPos);
                            boardManager.Pathfinding(playerIcon.transform.position, nextPos);

                            List<Vector3> path = new List<Vector3>();

                            foreach (Node n in boardManager.aStarGrid.path)
                                path.Add(n.worldPosition);

                            gameManager.ProceedToNextGamePhase();

                            StartCoroutine(playerIcon.GetComponent<PartyBehaviour>().MoveParty(path.ToArray()));

                        }
                    }
                    else
                    {
                        PlaceTilesInWorldSpace(position, currentGrid, currentTileMap);
                    }
                }

                previewSprite.gameObject.transform.position = cam.ScreenToWorldPoint(Input.mousePosition);

                if (tile != null)
                {
                    switch (CanBePlaced(tile, position))
                    {
                        case true:
                            previewSprite.color = canPlace;
                            break;

                        case false:
                            previewSprite.color = cannotPlace;
                            break;
                    }
                }
            }
        }
    }

    public void PlaceTilesInWorldSpace(Vector3Int position, Grid grid, Tilemap tileMap)
    {
        if (hasTile)
        {
            if (tile != null)
            {
                if (CanBePlaced(tile, position))
                {

                    gameManager.ProceedToNextGamePhase();

                    currentTileMap.SetTile(position, tile);
                    deckManager.RefreshHand(currentHandChoice.GetComponent<HandTile>().myIndex);

                    Vector3 nextPos = grid.GetCellCenterWorld(position);
                    nextPos.x += 0.0f;
                    nextPos.y += 0.5f;
                    nextPos.z = 0;

                    boardManager.aStarGrid.SetVariables(nextPos);
                    boardManager.Pathfinding(playerIcon.transform.position, nextPos);

                    List<Vector3> path = new List<Vector3>();
                    
                    foreach (Node n in boardManager.aStarGrid.path)
                        path.Add(n.worldPosition);

                    StartCoroutine(playerIcon.GetComponent<PartyBehaviour>().MoveParty(path.ToArray()));
                    
                    if (tile.GetType() == typeof(VariantTiles))
                    {
                        VariantTiles vTile = tile as VariantTiles;
                        
                        if(vTile.variantType == VariantTiles.VariantType.Enemy)
                        {
                            nextPos.y -= 0.25f;

                            Instantiate(gameManager.enemyPartyTemplate, nextPos, Quaternion.identity);
                        }
                        else if (vTile.variantType == VariantTiles.VariantType.Boss)
                        {
                            nextPos.y -= 0.25f;

                            Instantiate(gameManager.enemyPartyTemplate, nextPos, Quaternion.identity);
                        }
                    }
                }
            }

            hasTile = false;
            tile = null;
            previewSprite.sprite = null;
        }
    }

    void MoveCamera()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        float moveX = Input.GetAxisRaw("Horizontal") * cameraMoveSpeed * Time.deltaTime;
        float moveY = Input.GetAxisRaw("Vertical") * cameraMoveSpeed * Time.deltaTime;

        Vector3 moveDirection = new Vector3(moveX, moveY, 0);

        cam.transform.position += moveDirection;

        if (cam.transform.position.x < startingPos.x - MAX_X_MOVEMENT)
        {
            cam.transform.position = new Vector3(startingPos.x - MAX_X_MOVEMENT, cam.transform.position.y);
        }
        else if (cam.transform.position.x > startingPos.x + MAX_X_MOVEMENT)
        {
            cam.transform.position = new Vector3(startingPos.x + MAX_X_MOVEMENT, cam.transform.position.y);
        }

        if (cam.transform.position.y < startingPos.y - MAX_Y_MOVEMENT)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, startingPos.y - MAX_Y_MOVEMENT);
        }
        else if (cam.transform.position.y > startingPos.y + MAX_Y_MOVEMENT)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, startingPos.y + MAX_Y_MOVEMENT);
        }

#elif UNITY_ANDROID || UNITY_IOS

        if (!hasTile)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                transform.Translate(-touchDeltaPosition.x * cameraMoveSpeed/2 * Time.deltaTime / 3, -touchDeltaPosition.y * cameraMoveSpeed/2 * Time.deltaTime / 3, 0);
            }
        }
#endif
    }
    public bool CanBePlaced(TileBase tile, Vector3Int position)
    {
        if (position.x > boardManager.gameBoardHeight && position.y > boardManager.gameBoardWidth)
        {
            return false;
        }

        if (currentTileMap.GetTile(position) != null)
        {
            if (currentTileMap.GetTile(position).name != "Grass")
            {
                return false;
            }
        }

        TileBase[] neighborTiles = new TileBase[tile.name.Length - 2];
        int numOfNeighbors = 0;
        
        for(int i = 0; i < neighborTiles.Length; i++)
        {
            char letter = tile.name[i];
            neighborTiles[i] = CheckNeighbor(letter);
        }

        foreach (TileBase t in neighborTiles)
        {
            if(t != null)
            {
                if(t.name == "Crossroads")
                {
                    return true;
                }
                numOfNeighbors++;
            }
        }



        if(numOfNeighbors == neighborTiles.Length && numOfNeighbors != 0 && neighborTiles.Length != 0)
        {
            foreach (TileBase t in neighborTiles)
            {
                for(int i = 0; i < tile.name.Length; i++)
                {
                    switch (tile.name[i])
                    {
                        case 'N':
                            if (t.name.Contains("T") || t.name.Contains("X") || t.name.Contains("x") || t.name.Contains("B"))
                            {
                                if (t.name.Contains("S"))
                                {
                                    return true;
                                }
                            }
                            break;
                        case 'S':
                            if (t.name.Contains("T") || t.name.Contains("X") || t.name.Contains("x") || t.name.Contains("B"))
                            {
                                if (t.name.Contains("N"))
                                {
                                    return true;
                                }
                            }
                            break;
                        case 'E':
                            if (t.name.Contains("T") || t.name.Contains("X") || t.name.Contains("x") || t.name.Contains("B"))
                            {
                                if (t.name.Contains("W"))
                                {
                                    return true;
                                }
                            }
                            break;
                        case 'W':
                            if (t.name.Contains("T") || t.name.Contains("X") || t.name.Contains("x") || t.name.Contains("B"))
                            {
                                if (t.name.Contains("E"))
                                {
                                    return true;
                                }
                            }
                            break;
                    }
                }    
               
                if (t == tile && !tile.name.Contains("T") && !tile.name.Contains("X"))
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < tile.name.Length - 2; i++)
                    {
                        if (t.name.Contains(tile.name[i].ToString()) == false)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        if (numOfNeighbors < neighborTiles.Length && numOfNeighbors != 0)
        {
            bool canBePlaced = false;

            for(int i = 0; i < neighborTiles.Length; i++)
            {
                if (neighborTiles[i] != null)
                {
                    switch (tile.name[i])
                    {
                        case 'N':
                            if (neighborTiles[i].name.Contains("N"))
                            {
                                if (neighborTiles[i].name.Contains("S"))
                                {
                                    canBePlaced = true;
                                }
                            }
                            if (neighborTiles[i].name[i] != 'N')
                            {
                                canBePlaced = true;
                            }
                            else
                            {
                                if (tile.name.Contains("T") || tile.name.Contains("X") || tile.name.Contains("x") || tile.name.Contains("B"))
                                {
                                    if (neighborTiles[i].name.Contains("S"))
                                    {
                                        canBePlaced = true;
                                    }
                                }
                            }
                            
                            break;

                        case 'S':
                            if (neighborTiles[i].name.Contains("S"))
                            {
                                if (neighborTiles[i].name.Contains("N"))
                                {
                                    canBePlaced = true;
                                }
                            }
                            if (neighborTiles[i].name[i] != 'S')
                            {
                                canBePlaced = true;
                            }
                            else
                            {
                                if (tile.name.Contains("T") || tile.name.Contains("X") || tile.name.Contains("x") || tile.name.Contains("B"))
                                {
                                    if (neighborTiles[i].name.Contains("N"))
                                    {
                                        canBePlaced = true;
                                    }
                                }
                            }
                            break;

                        case 'E':
                            if (neighborTiles[i].name.Contains("E"))
                            {
                                if (neighborTiles[i].name.Contains("W"))
                                {
                                    canBePlaced = true;
                                }
                            }
                            if (neighborTiles[i].name[i] != 'E')
                            {
                                canBePlaced = true;
                            }
                            else
                            {
                                if (tile.name.Contains("T") || tile.name.Contains("X") || tile.name.Contains("x") || tile.name.Contains("B"))
                                {
                                    if (neighborTiles[i].name.Contains("W"))
                                    {
                                        canBePlaced = true;
                                    }
                                }
                            }
                            break;

                        case 'W':
                            if (neighborTiles[i].name.Contains("W"))
                            {
                                if (neighborTiles[i].name.Contains("E"))
                                {
                                    canBePlaced = true;
                                }
                            }
                            if (neighborTiles[i].name[i] != 'W')
                            {
                                canBePlaced = true;
                            }
                            else
                            {
                                if (tile.name.Contains("T") || tile.name.Contains("X") || tile.name.Contains("x") || tile.name.Contains("B"))
                                {
                                    if (neighborTiles[i].name.Contains("E"))
                                    {
                                        canBePlaced = true;
                                    }
                                }
                            }

                            break;
                    }
                }
            }
            return canBePlaced;
        }

        return false;
    }

    public TileBase CheckNeighbor(char letter)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.origin.z);
        Vector3Int position = currentGrid.WorldToCell(worldPoint);
        position.z = 0;

        TileBase nextTile;

        switch (letter)
        {
            case 'N':
                if(currentTileMap.GetTile(new Vector3Int(position.x, position.y + 1, position.z)) != null)
                {
                    nextTile = currentTileMap.GetTile(new Vector3Int(position.x, position.y + 1, position.z));

                    if (nextTile.name.Contains("S"))
                    {
                        return nextTile;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            case 'S':
                if (currentTileMap.GetTile(new Vector3Int(position.x, position.y - 1, position.z)) != null)
                {
                    nextTile = currentTileMap.GetTile(new Vector3Int(position.x, position.y - 1, position.z));

                    if (nextTile.name.Contains("N"))
                    {
                        return nextTile;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            case 'E':
                if (currentTileMap.GetTile(new Vector3Int(position.x + 1, position.y, position.z)) != null)
                {
                    nextTile = currentTileMap.GetTile(new Vector3Int(position.x + 1, position.y, position.z));

                    if (nextTile.name.Contains("W"))
                    {
                        return nextTile;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            case 'W':
                if (currentTileMap.GetTile(new Vector3Int(position.x - 1, position.y, position.z)) != null)
                {
                    nextTile = currentTileMap.GetTile(new Vector3Int(position.x - 1, position.y, position.z));

                    if (nextTile.name.Contains("E"))
                    {
                        return nextTile;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
        }
        return null;
    }
}
