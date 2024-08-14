using DG.Tweening;
using UnityEngine;
public class EndSubWidget : SubWidgetControllerBase
{
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField, Range(0f, 5f)] private float fadeDuration = 2f;

    public override Tween ActivateWidget()
    {
        return mainGroup.DOFade(1, fadeDuration);
    }

    public override Tween DeactivateWidget()
    {
        return mainGroup.DOFade(0, fadeDuration);
    }
}
