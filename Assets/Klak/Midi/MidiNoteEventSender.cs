//
// MidiKlak - MIDI extension for Klak
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
    public class MidiNoteEventSender : MonoBehaviour
    {
        #region Nested Public Classes

        public enum EventType {
            Trigger, Gate, Toggle, Value
        }

        public enum VoiceMode {
            Mono, Poly
        }

        public enum NoteFilter {
            Off, NoteName, NoteNumber
        }

        public enum NoteName {
            C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B
        }

        [Serializable]
        public class NoteOnEvent : UnityEvent<int, float> {}

        [Serializable]
        public class NoteOffEvent : UnityEvent<int> {}

        [Serializable]
        public class ValueEvent : UnityEvent<float> {}

        #endregion

        #region Editable Properties

        [SerializeField]
        EventType _eventType = EventType.Trigger;

        [SerializeField]
        MidiChannel _channel = MidiChannel.All;

        [SerializeField]
        VoiceMode _voiceMode = VoiceMode.Mono;

        [SerializeField]
        NoteFilter _noteFilter = NoteFilter.Off;

        [SerializeField]
        NoteName _noteName;

        [SerializeField]
        int _lowestNote = 60; // C4

        [SerializeField]
        int _highestNote = 60; // C4

        [SerializeField, Range(0, 1)]
        float _velocityOffset = 0.0f;

        [SerializeField]
        float _offValue = 0.0f;

        [SerializeField]
        float _onValue = 1.0f;

        [SerializeField]
        FloatInterpolator.Config _interpolator;

        [SerializeField]
        ValueEvent _triggerEvent;

        [SerializeField]
        NoteOnEvent _noteOnEvent;

        [SerializeField]
        NoteOffEvent _noteOffEvent;

        [SerializeField]
        UnityEvent _toggleOnEvent;

        [SerializeField]
        UnityEvent _toggleOffEvent;

        [SerializeField]
        ValueEvent _valueEvent;

        #endregion

        #region Private Properties And Variables

        int _lastNote = -1;
        FloatInterpolator _value;
        bool _toggle;

        bool CompareNoteToName(int number, NoteName name)
        {
            return (number % 12) == (int)name;
        }

        bool FilterNote(MidiChannel channel, int note)
        {
            if (_channel != MidiChannel.All && channel != _channel) return false;
            if (_noteFilter == NoteFilter.Off) return true;
            if (_noteFilter == NoteFilter.NoteName)
                return CompareNoteToName(note, _noteName);
            else // NoteFilter.Number
                return _lowestNote <= note && note <= _highestNote;
        }

		void NoteOn(MidiChannel channel, int note, float velocity)
        {
            if (!FilterNote(channel, note)) return;

            velocity = Mathf.Lerp(_velocityOffset, 1.0f, velocity);

            if (_eventType == EventType.Trigger)
            {
                _triggerEvent.Invoke(velocity);
            }
            else if (_eventType == EventType.Gate)
            {
                if (_voiceMode == VoiceMode.Mono &&
                    _lastNote != -1 && _lastNote != note)
                    _noteOffEvent.Invoke(_lastNote);

                _noteOnEvent.Invoke(note, velocity);
                _lastNote = note;
            }
            else if (_eventType == EventType.Toggle)
            {
                _toggle ^= true;
                if (_toggle)
                    _toggleOnEvent.Invoke();
                else
                    _toggleOffEvent.Invoke();
            }
            else // EventType.Value
            {
                _value.targetValue = _onValue * velocity;
            }
        }

        void NoteOff(MidiChannel channel, int note)
        {
            if (!FilterNote(channel, note)) return;

            if (_eventType == EventType.Gate)
            {
                if (_voiceMode == VoiceMode.Poly || _lastNote == note)
                {
                    _noteOffEvent.Invoke(note);
                    _lastNote = -1;
                }
            }
            else if (_eventType == EventType.Value)
            {
                _value.targetValue = _offValue;
            }
        }

        #endregion

        #region MonoBehaviour Functions

        void OnEnable()
        {
            MidiMaster.noteOnDelegate += NoteOn;
            MidiMaster.noteOffDelegate += NoteOff;
        }

        void OnDisable()
        {
            MidiMaster.noteOnDelegate -= NoteOn;
            MidiMaster.noteOffDelegate -= NoteOff;
        }

        void Start()
        {
            _value = new FloatInterpolator(0, _interpolator);
        }

        void Update()
        {
            if (_eventType == EventType.Value)
                _valueEvent.Invoke(_value.Step());
        }

        #endregion
    }
}
