using System.Windows;
namespace BattleshipsAIvsAI
{
    public partial class MainView : Window
    {

        public MainView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
	}
}
