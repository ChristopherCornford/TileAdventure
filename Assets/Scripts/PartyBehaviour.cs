using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PartyBehaviour : MonoBehaviour
{
    private GameManager gameManager;

    public List<HeroClass> heroParty;

    public float transitionLength;
    public float boardMovementSpeed;
    public float pauseLength;

    public TileBase currentTile;
    public Grid grid;
    public Tilemap tilemap;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        grid = GameObject.FindObjectOfType(typeof(Grid)) as Grid;
        tilemap = GameObject.FindObjectOfType(typeof(Tilemap)) as Tilemap;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveParty(Vector3[] path)
    {
        for(int i = 0; i < path.Length; i++)
        {
            Vector3 startingPosition = this.transform.position;
            Vector3 distance = path[i] - this.transform.position;
            float currentLerpTime = 0f;

            while(this.transform.position != path[i])
            {
                currentLerpTime += Time.deltaTime;

                if (currentLerpTime >= transitionLength)
                    currentLerpTime = transitionLength;

                float percTraveled = currentLerpTime / transitionLength;
                this.transform.position = Vector3.Lerp(startingPosition, path[i], percTraveled);
                yield return new WaitForSeconds(percTraveled / boardMovementSpeed);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        
        this.transform.position = path[path.Length - 1];

        Vector3Int intPos = grid.WorldToCell(this.transform.position);
        currentTile = tilemap.GetTile(intPos);

        if (currentTile.GetType() == typeof(VariantTiles))
        {
            VariantTiles tile = currentTile as VariantTiles;

            Debug.Log("Event Tile");
            gameManager.ProceedToNextGamePhase();

            if (tile.variantType == VariantTiles.VariantType.Enemy)
            {
                gameManager.ProceedToNextGamePhase(true);
            }
            
        }
        else
        {
            gameManager.ProceedToNextGamePhase();
            gameManager.ProceedToNextGamePhase();
        }
    }
    
}
