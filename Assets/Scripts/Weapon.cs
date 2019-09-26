using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public Gem gemSlot;
    
    public Weapon EquipItem(HeroClass hero)
    {
        if (hero.weaponSlot != null)
        {
            Destroy(hero.armorSlot.gameObject);
            this.transform.SetParent(hero.transform);
            return this;
        }
        else
        {
            return this;
        }
    }
}
