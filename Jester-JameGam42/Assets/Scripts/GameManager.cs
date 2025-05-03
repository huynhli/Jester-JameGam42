using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Player player;
    private Rigidbody2D playerRigidBody;
    private List<DefenceCard> cardsInHand;
    public GameObject DefenceCardPrefab;
    public GameObject AttackCardPrefab;
    public GameObject titleScreen;
    public GameObject youWinScreen;

    private float randomX;
    private float randomY;

    

    private void Awake(){
        Application.targetFrameRate = 60;
        randomX = 0f;
        randomY = 0f;
        titleScreen.SetActive(true);
        youWinScreen.SetActive(false);
        Debug.Log("Game manager awake");
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
        InvokeRepeating(nameof(SpawnAttackCard), 2.5f/levelNumber, 2.5f/levelNumber);

        yield return new WaitForSeconds(10f);

        CancelInvoke(nameof(SpawnAttackCard));
    }

    private void SpawnAttackCard() {
        int random = UnityEngine.Random.Range(1, 14);

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

    public Vector2 pickDirection() {
        randomX = UnityEngine.Random.Range(-7.5f, 7.5f);
        randomY = UnityEngine.Random.Range(-2f, 2f);
        return new Vector2(randomX, randomY);
    }

    public void UseDefenceCard(int cardInHandIndex, Vector2 lookDirection, Vector2 playerRigidBodyPosition) {
        // summon as defence card
        DefenceCard selectedCard = cardsInHand[cardInHandIndex];
        cardsInHand.RemoveAt(cardInHandIndex);
        
        UpdateCardsInHand();

        GameObject summonedDefendCard = Instantiate(DefenceCardPrefab, playerRigidBodyPosition + lookDirection.normalized * 3f, Quaternion.LookRotation(lookDirection));
        summonedDefendCard.GetComponent<DefenceCard>().Initialize(selectedCard.GetHealth());
        Destroy(summonedDefendCard, 10f); // destroys object after 10 seconds
    }

    public void Pause() {
        Time.timeScale = 0f; // time doesn't update
        player.enabled = false;
    }

    private void UpdateCardsInHand() {
        // for each in cardsInHand {
        //      display at location based on total cards in hand --> cardsInHand.Length()
        // }
    }
    
}
