using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "EnemiesConfig", menuName = "Configs/Create EnemiesConfig")]
    public class EnemyConfig : ScriptableObject
    {
        public List<EnemyModel> Enemies;
    }
}
