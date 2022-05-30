using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchGame.Managers
{
    public class Poolable : MonoBehaviour
    {
        [SerializeField] private int objectCount = 10;
        public float ObjectCount { get => objectCount; }
    }

}
