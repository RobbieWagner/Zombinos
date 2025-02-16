using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Survivor
{
    public SurvivorInfo survivorInfo;

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
}
