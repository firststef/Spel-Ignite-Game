optiunea 1:
https://elthen.itch.io/
cu
https://szadiart.itch.io/2d-magic-lands-set2
optiunea 2:
https://clembod.itch.io/warrior-free-animation-set
cu
https://rvros.itch.io/pixel-monsters-3
cu
https://szadiart.itch.io/magic-pixels-dungeon1

- ce este un limbaj de programare inafara de a scrie chestii pe computer? cum functioneaza un limbaj de programare si ce anume pune el in miscare?
- deci, stiu ca poti invata un limbaj de programre prin exercitii (puzzle-uri practic) dar ce este un exercitiu?
dar SPEL ar fi trebui sa fie un fel de api bridge intre jucator si lume

---------------------------------------------------------------------------------------
A) spells with target awareness: gen momentan skill-urile sunt doar foc, bile de foc etc, dar ai putea sa faci ceva de genul:
daca aplici skill-ul foc pe zona din jurul tau, ai un area effect fire damage
daca aplici skill-ul foc pe sabie, atacul tau devine mai puternic
daca aplici skill-ul foc pe tine insuti, iei foc. daca aplici apa stingi focul
daca aplici skill-ul foc in mod normal, targeted, ai conceptul initial, in care ai un stream de foc

B) skill-uri diverse
un skill care transforma inamicii cu nivel scazut in broaste de exemplu
un skill de summoning

C) sistemul de magie sa fie cuprinzator: 
copacii sa poata lua foc
bolovanii sa nu dispara etc

D) jucatorii sa aiba afinitati diferite:
daca castezi mai mult elemente de foc:
cantitatea de mana consumata de acestea scade,
deblochezi anumite skill-uri avansate de foc etc

Things i have to do this week
1) invatat la CN
2) facut la GD
3) inceput scris licenta
4) proiect la MCE?

luni:
facut la GD

marti:
invatat la CN

miercuri:
invatat la CN/inceput scris licenta

GD:
game core mechanic:
pickup spells, fight monsters

[x]*change flake -> flame

1)skills that the player should learn:
fighting:
orb -> star orb (*required)[x]
    -> lunar orb (*required)
laser (not now)

fire mines (*should be useful)

puzzle:
-water extiquishes fire
-fire burns things
-throwing rocks breaks things
-ice freezes water (*required)[x]

2)monsters: how can i use different spritesheet but same animation[x]
enemies must have skills

slime -> physical[x]
snake -> physical[x]
bandit -> physical + 1 magical[x][x][x][]
leaf elemental -> projectile

decorator -> dictionary of modifiers

3)decompose skills to offer more control
cast -> cast element
throw -> (== cast pentru fireball) 

target is any. 

cast [fire]. => [
    charge [fire] mana.
    release from hand.
]

throw orb of [fire]. [
    create orb with [fire]. => [
        create orb. => [
            create [orb] in [left hand].
        ]
        enchant [orb] with [fire].
    ]
    throw [orb].
]

=======================================================================================
charge [fire] mana. => create [fire] in [soul].
release from hand. => move [fire] to [left hand]. release from [left hand].

create [orb] in [left hand].
charge [fire] mana. => create [fire] in [soul].
enchant [orb] with [fire].
throw [orb]. => release from [left hand].
=======================================================================================

create [fire] in [soul].
move [fire] to [left hand]. 
release from [left hand].

create [orb] in [left hand].
create [fire] in [soul].
enchant [orb] with [fire].
throw [orb]. => release from [left hand].

=======================================================================================

cast fire. => [
    cast [fire]. => [
        chant [fire release thy flames].
        create [fire] => [
            create [fire] in [soul].
            move [fire] to [left hand].
        ]
        release from [left hand].
    ]
]
0. discovers element cast
1. discovers fire, water, earth, ice
2. discovers fire, its chant, soul + left hand

throw orb of [fire]. [
    create orb with [fire]. => [
        chant [light of heaven show me the power of gods].
        create [orb] in [left hand].
        chant [fire release thy flames].
        create [fire] in [soul].
        `// [orb] becomes [enchant [orb] with [fire]].`
        enchant orb with [fire]. => [
            move [enchant orb with [fire]] to [left hand].
        ]
    ]
    release from [left hand].
]
0. discovers orb cast
1. discovers fire, water, earth, ice
2. discovers left hand
3. discovers orb, its chant, assignment, enchant

shield of water. => [
    chant [gods of war i will be your spear so be my shield].
    create [shield] in [soul].
    [shield] becomes [enchant [shield] with [fire]].
    move [shield] to [area]. 
    release from [area].
]
1. discovers shield cast
2. discovers shield, its chant, area

throw rock. => [
    move [rock] to [left hand].
    release from [left hand].
]
1. discovers object throw.
2. discovers any object.

dash => [

]

heal [soul].

```c
///////////////////////////////////////////////////////////////////////
```

1. battle content
2. skill tree system
3. story
4. interactions

- enemy that shoots projectile[x]
- thugs map[x]
- dialogue system [x]
- witch -> fatal transform [x]
[x] frog
[x] inherit ranged & write transform
[x] add transformed effect
- fix combat [x]
witch teaches you that you should approach enemies with different abilities
shield coward should  defend against flows
bandit leader is strong
! fire fist vs coward
? parry/
? dash
- shop[x]
- decomposing skills[X]
- interpreter implementations for all this[x]

- presentation[X]

- final boss
- add on death change first level

- puzzle ghicitoare
- unity loading UI  [x]
- sesam deschide-te
- vent boulders out of the way

- [x] remove unnecessary mobs on map 1
- [x] add throw buy
- [x] reorder map + add another shop
- [x] add npc "weaknesses"
- [x] add npc "cancelling"
- [x] tweak player health
- [ ] make snakes faster, or not stop so soon
- [x] make arrows less power
- [x] fire orb doesnt work
- [x] waterfall doesnt stop
- [x] god mode
- [ ] bundle it
- [ ] fireball destroys on sight
