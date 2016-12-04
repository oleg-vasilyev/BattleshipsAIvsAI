using System;
using System.Collections.Generic;

namespace BattleshipsAIvsAI.Models
{
    class PlayerShips
    {
        public IList<Ship> Ships { get; private set; }

        public PlayerShips(IList<Ship> ships)
        {
            ValidateShipsAndThrowExeption(ships);
            Ships = ships;
        }

        private void ValidateShipsAndThrowExeption(IList<Ship> ships)
        {
            int singleDecksCount = 0;
            int doubleDecksCount = 0;
            int trippleDecksCount = 0;
            int quadDecksCount = 0;

            if (ships == null) { throw new ArgumentNullException("shipList"); }

            if (ships.Count != 10) { throw new InvalidateShipsExeption("Incorrect number of ships", ships); }

            foreach (Ship ship in ships)
            {
                if (!ship.ValidateNumberOfDecks())
                {
                    throw new InvalidateShipsExeption("One of ships has invalid number of decks", ships, ship);
                }

                switch (ship.ShipType)
                {
                    case ShipType.SingleDeck:
                        singleDecksCount++;
                        if (singleDecksCount > 4) { throw new InvalidateShipsExeption("More than four SingleDecks", ships); }
                        break;
                    case ShipType.DoubleDeck:
                        doubleDecksCount++;
                        if (doubleDecksCount > 3) { throw new InvalidateShipsExeption("More than three DoubleDecks", ships); }
                        break;
                    case ShipType.TrippleDeck:
                        trippleDecksCount++;
                        if (trippleDecksCount > 2) { throw new InvalidateShipsExeption("More than two TrippleDecks", ships); }
                        break;
                    case ShipType.QuadDeck:
                        quadDecksCount++;
                        if (quadDecksCount > 1) { throw new InvalidateShipsExeption("More than one QuadDeck", ships); }
                        break;
                }

                // Проверка: координаты не за пределами поля, начальная координата по одной оси не больше конечной.
                if (ship.FirstCoordinateX < 0 ||
                    ship.FirstCoordinateY < 0 ||
                    ship.SecondCoordinateX > 9 ||
                    ship.SecondCoordinateY > 9 ||
                    ship.FirstCoordinateX > ship.SecondCoordinateX ||
                    ship.FirstCoordinateY > ship.SecondCoordinateY)
                {
                    throw new InvalidateShipsExeption("One of ships is out the field", ships, ship);
                }

                // Проверка: не соприкасаются ли корабли.
                if (ship.IsNextToAnotherShips(ships)) { throw new InvalidateShipsExeption("One of ships is crossed another", ships, ship); }
            }
        }
    }
}
