using System;
using System.Collections.Generic;
using System.Linq;
using BattleshipsAIvsAI.Models;

namespace BattleshipsAIvsAI.Players
{
    [Serializable]
    class PlayerKuril : IPlayer
    {
        private Random _random = new Random(Guid.NewGuid().ToString().ToCharArray().Where(c => "1234567890".Contains(c)).Select(c => int.Parse(c.ToString())).Sum());
        private List<List<Turn>> _exploringStrategies = new List<List<Turn>>();
        private List<List<Turn>> _attackingStrategies = new List<List<Turn>>();
        private int _currentExploringStrategyIndex;
        private int _currentAttackingStrategyIndex;
        private Turn _lastTurn = new Turn(-1, -1);
        private int _hitCounter;
        private int _fourDecksCount;
        private int _threeDecksCount;
        private int _twoDecksCount;
        private bool _ifEnemyShipIsFound;
        private int _fieldLength;
        private int _turnsPassed;

        public string Name()
        {
            return "Andrey";
        }

        public PlayerKuril()
        {
            _random = new Random(Guid.NewGuid().ToString().ToCharArray().Where(c => "1234567890".Contains(c)).Select(c => int.Parse(c.ToString())).Sum());
            _exploringStrategies = new List<List<Turn>>();
            _attackingStrategies = new List<List<Turn>>();
            _currentExploringStrategyIndex = 0;
            _currentAttackingStrategyIndex = 0;
            _lastTurn = new Turn(-1, -1);
        }
        private void BuildStrategies()
        {
            _exploringStrategies.Clear();
            _exploringStrategies.Add(GenerateDiagonalStrategy(4));
            _exploringStrategies.Add(GenerateDiagonalStrategy(2));
            _currentExploringStrategyIndex = 0;
        }

        private List<Turn> GenerateDiagonalStrategy(int step)
        {
            List<Turn> _strategy = new List<Turn>();

            for (int y = 0; y < _fieldLength; y++)
            {
                int x = 0;
                if (y < step) { x = step - y - 1; }
                else { x = step - (y % step) - 1; }
                while (x < _fieldLength)
                {
                    _strategy.Add(new Turn(x, y));
                    x += step;
                }
            }
            return _strategy;
        }
        private List<Turn> GenerateFinishingStrategy(Field playerField)
        {
            List<Turn> _strategy = new List<Turn>();
            Turn _tmpTurn = new Turn(0, 0);
            for (int x = 0; x < _fieldLength; x++)
            {
                for (int y = 0; y < _fieldLength; y++)
                {
                    _tmpTurn = new Turn(x, y);
                    if (playerField.GetCellState(_tmpTurn) == CellCondition.Unknown) { _strategy.Add(_tmpTurn); }
                }
            }
            return _strategy;
        }
        private void GenerateAttackingStrategies(int startingPointX, int startingPointY)
        {
            _attackingStrategies.Clear();
            _attackingStrategies.Add(new List<Turn>());
            _attackingStrategies[0].Add(new Turn(startingPointX + 1, startingPointY));
            _attackingStrategies[0].Add(new Turn(startingPointX + 2, startingPointY));
            _attackingStrategies[0].Add(new Turn(startingPointX + 3, startingPointY));
            _attackingStrategies.Add(new List<Turn>());
            _attackingStrategies[1].Add(new Turn(startingPointX - 1, startingPointY));
            _attackingStrategies[1].Add(new Turn(startingPointX - 2, startingPointY));
            _attackingStrategies[1].Add(new Turn(startingPointX - 3, startingPointY));
            _attackingStrategies.Add(new List<Turn>());
            _attackingStrategies[2].Add(new Turn(startingPointX, startingPointY + 1));
            _attackingStrategies[2].Add(new Turn(startingPointX, startingPointY + 2));
            _attackingStrategies[2].Add(new Turn(startingPointX, startingPointY + 3));
            _attackingStrategies.Add(new List<Turn>());
            _attackingStrategies[3].Add(new Turn(startingPointX, startingPointY - 1));
            _attackingStrategies[3].Add(new Turn(startingPointX, startingPointY - 2));
            _attackingStrategies[3].Add(new Turn(startingPointX, startingPointY - 3));

        }
        public List<Ship> GenerateShips(int fieldLength)
        {
            _fourDecksCount = 0;
            _threeDecksCount = 0;
            _twoDecksCount = 0;
            _ifEnemyShipIsFound = false;
            _fieldLength = fieldLength;
            BuildStrategies();

            List<Ship> _shipList = new List<Ship>();
            _shipList.Add(new Ship(ShipType.QuadDeck, 0, _fieldLength - 1, 3, _fieldLength - 1));
            _shipList.Add(new Ship(ShipType.TrippleDeck, 0, _fieldLength - 3, 2, _fieldLength - 3));
            _shipList.Add(new Ship(ShipType.TrippleDeck, 4, _fieldLength - 3, 6, _fieldLength - 3));
            _shipList.Add(new Ship(ShipType.DoubleDeck, 5, _fieldLength - 1, 6, _fieldLength - 1));
            _shipList.Add(new Ship(ShipType.DoubleDeck, 8, _fieldLength - 1, 9, _fieldLength - 1));
            _shipList.Add(new Ship(ShipType.DoubleDeck, 8, _fieldLength - 3, 9, _fieldLength - 3));


            List<Ship> _singleDecksList = new List<Ship>();

            while (_singleDecksList.Count < 4)
            {

                int _x = _random.Next(_fieldLength - 1);
                int _y = _random.Next(_fieldLength - 5);
                Ship _ship = new Ship(ShipType.SingleDeck, _x, _y, _x, _y);

                if (!_ship.IsNextToAnotherShips(_singleDecksList))
                {
                    _singleDecksList.Add(_ship);
                }
                foreach (Ship _tmpShip in _singleDecksList)
                {
                    if (_tmpShip.FirstCoordinateX == _x || _tmpShip.FirstCoordinateY == _y)
                    {
                        break;
                    }
                }
            }
            _shipList.AddRange(_singleDecksList);
            return _shipList;
        }

