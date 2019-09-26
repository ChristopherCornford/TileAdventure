using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Game Variables")]
    
    public Sprite sprite;
    [Header("Main Stats")]
    public int Health;
    [Space]
    public int currentHealth;
    [Header("Combat Stats")]
    public int Attack;
    [Space]
    public int Defense;
    [Space]
    public int Speed;
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