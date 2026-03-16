using System.Collections.Generic;
using UnityEngine;

public class OcclusionHandler : MonoBehaviour
{
    [Range(1, 3)]
    [SerializeField] private int tickLength;
    [Range(1, 8)]
    [SerializeField] private int maxTicks;
    
    [Header("Occlusion Settings")]
    [SerializeField] public LayerMask layerMask;
    [Range(0, 60)] public float spread = 4;
    [Range(0, 1)]  public float bounceValue = 0.25f;
    [SerializeField] private int linesOnEitherSide;
    [SerializeField] private int objectsToCheck;
    
    
    [Header("Wallcheck Settings")]
    [SerializeField] public LayerMask wallLayers;
    [SerializeField] public LayerMask groundLayers;
    [SerializeField] public float wallMinHitDistance;
    private const int MaxWalls = 3;
    
    private const float Offset = 0.02f;
    
    public bool drawDebug;
    public bool debugMsg;
    private readonly Color _lGrey = new Color(0.66f, 0.66f, 0.66f);
    private readonly Color _dGrey = new Color(0.33f, 0.33f, 0.33f);
    private float _debugLineDuration;
    
    private int _lineCount;
    private int _posModifier;
    private Vector3 _direction;
    private float _distance;
    private float _totalDistance;
    private Vector3 _sourcePos;
    private Vector3 _targetPos;

    private bool _newTick;
    
    private static GameObject Listener => AudioManager.Listener;

    public class LineData
    {
        public RaycastHit[] Hits;
        public Vector3[] Direction;
        public bool[] DidHit;
        public float LineScore;
        public bool LineDone;
        public Vector3 FirstPosition;
    }
    

    public class OcclusionData
    {
        public float OcclusionScore = 1;
        public int WallScore = 3;
        public float Distance;
        public Vector3 WallDirection;
        public RaycastHit[] WallHits;
        public int WallHitCount;
        public LineData[] OcclusionLines;
        public Vector3 SourcePos;
        
        public static OcclusionData CreateInstance()
        {
            return new OcclusionData();
        }
    }

    private OcclusionData _tempData;

    private static OcclusionData _data;

    private GameObject[] _objectsToSend;
    
    [SerializeField] private int currentTick;
    private int _currentTickTime;

    private static Dictionary<GameObject, OcclusionData> OcclusionObjects = new Dictionary<GameObject, OcclusionData>();
    private Dictionary<GameObject, float> _distances = new Dictionary<GameObject, float>();

    private struct ObjectAndDistance
    {
        public GameObject Obj;
        public float Distance;
    } 
    
    private List<ObjectAndDistance> _objectsAndDistances = new List<ObjectAndDistance>();

    private float _tempDistance;
    
    //Kallas av ljudskript vid Start
    public static void AddToOcclusionList(GameObject gameObject)
    {
        OcclusionObjects.TryAdd(gameObject, OcclusionData.CreateInstance());
    }

    //Kallas av ljudskript vid OnDestroy
    public static void RemoveFromOcclusionList(GameObject gameObject)
    {
        OcclusionObjects.Remove(gameObject);
    }
    
    private void FixedUpdate()
    {
        //Vid första tick kolla avstånd med alla objekt och sortera
        if (drawDebug)
        {
            _debugLineDuration = Time.fixedDeltaTime * tickLength * maxTicks;
        }
        if (_newTick)
        {
            _newTick = false;
            if (currentTick == 0)
            {
                CreateDistanceList();
                SortDistances();
                InitializeHitDatas();
                DoWallCheck();
                InitialOcclusionStep();
            }
        
            //Gör raycasts till sista tick
            if (currentTick > 0 && currentTick < maxTicks)
            {
                DoOcclusionStep();
            }

            //Sista tick gör sista checks och sätt occlusion
            if (currentTick >= maxTicks - 1)
            {
                CalculateOcclusions();
                SetOcclusionInstances();
            }
        }
        
        _currentTickTime++;
        if (_currentTickTime < tickLength) return;
        _currentTickTime = 0;
        currentTick++;
        _newTick = true;
        if (currentTick >= maxTicks)
        {
            currentTick = 0;
        }
    }
    
    private void CreateDistanceList()
    {
        _objectsAndDistances.Clear();
        foreach (var kvp in OcclusionObjects)
        {
            _objectsAndDistances.Add(new ObjectAndDistance { Obj = kvp.Key, Distance = Vector3.Distance(kvp.Key.transform.position, AudioManager.Listener.transform.position) });
            if (debugMsg) Debug.Log("Adding " + kvp.Key.name + " to Distance List");
        }
    }

