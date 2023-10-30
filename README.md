# Space Blaster Deluxe
This project was done as part of a job application. The prompt for the assignment was "make a clone of Asteroids Deluxe" in a week. I spent about 22 hours on this project across 4 days (plus a couple of minor tweaks on the 5th day :P).

Everything under 
Scripts/AsteroidsDeluxe was written from scratch for this project. I tried to minimize the use of third party libraries for the most part.

[Try it Here!](https://mjholtzem.itch.io/space-blaster-deluxe)

## How did it go?
I'm very happy with the amount of work I was able to get done in the time-frame and that the quality of the code remained pretty neat. There is a tendency in these fast paced type projects for code quality to deteriorate greatly towards the end and that didn't really happen. I more or less got all the features done that I identified in the real game and hit most of my stretch goals. A high level list of completed features

 - Player Controls
 - Player Shooting
 - Player Shield system
 - Screen wrapping for everything and most enemies tracking the player across screen boundaries
 - Asteroid Obstacle
 - Death Star Enemy
 - Chaser Enemies spawned from the Death Star that follow the player
 - Saucer Enemies with zig-zag behaviour and bullet spread
 - Wave spawning system that spawns in all enemies and asteroids and increases in difficulty over time
 - Point system with point values accurate to the arcade hardward (AFAIK)
 - Lives system with a new life being added every 10k points
 - Game Over state that returns you to the main menu
 - Main Menu with it's own wave spawning logic that will simulate forever in the background similar to the original arcade hardware
 - Audio for everything!!!

## What could be improved?
Of course given the time constraint there are some things I did not get done. Here are a few things that I think I would improve given more time

 - Audio integration was a bit rushed and could probably be cleaned up a bit
 - A bit of duplicate code in destruction logic of various objects
 - Object pooling. Decided to skip it for this project to prioritize features but ideally all Instantiations would be replaced with object pooling and everything would be pre-seeded to avoid any allocations at runtime
 - GetComponent calls. I was mostly able to avoid these but there are a few hanging around. I would maybe cache these components somewhere keyed by their gameobject for allocation free access. At the end of the day though this is not a huge deal
 - Leaderboards. Pretty important to the arcade game but unfortunatley didn't have time for it.
 - Controls: Would have liked to update to the new Input Manager and do the controls better. Adding controller support and maybe even touch-screen support
