using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Consumable : MonoBehaviour
{
    public GameObject target;

    public Vector3 targetPos{ get { return target.transform.position;}}

    public string statToModify;
    public int amountToModify;

    public int itemTier;

    public TextMeshProUGUI battleLog;

    Consumable (GameObject targetObj, string statToMod, int modAmount, TextMeshProUGUI log)
    {
        this.target = targetObj;
        this.statToModify = statToMod;
        this.amountToModify = modAmount;
        this.battleLog = log;
    }

    public void Use()
    {
        switch (statToModify)
        {
            case "Health":
                if (target.GetComponent<HeroClass>())
                {
                    target.GetComponent<HeroClass>().currentHealth += amountToModify;
                }
                else if (target.GetComponent<Enemy>())
                {
                    target.GetComponent<Enemy>().currentHealth += amountToModify;
                }
                break;

            case "Attack":
                if (target.GetComponent<HeroClass>())
                {
                    target.GetComponent<HeroClass>().Attack += amountToModify;
                }
                else if (target.GetComponent<Enemy>())
                {
                    target.GetComponent<Enemy>().Attack += amountToModify;
                }
                break;

            case "Defense":
                if (target.GetComponent<HeroClass>())
                {
                    target.GetComponent<HeroClass>().Defense += amountToModify;
                }
                else if (target.GetComponent<Enemy>())
                {
                    target.GetComponent<Enemy>().Defense += amountToModify;
                }
                break;

            case "Speed":
                if (target.GetComponent<HeroClass>())
                {
                    target.GetComponent<HeroClass>().Speed += amountToModify;
                }
                else if (target.GetComponent<Enemy>())
                {
                    target.GetComponent<Enemy>().Speed += amountToModify;
                }
                break;
        }

        Destroy(this.gameObject);
    }
}
