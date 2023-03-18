using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


class StringReveal
{
    string textToReveal = null;

    float currentTime;
    float secondsPerChar;
    int currentStringIndex = 0;

    public void StartReveal(string text, float duration)
    {
        secondsPerChar = duration / (text.Length + 2);
        textToReveal = text;

        currentStringIndex = 0;
        currentTime = 0.0f;
    }

    public bool isDone()
    {
        return (textToReveal == null || currentStringIndex == (textToReveal.Length));
    }

    public void ForceFinish()
    {
        currentStringIndex = (textToReveal.Length);
        currentTime = 0.0f;
    }


    public string GetCurrentRevealedText()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= secondsPerChar && currentStringIndex < (textToReveal.Length))
        {
            currentStringIndex++;
            currentTime = 0.0f;
        }

        return textToReveal.AsSpan(0, currentStringIndex).ToString();
    }
}

public class UISystem : Singleton<UISystem>
{
    public TMPro.TextMeshProUGUI dialogueText;
    public GameObject buttonContainer;
    public GameObject buttonPrefab;
    public GameObject UIRoot;

    private Queue<GameObject> buttonPool;
    private List<GameObject> activeButtons;

    private StringReveal typewriter = new StringReveal();

    private Color[] karmaColors = { Color.red, Color.green };

    private void Start()
    {
        activeButtons = new List<GameObject>();

        EvtSystem.EventDispatcher.AddListener<ShowDialogueText>(ShowUI);
        EvtSystem.EventDispatcher.AddListener<ShowResponses>(ShowResponseButtons);
        EvtSystem.EventDispatcher.AddListener<DisableUI>(HideUI);

        buttonPool = new Queue<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            var button = GameObject.Instantiate(buttonPrefab, buttonContainer.transform);
            button.SetActive(false);
            buttonPool.Enqueue(button);
        }
    }

    private void OnDisable()
    {
        EvtSystem.EventDispatcher.RemoveListener<ShowDialogueText>(ShowUI);
    }

    private void ShowUI(ShowDialogueText eventData)
    {
        UIRoot.SetActive(true);

        typewriter.StartReveal(eventData.text, eventData.duration);
    }

    private void HideUI(DisableUI data)
    {
        foreach (GameObject button in activeButtons)
        {
            button.SetActive(false);
            buttonPool.Enqueue(button);
        }
        activeButtons.Clear();

        UIRoot.SetActive(false);
    }

    private void ShowResponseButtons(ShowResponses eventData)
    {
        //Force the dialogue line to display fully.
        if (!typewriter.isDone())
        {
            typewriter.ForceFinish();
            dialogueText.text = typewriter.GetCurrentRevealedText();
        }

        foreach (ResponseData response in eventData.responses)
        {
            GameObject button = null;

            //If buttton not in the queue create one.
            if (!buttonPool.TryDequeue(out button))
            {
                button = GameObject.Instantiate(buttonPrefab, buttonContainer.transform);
            }
            button.SetActive(true);

            TMPro.TextMeshProUGUI label = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (label != null)
            {
                label.text = response.text;
            }

            //Decide the color index based on karma score.  Start with 0 for red.
            int ColorIndex = 0;
            if (response.karmaScore > 0)
            {
                ColorIndex = 1;
            }

            Button uiButton = button.GetComponent<Button>();

            //Set the Colors of the button based on karma
            ColorBlock block = uiButton.colors;
            block.normalColor = karmaColors[ColorIndex];
            uiButton.colors = block;

            uiButton.onClick.RemoveAllListeners();
            uiButton.onClick.AddListener(response.buttonAction);

            activeButtons.Add(button);
        }
    }

    private void Update()
    {
        if (UIRoot.activeSelf && !typewriter.isDone())
            dialogueText.text = typewriter.GetCurrentRevealedText();
    }
}