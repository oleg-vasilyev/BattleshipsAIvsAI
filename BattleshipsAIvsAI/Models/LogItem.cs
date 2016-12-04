using System;

namespace BattleshipsAIvsAI.Models
{
    [Serializable]
    class LogItem
    {
        public readonly PlayerNumber PlayerNumber;
        public readonly IPlayer Player;
        public readonly Field Field;

        public LogItem(PlayerNumber playerNumber, IPlayer player, Field field)
        {
            Player = player;
            PlayerNumber = playerNumber;
            Field = field;
        }
    }
}
