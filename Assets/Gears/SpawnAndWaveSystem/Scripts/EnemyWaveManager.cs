#define TIER1DEBUG
#undef TIER1DEBUG
#define TIER2DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using Gears;

namespace Gears
{
    /*=====================================================
            EnemyWaveManager ver 3.0 build 03182016
    =====================================================*/
    public class EnemyWaveManager : MonoBehaviour
    {
        [System.Serializable]
        public class OnWaveProceedInt : UnityEvent<int> { }

        [System.Serializable]
        public class OnWaveProceedString : UnityEvent<string> { }

        public enum SeparatedPattern { Equal, Random }

        public enum MachineState { Waiting, Prepare, Start, Executing, Finish, Exhausted, Clean }

        //State
        [SerializeField]
        private int currentWave = 0;
        public int currentWaveIndex { get { return currentWave; } }
        [SerializeField]
        private MachineState waveState = MachineState.Waiting;
        public MachineState currentWaveState { get { return waveState; } }
        //Gizmo
        [SerializeField]
        private bool debugToggle;
        [SerializeField]
        private Color gizmosColor = new Color(0f, 1f, 0.81f, 1f);
        [SerializeField]
        private float gizmosSize = 3f;
        //Setting
        [SerializeField]
        private bool settingToggle;
        [SerializeField]
        private int reportedPoints;
        public bool autoStart = false;
        public bool autoProceed = true;
        public int waveDelay = 5;
        public SeparatedPattern separatedPattern = SeparatedPattern.Equal;
        public List<EnemyWaveSequence> waveSequence = new List<EnemyWaveSequence>();
        //Editor
        public SpawnPoint[] childSpawnPoints;
        //Event
        //-Call each wave
        [SerializeField]
        private bool waveEventToggle;
        public UnityEvent onWaveStart;
        public UnityEvent onWaveFinish;
        public UnityEvent onWaveClear;
        //-Call per second
        [SerializeField]
        private bool waveProcessingEventToggle;
        public OnWaveProceedInt onWaveProceedInt;
        public OnWaveProceedString onWaveProceedString;
        //-Call when clear
        [SerializeField]
        private bool sequenceEventToggle;
        public UnityEvent onAllWaveClear;

        void Awake()
        {
            if (autoStart)
                waveState = MachineState.Prepare;
            else
                waveState = MachineState.Waiting;
        }

        void Start()
        {

        }

        void Update()
        {
            //if (waveState != WaveState.Exhausted)
            //{
            //    time += Time.deltaTime;
            //}
            int _start = currentWave;
            int _cleaned = 0;
            switch (waveState)
            {
                case MachineState.Waiting:
                    break;
                case MachineState.Prepare:
                    int _picked = PickEnemyWaveSet(waveSequence[_start].waveSet);
                    if (ApplySettingToSpawnPoints(waveSequence[_start].selectedSpawnPoints, waveSequence[_start].waveSet[_picked].wave))
                        waveState = MachineState.Start;
                    break;
                case MachineState.Start:
#if (TIER1DEBUG)
                Debug.Log("Start " + waveSequence[_start].name + " Wave");
#endif
                    if (ApplyWave(waveSequence[_start].selectedSpawnPoints))
                    {
#if (TIER2DEBUG)
                        Debug.Log("On Wave Start");
#endif
                        if (onWaveStart != null)
                            onWaveStart.Invoke();
                        waveState = MachineState.Executing;
                    }
                    break;
                case MachineState.Executing:
                    if (reportedPoints == waveSequence[_start].selectedSpawnPoints.Length)
                    {
#if (TIER1DEBUG)
                    Debug.Log("Executing");
#endif
#if (TIER2DEBUG)
                        Debug.Log("On Wave Finish");
#endif
                        if (onWaveFinish != null)
                            onWaveFinish.Invoke();
                        waveState = MachineState.Finish;
                    }
                    break;
                case MachineState.Finish:
                    reportedPoints = 0;
                    if (currentWave == waveSequence.Count - 1)
                    {
#if (TIER1DEBUG)
                    Debug.Log("Wave Exhausted, aka This's Final Wave");
#endif
                        waveState = MachineState.Exhausted;
                    }
                    else
                    {
#if (TIER1DEBUG)
                    Debug.Log("Ready for Next Wave");
#endif
                        if (autoProceed)
                        {
                            _cleaned = 0;
                            foreach (SpawnPoint s in waveSequence[_start].selectedSpawnPoints)
                            {
                                if (s.spawnState == MachineState.Clean)
                                    _cleaned++;
                            }
                            if (_cleaned == waveSequence[_start].selectedSpawnPoints.Length)
                            {
#if (TIER2DEBUG)
                                Debug.Log("On Wave Clear");
#endif
                                if (onWaveClear != null)
                                    onWaveClear.Invoke();
                                TimeNextWave(waveDelay);
                                waveState = MachineState.Waiting;
                            }
                        }
                        else
                        {
                            waveState = MachineState.Waiting;
                        }
                        //if (auto)
                        //{
                        //    currentWave++;
                        //    waveState = MachineState.Prepare;
                        //}
                        //else
                        //{

                        //}
                    }
                    break;
                case MachineState.Exhausted:
                    _cleaned = 0;
                    foreach (SpawnPoint s in waveSequence[_start].selectedSpawnPoints)
                    {
                        if (s.spawnState == MachineState.Clean)
                            _cleaned++;
                    }
                    if (_cleaned == waveSequence[_start].selectedSpawnPoints.Length)
                    {
#if (TIER2DEBUG)
                        Debug.Log("On All Wave Clear");
#endif
                        if (onAllWaveClear != null)
                            onAllWaveClear.Invoke();
                        waveState = MachineState.Clean;
                    }
                    break;
                case MachineState.Clean:
                    break;
            }
        }

