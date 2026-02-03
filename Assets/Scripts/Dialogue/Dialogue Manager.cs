using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")] 
    [SerializeField] private GameObject dialoguePanel;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueName;

    [SerializeField] private Animator portraitAnimator;
    //[SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    //[SerializeField] private DataStorage data;
    
    private bool advanceDialogue = false;
    private bool skipDialogue = false;
    private float dialogueSpeed = 0.035f;

    private string sentence;
    
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string COLOR_TAG = "color";
    private const string SIZE_TAG = "font_size";
    private const string SPEED_TAG = "speed";


    public UnityEvent PlayIntroStory, onDialogueExit;

    //[Header("Choices UI")] 
    [SerializeField] private GameObject[] choices;

    //todo text
    //private TextMeshProUGUI[] choicesText;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }
    
    private static DialogueManager instance;

    private bool isTyping = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Dialogue Manager found");
        }
        instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        /*if (data.playStoryAtStart)
        {
            PlayIntroStory.Invoke();
        }
        else
        {
            InputManager.GetInstance().isLevelPlaying = true;
        }*/

        /*choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }*/
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (GetAdvancePressed())
        {
            ContinueStory();
        }

        if (GetSkipPressed())
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    public void EnterDialogueMode(TextAsset InkJSON)
    {
        currentStory = new Story(InkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        //InputManager.GetInstance().isLevelPlaying = false;
        
        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
        //InputManager.GetInstance().isLevelPlaying = true;
        //data.playStoryAtStart = false;
        onDialogueExit.Invoke();
    }

    public void ContinueStory()
    {
        if (currentStory.currentChoices.Count > 0)
        {
            return;
        }
        if (currentStory.canContinue)
        {
            //audioSource.Stop();
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = sentence;
                isTyping = false;
                return;
            }
            sentence = currentStory.Continue();
            
            StopAllCoroutines();
            
            HandleTags(currentStory.currentTags);
            if (dialogueSpeed > 0)
            {
                StartCoroutine(TypeSentence(sentence));
            }
            else
            {
                dialogueText.text = sentence;
            }

            
            //audioSource.PlayOneShot(audioClip);
            
            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.Log($"Tag could not be appropriately parsed: {tag}");
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    dialogueName.text = tagValue;
                    break;
                
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                
                case COLOR_TAG:
                    if (ColorUtility.TryParseHtmlString($"#{tagValue}", out Color color))
                    {
                        dialogueText.color = color;
                    }
                    else
                    {
                        Debug.LogWarning($"color not found: {tagValue}");
                    }
                    break;
                
                case SIZE_TAG:
                    if (float.TryParse(tagValue, out float size))
                    {
                        dialogueText.fontSize = size;
                    }
                    else
                    {
                        Debug.LogWarning($"size not found: {tagValue}");
                    }
                    break;
                
                case SPEED_TAG:
                    if (float.TryParse(tagValue, out float speed))
                    {
                        dialogueSpeed = speed;
                    }
                    else
                    {
                        Debug.LogWarning($"Couldn't parse speed: {tagValue}");
                    }
                    break;
                
                default:
                    Debug.LogWarning($"Tag came in but is not currently being handled: {tag}");
                    break;
            }
        }
    }
    private IEnumerator TypeSentence(string sentence)
    {
        bool richText = false;
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            if (letter.ToString() == "<")
            {
                richText = true;
            }

            if (letter.ToString() == ">")
            {
                richText = false;
            }
            dialogueText.text += letter;
            if (!richText)
            {
                yield return new WaitForSeconds(dialogueSpeed);
            }
        }

        isTyping = false;
        //audioSource.Stop();
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.Log("Need more choices");
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            //todo text
            //choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int index)
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = sentence;
            isTyping = false;
            return;
        }
        else
        {
            currentStory.ChooseChoiceIndex(index);
        }
    }

    public void OnAdvancePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            advanceDialogue = true;
        }
        else
        {
            advanceDialogue = false;
        }
    }

    private bool GetAdvancePressed()
    {
        bool result = advanceDialogue;
        advanceDialogue = false;
        return result;
    }
    
    public void OnSkipPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            skipDialogue = true;
        }
        else
        {
            skipDialogue = false;
        }
    }
    private bool GetSkipPressed()
    {
        bool result = skipDialogue;
        skipDialogue = false;
        return result;
    }
}
