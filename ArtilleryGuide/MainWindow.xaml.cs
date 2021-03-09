using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArtilleryGuide
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _heading = 0.0;
        public double Heading
        {
            get
            {
                return _heading;
            }
            set
            {
                _heading = value;
                Recalculate();
            }
        }

        private double _distance = 100.0;
        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
                Recalculate();
            }
        }
        ObservableCollection<RangeInformation> hops;

        public MainWindow()
        {
            InitializeComponent();

            hops = new ObservableCollection<RangeInformation>();

            hops.Add(new RangeInformation() { RangeName = "Spotter to Gun", Azimuth = 180.0, Distance = 100.0 });
            hops[0].PropertyChanged += ListItem_PropertyChanged;
            HopsList.ItemsSource = hops;
            UpdateRemoveHopButton();

            hops.CollectionChanged += Hops_CollectionChanged;
            Recalculate();
        }

        private void EnemyHeading_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Recalculate();
        }

        private void ListItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Recalculate();
        }

        private void Hops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Recalculate();
        }

        private void AddHop_Click(object sender, RoutedEventArgs e)
        {
            hops.Insert(0, new RangeInformation() { RangeName = "First Hop from Spotter", Azimuth = 180.0, Distance = 100.0 });
            hops[0].PropertyChanged += ListItem_PropertyChanged;
            UpdateHopNames();

            UpdateRemoveHopButton();
        }

        private void RemoveHop_Click(object sender, RoutedEventArgs e)
        {
            hops.RemoveAt(0);

            UpdateHopNames();

            UpdateRemoveHopButton();
        }

        private void UpdateRemoveHopButton()
        {
            RemoveHopButton.IsEnabled = hops.Count > 1;
        }

        private void UpdateHopNames()
        {
            if (hops.Count == 1)
            {
                hops[0].RangeName = "Spotter to Gun";
            }
            else {
                hops[0].RangeName = "First Hop from Spotter";
                hops[hops.Count - 1].RangeName = "Last Hop to Gun";
            }

            for(int i = 1; i < hops.Count - 1; i++)
            {
                hops[i].RangeName = string.Format("Hop {0}", i);
            }
        }

        private void Recalculate()
        {
            HeadingInformation enemy = new HeadingInformation
            {
                heading = _heading,
                distance = _distance
            };

            List<HeadingInformation> gunHeadings = new List<HeadingInformation>();
            foreach(RangeInformation rangeInformation in hops)
            {
                gunHeadings.Add(new HeadingInformation()
                {
                    heading = rangeInformation.Azimuth,
                    distance = rangeInformation.Distance
                });
            }

            HeadingInformation totalGunHeading = new HeadingInformation()
            {
                heading = 0.0,
                distance = 0.0
            };

            foreach(HeadingInformation headingInformation in gunHeadings)
            {
                totalGunHeading += headingInformation;
            }

            HeadingInformation reversedGunHeading = -totalGunHeading;
            HeadingInformation taskingHeading = reversedGunHeading + enemy;

            CalculatedAzimuthBox.Text = taskingHeading.heading.ToString();
            CalculatedDistanceBox.Text = taskingHeading.distance.ToString();
        }
    }

    public class RangeInformation : INotifyPropertyChanged
    {
        private string _rangeName;
        public string  RangeName { 
            get 
            {
                return _rangeName;
            } 
            set 
            {
                _rangeName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RangeName"));
            } 
        }
        private double _azimuth;
        public double Azimuth { 
            get 
            {
                return _azimuth;
            } 
            set 
            {
                _azimuth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Azimuth"));
            } 
        }
        private double _distance;
        public double Distance { 
            get 
            {
                return _distance;
            } 
            set 
            {
                _distance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Distance"));
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
