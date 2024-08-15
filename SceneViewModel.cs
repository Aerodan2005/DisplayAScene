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


using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;

using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DisplayAScene
{

    public partial class SceneViewModel : INotifyPropertyChanged
    {


        public SceneViewModel()
        {
            SetupScene();
            //OnPropertyChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Scene? _scene;
        public Scene? Scene
        {
            get { return _scene; }
            set
            {
                _scene = value;
                OnPropertyChanged();
            }
        }
        private GraphicsOverlayCollection? _graphicsOverlays;
        public GraphicsOverlayCollection? GraphicsOverlays
        {
            get
            {
                return _graphicsOverlays;
            }
            set
            {
                _graphicsOverlays = value;
                OnPropertyChanged();
            }
        }

        private async Task SetupScene()
        {
            // Create a new scene with an imagery basemap.
            Scene scene = new Scene(BasemapStyle.ArcGISImageryStandard);

            // Create a file path to the scene package or scene layer package.
            string scenePath = @"C:\Users\urika\OneDrive\מסמכים\ArcGIS\Projects\Med2\Med2.mspk";

            try
            {
                // Load the mobile scene package.
                MobileScenePackage mobileScenePackage = await MobileScenePackage.OpenAsync(scenePath);

                // Check if the package contains any scenes.
                if (mobileScenePackage.Scenes.Count > 0)
                {
                    // Use the first scene in the package.
                    scene = mobileScenePackage.Scenes[0];

                    // Set the view model "Scene" property.
                    this.Scene = scene;

                    Console.WriteLine("Scene setup completed successfully.");
                }
                else
                {
                    Console.WriteLine("No scenes found in the mobile scene package.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during scene loading.
                Console.WriteLine("Failed to load the scene: " + ex.Message);
            }



            // Show the layer in the scene.
            //this.Scene.OperationalLayers.Add(TAGraphicsOverlay);

        }

        public List<TrajectoryPoint> Trajectory { get; set; }

        public void LoadTrajectoryData()
        {
            Trajectory = new List<TrajectoryPoint>
            {


                new TrajectoryPoint(35.6962,51.4229,0),
                new TrajectoryPoint(35.5962,50.6116,8888.89   ),
                new TrajectoryPoint(35.4962,49.8003,17777.78  ),
                new TrajectoryPoint(35.3962,48.9890,26666.67  ),
                new TrajectoryPoint(35.2962,48.1777,35555.56  ),
                new TrajectoryPoint(35.1962,47.3664,44444.44  ),
                new TrajectoryPoint(35.0962,46.5551,53333.33  ),
                new TrajectoryPoint(34.9962,45.7438,62222.22  ),
                new TrajectoryPoint(34.8962,44.9325,71111.11  ),
                new TrajectoryPoint(34.7962,44.1212,80000     ),
                new TrajectoryPoint(34.6962,43.3099,71111.11  ),
                new TrajectoryPoint(34.5962,42.4986,62222.22  ),
                new TrajectoryPoint(34.4962,41.6873,53333.33  ),
                new TrajectoryPoint(34.3962,40.8760,44444.44  ),
                new TrajectoryPoint(34.2962,40.0647,35555.56  ),
                new TrajectoryPoint(34.1962,39.2534,26666.67  ),
                new TrajectoryPoint(34.0962,38.4421,20000.78  ),
                new TrajectoryPoint(33.9962,37.6308,17000   ),
                new TrajectoryPoint(33.8962,36.8195,4000.0    ),
                new TrajectoryPoint(33.8886,35.495,0          )
            };
        }

        public async Task AddTrajectoryToScene()
        {
            // Check if the scene is null
            if (this.Scene == null)
            {
                Console.WriteLine("Scene is null. Cannot add trajectory.");
                return;
            }

            try
            {
                // Load the trajectory data
                PolylineBuilder polylineBuilder = new PolylineBuilder(SpatialReferences.Wgs84);

                foreach (var point in Trajectory)
                {
                    polylineBuilder.AddPoint(point.Longitude, point.Latitude, point.Altitude);
                }

                Polyline polyline = polylineBuilder.ToGeometry();

                if (polylineBuilder.Parts.Count > 0)
                {
                    // Create a polyline from the builder

                    var polylineGraphic = new Graphic(polyline);
                    polylineGraphic.IsVisible = true;
                    polylineGraphic.Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Red, 6);
                    //polylineGraphic.Symbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 13);
                    CreateGraphics(polylineGraphic);
                }
                // Create a symbol for the point


                Console.WriteLine("Trajectory added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load the trajectory: " + ex.Message);
            }
        }
        //public Esri.ArcGISRuntime.UI.GraphicsOverlay TAGraphicsOverlay;
        // Modified CreateGraphics to accept a Graphic parameter
        private void CreateGraphics(Graphic polylineGraphic)
        {
            // Check if the GraphicsOverlays collection is initialized, if not, initialize it.
            if (GraphicsOverlays == null)
            {
                GraphicsOverlays = new GraphicsOverlayCollection();
                // Set the SceneProperties of the GraphicsOverlay to use SurfacePlacement.Absolute
            }

            // Check if there is already a GraphicsOverlay to add the Graphic to, if not, create a new one.

            GraphicsOverlay TAGraphicsOverlay = new GraphicsOverlay();

            if (GraphicsOverlays.Count == 0)
            {
                TAGraphicsOverlay = new GraphicsOverlay();
                GraphicsOverlays.Add(TAGraphicsOverlay);
            }
            else
            {
                // Assuming you want to add the new graphic to the first overlay in the collection
                //   TAGraphicsOverlay = GraphicsOverlays.First();
            }

            // Add the polylineGraphic to the selected or new GraphicsOverlay
            if (polylineGraphic != null)
            {
                TAGraphicsOverlay.Graphics.Add(polylineGraphic);
                TAGraphicsOverlay.IsVisible = true;
                TAGraphicsOverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;

                OnPropertyChanged();
            }

        }
        //// Set the view model "Scene" property.
        //this.Scene = scene;

        private void AddPointToScene(double latitude, double longitude)
        {
            // Create a point geometry
            MapPoint point = new MapPoint(latitude, longitude, SpatialReferences.Wgs84);

            // Create a symbol for the point
            SimpleMarkerSymbol pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 10);

            // Create a graphic for the point
            Graphic pointGraphic = new Graphic(point, pointSymbol);

            // Add the point graphic to the graphics overlay
            CreateGraphics(pointGraphic);
        }

    }



    public class TrajectoryPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public TrajectoryPoint(double latitude, double longitude, double altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }
    }

}

