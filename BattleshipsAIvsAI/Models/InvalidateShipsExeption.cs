using System;
using System.Collections.Generic;

namespace BattleshipsAIvsAI.Models
{
    class InvalidateShipsExeption : ApplicationException
    {
        public Ship InvalidShip
        {
            get;
            private set;
        }

        public IEnumerable<Ship> Ships
        {
            get;
            private set;
        }


        public InvalidateShipsExeption(string message, IEnumerable<Ship> ships) : base(message)
        {
            Ships = ships;
        }

        public InvalidateShipsExeption(string message, IEnumerable<Ship> ships, Ship invalidShip) : base(message)
        {
            Ships = ships;
            InvalidShip = invalidShip;
        }
    }
}
