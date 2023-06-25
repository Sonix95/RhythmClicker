using Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class UpPanelView : MonoBehaviour
    {
        [Header("Health")] 
        [SerializeField] private TextMeshProUGUI _healthLabel;
        [SerializeField] private Slider _healthBar;

        [Header("Rhythm")] 
        [SerializeField] private RhythmViewComponent _rhythmViewComponent = default;

        private Enemy _enemy;

        public void Init(Enemy enemy)
        {
            _enemy = enemy;
            _enemy.OnDamage += OnDamageHandler;
            _enemy.OnDeath += OnDeathHandler;
            _rhythmViewComponent.OnClicked += OnRhythmBarClicked;

            _rhythmViewComponent.Init();
            InitEnemy();
        }

        private void InitEnemy()
        {
            _healthBar.minValue = 0;
            _healthBar.maxValue = _enemy.MaxHp;

            UpdateEnemyInfo();
        }

        public void OnClickHandler()
        {
            _rhythmViewComponent.OnClickHandler();
        }

        private void OnRhythmBarClicked(float damage)
        {
            _enemy.TakeDamage(damage);
        }

        public void OnEnemyDeath(Enemy enemy)
        {
            _rhythmViewComponent.OnEnemyDeath();

            _enemy.OnDamage -= OnDamageHandler;
            _enemy.OnDeath -= OnDeathHandler;
            _enemy = null;

            _enemy = enemy;
            _enemy.OnDamage += OnDamageHandler;
            _enemy.OnDeath += OnDeathHandler;
            InitEnemy();
        }

        private void OnDamageHandler(float damage)
        {
            UpdateEnemyInfo();
        }

        private void OnDeathHandler()
        {
            UpdateEnemyInfo();
        }

        private void UpdateEnemyInfo()
        {
            _healthBar.value = _enemy.CurrentHp;
            _healthLabel.text = $"{_enemy.CurrentHp}/{_enemy.MaxHp}";
        }
    }
}
