using System;
using System.Collections.Generic;

namespace BattleshipsAIvsAI.Models
{
    interface IPlayer
    {
        List<Ship> GenerateShips(int FieldLength);
        Turn MakeTurn(Field PlayerField);
        string Name();
    }
}
