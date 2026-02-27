using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomDialogue : MonoBehaviour
{
    [SerializeField] private List<TextAsset> greetingTexts = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dialogueTexts = new List<TextAsset>();

    private void Awake()
    {
        //FindAnyObjectByType<DialogueManager>().EnterDialogueMode(greetingTexts[Random.Range(0, greetingTexts.Count - 1)]);
    }

    public void PlayRandomDialogue()
    {
        //DialogueManager.GetInstance().EnterDialogueMode(dialogueTexts[Random.Range(0, dialogueTexts.Count - 1)]);
    }
}
