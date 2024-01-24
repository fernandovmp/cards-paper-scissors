# Cards Paper Scissors

A rock paper scissors game where your throw is limited by a hand of cards.
You can add three cards to a deck with other four random generated cards and other three cards from your opponent.
Two of the random generated cards will be revealed to both players.
Your hand will be formed from three cards out of the ten total cards in the deck.

> README file in docs/ is write in Portuguese

## Setup

Require:
    - git lfs: https://git-lfs.com/
    - dotnet 8: https://dotnet.microsoft.com/pt-br/
    - godot 4.2.1 (mono): https://github.com/godotengine/godot/releases/tag/4.2.1-stable 

Clone:
```
git clone git@github.com:fernandovmp/cards-paper-scissors.git

cd cards-paper-scissors

git lfs pull
```

Build:
```
dotnet build
```

The Godot project is inside `src/CardsPaperScissors.Game` and the solution file is in the root of this repository

