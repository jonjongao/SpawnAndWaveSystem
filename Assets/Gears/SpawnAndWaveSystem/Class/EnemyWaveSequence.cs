using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gears
{
    [System.Serializable]
    public class EnemyWaveSequence
    {
        public string name;
        public SpawnPoint[] selectedSpawnPoints;
        public List<EnemyWaveSet> waveSet;

        public EnemyWaveSequence()
        {
            this.name = "New Wave";
            this.selectedSpawnPoints = new SpawnPoint[0];
            this.waveSet = new List<EnemyWaveSet>();
            this.waveSet.Add(new EnemyWaveSet());
        }

        public EnemyWaveSequence(string name)
        {
            this.name = name;
            this.selectedSpawnPoints = new SpawnPoint[0];
            this.waveSet = new List<EnemyWaveSet>();
            this.waveSet.Add(new EnemyWaveSet());
        }

        public EnemyWaveSequence(EnemyWaveSequence clone)
        {
            this.name = clone.name;
            this.selectedSpawnPoints = clone.selectedSpawnPoints;
            this.waveSet = new List<EnemyWaveSet>();
            for (int i = 0; i < clone.waveSet.Count; i++)
            {
                this.waveSet.Add(new EnemyWaveSet(clone.waveSet[i]));
            }
        }
    }
}