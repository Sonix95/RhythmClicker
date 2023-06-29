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

        private Coroutine _coroutine;

        private float _sliderSpeed = default;
        private float _sliderValue = default;

        private float _bitOnCenterDelay = default;

        private bool _blocking = true;

        public float SliderValue => _sliderValue;

        public void Init(float speed, float bitOnCenterDelay)
        {
            name = $"Bit_Element_No{Random.Range(0, 99999)}";

            _sliderSpeed = speed;

            _bitOnCenterDelay = bitOnCenterDelay;

            _sliderValue = 0f;
            _blocking = false;
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

            _sliderValue += _sliderSpeed * Time.deltaTime;

            if (_sliderValue > 50)
            {
                _sliderValue = 50.01f;
            }

            _sliderLeft.value = _sliderValue;
            _sliderRight.value = _sliderValue;

            if (_sliderValue >= 50)
            {
                _blocking = true;
                _coroutine = StartCoroutine(DestroyElement());
            }
        }

        private IEnumerator DestroyElement()
        {
            yield return new WaitForSeconds(_bitOnCenterDelay);

            OnMoveCenter?.Invoke(this);

            _coroutine = null;

            yield return null;
            Destroy(gameObject);
        }
    }
}
