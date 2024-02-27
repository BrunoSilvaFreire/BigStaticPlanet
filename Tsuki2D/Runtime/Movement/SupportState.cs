using System;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement {
    public struct SupportSide {
        public static readonly SupportSide Up = new SupportSide {
            flags = DirectionFlags.Up,
            direction = Vector2Int.up,
            tangent = Vector2.right
        };
        public static readonly SupportSide Down = new SupportSide {
            flags = DirectionFlags.Down,
            direction = Vector2Int.down,
            tangent = Vector2.left
        };
        public static readonly SupportSide Left = new SupportSide {
            flags = DirectionFlags.Left,
            direction = Vector2Int.left,
            tangent = Vector2.up
        };
        public static readonly SupportSide Right = new SupportSide {
            flags = DirectionFlags.Right,
            direction = Vector2Int.right,
            tangent = Vector2.down

        };

        public static readonly SupportSide[] AllSides = {Up, Down, Left, Right};
        public DirectionFlags flags;
        public Vector2Int direction;
        public Vector2 tangent;

    }
    [Serializable]
    public struct SupportState : IComparable<SupportState> {
        public DirectionFlags flags;

        private bool GetBit(DirectionFlags f) {
            return (flags & f) == f;
        }

        private void SetBit(DirectionFlags directionFlags, bool value) {
            if (value) {
                flags |= directionFlags;
            } else {
                flags &= ~directionFlags;
            }
        } // ReSharper disable InconsistentNaming
        public bool up {
            get => GetBit(DirectionFlags.Up);
            set => SetBit(DirectionFlags.Up, value);
        }


        public bool down {
            get => GetBit(DirectionFlags.Down);
            set => SetBit(DirectionFlags.Down, value);
        }

        public bool left {
            get => GetBit(DirectionFlags.Left);
            set => SetBit(DirectionFlags.Left, value);
        }

        public bool right {
            get => GetBit(DirectionFlags.Right);
            set => SetBit(DirectionFlags.Right, value);
        }

        public int CompareTo(SupportState other) {
            var upComparison = up.CompareTo(other.up);
            if (upComparison != 0) {
                return upComparison;
            }

            var downComparison = down.CompareTo(other.down);
            if (downComparison != 0) {
                return downComparison;
            }

            var leftComparison = left.CompareTo(other.left);
            if (leftComparison != 0) {
                return leftComparison;
            }

            return right.CompareTo(other.right);
        }

        public bool Equals(SupportState other) {
            return flags == other.flags;
        }

        public override bool Equals(object obj) {
            return obj is SupportState other && Equals(other);
        }

        public override int GetHashCode() {
            return (int) flags;
        }

        public static bool operator ==(SupportState a, SupportState b) {
            return a.CompareTo(b) == 0;
        }

        public static bool operator !=(SupportState a, SupportState b) {
            return !(a == b);
        }

        public int HorizontalNormal {
            get {
                if (left == right) {
                    return 0;
                }

                if (left) {
                    return 1;
                }

                if (right) {
                    return -1;
                }

                return 0;
            }
        }

        public int HorizontalDirection {
            get {
                if (left == right) {
                    return 0;
                }

                if (left) {
                    return -1;
                }

                if (right) {
                    return 1;
                }

                return 0;
            }
        }

        public bool Any => !None;
        public bool None => flags == 0;

        public bool HasCollisionOnDirection(Vector2 velocity) {
            return GetBit(GetXDirectionFlags(velocity.x) | GetYDirectionFlags(velocity.y));
        }

        private DirectionFlags GetXDirectionFlags(float x) {
            switch (Math.Sign(x)) {
                case 1:
                    return DirectionFlags.Right;
                case -1:
                    return DirectionFlags.Left;
                default:
                    return 0;
            }
        }

        private DirectionFlags GetYDirectionFlags(float y) {
            switch (Math.Sign(y)) {
                case 1:
                    return DirectionFlags.Up;
                case -1:
                    return DirectionFlags.Down;
                default:
                    return 0;
            }
        }
        public bool HasSupport(SupportSide side) {
            return GetBit(side.flags);
        }
        public bool HasHorizontalCollision() {
            return left || right;
        }
        public bool HasVerticalCollision() {
            return up || down;
        }
        public void ToggleFlags(DirectionFlags value) {
            if ((flags & value) == value) {
                flags &= ~value;
            } else {
                flags |= value;
            }
        }
    }
}