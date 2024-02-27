using System;
using System.Collections.Generic;
using System.Linq;
using Lunari.Tsuki;
using Lunari.Tsuki.Editor;
using Lunari.Tsuki.Editor.Extenders;
using Lunari.Tsuki2D.Editor.Movement.Analysis.Editing;
using Lunari.Tsuki2D.Editor.Movement.Sampling.Clocks;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Events;
namespace Lunari.Tsuki2D.Editor.Movement.Sampling {
    public delegate MotorSamplingClock MotorSamplerFactory(Motor motor);

    public static class MotorSamplingRegistry {

        public static Dictionary<Type, MotorSamplerFactory> Registry { get; } = new Dictionary<Type, MotorSamplerFactory> {
            [typeof(InjectedFixedTimeClock)] = motor => new InjectedFixedTimeClock(motor),
            [typeof(EditorTimeClock)] = motor => new EditorTimeClock()
        };

        public static MotorSamplingClock CreateClockByType(Motor forMotor, Type type) {
            if (Registry.TryGetValue(type, out var factory)) {
                return factory(forMotor);
            }
            return null;
        }
        public static DropdownButton CreateClockSelectorButton(ListDropdown<Type>.OnSelectedCallback callback) {
            return new DropdownButton(
                new ListDropdown<Type>(
                    Registry.Keys.ToList(),
                    callback
                ),
                GetDefaultSamplerLabel()
            );
        }
        public static GUIContent GetDefaultSamplerLabel() {
            return EditorGUIUtility.TrTempContent("Select clock");
        }
        public static MotorSamplerFactory GetLastSelectedSamplerFactory() {
            var val = MotorAnalyzerSettings.LastSelectedSamplingClock.Value;
            return val.IsNullOrEmpty() ? null : Registry.FirstOrDefault(pair => pair.Key.FullName == val).Value;
        }
    }
    public class ListDropdown<T> : AdvancedDropdown {
        public delegate void OnSelectedCallback(int index, T item);

        private readonly List<T> items;
        private readonly OnSelectedCallback onSelectedCallback;
        public ListDropdown(List<T> items, OnSelectedCallback onSelectedCallback) : base(new AdvancedDropdownState()) {
            this.items = items;
            this.onSelectedCallback = onSelectedCallback;
        }
        private class ListDropdownItem : AdvancedDropdownItem {

            public ListDropdownItem(string name, int itemIndex) : base(name) {
                this.ItemIndex = itemIndex;
            }

            public int ItemIndex {
                get;
            }
        }
        protected override void ItemSelected(AdvancedDropdownItem item) {
            if (item is ListDropdownItem lItem) {

                var index = lItem.ItemIndex;
                onSelectedCallback(index, items[index]);
            }
        }
        protected override AdvancedDropdownItem BuildRoot() {
            var root = new AdvancedDropdownItem(string.Empty);
            for (var i = 0; i < items.Count; i++) {
                var item = items[i];
                root.AddChild(new ListDropdownItem(item.ToString(), i));
            }
            return root;
        }
    }
}