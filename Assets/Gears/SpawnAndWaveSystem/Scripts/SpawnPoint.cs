#define DEBUG
#undef DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using Gears;

public class SpawnPoint : MonoBehaviour
{
    #region OverallDefiner
    public bool controlled;
    public EnemyWaveManager manager;

    public enum Mode { Simple, Advanced }
    public Mode mode;

    public enum SpawnAreaType { Rectangle, Circle, Polygon }
    public SpawnAreaType spawnAreaShape;

    public enum SpawnPointType { Box, Sphere, Point }
    public SpawnPointType spawnPointType;
    #endregion

    public bool auto = false;

    #region AutoRespawn
    public bool autoRespawn = false;
    public float recoverTime = 60f;
    #endregion

    #region HordeLOD
    public bool hordeLod = false;
    public Transform player;
    public float activeDistance = 50f;
    public float hordeActiveRefreshTime = 0.5f;
    #endregion

    #region Basic
    public string hordeTag;
    public EnemyWaveManager.MachineState spawnState;
    public List<SpawnSequence> enemys;
    public float radius = 3f;
    public GameObject spawnEffect;
    public float separateDistance = 5f;
    public float spawnBuffer = 0.5f;
    public float etc = 0;
    #endregion

    #region Horde
    public float hordePower = 1f;
    #endregion

    private float timer;
    private float spawnTimer;

    private List<GameObject> spawnTracker = new List<GameObject>();
    private bool spawnFinish;
    private bool hordeActived;

    private bool startSpawn { get; set; }

    #region Editor
    [HideInInspector]
    public bool drawPoint = false;
    [HideInInspector]
    public Vector3[] selfPoints = new Vector3[0];
    [HideInInspector]
    public Vector3[] worldPoints = new Vector3[0];
    [HideInInspector]
    public Vector2[] closePoints = new Vector2[0];
    #endregion

    #region Event
    public UnityEvent onSequenceStart;
    public UnityEvent onSequenceFinish;
    #endregion

    void Start()
    {
        spawnState = EnemyWaveManager.MachineState.Prepare;
        if (auto)
            StartSequence();
    }

    public void StartSequence()
    {
        spawnState = EnemyWaveManager.MachineState.Start;
        startSpawn = true;
        StartCoroutine(ISequenceProcess());
        if (onSequenceStart != null)
            onSequenceStart.Invoke();
    }

    public void FinishSequence()
    {
        spawnState = EnemyWaveManager.MachineState.Finish;
        startSpawn = false;
        if (manager)
        {
            manager.OnSpawnPointFinish(this);
#if (DEBUG)
            Debug.Log("Call FinishSeq via " + name);
            Debug.LogWarning(this.name + " Finished Sequence with " + transform.childCount + " spawn count");
#endif

        }
        if (onSequenceFinish != null)
            onSequenceFinish.Invoke();
        spawnState = EnemyWaveManager.MachineState.Exhausted;
    }

    IEnumerator ISequenceProcess()
    {
        spawnState = EnemyWaveManager.MachineState.Executing;
        for (int y = 0; y < enemys.Count; y++)
        {
            for (int i = 0; i < enemys[y].number; i++)
            {
                SpawnEnemy(enemys[y].prefab);
                yield return new WaitForSeconds(spawnBuffer);
            }
        }
        FinishSequence();
    }

    void SpawnEnemy(GameObject prefab)
    {
        GameObject clone;
        Vector3 sourcePosition;
        Vector3 fixedPosition;

        switch (mode)
        {
            default:
                switch (spawnPointType)
                {
                    case SpawnPointType.Box:
                        //3D SPACE
                        //sourcePosition = new Vector3(Random.Range(-radius * .5f, radius * .5f), Random.Range(-radius * .5f, radius * .5f), Random.Range(-radius * .5f, radius * .5f));
                        //FLAT
                        sourcePosition = new Vector3(Random.Range(-radius * .5f, radius * .5f), 0, Random.Range(-radius * .5f, radius * .5f));
                        sourcePosition += transform.position;
                        break;
                    case SpawnPointType.Sphere:
                        //3D SPACE
                        sourcePosition = transform.position + (Random.insideUnitSphere * radius);
                        //FLAT
                        sourcePosition.y = 0;
                        break;
                    default:
                        sourcePosition = transform.position;
                        break;
                }
                fixedPosition = sourcePosition;
                break;
            case Mode.Advanced:
                int leak = 0;
                sourcePosition = CreateInsidePoint(worldPoints);
                while (Vector3.Distance(sourcePosition, lastSpawnPosition) < separateDistance)
                {
                    sourcePosition = CreateInsidePoint(worldPoints);
                    leak++;
                    if (Vector3.Distance(sourcePosition, lastSpawnPosition) > separateDistance || leak > 99)
                        break;
                }
                UnityEngine.AI.NavMeshHit closestHit;
                if (UnityEngine.AI.NavMesh.SamplePosition(sourcePosition, out closestHit, 500, 1 << UnityEngine.AI.NavMesh.GetAreaFromName("Walkable")))
                    fixedPosition = closestHit.position;
                else
                    fixedPosition = sourcePosition;
#if (DEBUG)
                Debug.DrawLine(sourcePosition, sourcePosition + (Vector3.up * 10f), Color.red, 3f);
#endif
                break;
        }

        clone = Instantiate(prefab, fixedPosition, Quaternion.identity) as GameObject;
        clone.transform.parent = this.transform;
        clone.name = prefab.name;
        //spawnTracker.Add(clone);

        if (spawnEffect)
        {
            GameObject effect = Instantiate(spawnEffect, fixedPosition, Quaternion.identity) as GameObject;
            effect.name = spawnEffect.name;
        }
    }

