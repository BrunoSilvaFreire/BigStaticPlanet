# Input

Dealing with user input can be a bit of a pain in the neck, mostly due to time steps.
If you haven't read it yet, [Fix your timestep!](https://gafferongames.com/post/fix_your_timestep/) by Glenn Fiedler
is a must read for any game developer.

The way input is dealt with in Tsuki2D is:
1. Create a new behaviour that inherits from `EntityInput`, add it to your object, and assign it to your motor.
2. Input is pooled in Unity's `Update` and stored into your input component.
3. That input is then consumed by the active state and attachments inside `FixedUpdate`.

This behaviour that extends from `EntityInput`, is referred to as the **input component**.

*Side note:*
If you take a look inside `EntityInput`, you will find that it's empty. The reason for this is that while, today, it may do nothing, Tsuki2D reserves it for future use, as we 
have plans to simply and provide a base for the input pooling process

## Reading input
Tsuki2D does not impose any input reading method for you.
We give you total freedom of any system you might want to use.
Whether that's the legacy input system, the new input system, or a third party asset like Rewired, anything will work.
The only requirement we have, it that you must write into your input component inside `Update`.

CSharp Pseudocode:
```csharp
public class MyEntityInput : EntityInput {
    public Vector2 aim;
    
    public void Update() {
        ReadInput();
    };
    
    private void ReadInput() {
        Vector2 mousePositionInWorld = GetMousePositionInWorld();
        Vector2 entityPositionInWorld = GetEntityPositionInWorld();
        aim = mousePositionInWorld - entityPositionInWorld;
        aim.Normalize();
    }
}
```
## Input helpers

The `EntityAction` class is your main helper class for pooling and consuming state.
It basically stores whenever an action has been requested, and keeps it flagged until you consume it inside your state.
This way, the intent of the player is preserved along the different time steps.

If instead, we merely checked our input directly inside FixedUpdate, we might start missing 
input, because FixedUpdate can run any number of times between Update, even zero, so by 
caching the player's input, we avoid that and assure we have responsive controls.

When using it, you should include it as fields in your input component.
The most simple example would be something like:
```csharp
public class MyEntityInput : EntityInput {
    public EntityAction jump;
    
    // Called every frame.
    private void ReadInput() {
        jump.Current = IsJumpKeyPressed(); 
    }
}

public class MyState : MotorState<MyEntityInput> {

    public override void Tick(Motor motor, MyEntityInput input, ref Vector2 velocity) {
        if (input.jump.Consume()) {
            PerformJump(ref velocity);
        }
    }
}
```