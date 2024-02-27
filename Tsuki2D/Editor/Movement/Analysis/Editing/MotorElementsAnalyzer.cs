using System;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis.Editing {
    public class MotorElementsAnalyzer : EditorWindow, ISerializationCallbackReceiver {
        [SerializeField]
        private Motor _persistedMotor;

        private MotorAnalyzerView _view;
        [MenuItem("Tools/Tsuki2D/MotorElementsAnalyzer")]
        public static MotorElementsAnalyzer Open() {
            var motorAnalyzer = CreateWindow<MotorElementsAnalyzer>();
            motorAnalyzer.ShowTab();
            return motorAnalyzer;
        }

        private void OnEnable() {
            rootVisualElement.Add(_view = new MotorAnalyzerView());
            if (_persistedMotor != null) {
                _view.MotorSlot.Value = _persistedMotor;
            }
        }
        public void OnBeforeSerialize() {
            _persistedMotor = _view.MotorSlot.Value;
        }
        public void OnAfterDeserialize() {
        }
    }
}