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
using UnityEditor;

namespace Klak.Midi
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NoteInput))]
    public class NoteInputEditor : Editor
    {
        SerializedProperty _eventType;
        SerializedProperty _channel;
        SerializedProperty _voiceMode;
        SerializedProperty _velocityOffset;

        SerializedProperty _noteFilter;
        SerializedProperty _noteName;
        SerializedProperty _lowestNote;
        SerializedProperty _highestNote;

        SerializedProperty _offValue;
        SerializedProperty _onValue;
        SerializedProperty _interpolator;

        SerializedProperty _triggerEvent;
        SerializedProperty _noteOnEvent;
        SerializedProperty _noteOffEvent;
        SerializedProperty _toggleOnEvent;
        SerializedProperty _toggleOffEvent;
        SerializedProperty _valueEvent;

        void OnEnable()
        {
            _eventType = serializedObject.FindProperty("_eventType");
            _channel = serializedObject.FindProperty("_channel");
            _voiceMode = serializedObject.FindProperty("_voiceMode");
            _velocityOffset = serializedObject.FindProperty("_velocityOffset");

            _noteFilter = serializedObject.FindProperty("_noteFilter");
            _noteName = serializedObject.FindProperty("_noteName");
            _lowestNote = serializedObject.FindProperty("_lowestNote");
            _highestNote = serializedObject.FindProperty("_highestNote");

            _offValue = serializedObject.FindProperty("_offValue");
            _onValue = serializedObject.FindProperty("_onValue");
            _interpolator = serializedObject.FindProperty("_interpolator");

            _triggerEvent = serializedObject.FindProperty("_triggerEvent");
            _noteOnEvent = serializedObject.FindProperty("_noteOnEvent");
            _noteOffEvent = serializedObject.FindProperty("_noteOffEvent");
            _toggleOnEvent = serializedObject.FindProperty("_toggleOnEvent");
            _toggleOffEvent = serializedObject.FindProperty("_toggleOffEvent");
            _valueEvent = serializedObject.FindProperty("_valueEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_eventType);
            EditorGUILayout.PropertyField(_channel);

            var showAllEvents = _eventType.hasMultipleDifferentValues;
            var eventType = (NoteInput.EventType)_eventType.enumValueIndex;

            if (showAllEvents ||
                eventType == NoteInput.EventType.Gate)
            {
                EditorGUILayout.PropertyField(_voiceMode);
            }

            EditorGUILayout.PropertyField(_noteFilter);

            var showAllFilters = _noteFilter.hasMultipleDifferentValues;
            var noteFilter = (NoteInput.NoteFilter)_noteFilter.enumValueIndex;

            if (showAllFilters ||
                noteFilter == NoteInput.NoteFilter.NoteName)
            {
                EditorGUILayout.PropertyField(_noteName);
            }

            if (showAllFilters ||
                noteFilter == NoteInput.NoteFilter.NoteNumber)
            {
                EditorGUILayout.PropertyField(_lowestNote);
                EditorGUILayout.PropertyField(_highestNote);
            }

            if (showAllEvents ||
                eventType != NoteInput.EventType.Toggle)
            {
                EditorGUILayout.PropertyField(_velocityOffset);
            }

            if (showAllEvents ||
                eventType == NoteInput.EventType.Value)
            {
                EditorGUILayout.PropertyField(_offValue);
                EditorGUILayout.PropertyField(_onValue);
                EditorGUILayout.PropertyField(_interpolator);
            }

            if (showAllEvents ||
                eventType == NoteInput.EventType.Trigger)
            {
                EditorGUILayout.PropertyField(_triggerEvent);
            }

            if (showAllEvents ||
                eventType == NoteInput.EventType.Gate)
            {
                EditorGUILayout.PropertyField(_noteOnEvent);
                EditorGUILayout.PropertyField(_noteOffEvent);
            }

            if (showAllEvents ||
                eventType == NoteInput.EventType.Toggle)
            {
                EditorGUILayout.PropertyField(_toggleOnEvent);
                EditorGUILayout.PropertyField(_toggleOffEvent);
            }

            if (showAllEvents ||
                eventType == NoteInput.EventType.Value)
            {
                EditorGUILayout.PropertyField(_valueEvent);
            }

            if (EditorApplication.isPlaying &&
                !serializedObject.isEditingMultipleObjects)
            {
                var instance = (NoteInput)target;
                instance.debugInput =
                    EditorGUILayout.Toggle("Debug", instance.debugInput);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