    //Sortera listan 
    private void SortDistances()
    {
        if (debugMsg) Debug.Log("Sorting " + _objectsAndDistances.Count + " distances");
        for (int i = 0; i < _objectsAndDistances.Count - 1; i++)
        {
            ObjectAndDistance temp = _objectsAndDistances[i];
            int min = i;
            for (int j = i + 1; j < _objectsAndDistances.Count; j++)
            {
                if (_objectsAndDistances[j].Distance < temp.Distance)
                {
                    temp = _objectsAndDistances[j];
                    min = j;
                }
            }
            _objectsAndDistances[min] = _objectsAndDistances[i];
            _objectsAndDistances[i] = temp;
        }
    }
    
    private void InitializeHitDatas()
    {
        _lineCount = linesOnEitherSide * 2 + 1;
        for (int i = 0; i < Mathf.Clamp(objectsToCheck, 0 ,_objectsAndDistances.Count); i++)
        {
            if (OcclusionObjects.TryGetValue(_objectsAndDistances[i].Obj, out _tempData))
            {
                _tempData.SourcePos = _objectsAndDistances[i].Obj.transform.position;
                _tempData.OcclusionLines = new LineData[_lineCount];
                _tempData.WallHits = new RaycastHit[MaxWalls * 2];
                for (int j = 0; j < _lineCount; j++)
                {
                    _tempData.OcclusionLines[j] = new LineData()
                    {
                        Hits = new RaycastHit[1 + maxTicks],
                        Direction = new Vector3[1 + maxTicks],
                        DidHit = new bool[1 + maxTicks]
                    };
                }
            }
        }
        if (debugMsg) Debug.Log("Initializing " + objectsToCheck + " hitDatas");
    }

    private void DoWallCheck()
    {
        //Loopa igenom alla objekt som ska kollas
        for (int i = 0; i < Mathf.Clamp(objectsToCheck, 0 ,_objectsAndDistances.Count); i++)
        {
            //Hämta occlusionData
            if (OcclusionObjects.TryGetValue(_objectsAndDistances[i].Obj, out _tempData))
            {
                //Ställ in riktning och gör först en linecast för att se om vi träffar mark
                _tempData.WallDirection = (Listener.transform.position - _objectsAndDistances[i].Obj.transform.position).normalized;
                if (Physics.Linecast(_tempData.SourcePos, Listener.transform.position, out _tempData.WallHits[0], groundLayers))
                {
                    //Om vi träffar mark gör en raycast som studsar från hitpunkten och sen gör en till linecast
                    if (Physics.Raycast(_tempData.WallHits[0].point + _tempData.WallHits[0].normal * Offset,
                            Vector3.Reflect(_tempData.WallDirection, _tempData.WallHits[0].normal),
                            out _tempData.WallHits[1], groundLayers + wallLayers))
                    {
                        if (!Physics.Linecast(_tempData.WallHits[1].point + _tempData.WallHits[1].normal * Offset,
                                Listener.transform.position, out _tempData.WallHits[2], groundLayers + wallLayers))
                        {
                            _tempData.WallScore = 0;
                            if (drawDebug)
                            {
                                Debug.DrawLine(_tempData.SourcePos, _tempData.WallHits[0].point, Color.white, _debugLineDuration);
                                Debug.DrawLine(_tempData.WallHits[0].point, _tempData.WallHits[1].point, Color.white, _debugLineDuration);
                                Debug.DrawLine(_tempData.WallHits[1].point, _tempData.WallHits[2].point, Color.white, _debugLineDuration);
                            }
                        }
                        else
                        {
                            _tempData.WallScore = MaxWalls;
                            if (drawDebug)
                            {
                                Debug.DrawLine(_tempData.SourcePos, _tempData.WallHits[0].point, Color.black, _debugLineDuration);
                                Debug.DrawLine(_tempData.WallHits[0].point, _tempData.WallHits[1].point, Color.black, _debugLineDuration);
                                Debug.DrawLine(_tempData.WallHits[1].point, _tempData.WallHits[2].point, Color.black, _debugLineDuration);
                            }
                        }
                    }
                    else
                    {
                        _tempData.WallScore = MaxWalls;
                        if (drawDebug) Debug.DrawLine(_tempData.SourcePos, _tempData.WallHits[0].point, Color.black, _debugLineDuration);
                    }
                }
                else
                {
                    //Om vi inte träffar mark gör vi en RayCastNonAlloc för att se hur många väggar finns mellan ljud och spelare
                    _tempData.WallHitCount = Physics.RaycastNonAlloc(_objectsAndDistances[i].Obj.transform.position, _tempData.WallDirection, _tempData.WallHits, Vector3.Distance(_objectsAndDistances[i].Obj.transform.position, Listener.transform.position), wallLayers);
                    
                    _tempData.WallScore = Mathf.Clamp(_tempData.WallHitCount, 0, MaxWalls);
                    if (_tempData.WallHitCount > 1)
                    {
                        //Om vi träffar mer eller fler väggar än maxwalls loopar vi igenom avstånden mellan alla punkter
                        //Och räknar inte med de som är under min distance (så att vi inte påverkas av överlappande väggar)
                        for (int j = 1; j < _tempData.WallHitCount; j++)
                        {
                            if (Vector3.Distance(_tempData.WallHits[j - 1].point, _tempData.WallHits[j].point) <
                                wallMinHitDistance)
                            {
                                _tempData.WallScore--;
                            }
                        }
                    }
                    if (drawDebug)
                    {
                        switch (_tempData.WallScore)
                        {
                            case <1:
                                Debug.DrawLine(_tempData.SourcePos, Listener.transform.position, Color.white, _debugLineDuration);
                                break;
                            case 1:
                                Debug.DrawLine(_tempData.SourcePos, Listener.transform.position, _lGrey, _debugLineDuration);
                                break;
                            case 2:
                                Debug.DrawLine(_tempData.SourcePos, Listener.transform.position , _dGrey, _debugLineDuration);
                                break;
                            case >2:
                                Debug.DrawLine(_tempData.SourcePos, Listener.transform.position, Color.black, _debugLineDuration);
                                break;
                        }
                    }
                }
                _tempData.WallScore = Mathf.Clamp(_tempData.WallScore, 0, MaxWalls);
            }
        }
    }

