using System;
using System.Collections;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public enum ActivatedBy
    {
        Trigger,
        Start,
        Other
    }

    public ActivatedBy activatedBy;
    
    public string tagToActivate;

    public bool activateOnce;

    public float activationDelay;
    
    public Instruction[] instructions;

    #region Triggering

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
    
    public void Activate()
    {
        if (activationDelay > 0)
        {
            StartCoroutine(ActivationDelay());
            return;
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

    private void InterpretInstructions()
    {
        foreach (Instruction instruction in instructions)
        {
            //switch ()
        }
    }
    
    #endregion
}
