using System;
using System.Collections.Generic;

namespace BattleshipsAIvsAI.Models
{
    class PlayerHasInvalidateShipsException : ApplicationException
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

        public IPlayer Player
        {
            get;
            private set;
        }

        public PlayerHasInvalidateShipsException(string message, IPlayer player, InvalidateShipsExeption e) : base(message)
        {
            Player = player;
            Ships = e.Ships;
            InvalidShip = e.InvalidShip;
        }
    }
}
