using System;

namespace BattleshipsAIvsAI.Models
{
    class CanNotApplyTurnExeption : ApplicationException
    {
        public Turn Turn
        {
            get;
            private set;
        }

        public CanNotApplyTurnExeption(string message, Turn turn) : base(message)
        {
            Turn = turn;
        }
    }
}
