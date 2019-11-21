using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PartyBehaviour : MonoBehaviour
{
    private GameManager gameManager;
    private ShopManager shopManager;

    private SpriteRenderer spriteRenderer;

    public int Gold;

    public List<HeroClass> heroParty;

    public float transitionLength;
    public float boardMovementSpeed;
    public float pauseLength;

    public TileBase startingTile;
    public Vector3 startingPos;
    public TileBase currentTile;
    public Grid grid;
    public Tilemap tilemap;

    public GameObject[] classTemplates;

    bool isInCombat;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        shopManager = FindObjectOfType(typeof(ShopManager)) as ShopManager;
        grid = GameObject.FindObjectOfType(typeof(Grid)) as Grid;
        tilemap = GameObject.FindObjectOfType(typeof(Tilemap)) as Tilemap;

        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isRoundProgressing)
        {
            this.spriteRenderer.sprite = heroParty[0].GetComponent<SpriteRenderer>().sprite;

            isInCombat = (gameManager.currentGamePhase == GameManager.GamePhase.Combat) ? true : false;

            if (isInCombat)
            {
                this.spriteRenderer.enabled = false;

                if (this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled == false)
                {
                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        this.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
            else
            {
                this.spriteRenderer.enabled = true;

                if (this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled == true)
                {
                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        this.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
        }
    }

    public void StartRound()
    {
        currentTile = startingTile;
        this.transform.position = startingPos;
    }

    public void AddCharacterToParty(string name, int index = -1)
    {
        if (heroParty.Count < 5)
        {
            for (int i = 0; i < classTemplates.Length; i++)
            {
                if (classTemplates[i].name == name)
                {
                    GameObject newHero = Instantiate(classTemplates[i], this.transform);
                    newHero.GetComponent<HeroClass>().CopyHero(newHero.GetComponent<HeroClass>(), classTemplates[i].GetComponent<HeroClass>());

                    newHero.GetComponent<HeroClass>().SetID();

                    if (newHero.GetComponent<HeroClass>().role == HeroRole.Arcanist)
                        newHero.GetComponent<HeroClass>().currentElement = Elements.Fire;

                    heroParty.Add(newHero.GetComponent<HeroClass>());
                }
            }
        }
        else
        {
            if (index >= 0)
            {
                heroParty.Remove(transform.GetChild(index).GetComponent<HeroClass>());
                Destroy(gameObject.transform.GetChild(index).gameObject);

                for (int i = 0; i < classTemplates.Length; i++)
                {
                    if (classTemplates[i].name == name)
                    {
                        GameObject newHero = Instantiate(classTemplates[i], this.transform);
                        newHero.GetComponent<HeroClass>().CopyHero(newHero.GetComponent<HeroClass>(), classTemplates[i].GetComponent<HeroClass>());

                        newHero.GetComponent<HeroClass>().SetID();

                        if (newHero.GetComponent<HeroClass>().role == HeroRole.Arcanist)
                            newHero.GetComponent<HeroClass>().currentElement = Elements.Fire;

                        heroParty.Add(newHero.GetComponent<HeroClass>());
                    }
                }
            }
        }
        spriteRenderer.sprite = heroParty[0].sprite;
    }

    public void EndCombat(bool endRound = false)
    {
        for (int i = 0; i < heroParty.Count; i++)
        {
            if (heroParty[i].isDead)
            {
                heroParty.RemoveAt(i);
                Destroy(this.transform.GetChild(i).gameObject);
            }

            if (heroParty[i].CanLevelUp())
            {
                heroParty[i].LevelUp(heroParty[i].Level + 1);
            }
        }

        Vector3Int intPos = grid.WorldToCell(this.transform.position);

        TileBase newTile = null;

        if (currentTile.name.Contains("T"))
        {
            for (int i = 0; i < gameManager.AllBaseTTiles.Count; i++)
            {
                if (gameManager.AllBaseTTiles[i].name == currentTile.name)
                {
                    Tile tile = gameManager.AllBaseTTiles[i] as Tile;
                    Tile xtile = currentTile as Tile;

                    if (tile.sprite == xtile.sprite)
                    {
                        newTile = gameManager.AllBaseTTiles[i];
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        else if (currentTile.name.Contains("L"))
        {
            for (int i = 0; i < gameManager.AllBaseLTiles.Count; i++)
            {
                if (gameManager.AllBaseLTiles[i].name == currentTile.name)
                {
                    Tile tile = gameManager.AllBaseLTiles[i] as Tile;
                    Tile xtile = currentTile as Tile;

                    if (tile.sprite == xtile.sprite)
                    {
                        newTile = gameManager.AllBaseLTiles[i];
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        else if (currentTile.name.Contains("X"))
        {
            for (int i = 0; i < gameManager.AllBaseXTiles.Count; i++)
            {
                if (gameManager.AllBaseXTiles[i].name == currentTile.name)
                {
                    Tile tile = gameManager.AllBaseXTiles[i] as Tile;
                    Tile xtile = currentTile as Tile;

                    if (tile.sprite == xtile.sprite)
                    {
                        newTile = gameManager.AllBaseXTiles[i];
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        else if (currentTile.name.Contains("B"))
        {
            for (int i = 0; i < gameManager.AllBaseBossTiles.Count; i++)
            {
                if (gameManager.AllBaseBossTiles[i].name == currentTile.name)
                {
                    Tile tile = gameManager.AllBaseBossTiles[i] as Tile;
                    Tile xtile = currentTile as Tile;

                    if (tile.sprite == xtile.sprite)
                    {
                        newTile = gameManager.AllBaseBossTiles[i];
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        tilemap.SetTile(intPos, newTile);


        if (endRound)
        {
            gameManager.StartCoroutine("ProceedToNextRound");
        }
    }

    public IEnumerator MoveParty(Vector3[] path)
    {
        if (path == null)
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
                while (this.transform.position != path[i])
                {

                    float step = boardMovementSpeed * Time.deltaTime;

                    this.transform.position = Vector3.MoveTowards(this.transform.position, path[i], step);

                    yield return new WaitForSeconds(step / 2);
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }


            this.transform.position = path[path.Length - 1];


            Vector3Int intPos = grid.WorldToCell(this.transform.position);
            currentTile = tilemap.GetTile(intPos);


            foreach (HeroClass hero in heroParty)
            {
                hero.XP += 1;
                
                if (hero.CanLevelUp())
                {
                    hero.LevelUp(hero.Level + 1);
                }
            }
            
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

                                foreach (HeroClass hero in heroParty)
                                {
                                    hero.currentHealth = hero.Health;
                                }

                                shopManager.promptPanel.SetActive(true);
                                break;

                            case VariantTiles.VariantType.Enemy:
                            case VariantTiles.VariantType.Boss:
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
