# SevensCards

## About the project

---
This is a VB.NET Windows Form App card game that implements a variation on the popular card game 'Sevens' (<https://en.wikipedia.org/wiki/Domino_(card_game)#Sevens>).

The game supports playing locally against other people or computers, or web play with additional bots.

The online repository can be found at <https://github.com/James-Wickenden/SevensCards>

## Development Tracking

---

TODO:

- Implement networking (WIP)
- Implement difficulty slider for COM players in local play
- Clean up GameModel & GameView interfaces
- Native UI scaling (!!)

- Networking:
  - Case for a client leaving and replacing with a COM
  - Stop server joining its own game
  - Case for already having a server running
  - Case for joining a game in session

BUGS:

- No way to change local difficulty
- Changing server difficulty currently doesn't do anything
