using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStateStructure
{
    public bool[] cellsState;
    public int[] cellsType;
    public int userClicks;
    public int userMatches;
    public Vector2 layout;
}
