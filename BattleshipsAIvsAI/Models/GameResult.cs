using System;

namespace BattleshipsAIvsAI.Models
{
    [Serializable]
    class GameResult
    {
        public Log GameLog { get; }
        public IPlayer Winner;

        public void AddLogItem(LogItem logItem)
        {
            GameLog.AddItem(logItem);
        }

        public GameResult(IPlayer player1, IPlayer player2)
        {
            GameLog = new Log(player1, player2);
        }

        public void SetWinner(IPlayer player)
        {
            Winner = player;
        }

        public IPlayer GetWinner()
        {
            return Winner;
        }

    }
}
