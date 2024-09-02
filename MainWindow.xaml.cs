// Copyright 2021 Esri
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace DisplayAScene
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SceneViewModel();

        }
        static int ind = 0;
        private async void ShowTraj_Click(object sender, RoutedEventArgs e)
        {
            SceneViewModel sceneViewModel = DataContext as SceneViewModel;
            sceneViewModel.LoadTrajectoryData();
             sceneViewModel.AddTrajectoryToScene();
        }
        private async void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            SceneViewModel sceneViewModel = DataContext as SceneViewModel;
            sceneViewModel.ClearAllGraphics();
            ind = 0;
        }

        private async void NextPt_Click(object sender, RoutedEventArgs e)
        {
            
            SceneViewModel sceneViewModel = DataContext as SceneViewModel;
            sceneViewModel.GoNextPt(ind);
            ind++;

        }

        private void AllTraj_Click(object sender, RoutedEventArgs e)
        {
            SceneViewModel sceneViewModel = DataContext as SceneViewModel;
            sceneViewModel.GoThroughTrajectory();
        }
    }
}

