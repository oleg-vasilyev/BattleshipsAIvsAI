using System.Windows;

using BattleshipsAIvsAI.ViewModels;
using BattleshipsAIvsAI.Models;
using BattleshipsAIvsAI.Players;
using System.Collections.Generic;

namespace BattleshipsAIvsAI
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainView view = new MainView();

            List<IPlayer> playerList = new List<IPlayer>();

            IPlayer playerOleg = new PlayerOleg();
            playerList.Add(playerOleg);

            IPlayer playerKuril = new PlayerKuril();
            playerList.Add(playerKuril);

            IPlayer playerRandom = new PlayerRandom();
            playerList.Add(playerRandom);

            IPlayer playerMasha = new PlayerMasha();
            playerList.Add(playerMasha);

            //IPlayer playerKristina = new PlayerKristina();
            //playerList.Add(playerKristina);

            IPlayer playerMax = new PlayerMax();
            playerList.Add(playerMax);

            MainViewModel viewModel = new MainViewModel(playerList);

            view.DataContext = viewModel;
            view.Show();
        }
    }
}
