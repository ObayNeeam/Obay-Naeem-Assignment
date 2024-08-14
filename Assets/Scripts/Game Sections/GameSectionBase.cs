using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSectionBase : MonoBehaviour
{
    protected bool sectionEnabled;
    public abstract event System.Action OnSectionEnded;
    public abstract Tween EnableSection();
    public abstract Tween DisableSection();
}
