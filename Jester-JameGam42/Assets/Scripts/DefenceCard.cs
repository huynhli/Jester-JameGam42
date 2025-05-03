using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceCard : MonoBehaviour
{
    // Start is called before the first frame update
    private int cardHealth;
    void Start()
    {
        
    }

    // Update is called once per frame
    public int GetHealth() {
        return cardHealth;
    }

    public void Initialize(int healthOfCard) {
        cardHealth = healthOfCard;
        // update Sprite!
    }

    void Update()
    {
        
    }
}
