using System.Collections;
using Configs;
using Models;
using TMPro;
using UnityEngine;
using Views;

namespace Windows
{
    public class MainWindow : MonoBehaviour
    {
        [SerializeField] private UpPanelView _upPanel = default;
        [SerializeField] private EnemyPanelView _enemyPanel = default;
        [SerializeField] private BottomPanelView _bottomPanel = default;

        private EnemyConfig _enemyConfig;
        private Enemy _currentEnemy;
        private GameObject _damageElement;

        private void Awake()
        {
            _enemyConfig = Resources.Load<EnemyConfig>("Configs/EnemiesConfig");
            _damageElement = Resources.Load<GameObject>("Prefabs/UI/DamageElement");

            var enemyModel = CreateEnemy(0);

            _enemyPanel.OnClicked += OnClickHandler;
            _bottomPanel.OnExitButtonClicked += OnExitButtonClickHandler;

            _upPanel.Init(_currentEnemy);
            _enemyPanel.Init(enemyModel.PathToResource);
            _bottomPanel.Init(enemyModel.Name);
        }

        private void OnExitButtonClickHandler()
        {
            _upPanel.ExitToMainMenu();
        }

        private void OnClickHandler()
        {
            _upPanel.OnClickHandler();
        }

        private void OnDeath()
        {
            _currentEnemy.OnDeath -= OnDeath;
            _currentEnemy.OnDamage -= OnDamage;

            var enemyIndex = _enemyConfig.Enemies.FindIndex(x => x.Name == _currentEnemy.Name);
            enemyIndex = enemyIndex == _enemyConfig.Enemies.Count - 1 ? 0 : enemyIndex + 1;

            var enemyModel = CreateEnemy(enemyIndex);

            _upPanel.OnEnemyDeath(_currentEnemy);
            _enemyPanel.OnEnemyDeath(enemyModel.PathToResource);
            _bottomPanel.OnEnemyDeath(enemyModel.Name);
        }

        private void OnDamage(float damage)
        {
            //Vector3 pos = Input.mousePosition;
            //Debug.LogError($"{pos}");
            //var go = GameObject.Instantiate(_damageElement, _enemyPanel.transform);
            //go.GetComponent<TextMeshProUGUI>().text = damage.ToString();
            //GameObject.Destroy(go,2f);

            _enemyPanel.OnEnemyDamage();
        }

        private EnemyModel CreateEnemy(int enemyIndex)
        {
            var enemyModel = _enemyConfig.Enemies[enemyIndex];
            _currentEnemy = new Enemy(enemyModel.Hp, enemyModel.Name);
            _currentEnemy.OnDeath += OnDeath;
            _currentEnemy.OnDamage += OnDamage;

            return enemyModel;
        }
    }
}
