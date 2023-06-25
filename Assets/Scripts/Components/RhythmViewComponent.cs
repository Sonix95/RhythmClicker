using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Models;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class RhythmViewComponent : MonoBehaviour
    {
        public Action<float> OnClicked;

        [Header("Rhythm")]
        [SerializeField] private float _areasSpacing = default;
        [SerializeField] private string _rhythmAreasPrefabPath = default;
        [SerializeField] private string _rhythmSliderPrefabPath = default;
        [SerializeField] private RectTransform _sliderRect = default;
        [SerializeField] private RectTransform _rhythmAreasRoot = default;
        [SerializeField] private RectTransform _sliderRoot = default;
        [SerializeField] private List<float> _spawnTime = default;
        [SerializeField] private float _sliderSpeed = default;
        [SerializeField] private float _delay = default;

        private RhythmConfig _rhythmsConfig = default;
        private RhythmSliderComponent _rhythmSliderComponentPrefab = default;
        private Dictionary<(float, float), float> _multipliers = default;
        private Queue<RhythmSliderComponent> _spawnedSliders;
        
        private bool _blocking = true;
        
        private float _currentLevelTime = 0;
        private int _currentSpawnIndex = 0;
        
        public void Init()
        {
            _multipliers = new Dictionary<(float, float), float>();
            _spawnedSliders = new Queue<RhythmSliderComponent>();
            _rhythmsConfig = Resources.Load<RhythmConfig>("Configs/RhythmsConfig");
            _rhythmSliderComponentPrefab = Resources.Load<RhythmSliderComponent>(_rhythmSliderPrefabPath);

            CreateRhythmAreas();
            StartCoroutine(StartDelayer());
        }

        public void OnClickHandler()
        {
            if (_spawnedSliders.Count > 0)
            {
                foreach (var key in _multipliers.Keys)
                {
                    if (_spawnedSliders.Peek().SliderValue >= key.Item1 && _spawnedSliders.Peek().SliderValue <= key.Item2)
                    {
                        var spawnedElement = _spawnedSliders.Dequeue();
                        spawnedElement.OnMoveCenter -= SliderElementOnMoveCenter;
                        spawnedElement.OnClick();

                        OnClicked?.Invoke(_multipliers[key]);
                        return;
                    }
                }
            }
        }
        
        public void OnEnemyDeath()
        {
            StartCoroutine(StartDelayer());
        }

        private IEnumerator StartDelayer()
        {
            yield return new WaitForSeconds(_delay);
            _blocking = false;
        }

        private void CreateRhythmAreas()
        {
            var slideRect = _sliderRect.sizeDelta.x - 10;
            var workingWidth = _sliderRect.sizeDelta.x - ((_rhythmsConfig.Rhythms[0].Rhythm.Count - 1) * _areasSpacing);
            var scaleDiff = workingWidth / _rhythmsConfig.Rhythms[0].Rhythm.Sum(x => x.Size);
            var spacingSize = (_areasSpacing * 100) / slideRect;


            var prefab = Resources.Load<GameObject>(_rhythmAreasPrefabPath);
            float positionDiff = 0;
            float leftBound = 0;
            float rightBound = 0;

            foreach (var rhythm in _rhythmsConfig.Rhythms[0].Rhythm)
            {
                CreateRhythmArea(ref prefab, rhythm, scaleDiff, ref positionDiff);
                AddMultiplier(rhythm, ref rightBound, ref leftBound, spacingSize, scaleDiff, slideRect);
            }
        }

        private void CreateRhythmArea(ref GameObject prefab, RhythmModel rhythm, float scaleDiff,
            ref float positionDiff)
        {
            var go = GameObject.Instantiate(prefab, _rhythmAreasRoot);
            var goRect = go.GetComponent<RectTransform>();
            var goRectImage = go.GetComponent<Image>();
            goRectImage.color = rhythm.Color;
            goRect.sizeDelta = new Vector2(rhythm.Size * scaleDiff, goRect.sizeDelta.y);
            goRect.anchoredPosition = new Vector2(positionDiff, 0);
            positionDiff += (rhythm.Size * scaleDiff) + _areasSpacing;
        }

        private void AddMultiplier(RhythmModel rhythm, ref float rightBound, ref float leftBound, float spacingSize,
            float scaleDiff, float slideRect)
        {
            var areaSize = (rhythm.Size * scaleDiff * 100) / slideRect;
            rightBound = leftBound + areaSize;
            _multipliers.Add((leftBound, rightBound), rhythm.Multiplier);
            leftBound = rightBound + spacingSize;
        }
        
        private void Update()
        {
            if (_blocking)
            {
                return;
            }

            _currentLevelTime += Time.deltaTime;

            if (_currentLevelTime >= _spawnTime[_currentSpawnIndex])
            {
                var slider = Instantiate(_rhythmSliderComponentPrefab, _sliderRoot);
                _spawnedSliders.Enqueue(slider);
                slider.Init(_sliderSpeed);
                slider.OnMoveCenter += SliderElementOnMoveCenter;
                _currentSpawnIndex++;
                if (_currentSpawnIndex >= _spawnTime.Count)
                {
                    _currentSpawnIndex = 0;
                    _currentLevelTime = 0;
                }
            }
        }

        private void SliderElementOnMoveCenter(RhythmSliderComponent obj)
        {
            _spawnedSliders.Dequeue().OnMoveCenter -= SliderElementOnMoveCenter;
        }
    }
}
