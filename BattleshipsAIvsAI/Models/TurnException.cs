using System;

namespace BattleshipsAIvsAI.Models
{
    class TurnException : ApplicationException
    {
        public Turn Turn
        {
            get;
            private set;
        }

        public Field Field
        {
            get;
            private set;
        }

        public TurnException(string message, Turn turn, Field field) : base(message)
        {
            Turn = turn;
            Field = field;
        }
    }
}
