using System;
using System.Collections.Generic;
using System.Linq;
using BattleshipsAIvsAI.Models;

namespace BattleshipsAIvsAI.Players
{
    [Serializable]
    class PlayerMasha : IPlayer
    {
        private Random _random = new Random();
        private Field _playerField;
        private int _fieldLenght;
        private List<Turn> _attackTurns;
        private List<Turn> _finishTurns;
        private Turn _lastTurn;
        private bool _isShipHit;


        public PlayerMasha()
        {
            _random = new Random(Guid.NewGuid().ToString().ToCharArray().Where(c => "1234567890".Contains(c)).Select(c => int.Parse(c.ToString())).Sum());
        }
        public string Name()
        {
            return "Masha";
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
        private List<Turn> CreateAttackTurns()
        {
            List<Turn> turnList = new List<Turn>();

            for (int y = 1; y <= _fieldLenght; y++)
            {
                for (int x = 1; x <= _fieldLenght; x++)
                {
                    if (y % 2 != 0)
                    {
                        if (x == 1 || x == 4 || x == 5 || x == 8 || x == 9)
                        {
                            turnList.Add(new Turn(x - 1, y - 1));
                        }
                    }
                    else
                    {
                        if (x == 2 || x == 3 || x == 6 || x == 7 || x == 10)
                        {
                            turnList.Add(new Turn(x - 1, y - 1));
                        }
                    }
                }
            }
            return turnList;
        }
        private Turn MakeAttackTurn()
        {
            Turn tmpTurn = GenerateRandomTurn();

            if (_attackTurns.Count > 0)
            {
                for (int i = 0; i < _attackTurns.Count; i++)
                {
                    if (_playerField.GetCellState(_attackTurns[0]) != CellCondition.Unknown)
                    {
                        _attackTurns.RemoveAt(0);
                    }
                    else
                    {
                        tmpTurn = _attackTurns[0];
                        _attackTurns.RemoveAt(0);
                        break;
                    }
                }
            }
            else
            {
                tmpTurn = GenerateRandomTurn();
            }
            return tmpTurn;
        }
        private List<Turn> CreateFinishTurns(Turn hitTurn)
        {
            List<Turn> finishTurns = new List<Turn>();
            //влево
            finishTurns.Add(new Turn(hitTurn.CoordinateX - 1, hitTurn.CoordinateY));
            finishTurns.Add(new Turn(hitTurn.CoordinateX - 2, hitTurn.CoordinateY));
            finishTurns.Add(new Turn(hitTurn.CoordinateX - 3, hitTurn.CoordinateY));
            //вправо
            finishTurns.Add(new Turn(hitTurn.CoordinateX + 1, hitTurn.CoordinateY));
            finishTurns.Add(new Turn(hitTurn.CoordinateX + 2, hitTurn.CoordinateY));
            finishTurns.Add(new Turn(hitTurn.CoordinateX + 3, hitTurn.CoordinateY));
            //вниз
            finishTurns.Add(new Turn(hitTurn.CoordinateX, hitTurn.CoordinateY - 1));
            finishTurns.Add(new Turn(hitTurn.CoordinateX, hitTurn.CoordinateY - 2));
            finishTurns.Add(new Turn(hitTurn.CoordinateX, hitTurn.CoordinateY - 3));
            //вверх
            finishTurns.Add(new Turn(hitTurn.CoordinateX, hitTurn.CoordinateY + 1));
            finishTurns.Add(new Turn(hitTurn.CoordinateX, hitTurn.CoordinateY + 2));
            finishTurns.Add(new Turn(hitTurn.CoordinateX, hitTurn.CoordinateY + 3));

            return finishTurns;
        }
        private Turn MakeFinishTurn()
        {
            Turn tmpTurn = GenerateRandomTurn();

            for (int i = 0; i < _finishTurns.Count; i++)
            {
                if (_finishTurns.Count > 0)
                {
                    if (_playerField.GetCellState(_finishTurns[0]) == CellCondition.Unknown &&
                        _finishTurns[0].CoordinateX >= 0 && _finishTurns[0].CoordinateX < _fieldLenght && _finishTurns[0].CoordinateY >= 0 && _finishTurns[0].CoordinateY < _fieldLenght)
                    {
                        tmpTurn = _finishTurns[0];
                        _finishTurns.RemoveAt(0);
                        break;
                    }
                    else
                    {
                        _finishTurns.RemoveAt(0);
                    }
                }
            }
            return tmpTurn;
        }

        public Turn MakeTurn(Field playerField)
        {
            _playerField = playerField;

            if (_lastTurn == null)
            {
                _lastTurn = MakeAttackTurn();
            }
            if (!_isShipHit)
            {
                if (_playerField.GetCellState(_lastTurn) == CellCondition.Hit)
                {
                    _isShipHit = true;
                    _finishTurns = CreateFinishTurns(_lastTurn);
                    _lastTurn = MakeFinishTurn();
                }
                else
                {
                    if (_attackTurns.Count > 0)
                    {
                        _lastTurn = MakeAttackTurn();
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
                if (_playerField.GetCellState(_lastTurn) != CellCondition.Sinked)
                {
                    _lastTurn = MakeFinishTurn();
                }
                else
                {
                    _finishTurns = null;
                    _isShipHit = false;
                    _lastTurn = MakeAttackTurn();
                }
                return _lastTurn;
            }
        }
        public List<Ship> GenerateShips(int fieldLength)
        {
            _fieldLenght = fieldLength;
            _attackTurns = CreateAttackTurns();

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
