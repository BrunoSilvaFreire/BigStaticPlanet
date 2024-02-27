#if TSUKI_ENTITIES
using Lunari.Tsuki;
using Lunari.Tsuki.Entities.Problems;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Lunari.Tsuki2D.Runtime.Movement {
    public static class TraitsExtensions {
        public static ProblemBuilder WithAddGlobalAttachmentSolution<T>(this ProblemBuilder builder, Motor motor, string message = null) where T : MotorAttachment {
            builder.WithSolution(message ?? $"Add {typeof(T).GetLegibleName()}",
                () => {
                    motor.AddGlobalAttachment<T>();
#if UNITY_EDITOR
                    EditorUtility.SetDirty(motor.gameObject);
#endif
                }
            );
            return builder;
        }
    }
}
#endif