using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "RhythmElementsConfig", menuName = "Configs/Create RhythmElementsConfig")]
    public class RhythmElementsConfig : ScriptableObject
    {
        public List<RhythmsModel> Rhythms;
    }
}