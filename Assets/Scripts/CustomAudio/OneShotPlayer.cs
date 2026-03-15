using System;
using System.Collections.Generic;
using UnityEngine;

public class OneShotPlayer : MonoBehaviour
{
    [Serializable]
    public class OneShotInstruction
    {
        public string path;
        public GameObject gameObject;
        public bool followObject;
        public ParameterToSet[] parametersToSet;
    }

    public bool emitterIsThisObject;
    public OneShotInstruction[] instructions;

    private void Start()
    {
        foreach (OneShotInstruction instruction in instructions)
        {
            if (instruction.gameObject || emitterIsThisObject) AudioOcclusionHandler.AddToOcclusionList(gameObject);
        }
    }

    public void Play(int index)
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        
        var nameList = new List<string>();
        var valueList = new List<float>();
        foreach (var paramToSet in instructions[index].parametersToSet)
        {
            nameList.Add(paramToSet.name);
            valueList.Add(paramToSet.value);
        }

        if (emitterIsThisObject)
        {
            AudioManager.Instance.PlayOneShot(instructions[index].path, nameList.ToArray(), valueList.ToArray(), gameObject, instructions[index].followObject);
        }
        else
        {
            AudioManager.Instance.PlayOneShot(instructions[index].path, nameList.ToArray(), valueList.ToArray(), instructions[index].gameObject, instructions[index].followObject);
        }
    }

    private void OnDestroy()
    {
        AudioOcclusionHandler.RemoveFromOcclusionList(gameObject);
    }
}
