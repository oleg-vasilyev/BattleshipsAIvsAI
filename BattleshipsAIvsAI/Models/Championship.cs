using System;
using System.Collections.Generic;
using System.IO;

namespace BattleshipsAIvsAI.Models
{
	class Championship
	{
		private readonly int NumberOfGames;
		private const int FieldLength = 10;

		public Championship(int numberOfGames)
		{
			if (numberOfGames <= 0) { throw new ArgumentException($"{nameof(numberOfGames)} should be more than zero "); }
			NumberOfGames = numberOfGames;

			string path = Directory.GetCurrentDirectory() + "\\Data";

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			foreach (FileInfo file in dirInfo.GetFiles())
			{
				file.Delete();
			}
		}

		public ChampionshipResult DoChampionship(IList<IPlayer> players)
		{
			if (players == null) { throw new ArgumentNullException(nameof(players)); }
			List<CompetitionResult> listChampionshipResult = new List<CompetitionResult>();
			for (int j = 0; j < players.Count; j++)
			{
				if (j + 1 == players.Count) { break; }
				IPlayer tmpPlayer = players[j];

				for (int i = j + 1; i < players.Count; i++)
				{
					listChampionshipResult.Add(GetResultCompetition(tmpPlayer, players[i]));
				}
			}
			ChampionshipResult championshipResult = new ChampionshipResult(listChampionshipResult);
			return championshipResult;

		}

		private CompetitionResult GetResultCompetition(IPlayer player1, IPlayer player2)
		{
			if (player1 == null) { throw new ArgumentNullException(nameof(player1)); }
			if (player2 == null) { throw new ArgumentNullException(nameof(player2)); }

			Game game = null;
			List<GameResult> gameResultList = new List<GameResult>();
			try
			{
				game = new Game(new Refeery(FieldLength));
			}
			catch (ApplicationException e)
			{
				throw e;
			}

			int firstPlayerWinsCount = 0;
			int secondPlayerWinsCount = 0;

			for (int i = 0; i < NumberOfGames; i++)
			{
				GameResult gameResult = null;
				try
				{
					gameResult = game.Play(player1, player2);
					gameResultList.Add(gameResult);
				}
				catch (ApplicationException e)
				{
					throw e;
				}

				if (gameResult.Winner == player1) { firstPlayerWinsCount++; }
				else if (gameResult.Winner == player2) { secondPlayerWinsCount++; }
			}

			double firstPlayerWinsInPercent = ((double)firstPlayerWinsCount / NumberOfGames) * 100;
			double secondPlayerWinsInPercent = ((double)secondPlayerWinsCount / NumberOfGames) * 100;
			Serializer.SaveToBinnary(string.Format("{0} vs {1}", player1.Name(), player2.Name()), gameResultList);

			return new CompetitionResult(player1, player2, Math.Round(firstPlayerWinsInPercent, 2), Math.Round(secondPlayerWinsInPercent, 2));
		}
	}
}
