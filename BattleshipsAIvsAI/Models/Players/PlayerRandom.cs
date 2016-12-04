using System;
using System.Collections.Generic;
using BattleshipsAIvsAI.Models;

namespace BattleshipsAIvsAI.Players
{
    [Serializable]
    class PlayerRandom : IPlayer
    {
        private Random _random = new Random();
        private Field _playerField;

        public List<Ship> GenerateShips(int fieldLength)
        {
            List<Ship> _shipList = new List<Ship>();
            _shipList.Add(new Ship(ShipType.QuadDeck, 0, fieldLength - 1, 3, fieldLength - 1));
            _shipList.Add(new Ship(ShipType.TrippleDeck, 0, fieldLength - 3, 2, fieldLength - 3));
            _shipList.Add(new Ship(ShipType.TrippleDeck, 4, fieldLength - 3, 6, fieldLength - 3));
            _shipList.Add(new Ship(ShipType.DoubleDeck, 5, fieldLength - 1, 6, fieldLength - 1));
            _shipList.Add(new Ship(ShipType.DoubleDeck, 8, fieldLength - 1, 9, fieldLength - 1));
            _shipList.Add(new Ship(ShipType.DoubleDeck, 8, fieldLength - 3, 9, fieldLength - 3));
            List<Ship> _singleDecksList = new List<Ship>();
            while (_singleDecksList.Count < 4)
            {
                int _x = _random.Next(fieldLength - 1);
                int _y = _random.Next(fieldLength - 5);
                Ship _ship = new Ship(ShipType.SingleDeck, _x, _y, _x, _y);
                if (!_ship.IsNextToAnotherShips(_singleDecksList))
                {
                    _singleDecksList.Add(_ship);
                }
            }
            _shipList.AddRange(_singleDecksList);
            return _shipList;
        }

        public Turn MakeTurn(Field playerField)
        {
            _playerField = playerField;
            return GetPossibleRandomTurn();
        }

        private Turn GetPossibleRandomTurn()
        {
            List<Turn> turnList = new List<Turn>();
            int fieldLength = _playerField.GetFieldLength();
            for (int i = 0; i < fieldLength; i++)
            {
                for (int j = 0; j < fieldLength; j++)
                {
                    if (_playerField.GetCellState(new Turn(i, j)) == CellCondition.Unknown)
                    {
                        turnList.Add(new Turn(i, j));
                    }
                }
            }

            int randTurnIndex = _random.Next(0, turnList.Count);
            return turnList[randTurnIndex];
        }

        public string Name()
        {
            return "Mr. Random";
        }
    }
}

