using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer survivorSprite;

    [SerializeField] private SurvivorInfo survivorInfo;
    public SurvivorInfo SurvivorInfo
    {
        get
        {
            return survivorInfo;
        }
        set
        {
            if (survivorInfo == value)
                return;
            survivorInfo = value;
            
            if(survivorInfo != null)
                survivorSprite.sprite = survivorInfo.survivorSprite;
        }
    }

    [SerializeField] private int hp;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            if (hp == value)
                return;
            hp = value;
            OnUpdateHealth?.Invoke(hp);
        }
    }
    public Action<int> OnUpdateHealth = (int health) => { };

    private void Awake()
    {
        if (survivorInfo != null)
            survivorSprite.sprite = survivorInfo.survivorSprite;
    }
}
