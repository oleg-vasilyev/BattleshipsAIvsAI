using System;

namespace BattleshipsAIvsAI.Models
{
    class CanNotApplyPlayerTurnExeption : ApplicationException
    {
        public Turn Turn
        {
            get;
            private set;
        }

        public IPlayer Player
        {
            get;
            private set;
        }

        public CanNotApplyPlayerTurnExeption(string message, IPlayer player, CanNotApplyTurnExeption e) : base(message)
        {
            Turn = e.Turn;
            Player = player;
        }

        public CanNotApplyPlayerTurnExeption(string message, IPlayer player, Turn turn) : base(message)
        {
            Turn = turn;
            Player = player;
        }
    }
}
