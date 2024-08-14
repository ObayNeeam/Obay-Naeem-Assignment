using DG.Tweening;
using UnityEngine;

public abstract class SubWidgetControllerBase : MonoBehaviour
{
    private void Start()
    {
        SetupWidget();
    }
    public Tween activateTween;
    public Tween deactivateTween;
    public event System.Action OnWidgetDiable;
    protected virtual void SetupWidget()
    {

    }
    public abstract Tween ActivateWidget();
    public abstract Tween DeactivateWidget();

}
