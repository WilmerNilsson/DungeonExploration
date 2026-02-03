using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public enum ActivatedBy
    {
        Trigger,
        Start,
        Other
    }

    //Trigger settings
    public ActivatedBy activatedBy;
    public string tagToActivate;
    public bool activateOnce;
    public float activationDelay;
    
    public Instruction[] instructions;

    #region Triggering 

    //Metoder för att aktivera triggern baserat på activatedBy
    private void Start()
    {
        if (activatedBy == ActivatedBy.Start)
        {
            Activate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activatedBy == ActivatedBy.Trigger && other.CompareTag(tagToActivate))
        {
            Activate();
        }
    }

    #endregion
    
    #region Activation
    
    [ContextMenu("Activate")]
    public void Activate() //Aktivera med delay eller direkt
    {
        if (activationDelay > 0)
        {
            StartCoroutine(ActivationDelay());
        }
        else
        {
            InterpretInstructions();
        }
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
                    AudioManager.Instance.CreateInstance(instruction.path, instruction.gameObj, instruction.attachToObject, instruction.followObject );
                    break;
                case Instruction.Command.ReleaseInstance:
                    AudioManager.Instance.ReleaseInstance(instruction.path, instruction.gameObj);
                    break;
                case Instruction.Command.StartEvent:
                    AudioManager.Instance.StartEvent(instruction.path, instruction.gameObj);
                    break;
                case Instruction.Command.StopEvent:
                    AudioManager.Instance.StopEvent(instruction.path, instruction.stopMode, instruction.gameObj);
                    break;
                case Instruction.Command.SetParameter:
                    foreach (var paramToSet in instruction.parametersToSet)
                    {
                        AudioManager.Instance.SetParameter(instruction.path, paramToSet.name, paramToSet.value, instruction.gameObj);
                    }
                    break;
                case Instruction.Command.SetGlobalParameter:
                    foreach (var paramToSet in instruction.parametersToSet)
                    {
                        AudioManager.Instance.SetGlobalParameter(paramToSet.name, paramToSet.value);
                    }
                    break;
                case Instruction.Command.KeyOff:
                    AudioManager.Instance.KeyOff(instruction.path, instruction.gameObj);
                    break;
                case Instruction.Command.PlayOneShot:
                    var nameList = new List<string>();
                    var valueList = new List<float>();
                    foreach (var paramToSet in instruction.parametersToSet)
                    {
                        nameList.Add(paramToSet.name);
                        valueList.Add(paramToSet.value);
                    }
                    AudioManager.Instance.PlayOneShot(instruction.path, nameList.ToArray(), valueList.ToArray(), instruction.gameObj, instruction.followObject);
                    break;
                case Instruction.Command.LoadBank:
                    AudioManager.Instance.LoadBank(instruction.bankName);
                    break;
                case Instruction.Command.UnloadBank:
                    AudioManager.Instance.UnloadBank(instruction.bankName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    #endregion
}
