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
    
    public static Dictionary<GameObject, float> OcclusionData = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> Distances = new Dictionary<GameObject, float>();

    private class ObjectAndDistance : IComparer<ObjectAndDistance>
    {
        public GameObject obj;
        public float distance;
        
        public int Compare(ObjectAndDistance x, ObjectAndDistance y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (y is null) return 1;
            if (x is null) return -1;
            return x.distance.CompareTo(y.distance);
        }
    } 
    
    [SerializeField] private List<ObjectAndDistance> _objectsAndDistances = new List<ObjectAndDistance>();

    private float _tempDistance;

    //Kallas av ljudskript vid Start
    public static void AddToOcclusionList(GameObject gameObject)
    {
        OcclusionData.TryAdd(gameObject, 1);
    }

    //Kallas av ljudskript vid OnDestroy
    public static void RemoveFromOcclusionList(GameObject gameObject)
    {
        OcclusionData.Remove(gameObject);
    }
    
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

        //Första tick compare avstånd med närmsta 
        
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

    private void CreateDistanceList()
    {
        _objectsAndDistances.Clear();
        foreach (var kvp in OcclusionData)
        {
            _tempDistance = Vector3.Distance(kvp.Key.transform.position, AudioManager.Listener.transform.position);
            _objectsAndDistances.Add(new ObjectAndDistance { obj = kvp.Key, distance = _tempDistance });
        }
    }

    //Sortera listan 
    private void SortDistances()
    {
        _objectsAndDistances.Sort();
    }
    
    private void SetOcclusionInstances()
    {
        //Säg till alla eventlists att kolla i listan med occlusion data och uppdatera
    }

    public void SetOcclusionOneShot()
    {
        //Behövs nog inte kommer nog bara göras av oneshot metoden
    }

    private void DoOcclusionStep(GameObject source)
    {
        
    }
}
