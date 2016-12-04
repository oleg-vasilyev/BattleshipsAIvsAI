using BattleshipsAIvsAI.Models;

namespace BattleshipsAIvsAI.ViewModels
{
    class GameResultViewModel
    {
        public readonly string NamePlayer1;
        public readonly string NamePlayer2;
        public readonly Log Log;

        public GameResultViewModel(GameResult gameResult)
        {
            NamePlayer1 = gameResult.GameLog.Player1.Name();
            NamePlayer2 = gameResult.GameLog.Player2.Name();
            Log = gameResult.GameLog;
        }
    }
}
