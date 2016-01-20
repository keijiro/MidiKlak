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
using MidiJack;

namespace Klak.Midi
{
    public class MidiNoteEventSender : MonoBehaviour
    {
        #region Nested Public Classes

        public enum VoiceMode { Mono, Poly }

        public enum FilterMode { Off, NoteName, NoteNumber }

        public enum NoteName {
            C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B
        }

        [System.Serializable]
        public class NoteOnEvent : UnityEvent<int, float> {}

        [System.Serializable]
        public class NoteOffEvent : UnityEvent<int> {}

        #endregion

        #region Editable Properties

        [SerializeField]
        MidiChannel _channel = MidiChannel.All;

        [SerializeField]
        VoiceMode _voiceMode = VoiceMode.Mono;

        [SerializeField]
        FilterMode _filterMode = FilterMode.Off;

        [SerializeField]
        NoteName _noteName;

        [SerializeField]
        int _lowestNote = 60; // C4

        [SerializeField]
        int _highestNote = 60; // C4

        [SerializeField]
        NoteOnEvent _noteOnEvent;

        [SerializeField]
        NoteOffEvent _noteOffEvent;

        #endregion

        #region Private Properties And Variables

        int _lastNote = -1;

        bool CompareNoteToName(int number, NoteName name)
        {
            return (number % 12) == (int)name;
        }

        bool FilterNote(MidiChannel channel, int note)
        {
            if (_channel != MidiChannel.All && channel != _channel) return false;
            if (_filterMode == FilterMode.Off) return true;
            if (_filterMode == FilterMode.NoteName)
                return CompareNoteToName(note, _noteName);
            else // FilterMode.Number
                return _lowestNote <= note && note <= _highestNote;
        }

		void NoteOn(MidiChannel channel, int note, float velocity)
        {
            if (FilterNote(channel, note))
            {
                if (_voiceMode == VoiceMode.Mono &&
                    _lastNote != -1 && _lastNote != note)
                    _noteOffEvent.Invoke(_lastNote);

                _noteOnEvent.Invoke(note, velocity);
                _lastNote = note;
            }
        }

        void NoteOff(MidiChannel channel, int note)
        {
            if (FilterNote(channel, note))
            {
                if (_voiceMode == VoiceMode.Poly || _lastNote == note)
                {
                    _noteOffEvent.Invoke(note);
                    _lastNote = -1;
                }
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

        #endregion
    }
}
