using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatsScroll : MonoBehaviour
{

    public GameObject noUnitText;
    public GameObject unitStats;

    public GameObject unitSprite;

    public GameObject heartIcon;
    public GameObject damageIcon;
    public GameObject defDamageIcon;
    public GameObject shieldIcon;
    public GameObject tileSpeedIcon;

    public Text heartText1;
    public Text heartText2;
    public Text damageText1;
    public Text damageText2;
    public Text defDamageText;
    public Text shieldText;
    public Text tileSpeedText;

    GameMaster gm;

    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
    }

    void Update()
    {
        if (gm.selectedUnit == null)
        {
            noUnitText.SetActive(true);
            unitStats.SetActive(false);
        }
        else
        {
            noUnitText.SetActive(false);
            unitStats.SetActive(true);

            heartText1.text = gm.selectedUnit.health.ToString();
            heartText2.text = gm.selectedUnit.maxHealth.ToString();

            damageText1.text = gm.selectedUnit.attackDamage.ToString();
            damageText2.text = gm.selectedUnit.normAttackDmg.ToString();

            defDamageText.text = gm.selectedUnit.defenceDamage.ToString();

            shieldText.text = gm.selectedUnit.shieldsLeft.ToString();

            tileSpeedText.text = gm.selectedUnit.tileSpeed.ToString();

            unitSprite.GetComponent<SpriteRenderer>().sprite = gm.selectedUnit.GetComponentInChildren<SpriteRenderer>().sprite;

        }
    }
}
