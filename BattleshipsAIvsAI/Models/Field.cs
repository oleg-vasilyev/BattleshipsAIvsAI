using System;

namespace BattleshipsAIvsAI.Models
{
    [Serializable]
    class Field : ICloneable
    {
        private CellCondition[,] _cellConditionArray;
        private int _fieldLength;

        public Field(int fieldLength)
        {
            if (fieldLength <= 0) { throw new FieldLengthException("fieldLength = " + fieldLength + ", although it should be more than zero", fieldLength); }

            _fieldLength = fieldLength;
            _cellConditionArray = new CellCondition[_fieldLength, _fieldLength];
            for (int i = 0; i < _fieldLength; i++)
            {
                for (int j = 0; j < _fieldLength; j++)
                {
                    _cellConditionArray[i, j] = CellCondition.Unknown;
                }
            }
        }

        public CellCondition GetCellState(Turn turn)
        {
            if (turn == null) { throw new ArgumentNullException("turn"); }

            int coordinate_x = turn.CoordinateX;
            int coordinate_y = turn.CoordinateY;

            if (coordinate_x >= 0 && coordinate_x < _fieldLength &&
                coordinate_y >= 0 && coordinate_y < _fieldLength)
            { return _cellConditionArray[coordinate_x, coordinate_y]; }
            return CellCondition.Miss;
            //throw new TurnException("Turn is out of field", turn, this);
        }

        public void SetCellState(Turn turn, CellCondition cellCondition)
        {
            if (turn == null) { throw new ArgumentNullException("turn"); }

            int coordinate_x = turn.CoordinateX;
            int coordinate_y = turn.CoordinateY;

            if (coordinate_x >= 0 && coordinate_x < _fieldLength &&
                coordinate_y >= 0 && coordinate_y < _fieldLength)
            { _cellConditionArray[coordinate_x, coordinate_y] = cellCondition; }
            else { throw new TurnException("Turn is out of field", turn, this); }
        }

        public void MarkCellsAfterShipSinks(Ship ship)
        {
            if (ship == null) { throw new ArgumentNullException("ship"); }

            // Пометка ячеек под кораблём
            int firstIndex = ship.FirstCoordinateX - 1;
            int secondIndex = ship.SecondCoordinateX + 1;
            int thirdIndex = ship.FirstCoordinateY - 1;

            if (thirdIndex >= 0)
            {
                if (firstIndex < 0) firstIndex = 0;
                if (secondIndex > _fieldLength - 1) secondIndex = _fieldLength - 1;
                for (int i = firstIndex; i <= secondIndex; i++)
                {
                    SetCellState(new Turn(i, thirdIndex), CellCondition.Miss);
                }
            }

            // Пометка ячеек над кораблём
            firstIndex = ship.FirstCoordinateX - 1;
            secondIndex = ship.SecondCoordinateX + 1;
            thirdIndex = ship.SecondCoordinateY + 1;

            if (thirdIndex <= _fieldLength - 1)
            {
                if (firstIndex < 0) firstIndex = 0;
                if (secondIndex > _fieldLength - 1) secondIndex = _fieldLength - 1;
                for (int i = firstIndex; i <= secondIndex; i++)
                {
                    SetCellState(new Turn(i, thirdIndex), CellCondition.Miss);
                }
            }

            // Пометка слева от корабля
            firstIndex = ship.FirstCoordinateY;
            secondIndex = ship.SecondCoordinateY;
            thirdIndex = ship.FirstCoordinateX - 1;

            if (thirdIndex >= 0)
            {
                for (int i = firstIndex; i <= secondIndex; i++)
                {
                    SetCellState(new Turn(thirdIndex, i), CellCondition.Miss);
                }
            }

            // Пометка справа от корабля
            firstIndex = ship.FirstCoordinateY;
            secondIndex = ship.SecondCoordinateY;
            thirdIndex = ship.SecondCoordinateX + 1;

            if (thirdIndex <= _fieldLength - 1)
            {
                for (int i = firstIndex; i <= secondIndex; i++)
                {
                    SetCellState(new Turn(thirdIndex, i), CellCondition.Miss);
                }
            }

            // Пометка корабля как затонувшего
            for (int i = ship.FirstCoordinateX; i <= ship.SecondCoordinateX; i++)
            {
                for (int j = ship.FirstCoordinateY; j <= ship.SecondCoordinateY; j++)
                {
                    SetCellState(new Turn(i, j), CellCondition.Sinked);
                }
            }
        }

        public int GetFieldLength()
        {
            return _fieldLength;
        }

        public object Clone()
        {
            Field outField = new Field(_fieldLength);
            try
            {
                for (int i = 0; i < _fieldLength; i++)
                {
                    for (int j = 0; j < _fieldLength; j++)
                    {
                        outField.SetCellState(new Turn(i, j), GetCellState(new Turn(i, j)));
                    }
                }
            }
            catch (OutOfMemoryException e) { throw e; }

            return outField;
        }
    }
}
