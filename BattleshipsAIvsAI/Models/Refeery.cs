using System;
using System.Collections.Generic;

namespace BattleshipsAIvsAI.Models
{
    class Refeery
    {
        private readonly int FieldLength;

        public Refeery(int fieldLength)
        {
            if (fieldLength <= 0)
            {
                throw new FieldLengthException("fieldLength = " + fieldLength + ", although it should be more than zero", fieldLength);
            }
            FieldLength = fieldLength;
        }

        public int GetFieldLength()
        {
            return FieldLength;
        }

        public CellCondition ApplyTurnAndThrowExeption(Turn turn, PlayerShips attackedShips, Field attackingPlayerField)
        {
            if (turn == null) { throw new ArgumentNullException("turn"); }

            if (attackedShips == null) { throw new ArgumentNullException("attackedShipsList"); }

            if (attackingPlayerField == null) { throw new ArgumentNullException("attackingPlayerField"); }

            if (turn.CoordinateX < 0 || turn.CoordinateX >= FieldLength || turn.CoordinateY < 0 || turn.CoordinateY >= FieldLength)
            {
                throw new CanNotApplyTurnExeption("Turn is out of field", turn);
            }

            for (int i = 0; i < attackedShips.Ships.Count; i++)
            {
                Ship tempShip = attackedShips.Ships[i];
                if (tempShip.IsHit(turn))
                {
                    attackingPlayerField.SetCellState(turn, CellCondition.Hit);
                    ShipCondition shipCondition = tempShip.GetConditionAfterHit(attackingPlayerField);

                    if (shipCondition == ShipCondition.Sinked)
                    {
                        attackedShips.Ships.Remove(tempShip);
                        attackingPlayerField.MarkCellsAfterShipSinks(tempShip);
                        return CellCondition.Sinked;
                    }
                    return CellCondition.Hit;
                }
            }
            attackingPlayerField.SetCellState(turn, CellCondition.Miss);
            return CellCondition.Miss;
        }
    }
}
