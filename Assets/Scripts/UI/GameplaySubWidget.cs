using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplaySubWidget : SubWidgetControllerBase
{
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField] private RectTransform cardParent;
    [SerializeField] private GridLayoutGroup cardsGridLayout;
    [SerializeField] private float gridElementsSpace;
    [SerializeField] private int gridElementsPadding;
    [SerializeField] private UICardBtn cardPrefab;
    [SerializeField] private TextMeshProUGUI playerScore;
    [SerializeField] private TextMeshProUGUI playerClicks;
    //[SerializeField] private List<Color> cardsColorTypes;
    [SerializeField] private List<Sprite> cardsSpriteTypes;
    public event System.Action<UICardBtn> OnUICardClicked;
    public event System.Action OnHomeBtnPressed;
    List<UICardBtn> cards;
    [SerializeField,Range(0f,5f)] private float fadeDuration = 2f;
#if UNITY_EDITOR
    public void BuildGridEditor(List<int> cardsData)
    {
        //BuildGrid(10);
    }
#endif
    public override Tween ActivateWidget()
    {
#if UNITY_EDITOR
        //BuildGridEditor();
#endif
        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(mainGroup.DOFade(1f, fadeDuration).OnComplete(()=>mainGroup.blocksRaycasts = true));
        return seq;
    }
    public void BuildGrid(List<int> cardsData, bool[] cardsStates)
    {
        ConfigureGrid();
        cards = new List<UICardBtn>();
        for (int i = 0; i < cardsData.Count; i++)
        {
            UICardBtn card =  Instantiate(cardPrefab, cardParent);
            int cardType = cardsData[i];

            card.SetCardData(i, cardType, cardsSpriteTypes[cardType], cardsStates[i]);

            cards.Add(card);
            card.OnCardClick += OnCardClicked;
        }
    }
    public void SetPlayerScore(int value)
    {
        playerScore.text = value.ToString();
    }
    public void SetPlayerClicks(int value)
    {
        playerClicks.text = value.ToString();
    }
    private void ConfigureGrid()
    {
        cardsGridLayout.spacing = new Vector2(gridElementsSpace, gridElementsSpace);
        cardsGridLayout.padding = new RectOffset(gridElementsPadding, gridElementsPadding, gridElementsPadding, gridElementsPadding);

        RectTransform gridRect = (cardsGridLayout.transform as RectTransform);
        Vector2 layout = GameController.Instance.CardsLayput;
        if (layout.x == layout.y)
        {
            cardsGridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            cardsGridLayout.constraintCount = Mathf.RoundToInt(layout.x);
        }
        if (layout.x > layout.y)
        {
            cardsGridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            cardsGridLayout.constraintCount = Mathf.RoundToInt(layout.x);
        }
        else
        {
            cardsGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            cardsGridLayout.constraintCount = Mathf.RoundToInt(layout.y);
        }
        float cellHeight = (gridRect.rect.height - (gridElementsPadding * 2) - ((layout.y - 1) * gridElementsSpace)) / layout.y;
        cardsGridLayout.cellSize = new Vector2(cellHeight, cellHeight);

    }
    private void OnCardClicked(UICardBtn card)
    {
        OnUICardClicked?.Invoke(card);
    }
    public void OnHomeBtnClick()
    {
        OnHomeBtnPressed?.Invoke();
    }
    public override Tween DeactivateWidget()
    {

        foreach (UICardBtn card in cards) card.OnCardClick -= OnCardClicked;
        cards.Clear();

        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(mainGroup.DOFade(0f, fadeDuration).OnStart(() => mainGroup.blocksRaycasts = false));
        return seq;
    }
}
