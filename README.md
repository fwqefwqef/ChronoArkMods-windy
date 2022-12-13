# Chrono Ark Mods

A list of various mods for Chrono Ark. Requires BepInEx.

The .dll files are the mod files. I included .cs source code for people who want to look at it. 

Check here for how to use mods: https://github.com/Neoshrimp/ChronoArk-gameplay-plugins

---
# [Expert+ Mod]

Full Documentation:

https://docs.google.com/document/d/1v1eJqPntX5T-lnDV4tWOFE66_IVwi6GWcdjPKxF9kL0/edit?usp=sharing 

Special Thanks: Neoshrimp

# [505Error Skill Randomizer]

This is Chrono Ark but 505Error taken to the extreme.

Level Up: The effects of 505Error are activated by default. However, the first 3 skills are gone meaning you choose between 3 random skills from all investigators.

Skill Book, Infinite Skill Book: Scrambled to show random Investigator skills. 

Golden Skill Book: Shows you random Investigator Rare Skills.

Extra settings

Chaos Mode: Include every existing skill in the game (such as enemy skills and item effects) into the selection pool. Gets silly quickly. (Default OFF)

Healing 101: Gives 2 Healing 101 at the start of the game to help support characters pick up heal skills. (Default OFF) 

(Go to BepInEx -> config -> open in Notepad: org.windy.chronoark.cardmod.randomskillmod.cfg)

# [Boss Rush Mode]

Despawn all regular encounters and give their rewards at the start of the stage. Suited for quick playthroughs.

Extra Settings

Easy Crimson Wilderness: Receive ??? for free in Bloody Park 1. Receive 2 less Soulstones, remove starting gold, and remove guaranteed shop key in Misty Garden 2 and Bloody Park 1. (Default OFF)

(Go to BepInEx -> config -> open in Notepad: windy.bossrush.cfg)

# [Fox Orb Genderlock Removal]

Fox Orb can be used on male characters.

# [Helia Selena Split]

Make Helia and Selena into seperate characters. They can be selected seperately in the starting recruit screen, and they can be selected in Lone Wolf challenge. They do not appear in campfire (yet). 

# [Minimum Skill Num]

Change the number of minimum skills a character can have on their deck.

Default 1. To change the number,

Go to BepInEx -> config -> open in Notepad: windy.minskill.cfg


# [Parry Skill Book]

You can obtain parrying attack skillbook from Lian even after unlocking her. It costs 0 credits. 

(Optional) The game text will say that it still requires 8 credits. To fix this: Chrono Ark > ChronoArk_Data > StreamingAssets > LangDialogueDB.csv: Ctrl+F "8 credits", delete the parenthesis

# [Recruit Select Num]

Change the number of party members available in campfire recruit. 

Default 8 characters. To change the number,

Go to BepInEx -> config -> open in Notepad: org.windy.chronoark.recruitmod.recruitselectnum

# [Remove Fixed Skill Limit]

Remove 'Cannot be Fixed' keyword from all skills. 

# [windy's mod]

Miscellaneous gameplay changes that I personally like.

Dark Sun, Dark Moon, Storming Blade: Gain Tracking

Time to Move!: Gain Swiftness

Fox Orb: Can be used on male characters

Helia & Selena: Can be selected seperately on recruit screen

Parry Skill Book: Can be obtained from Lian after unlocking her

Game Start: Game runs at 3x speed until you reach first battle