        /// <summary>
        /// Wait amount of time for next wave.
        /// <para/>Invoke OnWaveProceed event per second, so it can use for countdown effect.
        /// </summary>
        /// <returns></returns>
        public void TimeNextWave(int time)
        {
            StartCoroutine(ProcessingNextWave(time));
        }
        IEnumerator ProcessingNextWave(int time)
        {
            for (int i = 0; i < time; i++)
            {
                if (onWaveProceedInt != null)
                    onWaveProceedInt.Invoke(time - i);
                if (onWaveProceedString != null)
                    onWaveProceedString.Invoke((time - i).ToString());
#if (TIER2DEBUG)
                Debug.Log("Prepare for next wave:" + (time - i));
#endif
                yield return new WaitForSeconds(1);
            }
            if (onWaveProceedInt != null)
                onWaveProceedInt.Invoke(0);
            if (onWaveProceedString != null)
                onWaveProceedString.Invoke("0");
#if (TIER2DEBUG)
            Debug.Log("Prepare for next wave:" + 0);
#endif
            NowNextWave();
        }

        /// <summary>
        /// Start current wave.
        /// <para/>If currentWave number not increase before call, it'll repeatedly start same Wave. 
        /// </summary>
        public void StartWave()
        {
            waveState = MachineState.Prepare;
        }

        /// <summary>
        /// Proceed to next wave then start immediately.
        /// </summary>
        public void NowNextWave()
        {
            if (currentWave == waveSequence.Count - 1)
            {
                Debug.LogError("No more wave to spawn");
                return;
            }
            currentWave++;
            StartWave();
        }

        /// <summary>
        /// Set current to value then start.
        /// </summary>
        /// <param name="index"></param>
        public void GoToAndStartWave(int index)
        {
            currentWave = index;
            StartWave();
        }

        /// <summary>
        /// Pick EnemyWave base on chance. If only got one set, it'll pick it anyway.
        /// </summary>
        /// <param name="sets"></param>
        /// <returns></returns>
        private int PickEnemyWaveSet(List<EnemyWaveSet> sets)
        {
            if (sets.Count == 1)
                return 0;
            var e = sets;
            int times = 0;
            int choose = 0;
            int total = 0;
            foreach (EnemyWaveSet s in e)
                total += s.chance;
            bool pass = false;
            while (true)
            {
                times++;
                for (int i = 0; i < e.Count; i++)
                {
                    int dice = Random.Range(0, total);
                    if (dice < e[i].chance)
                    {
                        choose = i;
                        pass = true;
                        break;
                    }
                    else
                        total -= e[i].chance;
                }
                if (pass || times > 99)
                {
#if (TIER1DEBUG)
                Debug.Log("Choose " + choose + " Tryed " + times + " times");
#endif
                    break;
                }
            }
            return choose;
        }

        /// <summary>
        /// Separated enemy number feed by random or equal number.
        /// <para/>e.g.: total = 10, part = 5. Equal => 2, 2, 2, 2, 2. Random => 3, 1, 4, 1, 1.
        /// </summary>
        /// <param name="total"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        private int[] GetSeparatedFeed(int total, int part)
        {
            int[] feed = new int[part];
            float f_total = (float)total;
            float f_part = (float)part;
            int num = 0;

            if (separatedPattern == SeparatedPattern.Equal)
                num = Mathf.RoundToInt((float)f_total / (float)f_part);

            for (int i = 0; i < feed.Length; i++)
            {
                if (separatedPattern == SeparatedPattern.Random)
                    num = Random.Range(0, total);
                if (i == feed.Length - 1)
                {
                    feed[i] = Mathf.RoundToInt(f_total);
                }
                else
                {
                    feed[i] = num;
                    f_total -= num;
                }
            }
            var rnd = new System.Random();
            var s_feed = feed.ToList().OrderBy(a => rnd.Next());
            return s_feed.ToArray();
        }

        /// <summary>
        /// Get SpawnPoint start working.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private bool ApplyWave(SpawnPoint[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].StartSequence();
            }
            return true;
        }

        /// <summary>
        /// Apply chosen EnemyWave setting to SpawnPoints.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        private bool ApplySettingToSpawnPoints(SpawnPoint[] points, EnemyWave set)
        {
            var e = points;

            //Refresh
            foreach (SpawnPoint p in e)
            {
                p.enemys = new List<SpawnSequence>();
                p.hordeTag = set.tag;
                p.spawnBuffer = set.delay;
                p.spawnEffect = set.effect;
            }

            //Push data point by point a round, then next data again
            //X = points[i] : Y = points[i].enemys.Add()
            //#0 : X1Y1 => X2Y1 => X3Y1
            //#1 : X1Y2 => X2Y2 => X3Y2
            for (int y = 0; y < set.enemys.Count; y++)
            {
                int total = set.enemys[y].number;
                int[] feed = GetSeparatedFeed(total, points.Length);

                for (int x = 0; x < points.Length; x++)
                {
                    e[x].enemys.Add(new SpawnSequence(set.enemys[y].prefab, feed[x]));
                }
            }
            return true;
        }

        /// <summary>
        /// SpawnPoint will call this method when they 'Finish" spawning.
        /// <para/>warning: 'Finish' doesn't mean 'Clear'
        /// </summary>
        /// <param name="point"></param>
        public void OnSpawnPointFinish(SpawnPoint point)
        {
#if (TIER1DEBUG)
        Debug.Log("Get FinishSeq via " + point.name);
#endif
            reportedPoints++;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, gizmosSize);
            Gizmos.color = Color.white;
        }
    }
}