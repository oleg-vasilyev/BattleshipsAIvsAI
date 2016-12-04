using System;
using System.Linq;

namespace BattleshipsAIvsAI.Models
{
    class Game
    {
        private readonly int FieldLength;

        private readonly Refeery _refeery;

        private Random _random;

        public Game(Refeery refeery)
        {
            if (refeery == null) { throw new ArgumentNullException("refeery"); }

            _refeery = refeery;
            FieldLength = _refeery.GetFieldLength();
            _random = new Random(Guid.NewGuid().ToString().ToCharArray().Where(c => "1234567890".Contains(c)).Select(c => int.Parse(c.ToString())).Sum());
        }

        public GameResult Play(IPlayer firstPlayer, IPlayer secondPlayer)
        {
            if (firstPlayer == null) { throw new ArgumentNullException("firstPlayer"); }
            if (secondPlayer == null) { throw new ArgumentNullException("secondPlayer"); }
            GameResult gameResult = new GameResult(firstPlayer, secondPlayer);
            IPlayer attackingPlayer = null;
            CellCondition turnResult = new CellCondition();

            Field firstPlayerField = new Field(FieldLength);
            Field secondPlayerField = new Field(FieldLength);
            Field attackedPlayerField = new Field(FieldLength);

            PlayerShips firstPlayerShips = null;
            PlayerShips secondPlayerShips = null;
            PlayerShips attackedShips = null;

            try
            {
                firstPlayerShips = new PlayerShips(firstPlayer.GenerateShips(FieldLength));
            }
            catch (InvalidateShipsExeption e)
            {
                throw new PlayerHasInvalidateShipsException(firstPlayer.Name() + " " + e.Message, firstPlayer, e);
            }

            try
            {
                secondPlayerShips = new PlayerShips(secondPlayer.GenerateShips(FieldLength));
            }
            catch (InvalidateShipsExeption e)
            {
                throw new PlayerHasInvalidateShipsException(secondPlayer.Name() + " " + e.Message, secondPlayer, e);
            }

            if (_random.Next(2) == 0)
            {
                attackingPlayer = firstPlayer;
                attackedPlayerField = firstPlayerField;
                attackedShips = secondPlayerShips;
            }
            else
            {
                attackingPlayer = secondPlayer;
                attackedPlayerField = secondPlayerField;
                attackedShips = firstPlayerShips;
            }

            while (attackedShips.Ships.Count != 0)
            {
                try
                {
                    Turn turn = attackingPlayer.MakeTurn(attackedPlayerField);
                    turnResult = _refeery.ApplyTurnAndThrowExeption(turn, attackedShips, attackedPlayerField);
                    PlayerNumber playerNumber = PlayerNumber.Unknown;
                    if (attackingPlayer == firstPlayer) playerNumber = PlayerNumber.First;
                    else if(attackingPlayer == secondPlayer) playerNumber = PlayerNumber.Second;   
                    gameResult.AddLogItem(new LogItem(playerNumber, attackingPlayer, (attackedPlayerField.Clone() as Field)));                                     
                }
                catch (CanNotApplyTurnExeption e)
                {
                    throw new CanNotApplyPlayerTurnExeption(attackingPlayer.Name() + " " + e.Message, attackingPlayer, e);
                }
                catch (TurnException e)
                {
                    throw new CanNotApplyPlayerTurnExeption(attackingPlayer.Name() + " " + e.Message, attackingPlayer, e.Turn);
                }
                catch(OutOfMemoryException e)
                {
                    throw e;
                }

                if (turnResult == CellCondition.Miss)
                {
                    if (attackingPlayer == firstPlayer)
                    {
                        attackingPlayer = secondPlayer;
                        attackedPlayerField = secondPlayerField;
                        attackedShips = firstPlayerShips;
                    }
                    else
                    {
                        attackingPlayer = firstPlayer;
                        attackedPlayerField = firstPlayerField;
                        attackedShips = secondPlayerShips;
                    }
                }
            }
            gameResult.SetWinner(attackingPlayer);
            return gameResult;
        }
    }
}
