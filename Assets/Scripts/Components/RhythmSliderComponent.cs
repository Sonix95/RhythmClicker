using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Components
{
    public class RhythmSliderComponent : MonoBehaviour
    {
        public event Action<RhythmSliderComponent> OnMoveCenter;

        [SerializeField] private Slider _sliderLeft = default;
        [SerializeField] private Slider _sliderRight = default;

        private float _sliderSpeed = 0f;
        private float _sliderValue = 0f;
        private bool _blocking = true;
        private Coroutine _coroutine;
        public float SliderValue => _sliderValue;

        public void Init(float speed)
        {
            _sliderSpeed = speed;
            _sliderValue = 0f;
            _blocking = false;
            name = $"{Random.Range(0, 99999)}";
        }

        public void OnClick()
        {
            _blocking = true;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            Destroy(gameObject);
        }

        private void Update()
        {
            if (_blocking)
            {
                return;
            }

            _sliderValue += Time.deltaTime * _sliderSpeed;

            _sliderLeft.value = _sliderValue;
            _sliderRight.value = _sliderValue;

            if (_sliderValue >= 50)
            {
                _blocking = true;
                OnMoveCenter?.Invoke(this);
                _coroutine = StartCoroutine(DestroyElement());
            }
        }

        private IEnumerator DestroyElement()
        {
            yield return new WaitForSeconds(.1f);
            _coroutine = null;
            Destroy(gameObject);
        }
    }
}
