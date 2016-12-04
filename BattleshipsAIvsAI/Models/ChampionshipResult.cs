using System;
using System.Collections.Generic;

namespace BattleshipsAIvsAI.Models
{
    class ChampionshipResult
    {
        private IEnumerable<CompetitionResult> _compititionsResults;

        public IEnumerable<CompetitionResult> CompititionsResults { get { return _compititionsResults; } }

        public ChampionshipResult(IEnumerable<CompetitionResult> compititionsResults)
        {
            if (compititionsResults == null)
            {
                throw new ArgumentNullException(nameof(compititionsResults));
            }

            _compititionsResults = compititionsResults;
        }
    }
}
