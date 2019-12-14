using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public double uniqueID;

    [Header("Game Variables")]
    
    public Sprite sprite;
    [Header("Main Stats")]
    public int Level;
    [Space]
    public int Health;
    [Space]
    public int currentHealth;
    [Header("Combat Stats")]
    public int Attack;
    [Space]
    public int Defense;
    [Space]
    public int Speed;
    [Space]
    public Elements statusEffect;
    [Space]
    public int turnsLeftOfStatus;
    [Space]
    public Character forcedTarget;
    [Space]
    public bool isStunned;

    public float cooldownTimer;

    public int combatIndex;

    public GameObject selectionMarker;

    public GameObject selectionFillObj;

    public SpriteRenderer spriteRenderer;

    public Button button;

    public GameObject comsumableEffect;

    public Consumable 

    public TextMeshProUGUI gameLog;


    public void SetID()
    {
        this.uniqueID = Random.value * Time.time;
    }


    public void ResolveStatusEffect()
    {
        if (statusEffect != Elements.None)
        {
            if(turnsLeftOfStatus > 0)
            {
                switch (statusEffect)
                {
                    case Elements.Fire:
                        this.currentHealth--;
                        gameLog.text = this.name + " took 1 damage from their burns.";
                        break;

                    case Elements.Ice:
                        this.Speed--;
                        gameLog.text = this.name + " lost 1 Speed from the cold.";
                        break;

                    case Elements.Lightning:
                        isStunned = true;
                        break;

                    case Elements.Earth:
                        this.Defense--;
                        gameLog.text = this.name + " lost 1 Defence from the blunt force.";
                        break;
                }

                turnsLeftOfStatus--;
            }
            else if (turnsLeftOfStatus <= 0)
            {
                statusEffect = Elements.None;
            }
        }
    }
}

public enum Elements
{
    None,
    Fire,
    Ice,
    Lightning,
    Earth,
}

public enum HeroRole
{
    Knight,
    Arcanist,
    Fencer,
    Guardian,
    Mender,
    Dog,
}