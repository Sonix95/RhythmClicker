using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Components
{
    public class RhythmViewComponent : MonoBehaviour
    {
        private static float Slider_Speed = 65f;
        private static float Slider_Devider = 100f;
        private static float Bit_On_Center_Delay = 0.3f;
        private static float Bit_On_Center_Delay_Devider = 1f;

        public Action<float> OnClicked;

        [Header("Rhythm")]
        [SerializeField] private AudioSource _audioSource = default;
        [SerializeField] private GameObject _buttonRoot = default;
        [SerializeField] private Button _buttonStart2 = default;
        [SerializeField] private Button _buttonStart4 = default;
        [SerializeField] private Button _buttonStartAll = default;
        [SerializeField] private float _areasSpacing = default;
        [SerializeField] private string _rhythmAreasPrefabPath = default;
        [SerializeField] private string _rhythmSliderPrefabPath = default;
        [SerializeField] private RectTransform _sliderRect = default;
        [SerializeField] private RectTransform _rhythmAreasRoot = default;
        [SerializeField] private RectTransform _sliderRoot = default;
        [SerializeField] private float _maxBitsOnScreen = default;

        private RhythmElementsConfig _rhythmsConfig = default;
        private SongsConfig _songsConfig = default;
        private RhythmSliderComponent _rhythmSliderComponentPrefab = default;
        private Dictionary<(float, float), float> _multipliers = default;
        private Queue<RhythmSliderComponent> _spawnedSliders;

        private Coroutine _startCoroutine = null;

        private bool _isPlayingSound;
        private bool _isStarted;

        private float _currentLevelTime;
        private int _currentSpawnIndex;

        private SongsModel _currentSongModel = null;
        private AudioClip _currentAudioClip = null;

        public void Init()
        {
            _rhythmsConfig = Resources.Load<RhythmElementsConfig>("Configs/RhythmElementsConfig");
            _songsConfig = Resources.Load<SongsConfig>("Configs/SongsConfig");
            _rhythmSliderComponentPrefab = Resources.Load<RhythmSliderComponent>(_rhythmSliderPrefabPath);

            _buttonStart2.onClick.AddListener(ClickButtonHandler2);
            _buttonStart4.onClick.AddListener(ClickButtonHandler4);
            _buttonStartAll.onClick.AddListener(ClickButtonHandlerAll);

            _multipliers = new Dictionary<(float, float), float>();
            _spawnedSliders = new Queue<RhythmSliderComponent>();

            _startCoroutine = null;
            _isPlayingSound = false;
            _isStarted = false;
            _currentLevelTime = 0;
            _currentSpawnIndex = 0;

            ClearSongInfo();

            CreateRhythmAreas();
        }

        private void ClickButtonHandler2()
        {
            _maxBitsOnScreen = 2;
            StartSong();
        }
        private void ClickButtonHandler4()
        {
            _maxBitsOnScreen = 4;
            StartSong();
        }
        private void ClickButtonHandlerAll()
        {
            _maxBitsOnScreen = 9999;
            StartSong();
        }

        private void StartSong()
        {
            _buttonRoot.SetActive(false);

            SetSongInfo("Drumnbass");

            _isStarted = _currentSongModel != null;
        }

        private void SetSongInfo(string name)
        {
            _currentSongModel = _songsConfig.Songs.FirstOrDefault(x => x.Name == name);
            _currentAudioClip = Resources.Load<AudioClip>(_currentSongModel.SongResourcePath);
        }

        private void ClearSongInfo()
        {
            _currentAudioClip = null;
            _currentSongModel = null;
        }

        public void OnClickHandler()
        {
            if (_spawnedSliders.Count > 0)
            {
                foreach (var key in _multipliers.Keys)
                {
                    if (_spawnedSliders.Peek().SliderValue >= key.Item1 &&
                        _spawnedSliders.Peek().SliderValue <= key.Item2)
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

        private void AddMultiplier(RhythmModel rhythm, ref float rightBound, ref float leftBound, float spacingSize, float scaleDiff, float slideRect)
        {
            var areaSize = (rhythm.Size * scaleDiff * 100) / slideRect;
            rightBound = leftBound + areaSize;
            _multipliers.Add((leftBound, rightBound), rhythm.Multiplier);
            leftBound = rightBound + spacingSize;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_isPlayingSound && _startCoroutine == null)
                {
                    SetSongInfo("Drumnbass");
                    
                    _isStarted = _currentSongModel != null;
                }
            }

            if (!_isStarted)
            {
                return;
            }

            if (!_isPlayingSound)
            {
                _audioSource.PlayOneShot(_currentAudioClip);
                _isPlayingSound = true;
                _startCoroutine = null;
            }

            _currentLevelTime += Time.deltaTime;

            if (_currentSpawnIndex < _currentSongModel.Bits.Count && _currentLevelTime >= _currentSongModel.Bits[_currentSpawnIndex] - 65f / 100f + 0.3f)
            {
                if (_spawnedSliders.Count < _maxBitsOnScreen)
                {
                    var slider = Instantiate(_rhythmSliderComponentPrefab, _sliderRoot);
                    _spawnedSliders.Enqueue(slider);
                    slider.Init(65f, 0.3f);
                    slider.OnMoveCenter += SliderElementOnMoveCenter;
                }

                _currentSpawnIndex++;
            }

            if (!_audioSource.isPlaying)
            {
                ClearSongInfo();
                _isStarted = false;
                _isPlayingSound = false;
                _currentSpawnIndex = 0;
                _currentLevelTime = 0;
                _buttonRoot.SetActive(true);
            }
        }

        private void SliderElementOnMoveCenter(RhythmSliderComponent obj)
        {
            _spawnedSliders.Dequeue().OnMoveCenter -= SliderElementOnMoveCenter;
        }
    }
}
