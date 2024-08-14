using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UICardBtn : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private Image mainImage;
    [SerializeField] private Image childImage;
    [SerializeField] private Button cardBtn;
    public RectTransform CardRect => (RectTransform)transform;
    public int CardType => cardType;
    public int CardIndex => cardIndex;
    public event System.Action<UICardBtn> OnCardClick;

    private int cardType;
    private int cardIndex;
    private Sprite flipSprite;
    public void SetCardData(int index, int type, Sprite flipSprite, bool revealed)
    {
        cardType = type;
        cardIndex = index;
        this.flipSprite = flipSprite;
        childImage.sprite = flipSprite;
        if (revealed)
        {
            mainImage.enabled = false;
            childImage.enabled = true;
            cardBtn.interactable = false;
        }
        else
        {
            mainImage.enabled = true;
            childImage.enabled = false;
        }

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!cardBtn.interactable) return;
        OnCardClick?.Invoke(this);
    }
    public Tween FlipCard(bool reveal, float tweenTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(CardRect.DOScaleX(0, tweenTime / 2f));
        seq.AppendCallback(() =>
        {
            if (reveal)
            {
                mainImage.enabled = false;
                childImage.enabled = true;
            }
            else
            {
                mainImage.enabled = true;
                childImage.enabled = false;
            }
        });
        seq.Append(CardRect.DOScaleX(1, tweenTime/2f));
        return seq;
    }
    public Tween ShakeCard(float tweenTime)
    {
        return CardRect.DOPunchRotation(Vector3.one * 7.5f,tweenTime,15);
    }

    public void SetBtnInteractable(bool state)
    {
        cardBtn.interactable = state;
    }
}
