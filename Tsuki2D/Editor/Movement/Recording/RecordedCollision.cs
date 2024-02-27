using System;
using Lunari.Tsuki.Misc;
using UnityEngine;
namespace Lunari.Tsuki2D.Editor.Movement.Recording {
    [Serializable]
    public struct RecordedCollider {
        public string name;
        public Bounds2D bounds;
    }
    [Serializable]
    public struct RecordedContact {
        public Vector2 point;
        public Vector2 normal;
        public Vector2 relativeVelocity;
        public float separation;
        public float normalImpulse;
        public float tangentImpulse;
    }
    [Serializable]
    public struct RecordedCollision {
        public RecordedCollider collider;
        public RecordedContact[] contacts;
    }
}