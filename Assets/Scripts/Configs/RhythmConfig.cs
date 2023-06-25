using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "RhythmsConfig", menuName = "Configs/Create RhythmsConfig")]
    public class RhythmConfig : ScriptableObject
    {
        public List<RhythmsModel> Rhythms;
    }
}