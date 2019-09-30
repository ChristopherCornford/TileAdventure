using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PartyBehaviour : MonoBehaviour
{
    private GameManager gameManager;
    private ShopManager shopManager;

    public List<HeroClass> heroParty;

    public float transitionLength;
    public float boardMovementSpeed;
    public float pauseLength;

    public TileBase startingTile;
    public TileBase currentTile;
    public Grid grid;
    public Tilemap tilemap;

    public HeroClass[] classTemplates;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        shopManager = FindObjectOfType(typeof(ShopManager)) as ShopManager;
        grid = GameObject.FindObjectOfType(typeof(Grid)) as Grid;
        tilemap = GameObject.FindObjectOfType(typeof(Tilemap)) as Tilemap;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentTile = startingTile;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCharacterToParty(string name, int index = -1)
    {
        if (heroParty.Count < 5)
        {
            GameObject newHero = new GameObject(name);

            foreach (HeroClass hero in classTemplates)
                if (hero.name == name)
                {
                    newHero.AddComponent<HeroClass>();
                    newHero.GetComponent<HeroClass>().CopyHero(newHero.GetComponent<HeroClass>(), hero);
                }
            newHero.transform.SetParent(this.transform);

            heroParty.Add(newHero.GetComponent<HeroClass>());
        }
        else
        {
            if (index >= 0)
            {
                heroParty.Remove(transform.GetChild(index).GetComponent<HeroClass>());
                Destroy(gameObject.transform.GetChild(index).gameObject);

                GameObject newHero = new GameObject(name);

                foreach (HeroClass hero in classTemplates)
                    if (hero.name == name)
                    {
                        newHero.AddComponent<HeroClass>();
                        newHero.GetComponent<HeroClass>().CopyHero(newHero.GetComponent<HeroClass>(), hero);
                    }
                newHero.transform.SetParent(this.transform);

                heroParty.Add(newHero.GetComponent<HeroClass>());
            }
        }

    }

    public void EndCombat()
    {
        Vector3Int intPos = grid.WorldToCell(this.transform.position);

        TileBase newTile = null;

        for (int i = 0; i < gameManager.AllBaseTiles.Count; i++)
        {
            if (gameManager.AllBaseTiles[i].name == currentTile.name)
                newTile = gameManager.AllBaseTiles[i];
        }

        tilemap.SetTile(intPos, newTile);
    }

    public IEnumerator MoveParty(Vector3[] path = null)
    {
        if(path == null)
        {
            Vector3Int intPos = grid.WorldToCell(this.transform.position);
            currentTile = tilemap.GetTile(intPos);

            if (currentTile.GetType() == typeof(VariantTiles))
            {
                VariantTiles tile = currentTile as VariantTiles;

                Debug.Log("Event Tile");
                gameManager.ProceedToNextGamePhase();

                if (gameManager.currentGamePhase == GameManager.GamePhase.Event)
                {
                    if (currentTile.GetType() == typeof(VariantTiles))
                    {
                        VariantTiles currentEventTile = currentTile as VariantTiles;

                        switch (currentEventTile.variantType)
                        {
                            case VariantTiles.VariantType.Town:
                                shopManager.promptPanel.SetActive(true);
                                break;

                            case VariantTiles.VariantType.Enemy:
                                gameManager.ProceedToNextGamePhase(true);
                                break;
                        }
                    }
                }
            }
            else
            {
                gameManager.ProceedToNextGamePhase();
                gameManager.ProceedToNextGamePhase();
            }
        }
        else
        {
            for (int i = 0; i < path.Length; i++)
            {
                Vector3 startingPosition = this.transform.position;
                Vector3 distance = path[i] - this.transform.position;
                float currentLerpTime = 0f;

                while (this.transform.position != path[i])
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

                if (gameManager.currentGamePhase == GameManager.GamePhase.Event)
                {
                    if (currentTile.GetType() == typeof(VariantTiles))
                    {
                        VariantTiles currentEventTile = currentTile as VariantTiles;

                        switch (currentEventTile.variantType)
                        {
                            case VariantTiles.VariantType.Town:

                                foreach(HeroClass hero in heroParty)
                                {
                                    hero.currentHealth = hero.Health;
                                }

                                shopManager.promptPanel.SetActive(true);
                                break;

                            case VariantTiles.VariantType.Enemy:
                                gameManager.ProceedToNextGamePhase(true);
                                break;
                        }
                    }
                }
            }
            else
            {
                gameManager.ProceedToNextGamePhase();
                gameManager.ProceedToNextGamePhase();
            }
        }
    }
}
