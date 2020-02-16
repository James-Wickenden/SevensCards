# SevensCards

## About the project

---
This is a VB.NET Windows Form App card game that implements a variation on the popular card game 'Sevens' (<https://en.wikipedia.org/wiki/Domino_(card_game)#Sevens>).

The game supports playing locally against other people or computers, or web play with additional bots. (Currently WIP!)

The online repository can be found at <https://github.com/James-Wickenden/SevensCards>

## Development Tracking

---

TODO:

- Implement networking
- Game loading bar
- Implement console/log for tracking/ online debugging
- Implement difficulty slider for COM players
- Clean up GameModel & GameView interfaces

BUGS:

- Freezing while loading/ solve with loading bar?
- GetCardText on a Nothing card results in crashing
- Solve with extra card attributes/constructors or case-by-case handling when calling
- WEB Games can freeze if a COM player moves first (?)
