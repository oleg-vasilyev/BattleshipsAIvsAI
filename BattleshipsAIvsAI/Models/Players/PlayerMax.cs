using System;
using System.Collections.Generic;
using System.Linq;
using BattleshipsAIvsAI.Models;

namespace BattleshipsAIvsAI.Players
{
    [Serializable]
    class PlayerMax : IPlayer
    {
        enum Direction { Not, Up, Down, Left, Right }

        private Random _random;
        private Field _playerField;
        private Queue<Turn> _attackingTurnes;
        private int _fieldLength;
        private bool _isShipHit = false;
        private Turn _lastTurn;
        private Turn _hitTurn;
        private Direction _direction = Direction.Not;
        private int _hitCellsCount = 0;

        public PlayerMax()
        {
            _random = new Random(Guid.NewGuid().ToString().ToCharArray().Where(c => "1234567890".Contains(c)).Select(c => int.Parse(c.ToString())).Sum());
        }
        public string Name()
        {
            return "Max";
        }
        private Queue<Turn> GenerateAttackingTurns()
        {
            Queue<Turn> _turnes = new Queue<Turn>();
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(1, 4))
            {
                _turnes.Enqueue(tmpTurn);
            }
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(7, _fieldLength))
            {
                _turnes.Enqueue(tmpTurn);
            }
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(3, _fieldLength))
            {
                _turnes.Enqueue(tmpTurn);
            }
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(1, 8))
            {
                _turnes.Enqueue(tmpTurn);
            }
            //
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(1, 2))
            {
                _turnes.Enqueue(tmpTurn);
            }
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(1, 6))
            {
                _turnes.Enqueue(tmpTurn);
            }
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(5, _fieldLength))
            {
                _turnes.Enqueue(tmpTurn);
            }
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(9, _fieldLength))
            {
                _turnes.Enqueue(tmpTurn);
            }
            foreach (Turn tmpTurn in GenerateDiagonalTurnes(1, _fieldLength))
            {
                _turnes.Enqueue(tmpTurn);
            }
            return _turnes;
        }
        private List<Turn> GenerateDiagonalTurnes(int coordinateX, int coordinateY)
        {
            List<Turn> _turnes = new List<Turn>();
            int y = coordinateY - 1;
            for (int x = coordinateX - 1; x <= _fieldLength - 1; x++)
            {
                if (x < _fieldLength && y < _fieldLength && x >= 0 && y >= 0)
                {
                    _turnes.Add(new Turn(x, y));
                    y--;
                }
                else
                {
                    break;
                }
            }
            return _turnes;
        }
        private Turn GenerateRandomTurn()
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
        private bool IsCellValid(int coordinateX, int coordinateY)
        {
            Turn turn = new Turn(coordinateX, coordinateY);
            if (turn.CoordinateX < 0 || turn.CoordinateY < 0 || turn.CoordinateX > _fieldLength || turn.CoordinateY > _fieldLength)
            {
                return false;
            }
            if (_playerField.GetCellState(turn) == CellCondition.Miss)
            {
                return false;
            }
            return true;
        }

        private Turn GenerateTurnAfterFirstHit(Turn turn)
        {
            Turn tmpTurn = new Turn(-1, -1);

            if (turn == _hitTurn)
            {
                if (IsCellValid(turn.CoordinateX, turn.CoordinateY + 1))
                {
                    _direction = Direction.Up;
                    tmpTurn = new Turn(turn.CoordinateX, turn.CoordinateY + 1);
                }
                else if (IsCellValid(turn.CoordinateX + 1, turn.CoordinateY))
                {
                    _direction = Direction.Right;
                    tmpTurn = new Turn(turn.CoordinateX + 1, turn.CoordinateY);
                }
                else if (IsCellValid(turn.CoordinateX, turn.CoordinateY - 1))
                {
                    _direction = Direction.Down;
                    tmpTurn = new Turn(turn.CoordinateX, turn.CoordinateY - 1);
                }
                else if (IsCellValid(turn.CoordinateX - 1, turn.CoordinateY))
                {
                    _direction = Direction.Left;
                    tmpTurn = new Turn(turn.CoordinateX - 1, turn.CoordinateY);
                }
            }
            else
            {
                tmpTurn = new Turn(0, 0);
            }
            return tmpTurn;
        }
        private Turn GenerateTurnAfterMiss(Turn turn)
        {
            Turn tmpTurn = new Turn(-1, -1);

            if (turn != _hitTurn)
            {
                if (_hitCellsCount == 1)
                {
                    switch (_direction)
                    {
                        case Direction.Up:
                            if (IsCellValid(_hitTurn.CoordinateX + 1, _hitTurn.CoordinateY))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX + 1, _hitTurn.CoordinateY);
                                _direction = Direction.Right;
                            }
                            else if (IsCellValid(_hitTurn.CoordinateX, _hitTurn.CoordinateY - 1))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX, _hitTurn.CoordinateY - 1);
                                _direction = Direction.Down;
                            }
                            break;
                        case Direction.Down:
                            if (IsCellValid(_hitTurn.CoordinateX - 1, _hitTurn.CoordinateY))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX - 1, _hitTurn.CoordinateY);
                                _direction = Direction.Left;
                            }
                            else if (IsCellValid(_hitTurn.CoordinateX, _hitTurn.CoordinateY + 1))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX, _hitTurn.CoordinateY + 1);
                                _direction = Direction.Up;
                            }
                            break;
                        case Direction.Left:
                            if (IsCellValid(_hitTurn.CoordinateX, _hitTurn.CoordinateY + 1))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX, _hitTurn.CoordinateY + 1);
                                _direction = Direction.Up;
                            }
                            else if (IsCellValid(_hitTurn.CoordinateX + 1, _hitTurn.CoordinateY))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX + 1, _hitTurn.CoordinateY);
                                _direction = Direction.Right;
                            }
                            break;
                        case Direction.Right:
                            if (IsCellValid(_hitTurn.CoordinateX, _hitTurn.CoordinateY - 1))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX, _hitTurn.CoordinateY - 1);
                                _direction = Direction.Down;
                            }
                            else if (IsCellValid(_hitTurn.CoordinateX - 1, _hitTurn.CoordinateY))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX - 1, _hitTurn.CoordinateY);
                                _direction = Direction.Left;
                            }
                            break;
                    }
                    return tmpTurn;
                }
                else if (_hitCellsCount > 1)
                {
                    switch (_direction)
                    {
                        case Direction.Up:
                            if (IsCellValid(_hitTurn.CoordinateX, _hitTurn.CoordinateY + 1))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX, _hitTurn.CoordinateY + 1);
                                _direction = Direction.Down;
                            }
                            break;
                        case Direction.Down:
                            if (IsCellValid(_hitTurn.CoordinateX, _hitTurn.CoordinateY - 1))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX, _hitTurn.CoordinateY - 1);
                                _direction = Direction.Up;
                            }
                            break;
                        case Direction.Left:
                            if (IsCellValid(_hitTurn.CoordinateX - 1, _hitTurn.CoordinateY))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX - 1, _hitTurn.CoordinateY);
                                _direction = Direction.Right;
                            }
                            break;
                        case Direction.Right:
                            if (IsCellValid(_hitTurn.CoordinateX + 1, _hitTurn.CoordinateY))
                            {
                                tmpTurn = new Turn(_hitTurn.CoordinateX + 1, _hitTurn.CoordinateY);
                                _direction = Direction.Left;
                            }
                            break;
                    }
                    return tmpTurn;
                }
            }
            return tmpTurn;
        }
        private Turn GenetateTurnAfterSecondHit(Turn turn)
        {
            Turn tmpTurn = new Turn(-1, -1);

            switch (_direction)
            {
                case Direction.Up:
                    if (IsCellValid(turn.CoordinateX, turn.CoordinateY + 1))
                    {
                        tmpTurn = new Turn(turn.CoordinateX, turn.CoordinateY + 1);
                    }
                    else
                    {
                        tmpTurn = new Turn(_hitTurn.CoordinateX, _hitTurn.CoordinateY - 1);
                        _direction = Direction.Down;
                    }
                    break;
                case Direction.Down:
                    if (IsCellValid(turn.CoordinateX, turn.CoordinateY - 1))
                    {
                        tmpTurn = new Turn(turn.CoordinateX, turn.CoordinateY - 1);
                    }
                    else
                    {
                        tmpTurn = new Turn(_hitTurn.CoordinateX, _hitTurn.CoordinateY + 1);
                        _direction = Direction.Up;
                    }
                    break;
                case Direction.Left:
                    if (IsCellValid(turn.CoordinateX - 1, turn.CoordinateY))
                    {
                        tmpTurn = new Turn(turn.CoordinateX - 1, turn.CoordinateY);
                    }
                    else
                    {
                        tmpTurn = new Turn(_hitTurn.CoordinateX + 1, _hitTurn.CoordinateY);
                        _direction = Direction.Right;
                    }
                    break;
                case Direction.Right:
                    if (IsCellValid(turn.CoordinateX + 1, turn.CoordinateY))
                    {
                        tmpTurn = new Turn(turn.CoordinateX + 1, turn.CoordinateY);
                    }
                    else
                    {
                        tmpTurn = new Turn(_hitTurn.CoordinateX - 1, _hitTurn.CoordinateY);
                        _direction = Direction.Left;
                    }
                    break;
            }
            return tmpTurn;
        }


        public Turn MakeTurn(Field playerField)
        {
            _playerField = playerField;

            if (_lastTurn == null)
            {
                _lastTurn = _attackingTurnes.Dequeue();
                return _lastTurn;
            }
            else
            {
                if (!_isShipHit)
                {
                    if (playerField.GetCellState(_lastTurn) == CellCondition.Hit)
                    {
                        _isShipHit = true;
                        _hitCellsCount++;
                        _hitTurn = _lastTurn;
                        _lastTurn = GenerateTurnAfterFirstHit(_lastTurn);
                    }
                    else
                    {
                        if (_attackingTurnes.Count != 0)
                        {
                            for (int i = 0; i < _attackingTurnes.Count; i++)
                            {
                                if (playerField.GetCellState(_attackingTurnes.Peek()) != CellCondition.Unknown)
                                {
                                    _attackingTurnes.Dequeue();
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (_attackingTurnes.Count != 0)
                        {
                            _lastTurn = _attackingTurnes.Dequeue();
                        }
                        else
                        {
                            _lastTurn = GenerateRandomTurn();
                        }
                    }
                    return _lastTurn;
                }
                else
                {
                    if (playerField.GetCellState(_lastTurn) == CellCondition.Miss)
                    {
                        _lastTurn = GenerateTurnAfterMiss(_lastTurn);
                    }
                    else if (playerField.GetCellState(_lastTurn) == CellCondition.Sinked)
                    {
                        _isShipHit = false;
                        _hitCellsCount = 0;
                        _hitTurn = null;
                        _direction = Direction.Not;
                        _lastTurn = GenerateRandomTurn();
                    }
                    else
                    {
                        _lastTurn = GenetateTurnAfterSecondHit(_lastTurn);
                    }
                    return _lastTurn;
                }
            }
        }
        public List<Ship> GenerateShips(int fieldLength)
        {
            _fieldLength = fieldLength;
            _attackingTurnes = GenerateAttackingTurns();

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
    }
}
