using System;
using System.Collections.Generic;

namespace BattleshipsAIvsAI.Models
{
    class Ship
    {
        public int FirstCoordinateX { get; private set; }
        public int SecondCoordinateX { get; private set; }
        public int FirstCoordinateY { get; private set; }
        public int SecondCoordinateY { get; private set; }

        public ShipType ShipType { get; private set; }

        public ShipCondition ShipCondition { get; set; }

        public Ship(ShipType shipType, int firstCoordinateX, int firstCoordinateY, int secondCoordinateX, int secondCoordinateY)
        {
            FirstCoordinateX = firstCoordinateX;
            SecondCoordinateX = secondCoordinateX;
            FirstCoordinateY = firstCoordinateY;
            SecondCoordinateY = secondCoordinateY;
            ShipType = shipType;
            ShipCondition = ShipCondition.Unbroken;
        }

        public bool IsHit(Turn turn)
        {
            if (turn == null) { throw new ArgumentNullException("turn"); }

            if (SecondCoordinateX >= turn.CoordinateX && FirstCoordinateX <= turn.CoordinateX && SecondCoordinateY >= turn.CoordinateY && FirstCoordinateY <= turn.CoordinateY)
            {
                return true;
            }
            return false;
        }

        public bool ValidateNumberOfDecks()
        {
            int numberOfDecks = Convert.ToInt32(ShipType);

            return ((SecondCoordinateX - FirstCoordinateX == numberOfDecks &&
                     SecondCoordinateY - FirstCoordinateY == 0)
                     ||
                    (SecondCoordinateX - FirstCoordinateX == 0 &&
                   SecondCoordinateY - FirstCoordinateY == numberOfDecks));
        }

        public bool IsNextToAnotherShips(IEnumerable<Ship> ships)
        {
            if (ships == null) { throw new ArgumentNullException("ships"); }

            // Поиск под и над кораблём
            int firstCoord = FirstCoordinateX - 1;
            int secondCoord = SecondCoordinateX + 1;
            int thirdCoord = FirstCoordinateY - 1;
            int fourthCoord = SecondCoordinateY + 1;

            foreach (Ship ship in ships)
            {
                if ((ship.SecondCoordinateY == thirdCoord ||
                    ship.FirstCoordinateY == fourthCoord) &&
                    ((ship.FirstCoordinateX >= firstCoord &&
                    ship.FirstCoordinateX <= secondCoord) ||
                    (ship.SecondCoordinateX >= firstCoord &&
                    ship.SecondCoordinateX <= secondCoord)))
                {
                    return true;
                }
                if (ship != this && ship.FirstCoordinateX == FirstCoordinateX && ship.FirstCoordinateY == FirstCoordinateY)
                {
                    return true;
                }
            }

            // Поиск справа и слева от корабля
            firstCoord = FirstCoordinateY;
            secondCoord = SecondCoordinateY;
            thirdCoord = FirstCoordinateX - 1;
            fourthCoord = SecondCoordinateX + 1;


            foreach (Ship ship in ships)
            {
                if ((ship.SecondCoordinateX == thirdCoord ||
                    ship.FirstCoordinateX == fourthCoord) &&
                    ((ship.FirstCoordinateY >= firstCoord &&
                    ship.FirstCoordinateY <= secondCoord) ||
                    (ship.SecondCoordinateY >= firstCoord &&
                    ship.SecondCoordinateY <= secondCoord)))
                {
                    return true;
                }
            }

            return false;
        }

        public ShipCondition GetConditionAfterHit(Field attackingPlayerField)
        {
            if (attackingPlayerField == null) { throw new ArgumentNullException("field"); }

            if (SecondCoordinateX - FirstCoordinateX == 0)
            {
                for (int i = FirstCoordinateY; i <= SecondCoordinateY; i++)
                {
                    if (attackingPlayerField.GetCellState(new Turn(SecondCoordinateX, i)) != CellCondition.Hit)
                    {
                        ShipCondition = ShipCondition.Damaged;
                        return ShipCondition.Damaged;
                    }
                }
                ShipCondition = ShipCondition.Sinked;
                return ShipCondition.Sinked;
            }
            else
            {
                for (int i = FirstCoordinateX; i <= SecondCoordinateX; i++)
                {
                    if (attackingPlayerField.GetCellState(new Turn(i, SecondCoordinateY)) != CellCondition.Hit)
                    {
                        ShipCondition = ShipCondition.Damaged;
                        return ShipCondition.Damaged;
                    }
                }
                ShipCondition = ShipCondition.Sinked;
                return ShipCondition.Sinked;
            }
        }
    }
}
