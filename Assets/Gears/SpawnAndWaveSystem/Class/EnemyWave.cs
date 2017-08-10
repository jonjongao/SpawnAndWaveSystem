using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Gears
{
    [System.Serializable]
    public class EnemyWave
    {
        public string tag;
        public List<SpawnSequence> enemys;
        public float delay;
        public GameObject effect;

        public EnemyWave()
        {
            this.tag = string.Empty;
            this.enemys = new List<SpawnSequence>();
            this.enemys.Add(new SpawnSequence());
            this.delay = 0;
            this.effect = null;
        }

        public EnemyWave(EnemyWave clone)
        {
            this.tag = clone.tag;
            SpawnSequence[] array = new SpawnSequence[clone.enemys.Count];
            for (int i = 0; i < clone.enemys.Count; i++)
                array[i] = new SpawnSequence(clone.enemys[i]);
            this.enemys = array.ToList();
            this.delay = clone.delay;
            this.effect = clone.effect;
        }
    }
}