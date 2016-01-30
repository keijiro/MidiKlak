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
    [CustomEditor(typeof(KnobInput))]
    public class KnobInputEditor : Editor
    {
        SerializedProperty _channel;
        SerializedProperty _knobNumber;
        SerializedProperty _inputCurve;

        SerializedProperty _eventType;
        SerializedProperty _outputValue0;
        SerializedProperty _outputValue1;
        SerializedProperty _interpolator;

        SerializedProperty _valueEvent;
        SerializedProperty _triggerEvent;
        SerializedProperty _toggleOnEvent;
        SerializedProperty _toggleOffEvent;

        void OnEnable()
        {
            _channel = serializedObject.FindProperty("_channel");
            _knobNumber = serializedObject.FindProperty("_knobNumber");
            _inputCurve = serializedObject.FindProperty("_inputCurve");

            _eventType = serializedObject.FindProperty("_eventType");
            _outputValue0 = serializedObject.FindProperty("_outputValue0");
            _outputValue1 = serializedObject.FindProperty("_outputValue1");
            _interpolator = serializedObject.FindProperty("_interpolator");

            _valueEvent = serializedObject.FindProperty("_valueEvent");
            _triggerEvent = serializedObject.FindProperty("_triggerEvent");
            _toggleOnEvent = serializedObject.FindProperty("_toggleOnEvent");
            _toggleOffEvent = serializedObject.FindProperty("_toggleOffEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_channel);
            EditorGUILayout.PropertyField(_knobNumber);
            EditorGUILayout.PropertyField(_inputCurve);

            EditorGUILayout.Space();

            var showAllOptions = _eventType.hasMultipleDifferentValues;
            var eventType = (KnobInput.EventType)_eventType.enumValueIndex;

            EditorGUILayout.PropertyField(_eventType);

            if (showAllOptions || eventType == KnobInput.EventType.Value)
            {
                EditorGUILayout.PropertyField(_outputValue0);
                EditorGUILayout.PropertyField(_outputValue1);
                EditorGUILayout.PropertyField(_interpolator);
                EditorGUILayout.PropertyField(_valueEvent);
            }

            if (showAllOptions || eventType == KnobInput.EventType.Trigger)
                EditorGUILayout.PropertyField(_triggerEvent);

            if (showAllOptions || eventType == KnobInput.EventType.Toggle)
            {
                EditorGUILayout.PropertyField(_toggleOnEvent);
                EditorGUILayout.PropertyField(_toggleOffEvent);
            }

            if (EditorApplication.isPlaying &&
                !serializedObject.isEditingMultipleObjects)
            {
                var instance = (KnobInput)target;
                instance.debugInput =
                    EditorGUILayout.Slider("Debug", instance.debugInput, 0, 1);
                EditorUtility.SetDirty(target); // request repaint
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
