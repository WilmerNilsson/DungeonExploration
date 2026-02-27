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
    [SerializeField] private string StartDialogueName;
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private DialogueTree dialogueTree;
    [Header("Dialogue UI")] 
    [SerializeField] private GameObject dialoguePanel;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueName;

    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private List<DialogueSelectButton> selectButtons = new List<DialogueSelectButton>();
    //[SerializeField] private AudioSource audioSource;

    //[SerializeField] private DataStorage data;
    
    private bool advanceDialogue = false;
    private bool skipDialogue = false;
    private float dialogueSpeed = 0.035f;

    private string sentence;
    
    private const string NAME_TAG = "name";
    private const string ANIMATION_TAG = "animation";
    private const string COLOR_TAG = "color";
    private const string SIZE_TAG = "font_size";
    private const string SPEED_TAG = "speed";
    private const string INDEX_TAG = "index";
    private const string EVENT_TAG = "event";

    
    public UnityEvent<string> onDialogueEnter;
    public UnityEvent<int> onStartLine;
    public UnityEvent onEndLine, onDialogueExit;
    public List<UnityEvent> storyEvents = new List<UnityEvent>();
    private int lineIndex = 0;

    
    private TextMeshProUGUI[] choicesText;

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
        if (playOnStart)
        {
            EnterDialogueMode(StartDialogueName);
        }
        HandleDialogue();
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

    public void EnterDialogueMode(string DialogueName)
    {
        TextAsset InkJSON = new TextAsset();
        for (int i = 0; i < dialogueTree.Dialogues.Count; i++)
        {
            if (dialogueTree.Dialogues[i].Name == DialogueName)
            {
                onDialogueEnter?.Invoke(DialogueName);
                dialogueTree.Dialogues[i].HasBeenRead = true;
                //Debug.Log("entering dialogue mode");
                isTyping = false;
                lineIndex = 0;
                currentStory = new Story(dialogueTree.Dialogues[i].InkJson.text);
                dialogueIsPlaying = true;
                dialoguePanel.SetActive(true);
                //InputManager.GetInstance().isLevelPlaying = false;
        
                ContinueStory();
                return;
            }
        }
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        //InputManager.GetInstance().isLevelPlaying = true;
        //data.playStoryAtStart = false;
        onDialogueExit.Invoke();
        //Debug.Log("exitDialogueMode");
    }

    public void ContinueStory()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        if (isTyping)
        {
            onEndLine?.Invoke();
            //Debug.Log("endLine type");
            StopAllCoroutines();
            dialogueText.text = sentence;
            isTyping = false;
            return;
        }
        if (currentStory.canContinue)
        {
            //audioSource.Stop();
            //Debug.Log("startLine");
            onStartLine?.Invoke(lineIndex);
            sentence = currentStory.Continue();
            lineIndex++;
            
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
                case NAME_TAG:
                    dialogueName.text = tagValue;
                    break;
                
                case ANIMATION_TAG:
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
                
                case INDEX_TAG:
                    if (int.TryParse(tagValue, out int index))
                    {
                        lineIndex += index;
                    }
                    else
                    {
                        Debug.LogWarning($"Couldn't parse index: {tagValue}");
                    }
                    break;
                
                case EVENT_TAG:
                    if (int.TryParse(tagValue, out int eventIndex))
                    {
                        storyEvents[eventIndex]?.Invoke();
                    }
                    else
                    {
                        Debug.LogWarning($"Couldn't parse event: {tagValue}");
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

        //Debug.Log("endLine done");
        onEndLine?.Invoke();
        isTyping = false;
        //audioSource.Stop();
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

    private void HandleDialogue()
    {
        for (int i = 0; i < selectButtons.Count; i++)
        {
            selectButtons[i].gameObject.SetActive(true);
        }
        int currentButtons = 0;
        for (int i = 0; i < dialogueTree.Dialogues.Count && currentButtons < selectButtons.Count; i++) //all dialogues in tree
        {
            if (!dialogueTree.Dialogues[i].HasBeenRead || !dialogueTree.Dialogues[i].IsUnreadable) //do not display dialogue that has already been read
            {
                bool prerequisitesRead = true;
                for (int j = 0; j < dialogueTree.Dialogues[i].PrerequisiteNames.Count && prerequisitesRead; j++) //all prerequisites to play this dialogue
                {
                    for (int k = 0; k < dialogueTree.Dialogues.Count; k++) //all dialogues in tree
                    {
                        if (dialogueTree.Dialogues[i].PrerequisiteNames[j] == dialogueTree.Dialogues[k].Name) //find prerequisites
                        {
                            if (!dialogueTree.Dialogues[k].HasBeenRead) //check if prerequisite is read
                            {
                                prerequisitesRead = false;
                                break;
                            }
                        }
                    }
                }

                if (prerequisitesRead)
                {
                    selectButtons[currentButtons].DialogueName =  dialogueTree.Dialogues[i].Name;
                    selectButtons[currentButtons].buttonText.text = dialogueTree.Dialogues[i].ButtonText;
                    currentButtons++;
                }
            }
        }

        if (currentButtons < selectButtons.Count)
        {
            for (int i = currentButtons; i < selectButtons.Count; i++)
            {
                selectButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
