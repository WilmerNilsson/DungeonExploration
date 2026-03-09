using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class NewDialogueManager : MonoBehaviour
{
    [SerializeField] private bool playOnStart = false;
    [SerializeField] public DialogueContainer dialogueTree;
    [Header("Dialogue UI")] 

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueName;
    [SerializeField] private GameObject continueButton;

    [SerializeField] private Animator portraitAnimator;
    private DialogueNodeData currentDialogueNode;
    public int RunCount = 0;
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
    
    private static NewDialogueManager instance;

    private bool isTyping = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Dialogue Manager found");
        }
        instance = this;
        continueButton.SetActive(false);
        RunCount = FindAnyObjectByType<TownFromDataCreator>().RunCount;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        if (playOnStart)
        {
            List<DialogueNodeData> greetingDatas = new List<DialogueNodeData>();
            for (int i = 0; i < dialogueTree.DialogueNodeDatas.Count; i++)
            {
                FindGreeting(dialogueTree, greetingDatas, dialogueTree.DialogueNodeDatas[i]);
            }
            EnterDialogueMode(greetingDatas[Random.Range(0, greetingDatas.Count - 1)].Title);
        }
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

    public static NewDialogueManager GetInstance()
    {
        return instance;
    }

    public void EnterDialogueMode(string DialogueName)
    {
        currentDialogueNode = dialogueTree.DialogueNodeDatas.Find(x => x.Title == DialogueName);
        if (!currentDialogueNode.DialogueAsset)
        {
            Debug.LogWarning("Dialogue node contains no dialogue asset");
            return;
        }
        onDialogueEnter?.Invoke(DialogueName);
        isTyping = false;
        lineIndex = 0;
        currentStory = new Story(currentDialogueNode.DialogueAsset.text);
        dialogueIsPlaying = true;
        continueButton.SetActive(true);
        ContinueStory();
    }
    
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        onDialogueEnter?.Invoke(inkJSON.name);
        //Debug.Log("entering dialogue mode");
        isTyping = false;
        lineIndex = 0;
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        //InputManager.GetInstance().isLevelPlaying = false;
        continueButton.SetActive(true);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        
        dialogueIsPlaying = false;
        dialogueText.text = "";
        continueButton.SetActive(false);
        currentDialogueNode.HasBeenRead = true;
        currentDialogueNode.ReadRun = RunCount;
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
    
    private void FindGreeting(DialogueContainer dialogueContainer, List<DialogueNodeData> dialogueNodeDatas, DialogueNodeData dialogueNodeData)
    {
        //Check if is valid greeting
        if ((dialogueNodeData.HasBeenRead && dialogueNodeData.ReadOnlyOnce) || !dialogueNodeData.DialogueAsset || !dialogueNodeData.IsGreeting)
        {
            return;
        }
        //Find links
        List<NodeLinkData> links = dialogueContainer.NodeLinks.FindAll(x => x.TargetNodeGuid == dialogueNodeData.Guid);
        bool parentsRead = true;
        for (int i = 0; i < links.Count && parentsRead; i++)
        {
            //Check if parent is read
            parentsRead = dialogueContainer.DialogueNodeDatas.Find(x => x.Guid == links[i].BaseNodeGuid).HasBeenRead;
        }

        if (parentsRead)
        {
            dialogueNodeDatas.Add(dialogueNodeData);
        }
    }
}
