using System;
using System.Collections;
using UnityEngine;
using CustomAudio;

public class AudioTrigger : MonoBehaviour
{
    public enum ActivatedBy
    {
        OnTriggerEnter,
        OnTriggerExit,
        Start,
        Other,
        OnDestroy
    }

    //Trigger settings
    [Tooltip("How this audio trigger is activated")]
    public ActivatedBy activatedBy;
    public string tagToActivate;
    public bool activateOnce;
    private bool _hasActivated;
    public float activationDelay;
    
    public Instruction[] instructions;

    #region Triggering 

    //Metoder för att aktivera triggern baserat på activatedBy
    
    private void Start()
    {
        if (activatedBy == ActivatedBy.Start)
        {
            Activate();
            AudioDebug.Print("Trigger activated on start");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activatedBy == ActivatedBy.OnTriggerEnter && other.CompareTag(tagToActivate))
        {
            Activate();
            AudioDebug.Print("AudioTrigger activated on enter by " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (activatedBy == ActivatedBy.OnTriggerExit && other.CompareTag(tagToActivate))
        {
            Activate();
            AudioDebug.Print("AudioTrigger activated on exit by " + other.gameObject.name);
        }
    }

    private void OnDestroy()
    {
        if (activatedBy == ActivatedBy.OnDestroy)
        {
            activationDelay = 0;
            Activate();
            AudioDebug.Print("AudioTrigger activated on destroy");
        }
    }

    #endregion
    
    #region Activation
    
    [ContextMenu("Activate")]
    public void Activate() //Aktivera med delay eller direkt
    {
        if (activateOnce && _hasActivated) return;
        _hasActivated = true;
        if (activatedBy == ActivatedBy.Other)
        {
            AudioDebug.Print("AudioTrigger activated by other script or event");
        }
        if (activationDelay > 0)
        {
            StartCoroutine(ActivationDelay());
        }
        else
        {
            InterpretInstructions();
        }
    }

    private void ActivatePrivate()
    {
        InterpretInstructions();
    }

    private IEnumerator ActivationDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        InterpretInstructions();
    }

    private void InterpretInstructions() //Beroende på commmand i instruction, kalla rätt metod i audioManager
    {
        foreach (Instruction instruction in instructions)
        {
            switch (instruction.command)
            {
                case Instruction.Command.CreateInstance:
                    AudioSystem.instance.EventHandler.CreateInstance(instruction.path, instruction.gameObj, instruction.followObject);
                    break;
                case Instruction.Command.LoadSampleData:
                    AudioSystem.instance.EventHandler.LoadSampleData(instruction.path);
                    break;
                case Instruction.Command.StartEvent:
                    AudioSystem.instance.EventHandler.StartEvent(instruction.path, instruction.gameObj);
                    break;
                case Instruction.Command.StopEvent:
                    AudioSystem.instance.EventHandler.StopEvent(instruction.path, instruction.stopMode, instruction.gameObj);
                    break;
                case Instruction.Command.ReleaseInstance:
                    AudioSystem.instance.EventHandler.ReleaseInstance(instruction.path, instruction.gameObj);
                    break;
                case Instruction.Command.UnloadSampleData:
                    AudioSystem.instance.EventHandler.UnloadSampleData(instruction.path);
                    break;
                case Instruction.Command.SetParameter:
                    foreach (var paramToSet in instruction.parametersToSet)
                    {
                        AudioSystem.instance.EventHandler.SetParameter(instruction.path, paramToSet.name, paramToSet.value, instruction.gameObj);
                    }
                    break;
                case Instruction.Command.KeyOff:
                    AudioSystem.instance.EventHandler.KeyOff(instruction.path, instruction.gameObj);
                    break;
                case Instruction.Command.PlayOneShot:
                    var nameList = new string[instruction.parametersToSet.Length];
                    var valueList = new float[instruction.parametersToSet.Length];
                    for (int i = 0; i < instruction.parametersToSet.Length; i++)
                    {
                        nameList[i] = instruction.parametersToSet[i].name;
                        valueList[i] = instruction.parametersToSet[i].value;
                    }
                    AudioSystem.instance.EventHandler.PlayOneShot(instruction.path, nameList, valueList, instruction.gameObj, instruction.followObject);
                    break;
                case Instruction.Command.SetGlobalParameter:
                    foreach (var paramToSet in instruction.parametersToSet)
                    {
                        AudioSystem.instance.ParameterHandler.SetGlobalParameter(paramToSet.name, paramToSet.value);
                    }
                    break;
                case Instruction.Command.LoadBank:
                    AudioSystem.instance.BankHandler.LoadBank(instruction.bankName, instruction.loadSampleData);
                    break;
                case Instruction.Command.UnloadBank:
                    AudioSystem.instance.BankHandler.UnloadBank(instruction.bankName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    #endregion
}
