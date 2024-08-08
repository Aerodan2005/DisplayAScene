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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DisplayAScene
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPTxy8BH1VEsoebNVZXo8HurIaDWYvvUyjPrVsCC9dvPfJZ0Xkkt8eLXgb5ypkvm5Uq6wbaoVW - tX_zxPooZcOcvu0Gvi8iq3ii6uvJYYaW0ez8JTBtBTKQySc_FdV9ISrNENPbYk76sII364NTsU66AmNA - mkjSlhU0loOCvJImkQaiz5KQXXnSZcZWXQxO1 - I - RBSfs5IkOdRX86DWqhhsMKo_kpw2YvPKoy8ua39 - Vs.AT1_rxWg8l2y";
        }

    }
}


