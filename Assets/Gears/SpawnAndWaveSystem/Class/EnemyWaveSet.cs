using UnityEngine;
using System.Collections;

namespace Gears
{
    [System.Serializable]
    public class EnemyWaveSet
    {
        public string name;
        public EnemyWave wave;
        public int chance;

        public EnemyWaveSet()
        {
            this.name = "New Set";
        }

        public EnemyWaveSet(EnemyWaveSet clone)
        {
            this.name = clone.name;
            this.wave = new EnemyWave(clone.wave);
            this.chance = clone.chance;
        }

        public void Override(EnemyWaveSet clone)
        {
            this.name = clone.name;
            this.wave = new EnemyWave(clone.wave);
            this.chance = clone.chance;
        }
    }
}