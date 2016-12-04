using System;
using System.Collections.Generic;
using System.Linq;
using BattleshipsAIvsAI.Models;

namespace BattleshipsAIvsAI.Players
{
    [Serializable]
    class PlayerOleg : IPlayer
    {
        private enum TurnWay { ImageUnknown, Left, Right, Up, Down }

        private Random _random;
        private Field _playerField;
        private Turn _previousTurn;
        private Turn _hitTurn;
        private TurnWay _turnWay;

        public PlayerOleg()
        {
            _random = new Random(Guid.NewGuid().ToString().ToCharArray().Where(c => "1234567890".Contains(c)).Select(c => int.Parse(c.ToString())).Sum());
        }

        public string Name() => "Oleg";

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
            if (playerField == null) { throw new ArgumentNullException("playerField"); }

            _playerField = playerField;

            if (_previousTurn != null)
            {
                if (_playerField.GetCellState(_previousTurn) == CellCondition.Hit)
                {
                    _hitTurn = _previousTurn;
                }
                if (_hitTurn != null)
                {
                    if (_playerField.GetCellState(_hitTurn) != CellCondition.Sinked)
                    {
                        _previousTurn = GetTurnNearHitTurn();
                        return _previousTurn;
                    }
                }
            }

            _previousTurn = GetPossibleRandomTurn();
            return _previousTurn;
        }

        private Turn GetPossibleRandomTurn()
        {
            _turnWay = TurnWay.ImageUnknown;
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

        private Turn GetTurnNearHitTurn()
        {
            List<object[]> turnDictionary = new List<object[]>(4);
            Turn tmpTurn;

            int coordinateX = _hitTurn.CoordinateX;
            int coordinateY = _hitTurn.CoordinateY;

            #region Проверка доступных направлений атаки
            bool left, right, up, down;

            up = false;
            down = false;
            left = false;
            right = false;

            tmpTurn = new Turn(coordinateX - 1, coordinateY);
            up = IsCellUnknow(tmpTurn);
            if (up) { turnDictionary.Add(new object[] { TurnWay.Up, tmpTurn }); }

            tmpTurn = new Turn(coordinateX + 1, coordinateY);
            down = IsCellUnknow(tmpTurn);
            if (down) { turnDictionary.Add(new object[] { TurnWay.Down, tmpTurn }); }

            tmpTurn = new Turn(coordinateX, coordinateY - 1);
            right = IsCellUnknow(tmpTurn);
            if (right) { turnDictionary.Add(new object[] { TurnWay.Right, tmpTurn }); }

            tmpTurn = new Turn(coordinateX, coordinateY + 1);
            left = IsCellUnknow(tmpTurn);
            if (left) { turnDictionary.Add(new object[] { TurnWay.Left, tmpTurn }); }
            #endregion

            #region Смена направления при невозможности атаки в выбранном
            if (_turnWay != TurnWay.ImageUnknown)
            {
                if (_turnWay == TurnWay.Down && down == false)
                {
                    for (int i = coordinateX; i >= 0; i--)
                    {
                        tmpTurn = new Turn(i, coordinateY);
                        if (IsCellHit(new Turn(i + 1, coordinateY)))
                        {
                            if (IsCellUnknow(tmpTurn)) { _turnWay = TurnWay.Up; return tmpTurn; }
                        }
                        else { _turnWay = TurnWay.ImageUnknown; }
                    }
                }

                if (_turnWay == TurnWay.Up && up == false)
                {
                    for (int i = coordinateX; i < _playerField.GetFieldLength(); i++)
                    {
                        tmpTurn = new Turn(i, coordinateY);
                        if (IsCellHit(new Turn(i - 1, coordinateY)))
                        {
                            if (IsCellUnknow(tmpTurn)) { _turnWay = TurnWay.Down; return tmpTurn; }
                        }
                        else { _turnWay = TurnWay.ImageUnknown; }
                    }
                }

                if (_turnWay == TurnWay.Left && left == false)
                {
                    for (int i = coordinateY; i >= 0; i--)
                    {
                        tmpTurn = new Turn(coordinateX, i);
                        if (IsCellHit(new Turn(coordinateX, i + 1)))
                        {
                            if (IsCellUnknow(tmpTurn)) { _turnWay = TurnWay.Right; return tmpTurn; }
                        }
                        else { _turnWay = TurnWay.ImageUnknown; }
                    }
                }

                if (_turnWay == TurnWay.Right && right == false)
                {
                    for (int i = coordinateY; i < _playerField.GetFieldLength(); i++)
                    {
                        tmpTurn = new Turn(coordinateX, i);
                        if (IsCellHit(new Turn(coordinateX, i - 1)))
                        {
                            if (IsCellUnknow(tmpTurn)) { _turnWay = TurnWay.Left; return tmpTurn; }
                        }
                        else { _turnWay = TurnWay.ImageUnknown; }
                    }
                }
            }
            #endregion

            #region Выбор направления атаки
            if (left && (IsCellHit(new Turn(coordinateX, coordinateY - 1)) || (!up && !down && !right)))
            {
                _turnWay = TurnWay.Left;
                return new Turn(coordinateX, coordinateY + 1);
            }

            else if (right && (IsCellHit(new Turn(coordinateX, coordinateY + 1)) || (!up && !down && !left)))
            {
                _turnWay = TurnWay.Right;
                return new Turn(coordinateX, coordinateY - 1);
            }

            else if (up && (IsCellHit(new Turn(coordinateX + 1, coordinateY)) || (!left && !down && !right)))
            {
                _turnWay = TurnWay.Up;
                return new Turn(coordinateX - 1, coordinateY);
            }

            else if (down && (IsCellHit(new Turn(coordinateX - 1, coordinateY)) || (!up && !left && !right)))
            {
                _turnWay = TurnWay.Down;
                return new Turn(coordinateX + 1, coordinateY);
            }

            else
            {
                if (turnDictionary.Count == 0) { return GetPossibleRandomTurn(); }

                int randTurnIndex = _random.Next(0, turnDictionary.Count);
                _turnWay = (TurnWay)turnDictionary[randTurnIndex][0];
                return (Turn)turnDictionary[randTurnIndex][1];
            }
            #endregion
        }

        private bool IsCellUnknow(Turn turn)
        {
            if (turn.CoordinateX < 0 || turn.CoordinateX >= _playerField.GetFieldLength() || turn.CoordinateY < 0 || turn.CoordinateY >= _playerField.GetFieldLength())
            {
                return false;
            }
            if (_playerField.GetCellState(turn) == CellCondition.Unknown) { return true; }
            return false;
        }

        private bool IsCellHit(Turn turn)
        {
            if (turn.CoordinateX < 0 || turn.CoordinateX >= _playerField.GetFieldLength() || turn.CoordinateY < 0 || turn.CoordinateY >= _playerField.GetFieldLength())
            {
                return false;
            }
            if (_playerField.GetCellState(turn) == CellCondition.Hit) { return true; }
            return false;
        }
    }
}