        private Turn MakeExploringTurn(Field playerField)
        {
            Turn _tempTurn = new Turn(0, 0);
            if (_currentExploringStrategyIndex == 0)
            {
                while (_exploringStrategies[_currentExploringStrategyIndex].Count > 0)
                {
                    _tempTurn = _exploringStrategies[_currentExploringStrategyIndex][0];
                    _exploringStrategies[_currentExploringStrategyIndex].RemoveAt(0);
                    if (playerField.GetCellState(_tempTurn) == CellCondition.Unknown)
                    {
                        return _tempTurn;
                    }
                }
            }
            if (_currentExploringStrategyIndex == 1)
            {

                int _maximumValue = 0;
                int _maximumEnemyShipLength = 0;
                if (_threeDecksCount < 2) { _maximumEnemyShipLength = 3; }
                else { _maximumEnemyShipLength = 3; }
                List<Turn> _turnsToBeDeleted = new List<Turn>();
                foreach (Turn turn in _exploringStrategies[_currentExploringStrategyIndex])
                {
                    if (playerField.GetCellState(turn) != CellCondition.Unknown)
                    {
                        _turnsToBeDeleted.Add(turn);
                    }
                    else
                    {
                        int _currentTurnValue = CountPossibleShipsOnCell(playerField, turn.CoordinateX, turn.CoordinateY, _maximumEnemyShipLength);
                        if (_currentTurnValue > _maximumValue)
                        {
                            _maximumValue = _currentTurnValue;
                            _tempTurn = turn;
                        }
                    }
                }

                foreach (Turn turn in _turnsToBeDeleted)
                {
                    _exploringStrategies[_currentExploringStrategyIndex].Remove(turn);
                }

                return _tempTurn;
            }

            int _randomIndex = 0;
            try
            {
                _randomIndex = _random.Next(_exploringStrategies[_currentExploringStrategyIndex].Count - 1);
            }
            catch (ApplicationException)
            {
                Console.Clear();
                for (int i = 0; i < playerField.GetFieldLength(); i++)
                {
                    for (int j = 0; j < playerField.GetFieldLength(); j++)
                    {
                        Console.Write(playerField.GetCellState(new Turn(i, j)).ToString());
                        Console.Write("\t");
                    }
                }
                Console.ReadKey();
            }

            _tempTurn = _exploringStrategies[_currentExploringStrategyIndex][_randomIndex];

            _exploringStrategies[_currentExploringStrategyIndex].RemoveAt(_randomIndex);

            return _tempTurn;

        }

