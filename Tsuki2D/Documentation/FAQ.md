# FAQ
# Why are you using Rigidbody2D and not doing manual raycasts / physics?
The short answer is: Box2D is probably smarter than we are.

The long answer is:  
Implementing custom physics is just not the current goal of Tsuki2D.

The current goal of Tsuki2D is:
* Having a clean, extensible, and straightforward architecture
* Having strong debugging and analysis tools for creating awesome movement

Implementing custom physics in order to provide absolute control, just adds a ton
of scope for a feature that isn't our current goal.