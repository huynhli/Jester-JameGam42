using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour {

    [Header("Player")]
    public Player player;
    private Rigidbody2D playerRigidBody;

    [Header("Prefabs")]
    public GameObject DefenceCardPrefab;
    public GameObject AttackCardPrefab;
    public GameObject HandCardPrefab;

    [Header("Screens")]
    public Canvas titleScreen;
    public Canvas introScreen;
    public Canvas controlsScreen;
    
    public Canvas healthBanner;
    public Canvas deathScreen;
    
    public Canvas youWinScreen;
    
    public Canvas creditScreen;

    [Header("Game Logic")]
    public GameObject startButton;
    private Coroutine levelFlowCoroutine;
    private List<GameObject> cardsInHand;
    private int maxCardsInHand;
    private float randomX;
    private float randomY;

    

    private void Awake(){
        Application.targetFrameRate = 60;
        randomX = 0f;
        enabled = false;
        titleScreen.enabled = true;
        startButton.SetActive(true);
        introScreen.enabled = false;
        controlsScreen.enabled = false;
        healthBanner.enabled = false;
        deathScreen.enabled = false;
        youWinScreen.enabled = false;
        creditScreen.enabled = false;
        cardsInHand = new List<GameObject>();
        maxCardsInHand = 5;
        Debug.Log("Awake");
        Pause();
    }

    public void Play(){
        StartCoroutine(GameFlow());

    }

    private IEnumerator GameFlow() {
        Time.timeScale = 1f;
        yield return StartCoroutine(IntroFlow());

        healthBanner.enabled = true;
        player.enabled = true;

        levelFlowCoroutine = StartCoroutine(LevelFlow());
        yield return levelFlowCoroutine;

        yield return StartCoroutine(WinnerFlow());

    }

    private IEnumerator IntroFlow() {
        titleScreen.enabled = false;
        startButton.SetActive(false);
        introScreen.enabled = true;
        yield return new WaitForSeconds(4f);
        introScreen.enabled = false;
        controlsScreen.enabled = true;
        yield return new WaitForSeconds(5f);
        controlsScreen.enabled = false;
    }

    // Level Flow and timings
    private IEnumerator LevelFlow() {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(RunLevel(1));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(RunLevel(1));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(RunLevel(2));
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator RunLevel(int levelNumber) {
        SpawnDefenseCards(levelNumber);
        InvokeRepeating(nameof(SpawnAttackCard), 1.6f/levelNumber, 1.6f/levelNumber);
        yield return new WaitForSeconds(5f);
        CancelInvoke(nameof(SpawnAttackCard));
    }


    // Spawn attack cards
    private void SpawnAttackCard() {
        int random = UnityEngine.Random.Range(0, 13);
        bool goodSpotToSpawn = false;
        Vector2 direction = Vector2.left;
        Vector2 directionToPlayer = Vector2.right;
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        while (!goodSpotToSpawn) {
            direction = pickDirectionHelper();
            directionToPlayer = direction - new Vector2(playerRigidBody.position.x, playerRigidBody.position.y);
            float magDir = directionToPlayer.magnitude; 
            if (magDir > 2) {
                goodSpotToSpawn = true;
            } 
        }
        GameObject summonedAttackCard = Instantiate(AttackCardPrefab, direction, Quaternion.identity);
        summonedAttackCard.GetComponent<AttackCard>().Initialize(random, directionToPlayer);
    }

    private Vector2 pickDirectionHelper() {
        randomX = UnityEngine.Random.Range(-7f, 7f);
        randomY = UnityEngine.Random.Range(-2f, 2f);
        return new Vector2(randomX, randomY);
    }


    // spawning defense cards per level into hand
    private void SpawnDefenseCards(int levelNum) {
        int cardPositionInHand = cardsInHand.Count;
        if (cardPositionInHand >= maxCardsInHand) {
            return;
        }
        int cardsToAdd = Mathf.Min(levelNum + 2, maxCardsInHand - cardPositionInHand);
        for (int i = 0; i < cardsToAdd; i++) {
            AddCardToHand(12, player.cardsLeft);
            player.cardsLeft++;
        }
    }

    private void AddCardToHand(int spritePicker, int cardPositionInHand) {
        GameObject cardInHand = Instantiate(HandCardPrefab, new Vector2((-4f + (1.5f*cardPositionInHand)), -4f), Quaternion.identity);
        cardInHand.GetComponent<SpriteRenderer>().sortingOrder = 10;
        cardInHand.GetComponent<DefenceCard>().Initialize(spritePicker);
        cardsInHand.Add(cardInHand);
    }


    // use defence card from hand and spawn it in level
    public void UseDefenceCard(int cardInHandIndex, Vector2 lookDirection, Vector2 playerRigidBodyPosition) {
        // get defence card from hand
        DefenceCard selectedCard = cardsInHand[cardInHandIndex].GetComponent<DefenceCard>();
        
        // summon hand card as defence card
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        GameObject summonedDefendCard = Instantiate(DefenceCardPrefab, playerRigidBodyPosition + lookDirection.normalized, Quaternion.Euler(0, 0, angle));
        summonedDefendCard.GetComponent<DefenceCard>().Initialize(selectedCard.spriteNum);
        Debug.Log(selectedCard.GetHealth());
        Destroy(summonedDefendCard, 1.5f); // destroys object after 1.5 seconds

        UpdateCardsInHand(cardInHandIndex); // re-instantiate the hand? or instantiate from index
    }

    public void Pause() {
        Debug.Log("Pause");
        Time.timeScale = 0f; // time doesn't update
        player.enabled = false;
    }

    private IEnumerator WinnerFlow() {
        healthBanner.enabled = false;
        player.enabled = false;
        youWinScreen.enabled = true;
        yield return new WaitForSeconds(3f);
        youWinScreen.enabled = false;
        creditScreen.enabled = true;
        yield return new WaitForSeconds(5f);
        creditScreen.enabled = false;
        titleScreen.enabled = true;
        startButton.SetActive(true);
    }

    private void UpdateCardsInHand(int cardInHandIndex) {
        // destroy hand card, update cards left
        GameObject tempToDestroy = cardsInHand[cardInHandIndex];
        cardsInHand.RemoveAt(cardInHandIndex);
        Destroy(tempToDestroy); 
        player.cardsLeft--;

        // edge case cards in hand = 0
        if (cardsInHand.Count == 0) {
            return;
        // edge case destroyed end of list
        } else if (cardInHandIndex == cardsInHand.Count) {
            cardInHandIndex--;
            return;
        
        } else {
            for (int counter = cardInHandIndex; counter < cardsInHand.Count; counter++) {
                GameObject card = cardsInHand[counter];
                card.transform.position = new Vector2(-4f + (1.5f * counter), -4f);
            }
        }
    }


    private void FixedUpdate() {
        if (player.totalHealth <= 0) {
            StartCoroutine(Dead());
        }
    }

    private IEnumerator Dead() {
        if (levelFlowCoroutine != null) {
            StopCoroutine(levelFlowCoroutine);
        }
        Pause();
        healthBanner.enabled = false;
        player.enabled = true;
        player.animator.SetTrigger("died");
        yield return new WaitForSeconds(5f); // duration of death animation
        deathScreen.enabled = true;
        yield return new WaitForSeconds(5f);
        deathScreen.enabled = false;
        titleScreen.enabled = true;
        startButton.SetActive(true);
        cardsInHand.Clear();
        player.Reset();
    }
}