    private void InitialOcclusionStep()
    {
        //Har clampat objectstocheck här ifall det finns färre objekt än de som ska checkas för att undvika indexoutofrange
        for (int i = 0; i < Mathf.Clamp(objectsToCheck, 0 ,_objectsAndDistances.Count); i++)
        {
            _posModifier = -linesOnEitherSide - 1;
            if (OcclusionObjects.TryGetValue(_objectsAndDistances[i].Obj, out _tempData))
            {
                _tempData.Distance = Vector3.Distance(_tempData.SourcePos, Listener.transform.position);
                //Gör raycast för alla linjer på ett objekt
                for (int j = 0; j < _lineCount; j++)
                {
                    if (_tempData.WallScore >= MaxWalls)
                    {
                        _tempData.OcclusionLines[j].LineDone = true;
                        continue;
                    }
                    
                    _posModifier++;
                    _tempData.OcclusionLines[j].Direction[currentTick] = (Quaternion.AngleAxis(spread * _posModifier, Vector3.up) * (Listener.transform.position - _tempData.SourcePos).normalized).normalized;
                    _tempData.OcclusionLines[j].FirstPosition = Listener.transform.position + Listener.transform.right * (_posModifier * spread);
                    if (Physics.Linecast(_tempData.SourcePos, _tempData.OcclusionLines[j].FirstPosition,
                            out _tempData.OcclusionLines[j].Hits[currentTick], layerMask))
                    {
                        _tempData.OcclusionLines[j].DidHit[currentTick] = true;
                        if (drawDebug)
                        {
                            Debug.DrawLine(_tempData.SourcePos, _tempData.OcclusionLines[j].Hits[currentTick].point, Color.cyan, _debugLineDuration);
                        }
                    }
                    else
                    {
                        _tempData.OcclusionLines[j].DidHit[currentTick] = false;
                        if (drawDebug)
                        {
                            Debug.DrawLine(_tempData.SourcePos, Listener.transform.position + Listener.transform.right * (_posModifier * spread), Color.cyan, _debugLineDuration);
                        }
                    }
                }
            }
        }
    }
    
