using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Player player;
    private Rigidbody2D playerRigidBody;
    private List<GameObject> cardsInHand;
    public GameObject DefenceCardPrefab;
    public GameObject AttackCardPrefab;
    public GameObject HandCardPrefab;
    public GameObject titleScreen;
    public GameObject youWinScreen;
    private int maxCardsInHand;

    private float randomX;
    private float randomY;

    

    private void Awake(){
        Application.targetFrameRate = 60;
        randomX = 0f;
        randomY = 0f;
        titleScreen.SetActive(true);
        youWinScreen.SetActive(false);
        Debug.Log("Game manager awake");
        cardsInHand = new List<GameObject>();
        maxCardsInHand = 3;
        // AttackCardPrefab.enabled = false;
        // Pause();
        Play();
    }

    public void Play(){
        Debug.Log("Playing");
        // player.ResetPlayer();
        titleScreen.SetActive(false);

        Time.timeScale = 1f;
        // player.enabled = true;

        StartCoroutine(LevelFlow());
    }

    private IEnumerator LevelFlow() {
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(RunLevel(1));

        yield return new WaitForSeconds(10f);
        yield return StartCoroutine(RunLevel(2));

        yield return new WaitForSeconds(10f);
        yield return StartCoroutine(RunLevel(3));

        youWinScreen.SetActive(true);
    }

    private IEnumerator RunLevel(int levelNumber) {
        SpawnDefenseCards(levelNumber);
        InvokeRepeating(nameof(SpawnAttackCard), 0.8f/levelNumber, 0.8f/levelNumber);
        yield return new WaitForSeconds(3f);
        CancelInvoke(nameof(SpawnAttackCard));
    }

    private void SpawnAttackCard() {
        int random = UnityEngine.Random.Range(0, 13);
        bool goodSpotToSpawn = false;
        Vector2 direction = Vector2.zero;
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        while (!goodSpotToSpawn) {
            direction = pickDirection();
            Vector2 directionToPlayer = direction - new Vector2(playerRigidBody.position.x, playerRigidBody.position.y);
            float magDir = directionToPlayer.magnitude; 
            if (magDir > 2) {
                goodSpotToSpawn = true;
            } 
        }
        GameObject summonedAttackCard = Instantiate(AttackCardPrefab, direction, Quaternion.identity);
        summonedAttackCard.GetComponent<AttackCard>().Initialize(random);
        Destroy(summonedAttackCard, 1f);

    }

    private Vector2 pickDirection() {
        randomX = UnityEngine.Random.Range(-7.5f, 7.5f);
        randomY = UnityEngine.Random.Range(-2f, 2f);
        return new Vector2(randomX, randomY);
    }

    private void SpawnDefenseCards(int levelNum) {
        int cardPositionInHand = cardsInHand.Count;
        if (cardPositionInHand >= maxCardsInHand) {
            return;
        }
        int cardsToAdd = Mathf.Min(levelNum + 1, maxCardsInHand - cardPositionInHand);

        for (int i = 0; i < cardsToAdd; i++) {
            AddCardToHand(i, cardPositionInHand);
            player.cardsLeft++;
            cardPositionInHand++;
        }
    }

    private void AddCardToHand(int counter, int cardPositionInHand) {
        GameObject cardInHand = Instantiate(HandCardPrefab, new Vector2((-4f + (1.5f*cardPositionInHand)), -4f), Quaternion.identity);
        cardInHand.GetComponent<SpriteRenderer>().sortingOrder = 10;
        cardInHand.GetComponent<DefenceCard>().Initialize(counter);
        cardsInHand.Add(cardInHand);
    }

    public void UseDefenceCard(int cardInHandIndex, Vector2 lookDirection, Vector2 playerRigidBodyPosition) {
        // summon as defence card
        DefenceCard selectedCard = cardsInHand[cardInHandIndex].GetComponent<DefenceCard>();
        cardsInHand.RemoveAt(cardInHandIndex);
        
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        GameObject summonedDefendCard = Instantiate(DefenceCardPrefab, playerRigidBodyPosition + lookDirection.normalized, Quaternion.Euler(0, 0, angle));
        summonedDefendCard.GetComponent<DefenceCard>().Initialize(selectedCard.GetHealth());
        Destroy(summonedDefendCard, 1.5f); // destroys object after 1.5 seconds
        Destroy(selectedCard); 
        player.cardsLeft--;
        UpdateCardsInHand(cardInHandIndex); // re-instantiate the hand? or instantiate from index
    }

    public void Pause() {
        Time.timeScale = 0f; // time doesn't update
        player.enabled = false;
    }

    private void UpdateCardsInHand(int cardInHandIndex) {
        // edge case at end of list
        // if (cardInHandIndex == cardsInHand.Count) {
        //     cardInHandIndex--;
        //     return;
        // // edge case no cards left
        // } else if (cardsInHand.Count == 0) {
        //     return;
        // } else {
        // // can destroy all and reform list    not this
        // // or can destroy single and shift all right elements to the left by one (single already destroyed, list updated, so just destroy and instantiate starting at counter)
        //     for (int counter = cardInHandIndex; counter < cardsInHand.Count; counter++) {
        //         int tempCardHealth = cardsInHand[counter].GetComponent<DefenceCard>().GetHealth(); 
        //         Destroy(cardsInHand[counter]);
        //         AddCardToHand(tempCardHealth, counter);
        //     }
        // }
    }


    
}