        private int CountPossibleShipsOnCell(Field playerField, int coordX, int coordY, int shipLength)
        {
            int CountHorizontal = 0;
            int CountVertical = 0;
            for (int i = coordX; i < coordX + shipLength - 1; i++)
            {
                if (playerField.GetCellState(new Turn(i, coordY)) != CellCondition.Unknown) { break; }
                else { CountHorizontal++; }
            }
            for (int i = coordX; i > coordX - shipLength + 1; i--)
            {
                if (playerField.GetCellState(new Turn(i, coordY)) != CellCondition.Unknown) { break; }
                else { CountHorizontal++; }
            }
            for (int i = coordY; i < coordY + shipLength - 1; i++)
            {
                if (playerField.GetCellState(new Turn(coordX, i)) != CellCondition.Unknown) { break; }
                else { CountVertical++; }
            }
            for (int i = coordX; i < coordX - shipLength + 1; i--)
            {
                if (playerField.GetCellState(new Turn(coordX, i)) != CellCondition.Unknown) { break; }
                else { CountVertical++; }
            }
            int result = CountHorizontal + CountVertical - (2 * shipLength) + 4;
            return result;
        }

        private Turn MakeAttackingTurn(Field playerField)
        {
            Turn _tempTurn = new Turn(0, 0);
            while (_attackingStrategies[_currentAttackingStrategyIndex].Count > 0)
            {
                _tempTurn = _attackingStrategies[_currentAttackingStrategyIndex][0];
                _attackingStrategies[_currentAttackingStrategyIndex].RemoveAt(0);
                if (playerField.GetCellState(_tempTurn) == CellCondition.Unknown)
                {
                    return _tempTurn;
                }
                else
                {
                    _currentAttackingStrategyIndex++;
                }
            }
            return _tempTurn;
        }

        public Turn MakeTurn(Field playerField)
        {
            _turnsPassed++;
            if (!_ifEnemyShipIsFound)
            {
                if (playerField.GetCellState(_lastTurn) == CellCondition.Hit)
                {
                    _ifEnemyShipIsFound = true;
                    _hitCounter = 1;
                    GenerateAttackingStrategies(_lastTurn.CoordinateX, _lastTurn.CoordinateY);
                    _currentAttackingStrategyIndex = 0;
                    _lastTurn = MakeAttackingTurn(playerField);
                    return _lastTurn;
                }
                _lastTurn = MakeExploringTurn(playerField);
                return _lastTurn;
            }
            else
            {
                if (playerField.GetCellState(_lastTurn) == CellCondition.Sinked)
                {
                    _ifEnemyShipIsFound = false;

                    if (_hitCounter == 1)
                    {
                        _twoDecksCount++;
                        if (_twoDecksCount == 3 && _threeDecksCount == 2 && _fourDecksCount == 1)
                        {
                            _exploringStrategies.Add(GenerateFinishingStrategy(playerField));
                            _currentExploringStrategyIndex = 2;
                        }
                    }
                    if (_hitCounter == 2)
                    {
                        _threeDecksCount++;
                        if (_twoDecksCount == 3 && _threeDecksCount == 2 && _fourDecksCount == 1)
                        {
                            _exploringStrategies.Add(GenerateFinishingStrategy(playerField));
                            _currentExploringStrategyIndex = 2;
                        }
                    }
                    if (_hitCounter == 3)
                    {
                        _fourDecksCount++;
                        if (_twoDecksCount == 3 && _threeDecksCount == 2)
                        {
                            _exploringStrategies.Add(GenerateFinishingStrategy(playerField));
                            _currentExploringStrategyIndex = 2;
                        }
                        else { _currentExploringStrategyIndex = 1; }
                    }
                    _lastTurn = MakeExploringTurn(playerField);
                    return _lastTurn;
                }
                if (playerField.GetCellState(_lastTurn) == CellCondition.Miss)
                {
                    _currentAttackingStrategyIndex++;
                }
                else { _hitCounter++; }
                _lastTurn = MakeAttackingTurn(playerField);
                return _lastTurn;
            }
        }
    }
}
