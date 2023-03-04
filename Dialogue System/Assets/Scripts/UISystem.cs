using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : Singleton<UISystem>
{
    public TMPro.TextMeshProUGUI dialogueText;
    public GameObject buttonContainer;
    public GameObject buttonPrefab;
    public GameObject UIRoot;

    private Queue<GameObject> buttonPool;
    private List<GameObject> activeButtons;
    

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
        dialogueText.text = eventData.text;
    }

    private void HideUI(DisableUI eventData)
    {
        foreach (GameObject button in activeButtons)
        {
            button.SetActive(false);
            buttonPool.Enqueue(button);
        }
        activeButtons.Clear();

        UIRoot.SetActive(false);
    }

    private void ShowResponseButtons (ShowResponses eventdata)
    {
        foreach(ResponseData response in eventdata.responses)
        {
            GameObject button = null;
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

            Button uiButton = button.GetComponent<Button>();
            uiButton.onClick.RemoveAllListeners();
            uiButton.onClick.AddListener(response.buttonAction);

            activeButtons.Add(button);
        }
        
    }
}
