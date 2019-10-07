using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoardManager : MonoBehaviour
{
    public Tilemap gameBoard;
    public Grid grid;
    public AStarGrid aStarGrid;

    public int gameBoardWidth;
    public int gameBoardHeight;
    public Vector3Int startingPos;

    public TileBase startingTile;
    public TileBase fillTile;
    public TileBase rimTile;

    public Transform seeker;
    public Transform target;

    public void StartRound()
    {
        gameBoard.SetTile(startingPos, startingTile);

        FillMap();

        aStarGrid.SetVariables(GameObject.FindGameObjectWithTag("Player").transform.position);
    }

    private void FillMap()
    {
        for (int i = 0; i < gameBoardHeight; i++)
        {
            for (int j = 0; j < gameBoardWidth; j++)
            {
                Vector3Int newPos = new Vector3Int(j, i, 0);
                
                if (gameBoard.GetTile(newPos) == null)
                {
                    gameBoard.SetTile(newPos, fillTile);
                }
            }
        }
        FillRim();
    }

    private void FillRim()
    {
        for (int i = -5; i < gameBoardHeight + 5; i++)
        {
            for (int j = -5; j < gameBoardWidth + 5; j++)
            {
                if (i <= -1 || i >= gameBoardHeight || j <= -1 || j >= gameBoardHeight)
                {

                    Vector3Int newPos = new Vector3Int(j, i, 0);

                    gameBoard.SetTile(newPos, rimTile);

                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(aStarGrid.gameBoardSize.x, aStarGrid.gameBoardSize.y, -0.5f));

        if (aStarGrid.grid != null)
        {
            Node playerNode = aStarGrid.NodeFromWorldPoint(aStarGrid.playerPosition);
            foreach (Node n in aStarGrid.grid)
            {
                if (n == playerNode)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = (n.isWalkable) ? Color.white : Color.red;
                }

                if(aStarGrid.path != null)
                {
                    if (aStarGrid.path.Contains(n))
                    {
                        Gizmos.color = Color.blue;
                    }
                }

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (aStarGrid.nodeDiameter - 0.1f));
            }
        }
    }

    public void Pathfinding(Vector3 startingPos, Vector3 targetPos)
    {
        Node startNode = aStarGrid.NodeFromWorldPoint(startingPos);
        Node targetNode = aStarGrid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for(int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentNode.fCost || 
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in aStarGrid.FindNeighborNodes(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int newMovementDistance = currentNode.gCost + GetDistance(currentNode, neighbor);

                if( newMovementDistance < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementDistance;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parentNode = currentNode;

                    if(!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
            
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        path.Reverse();

        aStarGrid.path = path;
    }

    int GetDistance (Node nodeA, Node nodeB)
    {
        int deltaX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int deltaY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return deltaY + deltaX;
    }
}

[System.Serializable]
public class AStarGrid
{
    public GameBoardManager manager;

    public Vector2 gameBoardSize;
    public float nodeRadius = 1;
    public Node[,] grid;
    public List<Node> path;

    public Vector3 playerPosition;

    public float nodeDiameter;
    public int gridSizeX;
    public int gridSizeY;

    public void SetVariables(Vector3 newPosition)
    {
        nodeDiameter = nodeRadius * 2;

        gameBoardSize = new Vector2(manager.gameBoardWidth, manager.gameBoardHeight);

        gridSizeX = Mathf.RoundToInt(gameBoardSize.x);
        gridSizeY = Mathf.RoundToInt(gameBoardSize.y);

        playerPosition = newPosition;

        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = new Vector3(0, 0, 0);

        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3Int cellPoint = new Vector3Int(x, y, 0);

                Vector3 nextPos = manager.grid.GetCellCenterWorld(cellPoint);
                nextPos.x += 0.2f;
                nextPos.y += 0.2f;
                nextPos.z = 0;


                bool walkable = (manager.gameBoard.GetTile(cellPoint).name != "Grass") ? true : false;
                grid[x, y] = new Node(walkable, nextPos, manager.gameBoard.GetTile(cellPoint), x, y);
            }
        }
    }

    public List<Node> FindNeighborNodes(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if ((x == 0 && y == 1) || (x == 0 && y == -1) ||
                    (x == -1 && y == 0) || (x == 1 && y == 0))
                {
                    if (checkX < gridSizeX && checkX >= 0 && checkY < gridSizeY && checkY >= 0)
                    {
                        TileBase tile = grid[checkX,checkY].currentTile;
                        Vector3Int tilePos = manager.grid.WorldToCell(grid[checkX, checkY].worldPosition);

                        PlayerBehaviour player = GameObject.FindObjectOfType(typeof(PlayerBehaviour)) as PlayerBehaviour;

                        /*
                        if (player.CanBePlaced(tile, manager.grid.WorldToCell(grid[checkX, checkY].worldPosition), true))
                        {
                            neighbors.Add(grid[checkX, checkY]);
                        }
                        */
                       
                        neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }
        }

        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        worldPosition.x -= 0.2f;
        worldPosition.y -= 0.2f;

        Vector3Int cellPosition = manager.grid.WorldToCell(worldPosition);

        return grid[cellPosition.x, cellPosition.y];
    }
}
    

public class Node
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public TileBase currentTile;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parentNode;

    public Node(bool walkable, Vector3 thisPos, TileBase tile, int x, int y)
    {
        isWalkable = walkable;
        worldPosition = thisPos;

        currentTile = tile;

        gridX = x;
        gridY = y;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}