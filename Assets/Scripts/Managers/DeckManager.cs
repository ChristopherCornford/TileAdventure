using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    private GameManager gameManager;

    public HandTile[] tileHand;
    public int handSize;
    public int deckSize;

    [Header("Base Tile Variables")]
    public float percentT;
    public float percentL;
    public float percentX;

    [Header("Variant Tile Variables")]
    public int percentVariant;

    public int percentEnemy;
    public int percentAdventurer;
    public int percentTreasure;

    public List<TileBase> tileDeck;

    private int handSlotsExhausted;
    private bool bossInHand;

    public bool endTilePlacement;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    public void StartRound()
    {
       for(int i = 0; i < handSize; i++)
        {
            tileHand[i] = this.transform.GetChild(i).GetComponent<HandTile>();
        }

        tileDeck = CreateTileDeck(deckSize);

        for (int i = 0; i < handSize; i++)
        {
            RefreshHand(i);
        }
        
        bossInHand = false;
    }

    public void RefreshHand(int index)
    {
        if (tileDeck.Count > 0)
        {
            if (!bossInHand)
            {
                if (gameManager.currentTurn >= ((deckSize / 2) + 1))
                {
                    int randInt = Random.Range(0, 100);
                    int chanceMod = gameManager.currentTurn - (deckSize / 2);
                    double chancePerc = (chanceMod + (chanceMod * 0.5)) * 10;

                    if (randInt <= chancePerc)
                    {
                        tileHand[index].heldTile = gameManager.AllBossTiles[Random.Range(0, gameManager.AllBossTiles.Count)];
                        tileHand[index].tile = gameManager.AllBossTiles[Random.Range(0, gameManager.AllBossTiles.Count)];

                        tileHand[index].myIndex = index;

                        bossInHand = true;

                        return;
                    }
                }
            }
            
            tileHand[index].heldTile = tileDeck[0] as Tile;
            tileHand[index].tile = tileDeck[0];
            tileHand[index].myIndex = index;

            tileDeck.RemoveAt(0);
        }
        else if (tileDeck.Count == 0)
        {
            Destroy(tileHand[index].gameObject);
            handSlotsExhausted++;

            if(handSlotsExhausted == handSize)
            {
                endTilePlacement = true;
            }
        }
    }

    private List<TileBase> CreateTileDeck(int size)
    {
        List<TileBase> tempDeck = new List<TileBase>(size);

        int numOfT = Mathf.RoundToInt(size * (percentT / 100));
        int numOfL = Mathf.RoundToInt(size * (percentL / 100));
        int numOfX = Mathf.RoundToInt(size * (percentX / 100));

        
        for (int i = 0; i < size; i++)
        {
            int randType = Random.Range(0, 100);

            if(randType > percentVariant) //Base Tiles
            {
                if (numOfT > 0)
                {
                    switch (gameManager.currentSeason)
                    {
                        case GameManager.Season.Summer:
                            tempDeck.Add(gameManager.AllBaseTTiles[Random.Range(0, 4)]);
                            numOfT--;
                            break;

                        case GameManager.Season.Fall:
                            tempDeck.Add(gameManager.AllBaseTTiles[Random.Range(4, 8)]);
                            numOfT--;
                            break;

                        case GameManager.Season.Winter:
                            tempDeck.Add(gameManager.AllBaseTTiles[Random.Range(8, 12)]);
                            numOfT--;
                            break;

                        case GameManager.Season.Spring:
                            tempDeck.Add(gameManager.AllBaseTTiles[Random.Range(12, 16)]);
                            numOfT--;
                            break;
                    }
                }
                else
                {
                    if (numOfL > 0)
                    {
                        switch (gameManager.currentSeason)
                        {
                            case GameManager.Season.Summer:
                                tempDeck.Add(gameManager.AllBaseLTiles[Random.Range(0, 4)]);
                                numOfL--;
                                break;

                            case GameManager.Season.Fall:
                                tempDeck.Add(gameManager.AllBaseLTiles[Random.Range(4, 8)]);
                                numOfL--;
                                break;

                            case GameManager.Season.Winter:
                                tempDeck.Add(gameManager.AllBaseLTiles[Random.Range(8, 12)]);
                                numOfL--;
                                break;

                            case GameManager.Season.Spring:
                                tempDeck.Add(gameManager.AllBaseLTiles[Random.Range(12, 16)]);
                                numOfL--;
                                break;
                        }
                    }
                    else
                    {
                        if (numOfX > 0)
                        {
                            switch (gameManager.currentSeason)
                            {
                                case GameManager.Season.Summer:
                                    tempDeck.Add(gameManager.AllBaseXTiles[Random.Range(0, 3)]);
                                    numOfX--;
                                    break;

                                case GameManager.Season.Fall:
                                    tempDeck.Add(gameManager.AllBaseXTiles[Random.Range(3, 6)]);
                                    numOfX--;
                                    break;

                                case GameManager.Season.Winter:
                                    tempDeck.Add(gameManager.AllBaseXTiles[Random.Range(6, 9)]);
                                    numOfX--;
                                    break;

                                case GameManager.Season.Spring:
                                    tempDeck.Add(gameManager.AllBaseXTiles[Random.Range(9, 12)]);
                                    numOfX--;
                                    break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            else // Variant Tiles
            {
                int randVariant = Random.Range(0, 100);

                if (randVariant >= 0 && randVariant <= (99 - (100 - percentEnemy))) // Enemy Variant
                {
                    if (numOfT > 0)
                    {
                        switch (gameManager.currentSeason)
                        {
                            case GameManager.Season.Summer:
                                tempDeck.Add(gameManager.AllVariantTTiles_Enemy[Random.Range(0, 4)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Fall:
                                tempDeck.Add(gameManager.AllVariantTTiles_Enemy[Random.Range(4, 8)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Winter:
                                tempDeck.Add(gameManager.AllVariantTTiles_Enemy[Random.Range(8, 12)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Spring:
                                tempDeck.Add(gameManager.AllVariantTTiles_Enemy[Random.Range(12, 16)]);
                                numOfT--;
                                break;
                        }
                        
                    }
                    else
                    {
                        if (numOfL > 0)
                        {
                            switch (gameManager.currentSeason)
                            {
                                case GameManager.Season.Summer:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Enemy[Random.Range(0, 4)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Fall:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Enemy[Random.Range(4, 8)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Winter:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Enemy[Random.Range(8, 12)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Spring:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Enemy[Random.Range(12, 16)]);
                                    numOfL--;
                                    break;
                            }
                        }
                        else
                        {
                            if (numOfX > 0)
                            {
                                switch (gameManager.currentSeason)
                                {
                                    case GameManager.Season.Summer:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Enemy[Random.Range(0, 3)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Fall:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Enemy[Random.Range(3, 6)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Winter:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Enemy[Random.Range(6, 9)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Spring:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Enemy[Random.Range(9, 12)]);
                                        numOfX--;
                                        break;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                else if (randVariant >= ((99 - (100 - percentEnemy)) + 1) && randVariant <= (99 - (100 - percentTreasure))) //Adventurer Variant
                {
                    if (numOfT > 0)
                    {
                        switch (gameManager.currentSeason)
                        {
                            case GameManager.Season.Summer:
                                tempDeck.Add(gameManager.AllVariantTTiles_Adventure[Random.Range(0, 4)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Fall:
                                tempDeck.Add(gameManager.AllVariantTTiles_Adventure[Random.Range(4, 8)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Winter:
                                tempDeck.Add(gameManager.AllVariantTTiles_Adventure[Random.Range(8, 12)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Spring:
                                tempDeck.Add(gameManager.AllVariantTTiles_Adventure[Random.Range(12, 16)]);
                                numOfT--;
                                break;
                        }
                    }
                    else
                    {
                        if (numOfL > 0)
                        {
                            switch (gameManager.currentSeason)
                            {
                                case GameManager.Season.Summer:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Adventure[Random.Range(0, 4)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Fall:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Adventure[Random.Range(4, 8)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Winter:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Adventure[Random.Range(8, 12)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Spring:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Adventure[Random.Range(12, 16)]);
                                    numOfL--;
                                    break;
                            }
                        }
                        else
                        {
                            if (numOfX > 0)
                            {
                                switch (gameManager.currentSeason)
                                {
                                    case GameManager.Season.Summer:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Adventure[Random.Range(0, 3)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Fall:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Adventure[Random.Range(3, 6)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Winter:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Adventure[Random.Range(6, 9)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Spring:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Adventure[Random.Range(9, 12)]);
                                        numOfX--;
                                        break;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                else if (randVariant >= ((99 - (100 - percentTreasure)) + 1) && randVariant <= 99) //Treasure Variant
                {
                    if (numOfT > 0)
                    {
                        switch (gameManager.currentSeason)
                        {
                            case GameManager.Season.Summer:
                                tempDeck.Add(gameManager.AllVariantTTiles_Treasure[Random.Range(0, 4)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Fall:
                                tempDeck.Add(gameManager.AllVariantTTiles_Treasure[Random.Range(4, 8)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Winter:
                                tempDeck.Add(gameManager.AllVariantTTiles_Treasure[Random.Range(8, 12)]);
                                numOfT--;
                                break;

                            case GameManager.Season.Spring:
                                tempDeck.Add(gameManager.AllVariantTTiles_Treasure[Random.Range(12, 16)]);
                                numOfT--;
                                break;
                        }
                    }
                    else
                    {
                        if (numOfL > 0)
                        {
                            switch (gameManager.currentSeason)
                            {
                                case GameManager.Season.Summer:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Treasure[Random.Range(0, 4)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Fall:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Treasure[Random.Range(4, 8)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Winter:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Treasure[Random.Range(8, 12)]);
                                    numOfL--;
                                    break;

                                case GameManager.Season.Spring:
                                    tempDeck.Add(gameManager.AllVariantLTiles_Treasure[Random.Range(12, 16)]);
                                    numOfL--;
                                    break;
                            }
                        }
                        else
                        {
                            if (numOfX > 0)
                            {
                                switch (gameManager.currentSeason)
                                {
                                    case GameManager.Season.Summer:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Treasure[Random.Range(0, 3)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Fall:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Treasure[Random.Range(3, 6)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Winter:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Treasure[Random.Range(6, 9)]);
                                        numOfX--;
                                        break;

                                    case GameManager.Season.Spring:
                                        tempDeck.Add(gameManager.AllVariantXTiles_Treasure[Random.Range(9, 12)]);
                                        numOfX--;
                                        break;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }

        for (int j = 10; j > 0; j--)
        {
            tempDeck = ShuffleDeck(tempDeck);
        }
        
        return tempDeck;
    }


    private List<TileBase> ShuffleDeck(List<TileBase> deck)
    {
       
        List<TileBase> tempDeck = new List<TileBase>();

        List<TileBase> topHalf = new List<TileBase>();
        List<TileBase> bottomHalf = new List<TileBase>();

        for (int j = 0; j < deck.Count; j++)
        {
            if (j < (deck.Count / 2))
            {
                topHalf.Add(deck[j]);
            }
            else
            {
                bottomHalf.Add(deck[j]);
            }
        }

        for (int x = 0; x < (deck.Count / 2); x++)
        {
            tempDeck.Add(topHalf[0]);
            topHalf.RemoveAt(0);

            tempDeck.Add(bottomHalf[0]);
            bottomHalf.RemoveAt(0);
        }
        
        return tempDeck;
    }
}
