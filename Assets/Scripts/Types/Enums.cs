using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerAction 
{ 
    AttackUp, 
    AttackDown 
}

public enum Rank 
{ 
    PERFECT, 
    GOOD, 
    BAD, 
    MISS,
    HIT,
    BONUS
}

public enum NoteType
{
    Normal,
    Bad,
    Heal,
    Invincible
}