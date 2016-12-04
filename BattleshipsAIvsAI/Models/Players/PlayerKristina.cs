using System;
using System.Collections.Generic;
using BattleshipsAIvsAI.Models;

namespace BattleshipsAIvsAI.Players
{
    [Serializable]
    class PlayerKristina : IPlayer
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
            return GetPossibleTurn();
        }

        private Turn GetPossibleTurn()
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
                    if (_playerField.GetCellState(new Turn(i, j)) == CellCondition.Hit)
                    {
                        return GetCleverTurn(i, j);
                    }
                }
            }

            int randTurnIndex = _random.Next(0, turnList.Count);
            return turnList[randTurnIndex];
        }

        private Turn GetCleverTurn(int i, int j)
        {
            int fieldLength = _playerField.GetFieldLength();
            List<Turn> turnList = new List<Turn>();
            if (((i - 1) > 0) && (_playerField.GetCellState(new Turn((i - 1), j)) == CellCondition.Unknown))
            {
                if (((j - 1) > 0) && (_playerField.GetCellState(new Turn(i, (j - 1))) == CellCondition.Unknown))
                {
                    turnList.Add(new Turn((i - 1), j));
                    turnList.Add(new Turn(i, (j - 1)));
                }
                else
                {
                    turnList.Add(new Turn((i - 1), j));
                }
            }
            else
            {
                if (((j - 1) > 0) && (_playerField.GetCellState(new Turn(i, (j - 1))) == CellCondition.Unknown))
                {
                    turnList.Add(new Turn(i, (j - 1)));
                }
            }
            //----------------------------------------------------------------
            if (((i + 1) < fieldLength) && (_playerField.GetCellState(new Turn((i + 1), j)) == CellCondition.Unknown))
            {
                if (((j + 1) < fieldLength) && (_playerField.GetCellState(new Turn(i, (j + 1))) == CellCondition.Unknown))
                {
                    turnList.Add(new Turn((i + 1), j));
                    turnList.Add(new Turn(i, (j + 1)));
                }
                else
                {
                    turnList.Add(new Turn((i + 1), j));
                }
            }
            else
            {
                if (((j + 1) < fieldLength) && (_playerField.GetCellState(new Turn(i, (j + 1))) == CellCondition.Unknown))
                {
                    turnList.Add(new Turn(i, (j + 1)));
                }
            }

            int randTurnIndex = _random.Next(0, turnList.Count);
            return turnList[randTurnIndex];

        }

        public string Name()
        {
            return "Kristina Gromuko";
        }
    }
}
