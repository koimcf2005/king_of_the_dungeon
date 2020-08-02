using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroIcons : MonoBehaviour
{

    public bool isAbility;
    public bool hasActivated;

    public Unit hero;
    public Unit[] skeletons;
    public GameObject necroHighlight;
    public GameObject highlight;
    public GameObject graveEffect;

    public Animator camAnim;
    GameMaster gm;

    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        camAnim = FindObjectOfType<Camera>().GetComponent<Animator>();
    }

    void Update()
    {
        if (hero == null) Destroy(this.gameObject);
        if (isAbility == true)
        {
            if (hero.playerNumber == gm.playerTurn && gm.selectedUnit == hero && hasActivated == false)
            {
                GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = Color.white;
            }
            else
            {
                GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = Color.black;
            }
        }
    }

    private void OnMouseDown()
    {
        if (isAbility == true && hasActivated == false)
        {
            if (hero.playerNumber == gm.playerTurn && gm.selectedUnit == hero)
            {
                if (hero.nameID == "necromancer")
                {
                    StartCoroutine(NecromancerAbility());
                }
            }
        }
    }

    public IEnumerator NecromancerAbility()
    {
        hero.anim.SetTrigger("Attack");
        hasActivated = true;
        gm.selectedUnit = null;
        gm.ResetTiles();
        hero.ResetAttackHighlights();

        yield return new WaitForSeconds(0.66f);

        camAnim.SetTrigger("Shake");
        Instantiate(graveEffect, hero.transform.position, Quaternion.identity);
        StartCoroutine(gm.EndTurn());
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            float XAbs = Mathf.Abs(hero.transform.position.x - unit.transform.position.x);
            float YAbs = Mathf.Abs(hero.transform.position.y - unit.transform.position.y);

            if (XAbs <= 2 && YAbs <= 2)
            {
                if (unit.playerNumber == hero.playerNumber && unit != hero && Random.value > 0.1)
                {
                    unit.hasGraveEffect = true;
                }
                else if (unit.playerNumber != hero.playerNumber)
                {
                    unit.health -= 1;
                    DamageIcon instance = Instantiate(unit.dmgIcon, unit.transform.position, Quaternion.identity);
                    instance.Setup(1);

                    if (unit.health <= 0)
                    {
                        Instantiate(unit.deathPar, unit.transform.position, Quaternion.identity);
                        Destroy(unit.gameObject);
                    }
                }
            }
        }
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            float XAbs = Mathf.Abs(hero.transform.position.x - tile.transform.position.x);
            float YAbs = Mathf.Abs(hero.transform.position.y - tile.transform.position.y);

            if (XAbs <= 2 && YAbs <= 2)
            {
                if (Random.value > 0.9 && tile.IsClear())
                {
                    Instantiate(skeletons[hero.playerNumber - 1], tile.transform.position, Quaternion.identity);
                }
            }
        }
    }

}
