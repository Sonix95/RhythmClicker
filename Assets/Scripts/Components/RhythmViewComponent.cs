using System;
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
        [SerializeField] private AudioClip _targetClip = default;
        [SerializeField] private AudioSource _audioSource = default;
        [SerializeField] private float _areasSpacing = default;
        [SerializeField] private string _rhythmAreasPrefabPath = default;
        [SerializeField] private string _rhythmSliderPrefabPath = default;
        [SerializeField] private RectTransform _sliderRect = default;
        [SerializeField] private RectTransform _rhythmAreasRoot = default;
        [SerializeField] private RectTransform _sliderRoot = default;
        [SerializeField] private float _sliderSpeed = default;
        [SerializeField] private float _bitOnCenterDelay = default;
        [SerializeField] private float _bitDestroyDelay = default;
        [SerializeField] private float _maxBitsOnScreen = default;
        [SerializeField] private List<float> _spawnTime = default;

        private RhythmConfig _rhythmsConfig = default;
        private RhythmSliderComponent _rhythmSliderComponentPrefab = default;
        private Dictionary<(float, float), float> _multipliers = default;
        private Queue<RhythmSliderComponent> _spawnedSliders;
        
        private bool _isPlayingSound = false;

        private float _currentLevelTime = 0;
        private int _currentSpawnIndex = 0;

        public void Init()
        {
            _multipliers = new Dictionary<(float, float), float>();
            _spawnedSliders = new Queue<RhythmSliderComponent>();
            _rhythmsConfig = Resources.Load<RhythmConfig>("Configs/RhythmsConfig");
            _rhythmSliderComponentPrefab = Resources.Load<RhythmSliderComponent>(_rhythmSliderPrefabPath);
            _spawnTime = new List<float>()
            {
                //1.2 - Intro
                5.61f, 6.28f, 6.81f, 6.97f, 7.50f, 7.65f, 8.17f, 8.52f, 9.01f, 9.54f, 9.70f, 10.05f, 10.21f,
                //1.3 - Section 1
                10.97f, 12.36f, 13.67f, 15.10f, 15.42f, 15.78f, 16.40f, 17.79f, 18.2f, 18.63f, 18.99f, 19.35f,
                //1.4 - Section 2
                21.93f, 22.27f, 22.62f, 22.96f, 23.30f, 23.64f, 23.98f, 24.34f, 24.68f, 25.02f, 25.36f, 25.70f, 26.03f, 26.38f, 26.76f, 27.05f, 27.41f, 27.78f, 28.10f, 28.43f, 28.77f, 29.12f, 29.46f, 29.83f, 30.12f, 30.50f, 30.83f, 31.18f, 31.52f, 31.89f, 32.23f, 32.55f,
                //1.5 - "Dialog"
                //A1
                32.92f, 33.57f, 33.94f, 34.27f, 34.95f, 35.33f,
                //P1
                35.67f, 36.34f, 37.04f, 37.71f,
                //A2
                38.38f, 39.06f, 39.42f, 39.75f, 40.43f, 40.76f,
                //P2
                41.12f, 41.84f, 42.49f, 43.18f,
                //1.6 Autro
                43.90f, 44.24f, 44.58f, 44.93f, 45.23f, 45.60f, 45.94f, 46.28f, 46.61f, 46.97f, 47.33f, 47.69f, 48.01f, 48.34f, 48.68f, 49.02f, 49.37f, 49.71f, 50.07f, 50.41f, 50.76f, 51.10f, 51.44f, 51.75f, 52.09f, 52.41f, 52.81f, 53.14f, 53.48f, 53.78f, 54.15f, 54.49f,
                //Final
                56.26f, 56.53f, 56.91f, 57.27f
            };

            CreateRhythmAreas();
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
            if (!_isPlayingSound)
            {
                _audioSource.PlayOneShot(_targetClip);
                _isPlayingSound = true;
            }

            _currentLevelTime += Time.deltaTime;
            if (_currentLevelTime >= _spawnTime[_currentSpawnIndex] - 1.5f)
            {
                if (_spawnedSliders.Count < _maxBitsOnScreen)
                {
                    var slider = Instantiate(_rhythmSliderComponentPrefab, _sliderRoot);
                    _spawnedSliders.Enqueue(slider);
                    slider.Init(_sliderSpeed, _bitOnCenterDelay, _bitDestroyDelay);
                    slider.OnMoveCenter += SliderElementOnMoveCenter;
                }
                
                _currentSpawnIndex++;
                if (_currentSpawnIndex >= _spawnTime.Count)
                {
                    _currentSpawnIndex = 0;
                    _currentLevelTime = 0;
                    _isPlayingSound = false;
                }
            }
        }

        private void SliderElementOnMoveCenter(RhythmSliderComponent obj)
        {
            _spawnedSliders.Dequeue().OnMoveCenter -= SliderElementOnMoveCenter;
        }
    }
}
