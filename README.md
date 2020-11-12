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
- Implement difficulty slider for COM players
- Clean up GameModel & GameView interfaces
- Variable player counts

BUGS:

- Freezing while loading/ solve with loading bar?
- Solve with extra card attributes/constructors or case-by-case handling when calling
- WEB Games can freeze if a COM player moves first (?)
- Scale for different screen sizes!

WEB plan:

- Host clicks host, goes to lobby
- Populates lobby with only host; shows host IP at top
- Clients click join in lobby and join using host IP
- Clients send their IP to host, added to lobby
- When happy, host starts game; empty spaces filled with bots of chosen AI level
- Host has list of joined IPs
- Clients send moves to host instead of playing them
- The host receives moves as well as player IP; only accepts if both are valid (valid move and player moves next)
- Whenever host plays anybodys move, broadcasts it to each stored player IP, who also plays the move