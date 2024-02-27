using System;
using System.Linq;
using Lunari.Tsuki.Editor.Plotting;
using Lunari.Tsuki2D.Editor.Movement.Sampling;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEditor;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Analysis {
    public partial class MotorAnalyzerWindow {
        private readonly Plot xVelocityPlot;
        private readonly Plot yVelocityPlot;
        private readonly SplitPlotter velocityPlotter;
        public MotorAnalyzerWindow() {
            yVelocityPlot = new Plot();

            xVelocityPlot = new Plot();
            velocityPlotter = new SplitPlotter(xVelocityPlot, yVelocityPlot) {
                LeftTitle = "X",
                RightTitle = "Y"
            };
        }
        private void Plot(Motor m) {
            var buf = analysisRecorder.Buffer.ToArray();
            xVelocityPlot.Enabled = EditorGUILayout.Toggle("Show X", xVelocityPlot.Enabled);
            yVelocityPlot.Enabled = EditorGUILayout.Toggle("Show Y", yVelocityPlot.Enabled);
            xVelocityPlot.Min = 0;
            xVelocityPlot.Max = m.maxSpeed;
            xVelocityPlot.Color = Color.red;
            yVelocityPlot.Color = Color.green;
            yVelocityPlot.Min = -64;
            yVelocityPlot.Max = 64;
            for (var i = 0; i < buf.Length; i++) {
                var frame = buf[i];
                xVelocityPlot.Data[i] = Mathf.Abs(frame.velocity.x);
                yVelocityPlot.Data[i] = frame.velocity.y;
            }
            velocityPlotter.Plot(256);
        }
        private void ResizePlotters(int numSamples) {
            xVelocityPlot.Resize(numSamples);
            yVelocityPlot.Resize(numSamples);
        }
    }
}