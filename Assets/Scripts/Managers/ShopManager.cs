using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Object References")]
    public GameObject shoppingPanel;
    public GameObject itemPanel;
    public GameObject mercenaryPanel;

    [Header("Current Items/Mercenaries Available")]
    public List<Item> itemsForPurchase;
    public List<HeroClass> heroesForHire;

    [Header("Item Sale Variables")]
    public int numOfItemsToSell = 8;
    public Button[] itemButtons;

    [Header("Mercenary Variables")]
    public int numOfMercenariesForHire = 3;
    public Button[] mercenaryButtons;


    /*
     * TODO:
        -Generate items and merc to sell, by referencing the respective Lists.

     *Steps for using Shop
     *  If Player accidentally closes the menu early, they may click on the town tile before placeing another tile to repoen the menu;
     * 
     *  1. Player Arrives in town
     *  2. Player is prompted to choose item shop or hero shop or leave
     *  3. Player shops (Detailed Below)
     *  4. When exiting the choosen shop, player is prompted again to choose shop or leave
        5. Player leaving moves the Current Phase to Tile Placement.

     *  3a. Player is presented with a grid of items
     *      Player selects an item, button opens second panel to select which hero to buy the item for.
     *      Player is returned to the item grid
     *      
     *  3b. Player is presented with a grid of Mercenaries
     *      Player selects an Mercenary, button opens second panel of current Heros in party and the player can select which slot to place the new hero.
     *      Player is returned to the mercenary grid;
    */



    public enum ShopType { items, mercenaries }
}
