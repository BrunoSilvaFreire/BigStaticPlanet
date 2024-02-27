using System;
using Lunari.Tsuki2D.Runtime.Movement.Updaters;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement {
    public class ContactsUpdater : SupportStateUpdater {
        private ContactPoint2D[] currentContacts;

        private void Awake() {
            currentContacts = new ContactPoint2D[4];
        }

        public override void Test(Motor motor, ref SupportState state) {
            ConsumeAndUpdateSupportState(ref state, motor.rigidbody.GetContacts(currentContacts));
        }

        private bool CheckHas(int total, Func<ContactPoint2D, bool> filter) {
            for (var i = 0; i < total; i++) {
                if (filter(currentContacts[i])) {
                    return true;
                }
            }

            return false;
        }

        private void ConsumeAndUpdateSupportState(ref SupportState supportState, int count) {
            supportState.down = CheckHas(count, DownCollision);
            supportState.up = CheckHas(count, UpCollision);
            supportState.left = CheckHas(count, LeftCollision);
            supportState.right = CheckHas(count, RightCollision);
            if (!supportState.down) {
                Debug.Log("Not grounded");
            }
#if UNITY_EDITOR

            foreach (var point in currentContacts) {
                Debug.DrawRay(point.point, point.normal, Color.red);
            }
#endif

            // currentContacts.Clear();
        }

        private static bool RightCollision(ContactPoint2D arg) {
            return arg.normal.x < 0;
        }

        private static bool LeftCollision(ContactPoint2D arg) {
            return arg.normal.x > 0;
        }

        private static bool UpCollision(ContactPoint2D arg) {
            return arg.normal.y < 0;
        }

        private static bool DownCollision(ContactPoint2D arg) {
            return arg.normal.y > 0;
        }
    }
}