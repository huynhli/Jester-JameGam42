using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Sprite[] sprites;
    private int spriteIndex;
    // Start is called before the first frame update
    void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        spriteIndex = 0;
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    // Update is called once per frame
    void AnimateSprite()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }
        playerSprite.sprite = sprites[spriteIndex];
    }
}
