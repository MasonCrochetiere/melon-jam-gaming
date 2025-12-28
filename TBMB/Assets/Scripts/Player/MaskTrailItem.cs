using System.Collections.Generic;
using UnityEngine;

public class MaskTrailItem : MonoBehaviour
{
    public MaskType type;
    public SpriteRenderer sprite { get; private set; }
    public Rigidbody2D rb;

    [SerializeField] List<Sprite> maskSprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetType(MaskType type)
    {
        this.type = type;
        switch (type)
        {
            case MaskType.Bag:
                sprite.sprite = maskSprites[0];
                break;
            case MaskType.Jump:
                sprite.sprite = maskSprites[1];
                break;
            case MaskType.Dash:
                sprite.sprite = maskSprites[2];
                break;
            case MaskType.Ball:
                sprite.sprite = maskSprites[3];
                break;
            case MaskType.None:
                sprite.sprite = maskSprites[4];
                break;
        }
    }
}
