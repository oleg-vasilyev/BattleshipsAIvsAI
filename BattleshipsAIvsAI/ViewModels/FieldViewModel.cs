using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using BattleshipsAIvsAI.Models;

namespace BattleshipsAIvsAI.ViewModels
{
    class FieldViewModel 
    {
        public List<ObservableCollection<BitmapImage>> PlayerField { get; private set; } = new List<ObservableCollection<BitmapImage>>();

        public FieldViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                var uriSource = new Uri(@"pack://application:,,,/Resources/Unknown.png", UriKind.Absolute);
                BitmapImage image = new BitmapImage(uriSource);

                imageListColumn1.Add(image);
                imageListColumn2.Add(image);
                imageListColumn3.Add(image);
                imageListColumn4.Add(image);
                imageListColumn5.Add(image);
                imageListColumn6.Add(image);
                imageListColumn7.Add(image);
                imageListColumn8.Add(image);
                imageListColumn9.Add(image);
                imageListColumn10.Add(image);
            }

            PlayerField.Add(imageListColumn1);
            PlayerField.Add(imageListColumn2);
            PlayerField.Add(imageListColumn3);
            PlayerField.Add(imageListColumn4);
            PlayerField.Add(imageListColumn5);
            PlayerField.Add(imageListColumn6);
            PlayerField.Add(imageListColumn7);
            PlayerField.Add(imageListColumn8);
            PlayerField.Add(imageListColumn9);
            PlayerField.Add(imageListColumn10);

        }

        public void ChangeValue(int column, int index, CellCondition cellCondition)
        {
            Uri uriSource = null;

            switch (cellCondition)
            {
                case CellCondition.Sinked:
                    uriSource = new Uri(@"pack://application:,,,/Resources/SinkedShip.png", UriKind.Absolute);
                    break;
                case CellCondition.Hit:
                    uriSource = new Uri(@"pack://application:,,,/Resources/DamagedShip.png", UriKind.Absolute);
                    break;
                case CellCondition.Miss:
                    uriSource = new Uri(@"pack://application:,,,/Resources/Miss.png", UriKind.Absolute);
                    break;
                case CellCondition.Unknown:
                    uriSource = new Uri(@"pack://application:,,,/Resources/Unknown.png", UriKind.Absolute);
                    break;
            }
          
            BitmapImage image = new BitmapImage(uriSource);
            PlayerField[column][index] = image;
        }

        private ObservableCollection<BitmapImage> _imageListColumn1  = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn1
        {
            get { return _imageListColumn1; }
            set
            {
                _imageListColumn1 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn2 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn2
        {
            get { return _imageListColumn2; }
            set
            {
                _imageListColumn2 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn3 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn3
        {
            get { return _imageListColumn3; }
            set
            {
                _imageListColumn3 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn4 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn4
        {
            get { return _imageListColumn4; }
            set
            {
                _imageListColumn4 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn5 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn5
        {
            get { return _imageListColumn5; }
            set
            {
                _imageListColumn5 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn6 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn6
        {
            get { return _imageListColumn6; }
            set
            {
                _imageListColumn6 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn7 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn7
        {
            get { return _imageListColumn7; }
            set
            {
                _imageListColumn7 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn8 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn8
        {
            get { return _imageListColumn8; }
            set
            {
                _imageListColumn8 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn9 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn9
        {
            get { return _imageListColumn9; }
            set
            {
                _imageListColumn9 = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<BitmapImage> _imageListColumn10 = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> imageListColumn10
        {
            get { return _imageListColumn10; }
            set
            {
                _imageListColumn10 = value;
                RaisePropertyChanged();
            }
        }

        private event PropertyChangedEventHandler propertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (propertyChanged != null) { propertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}
