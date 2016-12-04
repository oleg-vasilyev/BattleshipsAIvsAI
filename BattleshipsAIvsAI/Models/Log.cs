using System;
using System.Collections.Generic;

namespace BattleshipsAIvsAI.Models
{
    [Serializable]
    class Log
    {
        private List<LogItem> _log;

        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        public void AddItem(LogItem logItem)
        {
            _log.Add(logItem);
        }

        public Log(IPlayer player1, IPlayer player2)
        {
            _log = new List<LogItem>();
            Player1 = player1;
            Player2 = player2;
        }

        public List<LogItem> GetLog()
        {
            return _log;
        }
    }
}
