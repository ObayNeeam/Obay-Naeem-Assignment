using DG.Tweening;
using System;
using UnityEngine;

public class StartSection : GameSectionBase
{
    [SerializeField] private StartSubWidget startWidget;
    public override event Action OnSectionEnded;

    public override Tween DisableSection()
    {
        startWidget.OnGameStart -= OnPlayBtnClick;
        startWidget.OnSavedGameStart -= OnSavePlayBtnClick;
        return startWidget.DeactivateWidget();
    }


    public override Tween EnableSection()
    {
        startWidget.OnGameStart += OnPlayBtnClick;
        startWidget.OnSavedGameStart += OnSavePlayBtnClick;
        Tween widgetTween = startWidget.ActivateWidget();
        return widgetTween;
    }
    private void OnSavePlayBtnClick()
    {
        GameController.Instance.StateLoaded = true;
        OnSectionEnded?.Invoke();
    }
    private void OnPlayBtnClick(Vector2 layout)
    {
        GameController.Instance.CardsLayput = layout;
        OnSectionEnded?.Invoke();
    }
}
