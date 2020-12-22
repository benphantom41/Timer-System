# Timer-System
A Timer System I made for Unity

I made this system because I hated creatng multple variables and loops in my code just to run a simple line or function after a desired time.
This Timer system helps out with that


A Timer takes in 3 arguments. A Unity Action, a float that determines how long to wait, and an optional string for the name of the Timer.

Using DX.Timer.Create, you can create your own timer in one single line. 
You can also use DX.Timer.CreateRepeating if you want to run an action multiple times.