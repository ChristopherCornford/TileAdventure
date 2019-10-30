using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Object References")]
    public GameObject promptPanel;
    public GameObject shoppingPanel;
    public GameObject itemPanel;
    public GameObject mercenaryPanel;
    public GameObject currentPartyPanel;

    [Header("Button Templates")]
    public GameObject itemTemplate;
    public GameObject mercenaryTemplate;
    public GameObject partyHeroTemplate;

    [Header("Current Items/Mercenaries Available")]
    public List<Item> itemsForPurchase;
    public List<HeroClass> mercenariesForHire;

    [Header("Item Sale Variables")]
    public int numOfItemsToSell = 6;

    [Header("Mercenary Variables")]
    public int numOfMercenariesForHire = 3;

    [Header("Script References")]
    public GameManager gameManager;
    public PartyBehaviour partyBehaviour;

    [SerializeField]
    private Item currentlySelectedItem;
    [SerializeField]
    private HeroClass currentlySelectedMercenary;


    private List<GameObject> uiObjectsToDelete = new List<GameObject>();

    /*
     * TODO:
        -Generate items and merc to sell, by referencing the respective Lists.

     *Steps for using Shop
     *  If Player accidentally closes the menu early, they may click on the town tile before placing another tile to reopen the menu;
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
    private void Awake()
    {
        gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        partyBehaviour = FindObjectOfType(typeof(PartyBehaviour)) as PartyBehaviour;

        promptPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { OpenShop(ShopType.Items); });
        promptPanel.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { OpenShop(ShopType.Mercenaries); });
    }


    public void OpenShop(ShopType shopType)
    {
        promptPanel.SetActive(true);

        uiObjectsToDelete.Clear();
        PopulateSalesLists();

        GenerateShop(shopType);
    }

    void PopulateSalesLists()
    {
        itemsForPurchase.Clear();
        mercenariesForHire.Clear();

        for (int i = 0; i < (numOfItemsToSell / 3); i++)
        {
            itemsForPurchase.Add(gameManager.AllArmor[Random.Range(0, gameManager.AllArmor.Count)]);
            itemsForPurchase.Add(gameManager.AllAccessories[Random.Range(0, gameManager.AllAccessories.Count)]);
            itemsForPurchase.Add(gameManager.AllWeapons[Random.Range(0, gameManager.AllWeapons.Count)]);
        }

        for (int i = 0; i < numOfMercenariesForHire; i++)
        {
            mercenariesForHire.Add(gameManager.AllHeroClasses[Random.Range(0, gameManager.AllHeroClasses.Count)]);
        }
    }

    void GenerateShop(ShopType shopType)
    {
        promptPanel.SetActive(false);
        shoppingPanel.SetActive(true);

        switch (shopType)
        {
            case ShopType.Items:

                itemPanel.SetActive(true);

                foreach(Item item in itemsForPurchase)
                {
                    GameObject newItem = Instantiate(itemTemplate, itemPanel.transform);
                    newItem.name = item.name;
                    newItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.name;
                    newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.statToBuff + " +" + item.buffValue;
                    newItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.goldPrice.ToString() + " Gold";
                    newItem.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { currentlySelectedItem = item; OpenCurrentPartyPanel(shopType);});

                    if (item.goldPrice > partyBehaviour.Gold)
                        newItem.transform.GetChild(3).GetComponent<Button>().interactable = false;

                    uiObjectsToDelete.Add(newItem);
                }

                break;

            case ShopType.Mercenaries:

                mercenaryPanel.SetActive(true);

                foreach (HeroClass mercenary in mercenariesForHire)
                {
                    GameObject newMercenary = Instantiate(mercenaryTemplate, mercenaryPanel.transform);
                    newMercenary.name = mercenary.name;
                    newMercenary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = mercenary.name;
                    newMercenary.transform.GetChild(1).GetComponent<Image>().sprite = mercenary.sprite;
                    newMercenary.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = mercenary.PrimaryStat;
                    newMercenary.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = mercenary.goldCost.ToString() + " Gold";

                    if (mercenary.goldCost <= partyBehaviour.Gold)
                    {
                        newMercenary.transform.GetChild(4).GetComponent<Button>().interactable = true;
                        newMercenary.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate {currentlySelectedMercenary = mercenary; partyBehaviour.Gold -= currentlySelectedMercenary.goldCost; OpenCurrentPartyPanel(shopType); });
                    }
                    else
                    {
                        newMercenary.transform.GetChild(4).GetComponent<Button>().interactable = false;
                    }

                    uiObjectsToDelete.Add(newMercenary);
                }

                break;
        }
    }

    private void OpenCurrentPartyPanel(ShopType shopType)
    {
        if(shopType == ShopType.Mercenaries)
        {
            if(partyBehaviour.heroParty.Count < 5)
            {
                partyBehaviour.AddCharacterToParty(currentlySelectedMercenary.name);

                ReturnToPromptScreen();

                return;
            }
        }

        itemPanel.SetActive(false);
        mercenaryPanel.SetActive(false);

        currentPartyPanel.SetActive(true);

        for (int i = 0; i < 5; i ++)
        {
            if (i < partyBehaviour.heroParty.Count)
            {
                int index = i;

                currentPartyPanel.transform.GetChild(i).name = partyBehaviour.heroParty[i].name;
                currentPartyPanel.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = partyBehaviour.heroParty[i].name;
                currentPartyPanel.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().sprite = partyBehaviour.heroParty[i].sprite;

                currentPartyPanel.transform.GetChild(i).transform.GetChild(2).GetComponent<Button>().interactable = true;
                currentPartyPanel.transform.GetChild(i).transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { EquipCurrentThing(shopType, partyBehaviour.heroParty[index], index); });
                
            }
            else if(i >= partyBehaviour.heroParty.Count)
            {
                currentPartyPanel.transform.GetChild(i).name = "Empty";
                currentPartyPanel.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Empty";
                currentPartyPanel.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().sprite = null;
                currentPartyPanel.transform.GetChild(i).transform.GetChild(2).GetComponent<Button>().interactable = false;
            }
        }
    }

    private void EquipCurrentThing(ShopType shopType, HeroClass hero, int index = 0)
    {
        switch (shopType)
        {
            case ShopType.Items:

                if (currentlySelectedItem != null)
                {
                    if (currentlySelectedItem.GetType() == typeof(Armor))
                    {
                        hero.ModifyStatsFromItems(currentlySelectedItem);
                        //hero.armorSlot = currentlySelectedItem as Armor;
                    }
                    else if (currentlySelectedItem.GetType() == typeof(Accessory))
                    {
                        hero.ModifyStatsFromItems(currentlySelectedItem);
                        //hero.accessorySlot = currentlySelectedItem as Accessory;
                    }
                    else if (currentlySelectedItem.GetType() == typeof(Weapon))
                    {
                        hero.ModifyStatsFromItems(currentlySelectedItem);
                        //hero.weaponSlot = currentlySelectedItem as Weapon;
                    }
                }

                break;

            case ShopType.Mercenaries:

                if (currentlySelectedMercenary != null)
                    partyBehaviour.AddCharacterToParty(currentlySelectedMercenary.name, index);

                break;
        }

        ReturnToPromptScreen();
    }

    public void ReturnToPromptScreen()
    {
        for(int i = 0; i < uiObjectsToDelete.Count; i++)
        {
            Destroy(uiObjectsToDelete[i]);
        }

        uiObjectsToDelete.Clear();

        currentlySelectedItem = null;
        currentlySelectedMercenary = null;

        currentPartyPanel.SetActive(false);
        mercenaryPanel.SetActive(false);
        itemPanel.SetActive(false);
        shoppingPanel.SetActive(false);

        promptPanel.SetActive(true);
    }

    public enum ShopType { Items, Mercenaries }
}
