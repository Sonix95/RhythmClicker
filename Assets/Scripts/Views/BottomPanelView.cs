using TMPro;
using UnityEngine;

namespace Views
{
    public class BottomPanelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _enemyNameLabel;

        public void Init(string enemyName)
        {
            UpdateName(enemyName);
        }

        public void OnEnemyDeath(string enemyName)
        {
            UpdateName(enemyName);
        }

        private void UpdateName(string enemyName)
        {
            _enemyNameLabel.text = enemyName;
        }
    }
}
