using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartSubWidget : SubWidgetControllerBase
{
    [SerializeField] private CanvasGroup startGroup;
    [SerializeField] private CanvasGroup layputGroup;
    [SerializeField] private Button loadStateBtn;
    [SerializeField] private List<Vector2> options;
    [SerializeField] private TMP_Dropdown optionsDropDown;
    [SerializeField,Range(0f,5f)] private float fadeDuration = 2f;
    [SerializeField, Range(1f, 2f)] private float elementsScale = 1.2f;
    public event System.Action<Vector2> OnGameStart;
    public event System.Action OnSavedGameStart;
    private Vector2 currentOption;

    public override Tween ActivateWidget()
    {
        optionsDropDown.options = GetOptions();
        loadStateBtn.interactable = GameStateHelper.Instance.StateExits();
        currentOption = options[0];
        return startGroup.DOFade(1f, fadeDuration).OnComplete(()=> startGroup.blocksRaycasts=true);
    }
    private List<TMP_Dropdown.OptionData> GetOptions()
    {
        List<TMP_Dropdown.OptionData> dropDownList = new List<TMP_Dropdown.OptionData>();
        foreach(Vector2 vector in options)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = string.Concat(vector.x.ToString(), " X ", vector.y.ToString());
            dropDownList.Add(data);
        }
        return dropDownList;
    }
    public void OnOptionSelected(int index)
    {
        currentOption = options[index];
    }
    public override Tween DeactivateWidget()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(layputGroup.DOFade(0f, fadeDuration).OnStart(() => layputGroup.blocksRaycasts = false));
        seq.Join(startGroup.DOFade(0f, fadeDuration).OnStart(() => startGroup.blocksRaycasts = false));
        return seq;
    }
    public void OnLayoutSelectionBtnClick()
    {
        startGroup.blocksRaycasts = false;
        Sequence seq = DOTween.Sequence();
        seq.Append(startGroup.DOFade(0f, fadeDuration/2f));
        seq.Append(layputGroup.DOFade(1f, fadeDuration/2f).OnComplete(()=> layputGroup.blocksRaycasts = true));
    }
    public void OnUserSavedGameStart()
    {
        OnSavedGameStart?.Invoke();
    }
    public void OnUserGameStartBtn()
    {
        OnGameStart?.Invoke(currentOption);
    }
}
