# Weekly Reports

### __Week 1__

Project description:

1. Team composition: 

1 member - Petrovici Ștefan

2. Type of project:

A virtual assistant implemented as an extension/plugin for a game engine that aids game developers in various tasks.

3. Core concepts:

As the development process goes on, the designer/developer can access the functionalities of the assistant by using its interface (either by writing in natural language in an input box, by using voice commands, or with a ui interface).

4. Links to similar concepts: 

While the ideea of a virtual assistant is easy to find online, and there are plenty of materials, the ideea of a virtual assistant that helps people develop games is a widely unexplored ideea.

(There are AIs that run in games, and there is an agent that can fabricate simple games, but there is no AI that can converse about games. With regards to the assistant part, there are assistants like Cortana, etc. But also a popular assistant for an IDE or GameEngine is yet to arise.)

I am looking for a name for this agent, maybe I shall call it:
AGENDA - a game engineering and design asisstant

The first skill i want to teach the AI, by its name, Agenda, is to provide templates for games with which developer can start their prototype.

(Template projects already exist online, and there are also some frameworks that provide templates or simple blocks that make games, Unity has a ton of frameworks. But even if these templates exist, the goal of this project is for AI to aid the human through the process, even if the AI does something  the human can do it by himself.)

This week I did research about this concept. What kind of games should we teach the AI? How can AI help us make games? First of all, if we are to endow the AI with knowledge of types of games, we need a classification of games. This is what I am working on right now, find the optimal classes for AI. Also I have researched what an AddOn for Unity and Godot would look like. Unity supports what I call scriptable Editor Actions - actions that are usually made by the user by interacting with the UI. Scriptable means I can access them from script. Why is this important? An ideal AI should be able to help you by interacting with the engine itself. This could possibly be used to make a game just by scripting its actions on the editor - aren't all tutorials just commands given inside the editor? Coming back to the question "How can AI help us create games?" - can AI teach us by tutorials? i guess it could actually. Can AI be aware of the game design principles we find in the support books? Perhaps it can be. 

These are some thoughts with which i am starting this project.

One other aspect to note, as I was researching the previous days, something ocurred to me. We should be able to provide templates for a text prompt. If these templates are simple enough to construct, could it be possible to have an elaborate input format, maybe in a file format, that could construct an entire game just by the details specified in that file? I think eventually we will reach this, but I don't intend to accomplish this in this project.

### __Week 2__

I have chnaged my project because the previous ideea was too hard to grow (I couldn't find ideeas to improve its core concept).

Project description:

1. Team composition: 

1 member - Petrovici Ștefan

2. Type of project:

An RPG game where you program the magic spells by writing in a mystical programming language. 

3. Core concepts:

The core gameplay is essentially a 2D shooter, top-down, real-time bullet-hell style. The only thing that is different is that instead of having multiple weapons that you switch between, the switch is realized at the script level.

4. Links to similar concepts: 

There are a lot of scripting games but most of them are resource-based and I want to make an action game in 2d.

https://store.steampowered.com/app/324190/CodeSpells/

https://store.steampowered.com/app/370360/TIS100/

https://store.steampowered.com/app/464350/Screeps/

https://www.codingame.com/start

https://adventure.land/

__Status:__

This week i worked on creating a coding interface in React and embedding Unity in the React project. Whenver a click is made, Unity sends a notification to React and the code is executed - for each spell a function in unity is called.

### __Week 3__
I integrated react and unity, you can control a character and press a click to cast a skill that is written.

### __Week 4__
I created a small prototype for the game. 
Features:
- character movement
- enemy ai follows you around
- on right click the spells are cast
- tilemap and enemy with collision
- if enemy is hit by fire is destroyed
- npc that shows you a puzzle and the puzzle itself
- 3 skills: fire, water, earth
- character camera movement

### __Week 5__
- featurefixing
- added pause
- experimented with reactunity, and other options to gain performance
- added first build with electron
- moved editor to sidebar
- refactoring
- added character and enemy sprites
- added some ui stuff
- added somwewhat working example of blockly
