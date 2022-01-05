# Spel Ignite

![gamegif](docs/gamegif10.gif)

The game is comprised of 4 repositories:

The frontend (where the game is embedded and how the user interacts with the application):

[firststef/Spel-Ignite-Web](https://github.com/firststef/Spel-Ignite-Web)

The language implementation as a node package:

[firststef/Spel-Ignite](https://github.com/firststef/Spel-Ignite)

The game created in Unity:

[firststef/Spel-Ignite-Game](https://github.com/firststef/Spel-Ignite-Game)

A server that registers users and compiles the code for the running games (currently not a priority until the game is finished)

[firststef/Spel-Ignite-Server](https://github.com/firststef/Spel-Ignite-Server)

Documentation is in docs/ folder.

# What is this game

Actually let's start with why

# Why this game

As a coder you might have one day encountered a particular language feature that amazed you (for me, c++'s making recursive functions using variadic parameter packs was particularly addicting). Discovering a language is really fun. Not to mention, sometimes it feels like **magic**.

I particularly like weird languages. When i found ![esolangs](https://esolangs.org/wiki/Main_Page) I was really inspired. I also made a weird looking language, ![SPEL](https://github.com/firststef/SPEL), in my 2nd year in college, on which i based this project.

I wanted to craft a coding experience that was immersive, and the best example of this was found in games. I play a lot of games, but the games I love all have an interesting world, filled with mistery and beings that feel real. The game above is (just) the start of a project while pursuing this goal.

# What is this game

When I started the design for this game I asked myself some questions.
"What is a programming language in a magical world?"
"What is a program in a magical world?"
"If programming is magic, what is executing a program in a magical world?"

The answers I found, I tried implementing them in this game. Basically, in a fantasy world, a spell is a set of actions the wizard does in order to change something. Magic is what we call the execution of these instructions. About the nature of these actions: are they mana-oriented?

Mana, a source of energy found present in some creatures, is most of the times the generator for spells. This energy kinda resembles a program flow, by expanding or burning it in different ways you can obtain different effects. I imagined mana system that is based on the different attributes of mana:

```nim
store x = obtain mana from body 80.

store x = transmute mana to fire mana.

create fire orb y from x.

apply mana reinforcement to y.

throw y as fireball.
```

Even though it sounded cool, it felt very limiting, dwelling on the various aspects of mana. I wanted to focus on gameplay too, and also there is more to magic then mana. The way you interact with the world, the environment are also very important. Also i didn't want to make a game that feels like making chemistry with mana.

The technology used backdoors is really complex, which is why I had to limit my design time and come with a valid solution quick, to allow time for development. The thing is, at the most basic level, magic is about making actions. And doing magic is like doing some actions in an ordered fashion, first I charge my mana, then I apply some modifiers, etc etc. So in the end, to answer some of my questions, magic is about automatizing some basic instructions. Spells are a set of these instructions.

The most basic instruction at the time was releasing fire from your hand (or wand). The next level for that would be to make that fire take some form and throw it as a fireball. And the next level would be to `BURST A HUGE KAMEHAMEHA WAVE AND DESTROY EVERYTHING` :))))))).

For the current prototype I tried implementing some of the main points of the game, such as writing spells, casting, progressing by finding blocks (ah yes, btw at first it was just coding, but then some people said it was too hard and meh so I added some blocks, and it feels better now), a simple puzzle (because learning and puzzles work great together).

# How does it work

To explain brielfy, I set the Blockly system to generate SPEL code. This code is passed to a SPEL compiler, written in antlr, that gives me a simplified json that tells me what specific instructions are present, in a convenient tree-structure. This is sent by the javascript in the page to the unity build, which passes it to a SPEL interpreter written in C# and embedded in the game code.

# Help

Currently the game has implemented the following features:
```nim
cast [fire|water|earth].
```

Modifying spell attributes:
```nim
enchant [fire|water|earth|composed] with [growth|speed]. => returns a modified spell
```

Changing the nature of spells:
```nim
enchant [fire|water|earth|composed] with [orb]. => returns an upgraded spell
```

Note that some combinations might have some issues, but I do plan on improving the entire system.

```nim
as long as [playerMana] => while player has mana, throw fireball

 [cast enchant fire with orb.|any]

terminus
```

Note that some combinations might have some issues, but I do plan on improving the entire system.

# Contributing

`DESIGN:` As you can see I put some thought into the design of programming as a substitute for magic in a fantasy world, so you know I feel it is important. I could really use some more ideas to grow this concept, so if you want to work with me on this, feel free to message me.

`CODING:` For any bugs you find, please open an issue on the github repository.

Art credits go to ![elten](https://elthen.itch.io/).