    public string GetHordeTag()
    {
        return hordeTag;
    }

    public void SetHordeActive(bool active)
    {
        if (transform.childCount > 0)
        {
            //if (transform.GetChild(0).gameObject.activeInHierarchy != active)
            //{
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(active);
            }
            //}
        }
    }

    void Update()
    {
        if (spawnState == EnemyWaveManager.MachineState.Exhausted)
        {
            if (transform.childCount == 0)
            {
                spawnState = EnemyWaveManager.MachineState.Clean;
            }
        }
    }

    public void SetHordePower(float level)
    {
        spawnTracker = new List<GameObject>();
        hordePower = level;
        //Debug.Log("Set Horde Power");
        SpawnHorde();
    }

    void SpawnHorde()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SendMessage("RemoveMiniHealthBar");
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        //for (int i = 0; i < number; i++)
        //{
        //    StartCoroutine(SpawnEnemy(spawnBuffer * (i + 1)));
        //}
        startSpawn = true;
    }

    public Vector3 lastSpawnPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    public Vector3 LastSpawnPosition { set { lastSpawnPosition = value; } }

    public bool RemoveFromTracker(GameObject target)
    {
        spawnTracker.Remove(target);
        //StartCoroutine(SpawnEnemy(recoverTime));
        //Debug.LogWarning("Remove " + target.name);
        if (autoRespawn)
            Invoke("SpawnEnemy", recoverTime);
        return true;
    }

    Vector3 CreateInsidePoint(Vector3[] vertices)
    {
        float maxX = float.MinValue; float maxY = float.MinValue;
        float minX = float.MaxValue; float minY = float.MaxValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].x > maxX)
                maxX = vertices[i].x;
            if (vertices[i].x < minX)
                minX = vertices[i].x;

            if (vertices[i].z > maxY)
            {
                maxY = vertices[i].z;

            }
            if (vertices[i].z < minY)
                minY = vertices[i].z;
        }

        Vector3 point;

        int count = 0;
        while (true)
        {
            point = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minY, maxY));
            count++;
            if (IsContainPoint(vertices, point, false) || count > 100)
                break;
        }
        return point;
    }

    bool IsContainPoint(Vector3[] polygonPoints, Vector3 target, bool in2DMode)
    {
        Vector2[] a = new Vector2[polygonPoints.Length];
        Vector2 t;
        if (in2DMode)
        {
            for (int i = 0; i < polygonPoints.Length; i++)
                a[i] = new Vector2(polygonPoints[i].x, polygonPoints[i].y);
            t = new Vector2(target.x, target.y);
        }
        else
        {
            for (int i = 0; i < polygonPoints.Length; i++)
                a[i] = new Vector2(polygonPoints[i].x, polygonPoints[i].z);
            t = new Vector2(target.x, target.z);
        }
        return IsContainPoint(a, t);
    }
    bool IsContainPoint(Vector2[] polygonPoints, Vector2 target)
    {
        int j = polygonPoints.Length - 1;
        bool inside = false;
        for (int i = 0; i < polygonPoints.Length; j = i++)
        {
            if (((polygonPoints[i].y <= target.y && target.y < polygonPoints[j].y) ||
                (polygonPoints[j].y <= target.y && target.y < polygonPoints[i].y)) &&
               (target.x < (polygonPoints[j].x - polygonPoints[i].x) * (target.y - polygonPoints[i].y) / (polygonPoints[j].y - polygonPoints[i].y) + polygonPoints[i].x))
                inside = !inside;
        }
        return inside;
    }

    void OnDrawGizmosSelected()
    {
        if (mode == Mode.Simple)
        {
            switch (spawnPointType)
            {
                //Box
                case SpawnPointType.Box:
                    Gizmos.color = new Color(0, 1, 0, .5f);
                    Gizmos.DrawCube(transform.position, Vector3.one * radius);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(transform.position, Vector3.one * radius);
                    break;
                //Sphere
                case SpawnPointType.Sphere:
                    Gizmos.color = new Color(0, 1, 0, .5f);
                    Gizmos.DrawSphere(transform.position, radius);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(transform.position, radius);
                    break;
                //Point
                case SpawnPointType.Point:
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(transform.position, .1f);
                    Gizmos.DrawRay(transform.position, transform.up * radius);
                    break;
            }
            Gizmos.color = Color.white;
        }
        else if (mode == Mode.Advanced)
        {
            Gizmos.color = Color.green;
            GearsUtility.DrawLines(worldPoints);
        }
    }
}
