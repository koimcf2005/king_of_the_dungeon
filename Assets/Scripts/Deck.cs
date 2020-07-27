using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{

    List<int> blueList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    List<int> redList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public bool[] cardsInBlueSlot;
    public bool[] cardsInRedSlot;
    public bool cardInBlueHeroSlot;
    public bool cardInRedHeroSlot;
    public GameObject[] slotPos;
    public GameObject deckSprite;
    public GameObject blueCards;
    public GameObject blueBlanks;
    [HideInInspector]
    public float blueBlanksNormY;
    public GameObject redCards;
    public bool gameHasStarted;
    Card card;
    GameMaster gm;

    private void Start()
    {
        blueBlanksNormY = blueBlanks.transform.position.y;
    }

    private void Update()
    {
        foreach (Card card in FindObjectsOfType<Card>())
        {
            if (card.isInDeck == true && card.cardNumber == 1 && gameHasStarted == true)
            {
                card.slotInDeck = UniqueRandomBlue();
            }
            else if (card.isInDeck == true && card.cardNumber == 2 && gameHasStarted == true)
            {
                card.slotInDeck = UniqueRandomRed();
            }
        }

        /**if (gameHasStarted == false)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (Card card in FindObjectsOfType<Card>())
            {
                card.returnPos.x = card.blankCard.transform.position.x;
                card.returnPos.y = card.blankCard.transform.position.y;
                if (mousePos.x < -3.7 && card.transform.position.x > -10.7 && card.transform.position.x < -3.7)
                {
                    Vector3 cardPos = card.transform.position;
                    Vector2 blueBlankPos = blueBlanks.transform.position;

                    float cardMaxHeight = card.normPos.y + 2.35f;
                    float dist = card.normPos.y - card.transform.position.y;

                    float blanksMaxHeight = blueBlanksNormY + 2.35f;

                    cardPos.y += Input.mouseScrollDelta.y * -0.575f;
                    blueBlankPos.y += Input.mouseScrollDelta.y * -0.02f;
                    card.transform.position = cardPos;
                    blueBlanks.transform.position = blueBlankPos;

                    if (card.transform.position.y > cardMaxHeight)
                    {
                        cardPos.y = cardMaxHeight;
                        card.transform.position = cardPos;
                    }

                    if (blueBlanks.transform.position.y > blanksMaxHeight)
                    {
                        blueBlankPos.y = blanksMaxHeight;
                        blueBlanks.transform.position = blueBlankPos;
                    }

                    if (card.transform.position.y < card.normPos.y)
                    {
                        cardPos.y = card.normPos.y;
                        card.transform.position = cardPos;
                    }

                    if (blueBlanks.transform.position.y < blueBlanksNormY)
                    {
                        blueBlankPos.y = blueBlanksNormY;
                        blueBlanks.transform.position = blueBlankPos;
                    }
                }
            }
        }
        */
    }
    public int UniqueRandomBlue()
    {
        int rand = Random.Range(0, blueList.Count);
        int value = blueList[rand];
        blueList.RemoveAt(rand);
        return value;
    }
    public int UniqueRandomRed()
    {
        int rand = Random.Range(0, redList.Count);
        int value = redList[rand];
        redList.RemoveAt(rand);
        return value;
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

}
