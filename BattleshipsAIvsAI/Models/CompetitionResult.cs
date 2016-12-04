using System;

namespace BattleshipsAIvsAI.Models
{
    [Serializable]
    class CompetitionResult
    {
        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }
        public double FirstPlayerWinsInPercent { get; }
        public double SecondPlayerWinsInPercent { get; }

        public CompetitionResult(IPlayer player1, IPlayer player2, double firstPlayerWinsInPercent, double secondPlayerWinsInPercent)
        {
            if (player1 == null) { throw new ArgumentNullException(); }
            if (player2 == null) { throw new ArgumentNullException(); }

            Player1 = player1;
            Player2 = player2;
            FirstPlayerWinsInPercent = firstPlayerWinsInPercent;
            SecondPlayerWinsInPercent = secondPlayerWinsInPercent;
        }
    }
}
