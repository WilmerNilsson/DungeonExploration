using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class AudioOcclusionHandler : MonoBehaviour
{
    [SerializeField] private int tickLength;
    [SerializeField] private int maxBounces;
    private int _currentTick;
    private int _currentTickTime;
    [SerializeField] private OcclusionChecker occlusionChecker;
    
    public static Dictionary<GameObject, float> OcclusionData = new Dictionary<GameObject, float>();
    
    private void FixedUpdate()
    {
        _currentTickTime++;
        if (_currentTickTime >= tickLength)
        {
            _currentTickTime = 0;
            _currentTick++;
            if (_currentTick >= maxBounces)
            {
                _currentTick = 0;
            }
        }

        if (_currentTick < 1)
        {
            GetOcclusionInstances();
        }
        //Do bounces until last bounce

        if (_currentTick >= maxBounces)
        {
            CompareDistances();
            SetOcclusionInstances();
        }
        
    }
    
    private List<GameObject> _tempOcclusionObjects = new List<GameObject>();
    
    private void GetOcclusionInstances()
    {
        if (!AudioManager.IsValid) return;
        foreach (var eventList in AudioManager.Instance.eventLists)
        {
            eventList.GetOcclusionList(out _tempOcclusionObjects);
            foreach (var go in _tempOcclusionObjects)
            {
                OcclusionData.TryAdd(go, 0);
            }
        }
    }

    private void CompareDistances()
    {
        
    }

    private void SortDistances()
    {
        
    }
    
    

    private void SetOcclusionInstances()
    {
        
    }

    public void SetOcclusionOneShot()
    {
        //Om occlusion inte finns cachad på objekt gör en snabb occlusion Check
    }
}
