using System;

namespace BattleshipsAIvsAI.Models
{
    public class FieldLengthException : ApplicationException
    {
        public int FieldLength
        {
            get;
            private set;
        }

        public FieldLengthException(string message, int fieldLength) : base(message)
        {
            FieldLength = fieldLength;
        }
    }
}
