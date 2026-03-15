using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class AudioOcclusionHandler : MonoBehaviour
{
    [SerializeField] private int tickLength;
    
    [Header("Occlusion Settings")]
    [SerializeField] public LayerMask layerMask;
    [Range(0, 60)] public float spread = 4;
    [Range(0, 1)]  public float bounceValue = 0.25f;
    [SerializeField] private int maxBounces;
    [SerializeField] private int linesOnEitherSide;
    [SerializeField] private int objectsToCheck;
    
    private const float Offset = 0.02f;
    
    public bool drawDebug;
    public bool debugMsg;
    
    private int _lineCount;
    private int _posModifier;
    private Vector3 _direction;
    private float _distance;
    private float _totalDistance;
    private Vector3 _sourcePos;
    private Vector3 _targetPos;

    private bool newTick;

    private struct LineData
    {
        
        public RaycastHit[] Hits;
        public Vector3[] Direction;
        public float[] Distance;
        public bool[] DidHit;
    }

    private struct HitData
    {
        public float Score;
        public LineData[] Lines;
        public Vector3 SourcePos;
    }

    private HitData[] _hitDatas;
    
    private int _currentTick;
    private int _currentTickTime;
    
    public static Dictionary<GameObject, float> OcclusionData = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> _distances = new Dictionary<GameObject, float>();

    private class ObjectAndDistance
    {
        public GameObject obj;
        public float distance;
    } 
    
    private List<ObjectAndDistance> _objectsAndDistances = new List<ObjectAndDistance>();

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
        //Vid första tick kolla avstånd med alla objekt och sortera
        if (newTick)
        {
            newTick = false;
            if (_currentTick < 1)
            {
                CreateDistanceList();
                SortDistances();
                InitializeHitDatas();
                InitialOcclusionCheck();
            }
        
            //Gör raycasts till sista tick
            if (_currentTick > 0 && _currentTick < maxBounces)
            {
                DoOcclusionStep();
            }

            //Sista tick gör sista checks och sätt occlusion
            if (_currentTick >= maxBounces)
            {
                SetOcclusionInstances();
            }
        }
        
        
        _currentTickTime++;
        if (_currentTickTime >= tickLength)
        {
            _currentTickTime = 0;
            _currentTick++;
            newTick = true;
            if (_currentTick >= maxBounces)
            {
                _currentTick = 0;
            }
            Debug.Log(_currentTick);
        }
        
    }
    
    private void CreateDistanceList()
    {
        _objectsAndDistances.Clear();
        foreach (var kvp in OcclusionData)
        {
            _tempDistance = Vector3.Distance(kvp.Key.transform.position, AudioManager.Listener.transform.position);
            _objectsAndDistances.Add(new ObjectAndDistance { obj = kvp.Key, distance = _tempDistance });
            if (debugMsg) Debug.Log("Adding " + kvp.Key.name + " to Distance List");
        }
    }

    //Sortera listan 
    private void SortDistances()
    {
        for (int i = 0; i < _objectsAndDistances.Count - 1; i++)
        {
            ObjectAndDistance temp = _objectsAndDistances[i];
            int min = i;
            for (int j = i + 1; j < _objectsAndDistances.Count; j++)
            {
                if (_objectsAndDistances[j].distance < temp.distance)
                {
                    temp = _objectsAndDistances[j];
                    min = j;
                }
            }
            _objectsAndDistances[min] = _objectsAndDistances[i];
            _objectsAndDistances[i] = temp;
        }
        if (debugMsg) Debug.Log("Sorting " + _objectsAndDistances.Count + " distances");
    }


    private void InitializeHitDatas()
    {
        _hitDatas = new HitData[objectsToCheck];
        _lineCount = linesOnEitherSide * 2 + 1;
        for (int i = 0; i < objectsToCheck; i++)
        {
            _hitDatas[i] = new HitData()
            {
                Score = 0,
                Lines = new LineData[_lineCount],
                SourcePos = _objectsAndDistances[i].obj.transform.position,
            };
            for (int j = 0; j < _lineCount; j++)
            {
                _hitDatas[i].Lines[j] = new LineData()
                {
                    Hits = new RaycastHit[1 + maxBounces],
                    Direction = new Vector3[1 + maxBounces],
                    Distance = new float[1 + maxBounces],
                    DidHit = new bool[1 + maxBounces]
                };
            }
        }
        if (debugMsg) Debug.Log("Initializing " + objectsToCheck + " hitDatas");
    }

    private void InitialOcclusionCheck()
    {
        for (int i = 0; i < objectsToCheck; i++)
        {
            _posModifier = -linesOnEitherSide - 1;
            _sourcePos = _objectsAndDistances[i].obj.transform.position;
            for (int j = 0; j < _lineCount; j++)
            {
                _posModifier++;
                _hitDatas[i].Lines[j].Distance[_currentTick] = Vector3.Distance(_sourcePos, AudioManager.Listener.transform.position);
                _hitDatas[i].Lines[j].Direction[_currentTick] = (AudioManager.Listener.transform.position - _sourcePos).normalized;
                _hitDatas[i].Lines[j].Direction[_currentTick] = (Quaternion.AngleAxis(spread * _posModifier, Vector3.up) * _direction).normalized;

                
                if (Physics.Raycast(_sourcePos, _hitDatas[i].Lines[j].Direction[_currentTick], out _hitDatas[i].Lines[j].Hits[0], _hitDatas[i].Lines[j].Distance[_currentTick], layerMask))
                {
                    _hitDatas[i].Lines[j].DidHit[_currentTick] = true;
                    if (drawDebug) Debug.DrawLine(_sourcePos, _hitDatas[i].Lines[j].Direction[_currentTick] * _hitDatas[i].Lines[j].Distance[_currentTick], Color.green, Time.fixedDeltaTime * tickLength);
                }
                else //Om första raycast inte träffar något, spara det så att vi kan checka nästa tick
                {
                    _hitDatas[i].Lines[j].DidHit[_currentTick] = false;
                    if (drawDebug) Debug.DrawRay(_sourcePos, _hitDatas[i].Lines[j].Direction[_currentTick] * _hitDatas[i].Lines[j].Distance[_currentTick], Color.green, Time.fixedDeltaTime * tickLength);
                }
            }
        }
    }
    
    private void DoOcclusionStep()
    {
        for (int i = 0; i < objectsToCheck; i++)
        {
            for (int j = 0; j < _lineCount; j++)
            {
                if (_hitDatas[i].Lines[j].DidHit[_currentTick - 1])
                {
                    _hitDatas[i].Score += bounceValue;
                    if (_hitDatas[i].Score > 1)
                    {
                        break;
                    }
                }
                else
                {
                    if (_currentTick == 1)
                    {
                        if (!Physics.Linecast(
                                _hitDatas[i].SourcePos +
                                _hitDatas[i].Lines[j].Direction[_currentTick - 1].normalized *
                                _hitDatas[i].Lines[j].Distance[_currentTick - 1],
                                AudioManager.Listener.transform.position, layerMask))
                        {
                            _hitDatas[i].Score += bounceValue;
                            if (drawDebug) Debug.DrawLine(_hitDatas[i].SourcePos +
                                                          _hitDatas[i].Lines[j].Direction[_currentTick - 1].normalized *
                                                          _hitDatas[i].Lines[j].Distance[_currentTick - 1], AudioManager.Listener.transform.position, Color.green, Time.fixedDeltaTime * tickLength);
                        }
                        else
                        {
                            _hitDatas[i].Score = 1;
                        }
                    }
                    else
                    {
                        
                    }
                    
                }
                
            }
        }
    }

    private void FinalOcclusionCheck()
    {
        
    }
    
    private void SetOcclusionInstances()
    {
        //Säg till alla eventlists att kolla i listan med occlusion data och uppdatera
    }
}
