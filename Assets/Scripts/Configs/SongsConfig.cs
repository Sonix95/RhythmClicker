using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "SongsConfig", menuName = "Configs/Create SongsConfig")]
    public class SongsConfig : ScriptableObject
    {
        public List<SongsModel> Songs;
    }
}