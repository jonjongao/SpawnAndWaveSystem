using UnityEngine;

namespace Gears
{
    [System.Serializable]
    public class SpawnSequence
    {
        public GameObject prefab;
        public int number;

        public SpawnSequence()
        {
            this.prefab = null;
            this.number = 0;
        }

        public SpawnSequence(SpawnSequence clone)
        {
            this.prefab = clone.prefab;
            this.number = clone.number;
        }

        public SpawnSequence(GameObject prefab, int number)
        {
            this.prefab = prefab;
            this.number = number;
        }
    }
}
