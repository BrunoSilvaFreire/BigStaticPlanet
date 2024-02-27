# Quick Start

## Creating movement

First of all, we need to create a `MotorState`:

```csharp
public class DummyState : MotorState {
    public override void Begin(Motor motor, ref Vector2 velocity) {
    }
    public override void Tick(Motor motor, ref Vector2 velocity) {
    }
    public override void End(Motor motor, ref Vector2 velocity) {
    }
}
```

There are three methods you can override in your Motor State
* Begin
    * Called when this state becomes the active state in the motor.
* Tick
    * Called every fixed frame while this state is the active state in the motor.
* End
    * Called when this state no longer is the active state in the motor.

All your movement logic should be executed within these methods.
```csharp
/**
 * This state moves your character to the right, and has a 1% chance every 
 * fixed frame of transitioning to nextState. When it does, it will immediately
 * cancel all velocity on the motor.
 */
public class DummyState : MotorState {
    public MotorState nextState;
    public float speed;

    public override void Begin(Motor motor, ref Vector2 velocity) {
    }

    public override void Tick(Motor motor, ref Vector2 velocity) {
        if (Random.value < 0.01F) {
            motor.ActiveState = nextState;
        } else {
            velocity = Vector2.right;
        }
    }

    public override void End(Motor motor, ref Vector2 velocity) {
        velocity = Vector2.zero;
    }
}
```

There are also **MotorAttachment**, which are also very useful and you should read 
about in the [MotorComponents](MotorComponents.md) pages.
