//
// MidiKlack - MIDI extension for Klak
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using UnityEngine.Events;
using System;
using Klak.Math;
using MidiJack;

namespace Klak.Midi
{
    [AddComponentMenu("Klak/MIDI/Knob Input")]
    public class KnobInput : MonoBehaviour
    {
        #region Nested Public Classes

        public enum EventType {
            Value, Trigger, Toggle
        }

        [Serializable]
        public class ValueEvent : UnityEvent<float> {}

        #endregion

        #region Editable Properties

        [SerializeField]
        MidiChannel _channel = MidiChannel.All;

        [SerializeField]
        int _knobNumber = 0;

        [SerializeField]
        AnimationCurve _inputCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        EventType _eventType = EventType.Value;

        [SerializeField]
        float _outputValue0 = 0.0f;

        [SerializeField]
        float _outputValue1 = 1.0f;

        [SerializeField]
        FloatInterpolator.Config _interpolator;

        [SerializeField]
        ValueEvent _valueEvent;

        [SerializeField]
        UnityEvent _triggerEvent;

        [SerializeField]
        UnityEvent _toggleOnEvent;

        [SerializeField]
        UnityEvent _toggleOffEvent;

        #endregion

        #region Public Properties

        public float InputValue {
            get { return _lastInputValue; }
            set { DoKnobUpdate(value); }
        }

        #endregion

        #region Private Variables And Methods

        FloatInterpolator _value;
        float _lastInputValue;
        bool _toggleState;

        float CalculateTargetValue(float knobValue)
        {
            var p = _inputCurve.Evaluate(knobValue);
            return BasicMath.Lerp(_outputValue0, _outputValue1, p);
        }

        void OnKnobUpdate(MidiChannel channel, int knobNumber, float knobValue)
        {
            // do nothing if the setting doesn't match
            if (_channel != MidiChannel.All && channel != _channel) return;
            if (_knobNumber != knobNumber) return;
            // do the actual process
            DoKnobUpdate(_inputCurve.Evaluate(knobValue));
        }

        void DoKnobUpdate(float inputValue)
        {
            const float threshold = 0.5f;

            if (_eventType == EventType.Value)
            {
                // update the target value for the interpolator
                _value.targetValue =
                    BasicMath.Lerp(_outputValue0, _outputValue1, inputValue);
                // invoke the event in direct mode
                if (!_interpolator.enabled)
                    _valueEvent.Invoke(_value.Step());
            }
            else if (_lastInputValue < threshold && inputValue >= threshold)
            {
                if (_eventType == EventType.Trigger)
                {
                    _triggerEvent.Invoke();
                }
                else // EventType.Toggle
                {
                    _toggleState ^= true;
                    if (_toggleState)
                        _toggleOnEvent.Invoke();
                    else
                        _toggleOffEvent.Invoke();
                }
            }

            _lastInputValue = inputValue;
        }

        #endregion

        #region MonoBehaviour Functions

        void OnEnable()
        {
            MidiMaster.knobDelegate += OnKnobUpdate;
        }

        void OnDisable()
        {
            MidiMaster.knobDelegate -= OnKnobUpdate;
        }

        void Start()
        {
            _value = new FloatInterpolator(0, _interpolator);
        }

        void Update()
        {
            if (_eventType == EventType.Value &&_interpolator.enabled)
                _valueEvent.Invoke(_value.Step());
        }

        #endregion
    }
}
