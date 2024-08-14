using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameSection : GameSectionBase
{
    [SerializeField] private GameplaySubWidget gameplayWidget;
    [SerializeField, Range(1f,10f)] private float cardTimer = 5f;
    [SerializeField] private AudioSource effectAudioPlayer;
    [SerializeField] private AudioClip matchAudioClip;
    [SerializeField] private AudioClip ClickAudioClip;
    public override event Action OnSectionEnded;
    public List<int> cardsData;
    public bool[] cardsState;
    public Dictionary<int,UICardBtn> visualOpenCards;
    public Dictionary<int,UICardBtn> stateOpenCards;
    private Dictionary<int, float> openCardsTimer;
    private GameStateStructure gameState;
    private int playerMatches;
    private int playerClicks;
    public override Tween DisableSection()
    {
        sectionEnabled = false;
        gameplayWidget.OnUICardClicked -= OnCardPick;
        gameplayWidget.OnHomeBtnPressed -= OnHomeBtnClicked;
        return gameplayWidget.DeactivateWidget();
    }

    public override Tween EnableSection()
    {
        sectionEnabled = true;
        Tween widget = gameplayWidget.ActivateWidget();
        InitDataStructures();
        gameplayWidget.BuildGrid(cardsData, cardsState);
        widget.OnComplete(()=>
        {
            gameplayWidget.OnUICardClicked += OnCardPick;
            gameplayWidget.OnHomeBtnPressed += OnHomeBtnClicked;
        });
        return widget;
    }
    private void OnHomeBtnClicked()
    {
        GameStateHelper.Instance.SaveState(gameState);
        SceneManager.LoadScene(0);
    }
    private void OnApplicationQuit()
    {
        if (!sectionEnabled) return;
        GameStateHelper.Instance.SaveState(gameState);
    }
    private void InitDataStructures()
    {
        int totalCards = GameController.Instance.TotalCards;

        gameState = new GameStateStructure();
        
        if (GameController.Instance.StateLoaded)
        {
            bool result = GameStateHelper.Instance.LoadState(out gameState);
            if (result)
            {
                playerMatches = gameState.userMatches;
                playerClicks = gameState.userClicks;
                cardsState = gameState.cellsState;
                cardsData = gameState.cellsType.ToList();
                gameplayWidget.SetPlayerClicks(playerClicks);
                gameplayWidget.SetPlayerScore(playerMatches);
                GameController.Instance.CardsLayput = gameState.layout;
            }
            else
            {
                InitNewState(totalCards);
            }
        }
        else
        {
            InitNewState(totalCards);
        }

        visualOpenCards = new Dictionary<int, UICardBtn>();
        stateOpenCards = new Dictionary<int, UICardBtn>();
        openCardsTimer = new Dictionary<int, float>();
    }
    private void InitNewState(int totalCards)
    {
        gameState.layout = GameController.Instance.CardsLayput;
        cardsData = new List<int>();
        cardsState = new bool[totalCards];
        cardsData = PopulateValues(totalCards);
        gameState.cellsState = cardsState.ToArray();
        gameState.cellsType = cardsData.ToArray();
    }
    private List<int> PopulateValues(int totalCards)
    {
        List<int> possiableIndexes = Enumerable.Range(0, totalCards).ToList();
        List<int> possiableValue = Enumerable.Range(0, (totalCards / 2)).ToList();

        int[] cardsValues = new int[totalCards];

        for (int i = 0; i < possiableValue.Count; i++)
        {
            int firstIndex = UnityEngine.Random.Range(0, possiableIndexes.Count);
            cardsValues[possiableIndexes[firstIndex]] = possiableValue[i];
            possiableIndexes.RemoveAt(firstIndex);

            int secondIndex = UnityEngine.Random.Range(0, possiableIndexes.Count);
            cardsValues[possiableIndexes[secondIndex]] = possiableValue[i];
            possiableIndexes.RemoveAt(secondIndex);
        }
        return cardsValues.ToList();
    }
    private void OnCardPick(UICardBtn card)
    {
        effectAudioPlayer.PlayOneShot(ClickAudioClip);
        card.SetBtnInteractable(false);
        card.FlipCard(true, 0.25f).OnComplete(() =>
        {
            visualOpenCards.Add(card.CardIndex, card);
            stateOpenCards.Add(card.CardIndex, card);
            openCardsTimer.Add(card.CardIndex, cardTimer);
            CheckOpenCards();
        });
    }
    private void Update()
    {
        if (!sectionEnabled) return;
        
        if (openCardsTimer.Count <= 0) return;

        List<int> cardsToRemove = new List<int>();
        List<int> cardsKeys = new List<int>(openCardsTimer.Keys);
        foreach(int key in cardsKeys)
        {
            if (cardsState[key]) continue;

            openCardsTimer[key] -= Time.deltaTime;
            
            if (openCardsTimer[key] <= 0) cardsToRemove.Add(key);
        }
        RemoveIdleCards(cardsToRemove);
    }
    private void RemoveIdleCards(List<int> cardsToRemove)
    {
        foreach(int index in cardsToRemove)
        {
            openCardsTimer.Remove(index);
            if(stateOpenCards.ContainsKey(index)) stateOpenCards.Remove(index);
            UICardBtn card = visualOpenCards[index];
            visualOpenCards.Remove(index);
            card.FlipCard(false,0.25f);
            card.SetBtnInteractable(true);
        }
    }
    private void CheckOpenCards()
    {
        //if (stateOpenCards.Count <= 1) return;

        List<int> keys = new List<int>(stateOpenCards.Keys);
        for (int i = 0; i < keys.Count; i+=2)
        {
            if (i + 1 >= keys.Count)
            {
                //stateOpenCards.Remove(keys[i]);
                break;
            }
            playerClicks++;
            gameState.userClicks++;
            gameplayWidget.SetPlayerClicks(playerClicks);
            int key1 = keys[i];
            int key2 = keys[i + 1];
            UICardBtn card1 = stateOpenCards[key1];
            UICardBtn card2 = stateOpenCards[key2];
            //if (openCards[key].CardType == card.CardType && openCards[key].CardIndex != card.CardIndex && !cardsState[key])
            if (IsOpenValid(card1, card2))
            {
                effectAudioPlayer.PlayOneShot(matchAudioClip);
                Debug.Log($"A Match Index {card1.CardIndex} and {card2.CardIndex} | Type {card1.CardType}");
                HandleMatchingCards(key1, key2);
                continue;
            }
            else
            {
                stateOpenCards.Remove(key1);
                stateOpenCards.Remove(key2);

            }
        }
    }
    private bool IsOpenValid(UICardBtn card1, UICardBtn card2)
    {
        return card1.CardType == card2.CardType && card1.CardIndex != card2.CardIndex && !cardsState[card1.CardIndex];
    }
    private void HandleMatchingCards(int key1, int key2)
    {
        cardsState[key1] = true;
        cardsState[key2] = true;

        gameState.cellsState = cardsState;
        playerMatches++;
        gameState.userMatches++;

        gameplayWidget.SetPlayerScore(playerMatches);

        openCardsTimer.Remove(key1);
        openCardsTimer.Remove(key2);

        stateOpenCards[key1].SetBtnInteractable(false);
        stateOpenCards[key2].SetBtnInteractable(false);

        stateOpenCards[key1].ShakeCard(0.7f);
        stateOpenCards[key2].ShakeCard(0.7f);

        stateOpenCards.Remove(key1);
        stateOpenCards.Remove(key2);

        visualOpenCards.Remove(key1);
        visualOpenCards.Remove(key2);

        CheckGameEnding();
    }
    private void CheckGameEnding()
    {
        foreach(bool cellState in cardsState)
        {
            if (!cellState) return;
        }
        StartCoroutine(WaitTweenFinish());
    }
    private IEnumerator WaitTweenFinish()
    {
        yield return new WaitForSeconds(2f);
        OnGameEnd();
    }
    private void OnGameEnd()
    {
        GameStateHelper.Instance.DeleteState();
        // deal with game logic
        OnSectionEnded?.Invoke();
    }
}
