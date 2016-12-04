using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BattleshipsAIvsAI.Models;
using BattleshipsAIvsAI.Commands;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace BattleshipsAIvsAI.ViewModels
{
	class MainViewModel : INotifyPropertyChanged
	{
		private ChampionshipResult _championshipResult;
		private IList<IPlayer> _playerList;
		private Thread _calculateGlobalStatisticsThread;
		private Thread _gameThread;

		public FieldViewModel FirstPlayerField { get; private set; }

		public FieldViewModel SecondPlayerField { get; private set; }

		public MainViewModel(IList<IPlayer> playerList)
		{
			_playerList = playerList;
			FirstPlayerField = new FieldViewModel();
			SecondPlayerField = new FieldViewModel();

		}

		private ObservableCollection<Label> _competitionLabelList;
		public ObservableCollection<Label> CompetitionLabelList
		{
			get { return _competitionLabelList; }
			set
			{
				_competitionLabelList = value;
				RaisePropertyChanged();
			}
		}

		private Label _currentLabelCompetition;
		public Label CurrentLabelCompetition
		{
			get { return _currentLabelCompetition; }
			set
			{
				_currentLabelCompetition = value;
				RaisePropertyChanged();

				string competitionName = $"{Directory.GetCurrentDirectory()}\\Data\\{_currentLabelCompetition.Content}";
				_gameResultList = Serializer.LoadFromBinnary<List<GameResult>>(competitionName);

				int counter = 0;
				GameLabelList = new ObservableCollection<Label>();
				foreach (var gameResult in _gameResultList)
				{
					counter++;
					Label lbl = new Label();
					lbl.Content = $"[{counter}] { gameResult.GameLog.GetLog()[gameResult.GameLog.GetLog().Count - 1].Player.Name()} wins";
					GameLabelList.Add(lbl);
				}
				CurrentLabelGame = GameLabelList[0];
			}
		}

		private ObservableCollection<Label> _gameLabelList;
		public ObservableCollection<Label> GameLabelList
		{
			get { return _gameLabelList; }
			set
			{
				_gameLabelList = value;
				RaisePropertyChanged();
			}
		}

		private Label _currentLabelGame;
		public Label CurrentLabelGame
		{
			get { return _currentLabelGame; }
			set
			{
				_currentLabelGame = value;
				RaisePropertyChanged();
				if (CurrentLabelGame != null)
				{
					_currentGame = new GameResultViewModel(_gameResultList[GameLabelList.IndexOf(CurrentLabelGame)]);
				}
				else { _currentGame = new GameResultViewModel(_gameResultList[0]); }

				if (_currentGame != null)
				{
					NamePlayer1 = _currentGame.NamePlayer2;
					NamePlayer2 = _currentGame.NamePlayer1;
				}
				else
				{
					NamePlayer1 = "Player 2";
					NamePlayer2 = "Player 1";
				}
			}
		}

		private List<GameResult> _gameResultList;
		private GameResultViewModel _currentGame;

		private string _namePlayer1;
		public string NamePlayer1
		{
			get { return _namePlayer1; }
			set
			{
				_namePlayer1 = value;
				RaisePropertyChanged();
			}
		}

		private string _namePlayer2;
		public string NamePlayer2
		{
			get { return _namePlayer2; }
			set
			{
				_namePlayer2 = value;
				RaisePropertyChanged();
			}
		}

		private double _blurEffectRadius = 9.0;
		public double BlurEffectRadius
		{
			get { return _blurEffectRadius; }
			set
			{
				_blurEffectRadius = value;
				RaisePropertyChanged();
			}
		}

		private Visibility _startProgrammErrorMessageVisibility = Visibility.Hidden;
		public Visibility StartProgrammErrorMessageVisibility
		{
			get { return _startProgrammErrorMessageVisibility; }
			set
			{
				_startProgrammErrorMessageVisibility = value;
				RaisePropertyChanged();
			}
		}

		private Visibility _startProgrammGridVisibility = Visibility.Visible;
		public Visibility StartProgrammGridVisibility
		{
			get { return _startProgrammGridVisibility; }
			set
			{
				_startProgrammGridVisibility = value;
				RaisePropertyChanged();
			}
		}

		private Visibility _startProgrammPBarRegionVisibility = Visibility.Collapsed;
		public Visibility StartProgrammPBarRegionVisibility
		{
			get { return _startProgrammPBarRegionVisibility; }
			set
			{
				_startProgrammPBarRegionVisibility = value;
				RaisePropertyChanged();
			}
		}

		private int? _numberOfGames = null;
		public int? NumberOfGames
		{
			get { return _numberOfGames; }
			set
			{
				_numberOfGames = value;
				RaisePropertyChanged();
			}
		}

		private DelegateCommand exitDelegate;
		public ICommand Exit
		{
			get
			{
				exitDelegate = new DelegateCommand(delegate
				{
					if (_calculateGlobalStatisticsThread != null) { _calculateGlobalStatisticsThread.Abort(); }
					Application.Current.Shutdown();
				});
				return exitDelegate;
			}
		}

		private DelegateCommand calculateGlobalStatisticsDelegate;
		public ICommand СalculateGlobalStatistics
		{
			get
			{
				calculateGlobalStatisticsDelegate = new DelegateCommand(delegate
				{
					if (_calculateGlobalStatisticsThread != null) { _calculateGlobalStatisticsThread.Abort(); }
					StartProgrammErrorMessageVisibility = Visibility.Hidden;
					_calculateGlobalStatisticsThread = new Thread(delegate ()
					{
						StartProgrammPBarRegionVisibility = Visibility.Visible;
						if (NumberOfGames == null)
						{
							StartProgrammErrorMessageVisibility = Visibility.Visible;
							StartProgrammPBarRegionVisibility = Visibility.Collapsed;
							return;
						}

						Championship championship = new Championship((int)NumberOfGames);
						_championshipResult = championship.DoChampionship(_playerList);

						StartProgrammPBarRegionVisibility = Visibility.Collapsed;
						StartProgrammGridVisibility = Visibility.Collapsed;
						BlurEffectRadius = 0.0;

						Application.Current.Dispatcher.BeginInvoke(new ThreadStart(delegate ()
						{
							List<Label> labelList = new List<Label>();
							DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data");
							foreach (FileInfo file in dirInfo.GetFiles())
							{
								Label lbl = new Label();
								lbl.Content = file.Name;
								labelList.Add(lbl);
							}
							CompetitionLabelList = new ObservableCollection<Label>(labelList);
							CurrentLabelCompetition = CompetitionLabelList[0];
							DisplayGlobalStatistics();
						}));
					});
					_calculateGlobalStatisticsThread.Start();
				});
				return calculateGlobalStatisticsDelegate;
			}
		}

		private DelegateCommand displayDameDelegate;
		public ICommand DisplayDame
		{
			get
			{
				displayDameDelegate = new DelegateCommand(delegate
				{
					_gameThread?.Abort();
					_gameThread = new Thread(new ThreadStart(delegate
					{

						{
							FieldViewModel currentField = null;

							Log log = _currentGame.Log;

							int index = 0;

							currentField = FirstPlayerField;
							while (index < log.GetLog().Count)
							{
								switch (log.GetLog()[index].PlayerNumber)
								{
									case PlayerNumber.First:
										currentField = FirstPlayerField;
										break;
									case PlayerNumber.Second:
										currentField = SecondPlayerField;
										break;
								}

								Field field = log.GetLog()[index].Field;

								for (int i = 0; i < field.GetFieldLength(); i++)
								{
									for (int j = 0; j < field.GetFieldLength(); j++)
									{
										int i_temp = i;
										int j_temp = j;
										Application.Current.Dispatcher.BeginInvoke(new ThreadStart(delegate
										{
											currentField.ChangeValue(i_temp, j_temp, field.GetCellState(new Turn(i_temp, j_temp)));
										}));
									}
								}
								Thread.Sleep(100);
								index++;
							}
						}
					}));

					_gameThread.Start();

				});
				return displayDameDelegate;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
		{
			if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
		}

		private void DisplayGlobalStatistics()
		{
			string output = string.Format("Результаты:");
			foreach (var item in _championshipResult.CompititionsResults)
			{
				output += string.Format("\n{0} vs {1} \n{2}% - {3}%", item.Player1.Name(), item.Player2.Name(), item.FirstPlayerWinsInPercent, item.SecondPlayerWinsInPercent);
			}
			MessageBox.Show(output, "Глобальная статистика");
		}
	}
}
