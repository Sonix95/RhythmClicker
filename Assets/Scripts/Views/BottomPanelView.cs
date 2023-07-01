using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class BottomPanelView : MonoBehaviour
    {
        public Action OnExitButtonClicked;

        [SerializeField] private Button _exitButon;
        [SerializeField] private TextMeshProUGUI _enemyNameLabel;

        public void Init(string enemyName)
        {
            UpdateName(enemyName);
            _exitButon.onClick.AddListener(ClickExitButtonHandler);
        }

        private void ClickExitButtonHandler()
        {
            OnExitButtonClicked?.Invoke();
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
