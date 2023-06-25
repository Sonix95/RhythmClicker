using System;
using System.Collections;
using Components;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class EnemyPanelView : MonoBehaviour
    {
        public Action OnClicked;

        [SerializeField] private RectTransform _parent = default;
        [SerializeField] private Button _clickArea = default;

        private EnemyViewComponent _currentEnemyView;
        public EnemyViewComponent CurrentEnemyView => _currentEnemyView;

        public void Init(string enemyPrefabPath)
        {
            CreateEnemy(enemyPrefabPath);
            Subscribe();
        }

        private void CreateEnemy(string enemyPrefabPath)
        {
            var prefab = Resources.Load<EnemyViewComponent>(enemyPrefabPath);
            _currentEnemyView = Instantiate(prefab, _parent);
        }

        private IEnumerator DestroyEnemy()
        {
            _currentEnemyView.OnDeath();

            yield return new WaitForSeconds(1f);
            _currentEnemyView.OnDeath();
            Destroy(_currentEnemyView.gameObject);
            _currentEnemyView = null;
        }

        private void Subscribe()
        {
            _clickArea.onClick.AddListener(OnClickHandler);
        }

        private void Unsubscribe()
        {
            _clickArea.onClick.RemoveListener(OnClickHandler);
        }

        public void OnEnemyDeath(string enemyPrefabPath)
        {
            Unsubscribe();
            //StartCoroutine(DestroyEnemy());
            Destroy(_currentEnemyView.gameObject);
            _currentEnemyView = null;

            CreateEnemy(enemyPrefabPath);
            Subscribe();
        }

        public void OnEnemyDamage()
        {
            _currentEnemyView.OnDamage();
        }

        private void OnClickHandler()
        {
            OnClicked?.Invoke();
        }
    }
}
