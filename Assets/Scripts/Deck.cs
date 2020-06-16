using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{

    List<int> blueList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    List<int> redList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public bool[] cardsInBlueSlot;
    public bool[] cardsInRedSlot;
    public GameObject[] slotPos;
    public bool gameHasStarted;
    public GameObject deckSprite;
    Card card;
    GameMaster gm;

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
            if (card.isInDeck == false && gameHasStarted == true) Destroy(card.gameObject);
        }
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
