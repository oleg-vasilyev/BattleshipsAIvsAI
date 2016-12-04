using System;

namespace BattleshipsAIvsAI.Models
{
    [Serializable]
    class Turn
    {
        public int CoordinateX { get; private set; }
        public int CoordinateY { get; private set; }

        public Turn(int coordinateX, int coordinateY)
        {
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
        }
    }
}
