using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool isDragging;
    public bool isSpell;
    public bool isHero;
    private bool returnHome;
    private bool hasN;
    public bool isInDeck;
    public bool isInDeckAsHero;
    public int slotInDeck;
    public int cardNumber;
    public int price;
    private float hoverAmount;
    public bool[] isOverSlot;
    public bool isOverHeroSlot;
    public Vector3 returnPos;
    public Vector3 normPos;
    private Vector3 normScale;
    public Vector3 movePositions;
    public Unit unit;
    public GameObject spell;
    public GameObject coin;
    public GameObject N1;
    public GameObject N2;
    public GameObject highlight;
    public GameObject blankCard;

    [HideInInspector]
    Animator camAnim;
    GameMaster gm;
    Deck deck;

    private void Start()
    {
        normPos = transform.position;
        returnPos = transform.position;
        normScale = transform.localScale;
        deck = FindObjectOfType<Deck>();
        gm = FindObjectOfType<GameMaster>();
        camAnim = FindObjectOfType<Camera>().GetComponent<Animator>();
        Debug.Log(gm.playerTurn);
    }
    public void OnMouseDown()
    {
        isDragging = true;
    }
    public void OnMouseUp()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.attackHighlight.SetActive(false);
        }

        GameObject slot = GameObject.Find("Slot (" + order(isOverSlot, true).ToString() + ")");
        GameObject heroSlot = GameObject.Find("Slot (10)");
        if (deck.gameHasStarted == false)
        {
            if (count(isOverSlot, true) == 1 && isHero == false)
            {
                foreach (Card cards in FindObjectsOfType<Card>())
                {
                    if (cards.transform.position.x == slot.transform.position.x && cards.transform.position.y == slot.transform.position.y) cards.returnHome = true;
                }
                isDragging = false;
                isInDeck = true;
                transform.position = slot.transform.position + new Vector3(0, 0, returnPos.z);
                
            }
            else if (count(isOverSlot, true) == 0 && isOverHeroSlot == true && isHero == true)
            {
                foreach (Card cards in FindObjectsOfType<Card>())
                {
                    if (cards.transform.position.x == heroSlot.transform.position.x && cards.transform.position.y == heroSlot.transform.position.y) cards.returnHome = true;
                }
                isDragging = false;
                isInDeckAsHero = true;
                transform.position = heroSlot.transform.position + new Vector3(0, 0, returnPos.z);
            }
            else
            {
                isDragging = false;
                returnHome = true;
                camAnim.SetTrigger("Shake");
            }
        }
        else
        {
            foreach (Tile tile in FindObjectsOfType<Tile>())
            {
                if (tile.transform.position == movePositions && tile.isWalkable == true)
                {
                    Unit instance = Instantiate(unit, tile.transform.position, Quaternion.identity);
                    instance.spawnedFromCard = true;
                    instance.price = price;
                    instance.isInstance = false;
                    foreach (Card card in FindObjectsOfType<Card>())
                    {
                        card.UpdateCardSlot(slotInDeck, cardNumber);
                    }
                    slotInDeck = 9;
                    returnPos = deck.deckSprite.transform.position;
                }

                else if (isSpell == true && movePositions.x >= -7 && movePositions.x <= 6 && movePositions.y >= 9 && movePositions.y <= 15 && cardNumber == 1 && price <= gm.blueGold ||
                         isSpell == true && movePositions.x >= -6 && movePositions.x <= 7 && movePositions.y >= 9 && movePositions.y <= 15 && cardNumber == 2 && price <= gm.redGold)
                {
                    if (tile.transform.position == movePositions)
                    {
                        GameObject spellInstance = Instantiate(spell, tile.transform.position, Quaternion.identity);
                        foreach (Card card in FindObjectsOfType<Card>())
                        {
                            card.UpdateCardSlot(slotInDeck, cardNumber);
                        }
                        slotInDeck = 9;
                        returnPos = deck.deckSprite.transform.position;
                        isDragging = false;
                        if (cardNumber == 1) gm.blueGold -= price;
                        if (cardNumber == 2) gm.redGold -= price;
                    }
                }
                else 
                {
                    isDragging = false;
                    returnHome = true;
                    tile.Reset();
                    camAnim.SetTrigger("Shake");
                }
            }
        }
    }

    private void OnMouseOver()
    {
        hoverAmount = 0.1f;

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = false;
            returnHome = true;
        }
    }
    private void OnMouseExit()
    {
        hoverAmount = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Slot (0)") isOverSlot[0] = true;
        if (collision.name == "Slot (1)") isOverSlot[1] = true;
        if (collision.name == "Slot (2)") isOverSlot[2] = true;
        if (collision.name == "Slot (3)") isOverSlot[3] = true;
        if (collision.name == "Slot (4)") isOverSlot[4] = true;
        if (collision.name == "Slot (5)") isOverSlot[5] = true;
        if (collision.name == "Slot (6)") isOverSlot[6] = true;
        if (collision.name == "Slot (7)") isOverSlot[7] = true;
        if (collision.name == "Slot (8)") isOverSlot[8] = true;
        if (collision.name == "Slot (9)") isOverSlot[9] = true;
        if (collision.name == "Slot (10)") isOverHeroSlot = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Slot (0)") isOverSlot[0] = false;
        if (collision.name == "Slot (1)") isOverSlot[1] = false;
        if (collision.name == "Slot (2)") isOverSlot[2] = false;
        if (collision.name == "Slot (3)") isOverSlot[3] = false;
        if (collision.name == "Slot (4)") isOverSlot[4] = false;
        if (collision.name == "Slot (5)") isOverSlot[5] = false;
        if (collision.name == "Slot (6)") isOverSlot[6] = false;
        if (collision.name == "Slot (7)") isOverSlot[7] = false;
        if (collision.name == "Slot (8)") isOverSlot[8] = false;
        if (collision.name == "Slot (9)") isOverSlot[9] = false;
        if (collision.name == "Slot (10)") isOverHeroSlot = false;
    }

    void Update()
    {
        if (isInDeckAsHero == true && cardNumber == 1) deck.cardInBlueHeroSlot = true;
        if (isInDeckAsHero == true && cardNumber == 2) deck.cardInRedHeroSlot = true;
        movePositions = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y - 0.5f), 10);
        foreach (Card card in FindObjectsOfType<Card>())
        {
            if (this.isInDeck == true && this.cardNumber == 1)
            {
                deck.cardsInBlueSlot[order(isOverSlot, true)] = true;
            }
            else if (this.isInDeck == true && this.cardNumber == 2)
            {
                deck.cardsInRedSlot[order(isOverSlot, true)] = true;
            }
        }
        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            transform.Translate(mousePosition);
            if (isInDeck == true)
            {
                if (cardNumber == 1)
                {
                    deck.cardsInBlueSlot[order(isOverSlot, true)] = false;
                }
                else
                {
                    deck.cardsInRedSlot[order(isOverSlot, true)] = false;
                }
            }
            isInDeck = false;
            isInDeckAsHero = false;
            if (isHero && cardNumber == 1) deck.cardInBlueHeroSlot = false;
            if (isHero && cardNumber == 2) deck.cardInRedHeroSlot = false;

            if (isSpell == true && deck.gameHasStarted == true && cardNumber == 1 && price <= gm.blueGold || isSpell == true && deck.gameHasStarted == true && cardNumber == 2 && price <= gm.redGold)
            {
                foreach (Unit unit in FindObjectsOfType<Unit>())
                {
                    if (Mathf.Abs(movePositions.x - unit.transform.position.x) <= 1 && Mathf.Abs(movePositions.y - unit.transform.position.y) <= 1 && cardNumber == 1 && movePositions.x <= 6 && movePositions.x >= -7 && movePositions.y >= 9 && movePositions.y <= 15
                     || Mathf.Abs(movePositions.x - unit.transform.position.x) <= 1 && Mathf.Abs(movePositions.y - unit.transform.position.y) <= 1 && cardNumber == 2 && movePositions.x >= -6 && movePositions.x <= 7 && movePositions.y >= 9 && movePositions.y <= 15)
                    {
                        unit.attackHighlight.SetActive(true);
                    }
                    else
                    {
                        unit.attackHighlight.SetActive(false);
                    }
                }
            }

            //if (deck.gameHasStarted && movePositions.x >= -7 && movePositions.x <= 7 && movePositions.y >= 9 && movePositions.y <= 15)
            //{
                //highlight.transform.position = movePositions + new Vector3(0, 0, -5);
                //highlight.SetActive(true);
            //}
            //else
            //{
                //highlight.SetActive(false);
            //}
        }
        //else
        //{
            //highlight.SetActive(false);
        //}
        if (returnHome == true)
        {
            isInDeck = false;
            isInDeckAsHero = false;
            if (isHero && cardNumber == 1) deck.cardInBlueHeroSlot = false;
            if (isHero && cardNumber == 2) deck.cardInRedHeroSlot = false;
            if (cardNumber == 1)
            {
                deck.cardsInBlueSlot[order(isOverSlot, true)] = false;
            }
            else
            {
                deck.cardsInRedSlot[order(isOverSlot, true)] = false;
            }
            if (transform.position != returnPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, returnPos, Time.deltaTime * 60);
            }
            else returnHome = false;
        }

        if (isInDeck == true || isInDeckAsHero == true)
        {
            transform.localScale = normScale + Vector3.one * 0.3f + Vector3.one * hoverAmount / 2;
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
        }
        else
        {
            transform.localScale = normScale + Vector3.one * hoverAmount;
            transform.position = new Vector3(transform.position.x, transform.position.y, returnPos.z);
        }

        if (deck.gameHasStarted == true)
        {
            isInDeck = true;

            float solidDistance = 0.5f;

            float dist = transform.position.y - returnPos.y;
            if (dist < solidDistance) dist = solidDistance;
            if (dist > solidDistance + 2) dist = 2;
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, solidDistance / dist);
            coin.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, solidDistance / dist);
            N1.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, solidDistance / dist);
            N2.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, solidDistance / dist);
        }
        if (deck.gameHasStarted == true && isDragging == false && cardNumber == gm.playerTurn)
        {
            if (slotInDeck <= 4)
            {
                transform.position = Vector3.MoveTowards(transform.position, deck.slotPos[slotInDeck].transform.position, Time.deltaTime * 25);
            }
            else if (slotInDeck > 4)
            {
                transform.position = Vector3.MoveTowards(transform.position, deck.deckSprite.transform.position, Time.deltaTime * 25);
            }
        }
        else if (deck.gameHasStarted == true && isDragging == false && cardNumber != gm.playerTurn)
        {
            transform.position = Vector3.MoveTowards(transform.position, deck.deckSprite.transform.position, Time.deltaTime * 25);
        }
        if (transform.position == deck.slotPos[slotInDeck].transform.position && deck.gameHasStarted == true)
        {
            returnPos = deck.slotPos[slotInDeck].transform.position;
        }
    }

    public void UpdateCardSlot(int leavingSlot, int number)
    {
        if (cardNumber == number)
        {
            if (slotInDeck > leavingSlot) slotInDeck -= 1;
        }
    }

    public int count(bool[] array, bool flag)
    {
        int value = 0;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == flag) value++;
        }

        return value;
    }
    public int order(bool[] array, bool flag)
    {
        int value = 0;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == flag) value = i;
        }

        return value;
    }
}