    private void DoOcclusionStep()
    {
        for (int i = 0; i < Mathf.Clamp(objectsToCheck, 0 ,_objectsAndDistances.Count); i++)
        {
            if (OcclusionObjects.TryGetValue(_objectsAndDistances[i].Obj, out _tempData))
            {
                for (int j = 0; j < _lineCount; j++)
                {
                    if (_tempData.OcclusionLines[j].LineDone) continue; //Om en linje är klar gör inget mer med den
                    if (_tempData.OcclusionLines[j].DidHit[currentTick - 1])
                    {
                        _tempData.OcclusionLines[j].LineScore += bounceValue;
                        if (_tempData.OcclusionLines[j].LineScore >= 1)
                        {
                            _tempData.OcclusionLines[j].LineScore = 1;
                            _tempData.OcclusionLines[j].LineDone = true;
                            continue;
                        }

                        if (!Physics.Linecast(
                                _tempData.OcclusionLines[j].Hits[currentTick - 1].point +
                                _tempData.OcclusionLines[j].Hits[currentTick - 1].normal * Offset,
                                Listener.transform.position, out _tempData.OcclusionLines[j].Hits[currentTick],
                                layerMask))
                        {
                            _tempData.OcclusionLines[j].DidHit[currentTick] = false;
                            _tempData.OcclusionLines[j].LineDone = true;
                            if (drawDebug) Debug.DrawLine(_tempData.OcclusionLines[j].Hits[currentTick - 1].point + 
                                                          _tempData.OcclusionLines[j].Hits[currentTick - 1].normal * Offset,
                                Listener.transform.position, Color.green, _debugLineDuration);
                            continue;
                        }
                        
                        _tempData.OcclusionLines[j].Direction[currentTick] = Vector3.Reflect(_tempData.OcclusionLines[j].Direction[currentTick-1], _tempData.OcclusionLines[j].Hits[currentTick - 1].normal);
                        if (Physics.Raycast(_tempData.OcclusionLines[j].Hits[currentTick - 1].point,
                                _tempData.OcclusionLines[j].Direction[currentTick],
                                out _tempData.OcclusionLines[j].Hits[currentTick],_tempData.Distance * 2, layerMask))
                        {
                            _tempData.OcclusionLines[j].DidHit[currentTick] = true;
                            if (drawDebug)
                            {
                                Debug.DrawLine(_tempData.OcclusionLines[j].Hits[currentTick - 1].point, _tempData.OcclusionLines[j].Hits[currentTick].point, Color.cyan, _debugLineDuration);
                            }
                        }
                        else
                        {
                            //Om studs inte träffar något (troligen oob) så är linjen klar med max score
                            _tempData.OcclusionLines[j].DidHit[currentTick] = false;
                            _tempData.OcclusionLines[j].LineScore = 1;
                            _tempData.OcclusionLines[j].LineDone = true;
                            if (drawDebug)
                            {
                                Debug.DrawRay(_tempData.OcclusionLines[j].Hits[currentTick - 1].point, _tempData.OcclusionLines[j].Direction[currentTick] * 10, Color.black, _debugLineDuration);
                            }
                        }
                    }
                    else
                    {
                        //Om förra cast inte träffade något gör vi en till check
                        if (!Physics.Linecast(
                                _tempData.OcclusionLines[j].FirstPosition, Listener.transform.position, layerMask))
                        {
                            _tempData.OcclusionLines[j].LineScore += bounceValue;
                            _tempData.OcclusionLines[j].LineDone = true;
                            if (drawDebug) Debug.DrawLine(_tempData.OcclusionLines[j].FirstPosition, Listener.transform.position, Color.green, _debugLineDuration);
                        }
                        else
                        {
                            _tempData.OcclusionLines[j].LineScore = 1;
                            _tempData.OcclusionLines[j].LineDone = true;
                            if (drawDebug) Debug.DrawLine(_tempData.OcclusionLines[j].FirstPosition, Listener.transform.position, Color.red, _debugLineDuration);
                        }
                    }
                }
            }
        }
    }

    private void CalculateOcclusions()
    {
        for (int i = 0; i < Mathf.Clamp(objectsToCheck, 0, _objectsAndDistances.Count); i++)
        {
            if (OcclusionObjects.TryGetValue(_objectsAndDistances[i].Obj, out _tempData))
            {
                _tempData.OcclusionScore = 0;
                for (int j = 0; j < _lineCount; j++)
                {
                    _tempData.OcclusionScore += _tempData.OcclusionLines[j].LineScore / _lineCount;
                }
                _tempData.OcclusionScore = Mathf.Clamp01(_tempData.OcclusionScore);
            }
        }
    }

    private void SetOcclusionInstances()
    {
        _objectsToSend = new GameObject[Mathf.Clamp(objectsToCheck, 0 ,_objectsAndDistances.Count)];
        for (int i = 0; i < Mathf.Clamp(objectsToCheck, 0, _objectsAndDistances.Count); i++)
        {
            _objectsToSend[i] = _objectsAndDistances[i].Obj;
        }
        AudioManager.Instance.SetOcclusions(_objectsToSend);
    }

    public static bool TryGetOcclusionData(GameObject gameObject ,out float Occlusion, out float Walls)
    {
        if (OcclusionObjects.TryGetValue(gameObject, out _data))
        {
            Occlusion = _data.OcclusionScore;
            Walls = _data.WallScore;
            return true;
        }

        Occlusion = 0;
        Walls = 0;
        return false;
    }
}
