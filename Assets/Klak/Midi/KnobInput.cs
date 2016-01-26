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

        [Serializable]
        public class KnobEvent : UnityEvent<float> {}

        #endregion

        #region Editable Properties

        [SerializeField]
        MidiChannel _channel = MidiChannel.All;

        [SerializeField]
        int _knobNumber = 0;

        [SerializeField]
        FloatInterpolator.Config _interpolator;

        [SerializeField]
        KnobEvent _knobEvent;

        #endregion

        #region Private Variables

        FloatInterpolator _value;

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _value = new FloatInterpolator(0, _interpolator);
        }

        void Update()
        {
            _value.targetValue = MidiMaster.GetKnob(_channel, _knobNumber);
            _knobEvent.Invoke(_value.Step());
        }

        #endregion
    }
}
