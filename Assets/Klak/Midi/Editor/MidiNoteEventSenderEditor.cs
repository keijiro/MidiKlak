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
    [CustomEditor(typeof(MidiNoteEventSender))]
    public class MidiNoteEventSenderEditor : Editor
    {
        SerializedProperty _eventType;
        SerializedProperty _channel;
        SerializedProperty _voiceMode;
        SerializedProperty _velocityCurve;

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

        Vector3[] _rectVertices;
        Vector3[] _lineVertices;

        void OnEnable()
        {
            _eventType = serializedObject.FindProperty("_eventType");
            _channel = serializedObject.FindProperty("_channel");
            _voiceMode = serializedObject.FindProperty("_voiceMode");
            _velocityCurve = serializedObject.FindProperty("_velocityCurve");

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

            _rectVertices = new Vector3[4];
            _lineVertices = new Vector3[50];
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_eventType);
            EditorGUILayout.PropertyField(_channel);

            var showAllEvents = _eventType.hasMultipleDifferentValues;
            var eventType = (MidiNoteEventSender.EventType)_eventType.enumValueIndex;

            if (showAllEvents ||
                eventType == MidiNoteEventSender.EventType.Gate)
            {
                EditorGUILayout.PropertyField(_voiceMode);
            }

            EditorGUILayout.PropertyField(_noteFilter);

            var showAllFilters = _noteFilter.hasMultipleDifferentValues;
            var noteFilter = (MidiNoteEventSender.NoteFilter)_noteFilter.enumValueIndex;

            if (showAllFilters ||
                noteFilter == MidiNoteEventSender.NoteFilter.NoteName)
            {
                EditorGUILayout.PropertyField(_noteName);
            }

            if (showAllFilters ||
                noteFilter == MidiNoteEventSender.NoteFilter.NoteNumber)
            {
                EditorGUILayout.PropertyField(_lowestNote);
                EditorGUILayout.PropertyField(_highestNote);
            }

            if (showAllEvents ||
                eventType != MidiNoteEventSender.EventType.Toggle)
            {
                EditorGUILayout.PropertyField(_velocityCurve);
                if (!_velocityCurve.hasMultipleDifferentValues)
                    DrawVelocityCurve(_velocityCurve.floatValue);
            }

            if (showAllEvents ||
                eventType == MidiNoteEventSender.EventType.Value)
            {
                EditorGUILayout.PropertyField(_offValue);
                EditorGUILayout.PropertyField(_onValue);
                EditorGUILayout.PropertyField(_interpolator);
            }

            if (showAllEvents ||
                eventType == MidiNoteEventSender.EventType.Trigger)
            {
                EditorGUILayout.PropertyField(_triggerEvent);
            }

            if (showAllEvents ||
                eventType == MidiNoteEventSender.EventType.Gate)
            {
                EditorGUILayout.PropertyField(_noteOnEvent);
                EditorGUILayout.PropertyField(_noteOffEvent);
            }

            if (showAllEvents ||
                eventType == MidiNoteEventSender.EventType.Toggle)
            {
                EditorGUILayout.PropertyField(_toggleOnEvent);
                EditorGUILayout.PropertyField(_toggleOffEvent);
            }

            if (showAllEvents ||
                eventType == MidiNoteEventSender.EventType.Value)
            {
                EditorGUILayout.PropertyField(_valueEvent);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawVelocityCurve(float coeff)
        {
            var height = EditorGUIUtility.singleLineHeight +
                EditorGUIUtility.standardVerticalSpacing * 2;

            var rect = GUILayoutUtility.GetRect(128, height);

            // label area
            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;

            // line space
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            rect.height -= EditorGUIUtility.standardVerticalSpacing * 2;

            // background
            _rectVertices[0] = new Vector3(rect.x, rect.y);
            _rectVertices[1] = new Vector3(rect.xMax, rect.y);
            _rectVertices[2] = new Vector3(rect.xMax, rect.yMax);
            _rectVertices[3] = new Vector3(rect.x, rect.yMax);

            Handles.DrawSolidRectangleWithOutline(
                _rectVertices, Color.white * 0.1f, Color.clear);

            // curve
            for (var i = 0; i < _lineVertices.Length; i++)
            {
                var x = (float)i / (_lineVertices.Length - 1);
                var y = MidiNoteEventSender.VelocityCurve(x, coeff);
                _lineVertices[i] = PointInRect(rect, x, y);
            }

            // draw the line
            Handles.DrawAAPolyLine(2.0f, _lineVertices);
        }

        Vector3 PointInRect(Rect rect, float x, float y)
        {
            return new Vector3(
                Mathf.Lerp(rect.x, rect.xMax, x),
                Mathf.Lerp(rect.y, rect.yMax, 1 - y), 0);
        }
    }
}
