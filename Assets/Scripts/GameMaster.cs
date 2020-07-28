using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{

    public Unit selectedUnit;
    public Unit blueHero;
    public Unit redHero;
    public Unit blueCrystal;
    public Unit redCrystal;

    public int playerTurn = 1;
    public int movesLeft = 3;
    public int blueGold;
    public int redGold;
    public int cardsLeft = 3;

    public GameObject highlight;
    public GameObject turnIndicator;
    public GameObject coinColumns;
    public Text blueGoldText;
    public Text redGoldText;
    public Text blueCrystalText;
    public Text redCrystalText;
    public Text blueHeroText;
    public Text redHeroText;
    public Text cardsLeftText;

    public GameObject MovesBanner;
    public HealthIndicator healthIndicator;
    public Sprite[] blueSprites;
    public Sprite[] redSprites;
    public Transform[] crystalsPos;

    Deck deck;

    private bool canTurnChange = true;

    private void Start()
    {
        deck = FindObjectOfType<Deck>();
        blueGoldText.text = "  " + blueGold.ToString();
        redGoldText.text = "  " + redGold.ToString();
    }
    public void ResetTiles()
    {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.Reset();
        }
    }

    private void Update()
    {
        if (selectedUnit != null)
        {
            highlight.SetActive(true);
            highlight.transform.position = selectedUnit.transform.position + new Vector3(0, 0, -8);
        }
        else
        {
            highlight.SetActive(false);
        }

        foreach (Card card in FindObjectsOfType<Card>() as Card[])
        {
            if (card.isDragging == true)
            {
                selectedUnit = null;
                ResetTiles();
                foreach (Tile tile in FindObjectsOfType<Tile>())
                {
                    if (card.isSpell == false)
                    {
                        if (card.cardNumber == 1 && card.price <= blueGold)
                        {
                            if (tile.IsClear() && tile.transform.position.x > 0) tile.Highlight();
                        }
                        if (card.cardNumber == 2 && card.price <= redGold)
                        {
                            if (tile.IsClear() && tile.transform.position.x < 0) tile.Highlight();
                        }
                    }
                }
            }
        }

        if (blueGold >= 1000) blueGold = 999;
        if (redGold >= 1000) redGold = 999;

        if (blueGold < 10) blueGoldText.text = "  " + blueGold.ToString();
        else if (blueGold < 100) blueGoldText.text = " " + blueGold.ToString();
        else if (blueGold < 1000) blueGoldText.text = blueGold.ToString();
        if (redGold < 10) redGoldText.text = "  " + redGold.ToString();
        else if (redGold < 100) redGoldText.text = " " + redGold.ToString();
        else if (redGold < 1000) redGoldText.text = redGold.ToString();

        cardsLeftText.text = cardsLeft.ToString();
        blueCrystalText.text = blueCrystal.health.ToString();
        if (redCrystal.health >= 10) redCrystalText.text = redCrystal.health.ToString();
        else if (redCrystal.health < 10) redCrystalText.text = " " + redCrystal.health.ToString();

        if (blueHero.health >= 10) blueHeroText.text = blueHero.health.ToString();
        else if (blueHero.health < 10) blueHeroText.text = " " + blueHero.health.ToString();
        if (redHero.health >= 10) redHeroText.text = redHero.health.ToString();
        else if (redHero.health < 10) redHeroText.text = " " + redHero.health.ToString();
    }

    public void UpdateMovesLeft()
    {
        movesLeft -= 1;

        if (playerTurn == 1 && movesLeft != 0)
        {
            MovesBanner.GetComponent<SpriteRenderer>().sprite = blueSprites[movesLeft - 1];
        }
        else if (playerTurn == 2 && movesLeft != 0)
        {
            MovesBanner.GetComponent<SpriteRenderer>().sprite = redSprites[movesLeft - 1];
        }

        if (movesLeft == 0)
        {
            movesLeft = 3;
            StartCoroutine(EndTurn());
        }
    }

    public IEnumerator EndTurn()
    {
        canTurnChange = false;
        cardsLeft = 3;
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.hasMoved = false;
            unit.ResetAttackHighlights();
            unit.hasAttacked = false;
            unit.isAttacking = false;
            if (unit.nameID == "elite") unit.attacksLeft = 2;
            else unit.attacksLeft = 1;
            if (unit.isPoisoned == true)
            {
                if (unit.shieldsLeft >= 1)
                {
                    break;
                }
                else if (unit.health > 1)
                {
                    unit.health -= 1;
                    Instantiate(unit.dmgIcon, unit.transform.position, Quaternion.identity);
                }
                else if (unit.health == 1)
                {
                    unit.isPoisoned = false;
                }
            }
        }
        foreach (CoinColumn column in FindObjectsOfType<CoinColumn>())
        {
            if (column.columnNumber == 1)
            {
                blueGold += 5;
            }
            else if (column.columnNumber == 2)
            {
                redGold += 5;
            }
        }
        if (playerTurn == 1)
        {
            turnIndicator.GetComponent<Animator>().SetTrigger("Red");
            playerTurn = 2;
        }
        else if (playerTurn == 2)
        {
            turnIndicator.GetComponent<Animator>().SetTrigger("Blue");
            playerTurn = 1;
        }
        if (selectedUnit != null)
        {
            selectedUnit.selected = false;
            selectedUnit = null;
        }
        ResetTiles();
        yield return new WaitForSeconds(1.5f);
        if (playerTurn == 1)
        {
            MovesBanner.GetComponent<SpriteRenderer>().sprite = blueSprites[movesLeft - 1];
        }
        else if (playerTurn == 2)
        {
            MovesBanner.GetComponent<SpriteRenderer>().sprite = redSprites[movesLeft - 1];
        }
        yield return new WaitForSeconds(1.5f);
        canTurnChange = true;
    }
}
