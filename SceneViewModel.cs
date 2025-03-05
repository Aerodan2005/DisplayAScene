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


using DisplayAMap;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;

using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

// add using to the display a map namespace
//using DisplayAMap;

namespace DisplayAScene
{

    public partial class SceneViewModel : INotifyPropertyChanged
    {


        public SceneViewModel()
        {
            SetupScene();
            Scene = new Scene(BasemapStyle.ArcGISImageryStandard);
            MyBodyView = new Scene(BasemapStyle.ArcGISImageryStandard);

            // Initialize fields
            missileGraphic = new Graphic();
            ballGraphic = new Graphic();

            // Disable model provider to prevent interference with MetisDB
            MissileModelProvider.Disable();
            
            // Don't try to get missile info automatically on startup
            // as it might interfere with MetisDB application
            // TryGetModelFromMetisDB();
            
            // Subscribe to model changes for future updates
            MissileModelProvider.ModelInfoChanged += OnMissileModelInfoChanged;
        }

        // Try to get the missile model from MetisDB
        private void TryGetModelFromMetisDB()
        {
            if (MissileModelProvider.TryGetModelPathFromMetisDB())
            {
                // If successful, update our properties
                MissileModelPath = MissileModelProvider.ModelPath;
                SelectedMissileId = MissileModelProvider.SelectedMissileId;
                SelectedMissilePart = MissileModelProvider.SelectedMissilePart;
                Console.WriteLine($"Successfully loaded missile model: {MissileModelPath}");
            }
            else
            {
                Console.WriteLine("Failed to load missile model from MetisDB. Using default model.");
            }
        }
        
        // Event handler for missile model changes
        private void OnMissileModelInfoChanged(object sender, EventArgs e)
        {
            MissileModelPath = MissileModelProvider.ModelPath;
            SelectedMissileId = MissileModelProvider.SelectedMissileId;
            SelectedMissilePart = MissileModelProvider.SelectedMissilePart;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
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
        private Scene _myBodyView;
        public Scene? MyBodyView
        {
            get { return _myBodyView; }
            set
            {
                _myBodyView = value;
                OnPropertyChanged();
            }
        }

        private SceneView? _sceneView;
        public SceneView? SceneView
        {
            get { return _sceneView; }
            set
            {
                _sceneView = value;
                OnPropertyChanged();
            }
        }

        private string _missileAltTxt;
        public string MissileAltTxt
        {
            get { return _missileAltTxt; }
            set
            {
                _missileAltTxt = value;
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
        public Scene scene;
        public Scene myBodyView;
        private async Task SetupScene()
        {
            // Create a new scene with an imagery basemap.
            scene = new Scene(BasemapStyle.OSMHybrid);
            myBodyView = new Scene(BasemapStyle.OSMHybrid);
            SceneView = new SceneView();
 
            MissileAltTxt = "123";
            InitializeSceneView();
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
                    this.MyBodyView = scene;


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

            try
            {

                // Load the first TPK file
                string tpkPath1 = @"C:\Work\TilePackages\world_imagery_tpk.tpk";
                if (!File.Exists(tpkPath1))
                {
                    throw new FileNotFoundException("TPK file not found.", tpkPath1);
                }
                var tiledLayer1 = new ArcGISTiledLayer(new Uri(tpkPath1));
                //await tiledLayer1.LoadAsync();

                // Load the second TPK file
                string tpkPath2 = @"C:\Work\TilePackages\world_boundaries_and_places_4-11.tpk";
                if (!File.Exists(tpkPath2))
                {
                    throw new FileNotFoundException("TPK file not found.", tpkPath2);
                }
                var tiledLayer2 = new ArcGISTiledLayer(new Uri(tpkPath2));
                //await tiledLayer2.LoadAsync();

                //// Load the VTPK file
                //string vtpkPath = @"C:\Work\BaseMaps\Hybrid.vtpk";
                //if (!File.Exists(vtpkPath))
                //{
                //    throw new FileNotFoundException("VTPK file not found.", vtpkPath);
                //}
                //var vectorTiledLayer = new ArcGISVectorTiledLayer(new Uri(vtpkPath));
                //await vectorTiledLayer.LoadAsync();

                // Create a new basemap with the first tiled layer
                var basemap = new Basemap(tiledLayer1);

                // Set the basemap to the map
                this.Scene.Basemap = basemap;
                this.MyBodyView.Basemap = basemap;

                //Add the second tiled layer as an operational layer
                this.Scene.OperationalLayers.Add(tiledLayer2);
                //this.MyBodyView.OperationalLayers.Add(tiledLayer2);

                // Add the vector tiled layer as an operational layer
                //this.Map.OperationalLayers.Add(vectorTiledLayer);

                // Add the first map from the mobile map package as the last layer
                //var mobileMap = mobileMapPackage.Maps.First();
                //foreach (var layer in mobileMap.OperationalLayers)
                //{
                //    this.Map.OperationalLayers.Add(layer);
                //}
                // 


            }
            catch (Exception ex)
            {
                // Log the exception message
                Debug.WriteLine($"Exception: {ex.Message}");
                MessageBox.Show($"Failed to load map layers: {ex.Message}");
            }

            // Show the layer in the scene.
            //this.Scene.OperationalLayers.Add(TAGraphicsOverlay);

        }
        public void InitializeSceneView()
        {
            if (SceneView != null)
            {
                SceneView.Scene = MyBodyView;
            }
        }

        async private void SetupCamera()
        {

            // Change the scene's point of view
            double newLatitude = ((DataStore.Trajectory[0].Latitude + DataStore.Trajectory[DataStore.Trajectory.Count - 1].Latitude) / 2.0);
            double newLongitude = (DataStore.Trajectory[0].Longitude + DataStore.Trajectory[DataStore.Trajectory.Count - 1].Longitude) / 2.0;
            double newAltitude = 600000.0;

            // Create a new Camera instance with the specified parameters
            //Camera camera = new Camera(newLatitude, newLongitude, newAltitude, 0, 70, 0); // Heading = 0 (North), Pitch = 70, Roll = 0

            // Create the OrbitGeoElementCameraController to follow a specific point
            MapPoint targetPoint = new MapPoint(newLongitude, newLatitude, 200000, SpatialReferences.Wgs84);
            MapPoint cameraPoint = new MapPoint(newLongitude, newLatitude - 20, 100000, SpatialReferences.Wgs84);
            CameraController cameraController = new OrbitLocationCameraController(targetPoint, cameraPoint);
            this.SceneView.CameraController = cameraController;
            OnPropertyChanged();

        }


        public void LoadTrajectoryData()
        {
        }
        public double ThetaGeneral = 90.0;
        public double PsiGeneral = -90.0;
        public double PhiGeneral = 10.0;
        public async Task AddTrajectoryToScene()
        {
            // Check if the scene is null
            if (this.Scene == null)
            {
                Console.WriteLine("Scene is null. Cannot add trajectory.");
                return;
            }
            double Theta = 90.0;
            double Psi = -90.0;
            double Phi = 10.0;
            try
            {
                // Load the trajectory data
                PolylineBuilder polylineBuilder = new PolylineBuilder(SpatialReferences.Wgs84);

                foreach (var point in DataStore.Trajectory)
                {
                    polylineBuilder.AddPoint(point.Longitude, point.Latitude, point.Altitude * 1000.0);
                }

                Polyline polyline = polylineBuilder.ToGeometry();

                if (polylineBuilder.Parts.Count > 0)
                {
                    // Create a polyline from the builder

                    var polylineGraphic = new Graphic(polyline);
                    polylineGraphic.IsVisible = true;
                    polylineGraphic.Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(128, System.Drawing.Color.Red), 6);
                    CreateGraphics(polylineGraphic);
                }



                Console.WriteLine("Trajectory added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load the trajectory: " + ex.Message);
            }
        }

        // Modified CreateGraphics to accept a Graphic parameter
        private void CreateGraphics(Graphic anyGraphic)
        {
            // Check if the GraphicsOverlays collection is initialized, if not, initialize it.
            if (GraphicsOverlays == null)
            {
                GraphicsOverlays = new GraphicsOverlayCollection();
                // Set the SceneProperties of the GraphicsOverlay to use SurfacePlacement.Absolute
            }

            // Check if there is already a GraphicsOverlay to add the Graphic to, if not, create a new one.

            GraphicsOverlay TAGraphicsOverlay = new GraphicsOverlay();

            if (GraphicsOverlays.Count >= 0)
            {
                TAGraphicsOverlay = new GraphicsOverlay();
                GraphicsOverlays.Add(TAGraphicsOverlay);
            }


            // Add the anyGraphic to the selected or new GraphicsOverlay
            if (anyGraphic != null)
            {
                TAGraphicsOverlay.Graphics.Add(anyGraphic);
                TAGraphicsOverlay.IsVisible = true;
                TAGraphicsOverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;

                OnPropertyChanged();
            }

        }
        //// Set the view model "Scene" property.
        //this.Scene = scene;
        // Method to remove a specific graphic
        public void RemoveGraphic(Graphic graphic)
        {
            if (GraphicsOverlays != null)
            {
                foreach (var overlay in GraphicsOverlays)
                {
                    if (overlay.Graphics.Contains(graphic))
                    {
                        overlay.Graphics.Remove(graphic);
                        OnPropertyChanged();
                        break;
                    }
                }
            }
        }
        // Method to clear all graphics
        public void ClearAllGraphics()
        {
            if (GraphicsOverlays != null)
            {
                foreach (var overlay in GraphicsOverlays)
                {
                    overlay.Graphics.Clear();
                }
                OnPropertyChanged();
            }
        }
        public Graphic missileGraphic;
        public ModelSceneSymbol missileSymbol;
        public Graphic ballGraphic;
        // Camera controller for centering the camera on the missile
        private OrbitGeoElementCameraController _orbitCameraController;

        private string _missileModelPath = "C:\\Work\\display-a-scene\\3D Objects\\singleX.obj"; // Default path
        public string MissileModelPath
        {
            get 
            { 
                // If MissileModelProvider is enabled and has a valid path, use it
                if (!string.IsNullOrEmpty(MissileModelProvider.ModelPath) && 
                    File.Exists(MissileModelProvider.ModelPath) &&
                    !MissileModelProvider.IsDisabled)
                {
                    _missileModelPath = MissileModelProvider.ModelPath;
                }
                return _missileModelPath; 
            }
            set
            {
                _missileModelPath = value;
                OnPropertyChanged();
            }
        }

        private int _selectedMissileId;
        public int SelectedMissileId
        {
            get { return _selectedMissileId; }
            set
            {
                _selectedMissileId = value;
                OnPropertyChanged();
            }
        }

        private string _selectedMissilePart = "UN"; // Default to UN
        public string SelectedMissilePart
        {
            get { return _selectedMissilePart; }
            set
            {
                _selectedMissilePart = value;
                OnPropertyChanged();
            }
        }

        // Method to set missile information from external source (e.g., MetisDB)
        public void SetMissileInfo(int missileId, string missilePart, string modelPath)
        {
            SelectedMissileId = missileId;
            SelectedMissilePart = missilePart;
            if (!string.IsNullOrEmpty(modelPath))
            {
                MissileModelPath = modelPath;
            }
        }

        private async Task AddPointToScene(double latitude, double longitude, double altitude, string input, string modelPath = null)
        {
            // Create a point geometry
            MapPoint point = new MapPoint(longitude, latitude, altitude, SpatialReferences.Wgs84);

            if (!string.IsNullOrEmpty(modelPath))
            {
                try
                {
                    // Create a 3D model symbol using the provided model path
                    ModelSceneSymbol pointModelSymbol = await ModelSceneSymbol.CreateAsync(new Uri(modelPath), 1.0);
                    
                    // Create a graphic using the model symbol
                    Graphic modelGraphic = new Graphic(point, pointModelSymbol);
                    
                    // Add the model graphic to the graphics overlay
                    CreateGraphics(modelGraphic);
                }
                catch (Exception ex)
                {
                    // Fallback to simple marker if model loading fails
                    Console.WriteLine($"Failed to load model: {ex.Message}");
                    SimpleMarkerSymbol pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 10);
                    Graphic pointGraphic = new Graphic(point, pointSymbol);
                    CreateGraphics(pointGraphic);
                }
            }
            else
            {
                // Create a symbol for the point
                SimpleMarkerSymbol pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Blue, 10);

                // Create a graphic for the point
                Graphic pointGraphic = new Graphic(point, pointSymbol);

                // Add the point graphic to the graphics overlay
                CreateGraphics(pointGraphic);
            }

            // Display the input above the point
            if (!string.IsNullOrEmpty(input))
            {
                TextSymbol textSymbol = new TextSymbol(input, System.Drawing.Color.Blue, 24, Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Left, Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                Graphic textGraphic = new Graphic(point, textSymbol);
                CreateGraphics(textGraphic);
            }
        }

        public async Task GoNextPt(int ind = 0)
        {
            RemoveGraphic(missileGraphic);
            RemoveGraphic(ballGraphic);
            LoadTrajectoryData();
            try
            {
                double latitude = DataStore.Trajectory[ind].Latitude;
                double longitude = DataStore.Trajectory[ind].Longitude;
                double altitude = DataStore.Trajectory[ind].Altitude;
                CalculateBodyAngles(ind);
                
                // Try to get the latest missile model path from MissileModelProvider
                // This will ensure we use the currently selected missile in MetisDB
                if (string.IsNullOrEmpty(MissileModelPath) || MissileModelPath == "C:\\Work\\display-a-scene\\3D Objects\\singleX.obj")
                {
                    // If no model is currently set or we're using the default, try to get the latest
                    MissileModelPath = MissileModelProvider.ModelPath;
                    SelectedMissileId = MissileModelProvider.SelectedMissileId;
                    SelectedMissilePart = MissileModelProvider.SelectedMissilePart;
                    Console.WriteLine($"Using missile model: ID={SelectedMissileId}, Part={SelectedMissilePart}, Path={MissileModelPath}");
                }

                try
                {
                    // Create the model symbol with the proper orientation
                    missileSymbol = await ModelSceneSymbol.CreateAsync(new Uri(MissileModelPath), 1.0);
                    missileSymbol.Heading = DataStore.Trajectory[ind].Heading + PhiGeneral;
                    missileSymbol.Pitch = DataStore.Trajectory[ind].Pitch + ThetaGeneral;
                    missileSymbol.Roll = DataStore.Trajectory[ind].Roll;

                    // Create a graphic using the missile symbol.
                    missileGraphic = new Graphic(new MapPoint(longitude, latitude, altitude * 1000, SpatialReferences.Wgs84), missileSymbol);
                    CreateGraphics(missileGraphic);

                    // Add a ball (point) at the same location
                    MapPoint point = new MapPoint(longitude, latitude, altitude * 1000.0, SpatialReferences.Wgs84);
                    SimpleMarkerSymbol pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Red, 15);
                    ballGraphic = new Graphic(point, pointSymbol);
                    CreateGraphics(ballGraphic);
                }
                catch (Exception ex)
                {
                    // Handle the exception
                    Console.WriteLine("Failed to go to next point: " + ex.Message);
                    
                    // Fallback to simple point representation - use the already declared variables
                    await AddPointToScene(latitude, longitude, altitude * 1000, $"Point {ind}", MissileModelPath);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GoNextPt: {ex.Message}");
            }
        }

        public async Task GoThroughTrajectory()
 {



            for (int i = 0; i < DataStore.Trajectory.Count; i++)
            {
                await GoNextPt(i);
                MissileAltTxt = DataStore.Trajectory[i].Altitude.ToString()  + " km";
                RemoveGraphic(ballGraphic);


                // Change the scene's point of view
                double newLatitude = DataStore.Trajectory[i].Latitude;
                double newLongitude =DataStore.Trajectory[i].Longitude;
                double newAltitude = DataStore.Trajectory[i].Altitude * 1000;

                // Create a new Camera instance with the specified parameters
                //Camera camera = new Camera(newLatitude, newLongitude, newAltitude, 0, 70, 0); // Heading = 0 (North), Pitch = 70, Roll = 0

                // Create the OrbitGeoElementCameraController to follow a specific point
                MapPoint targetPoint = new MapPoint(newLongitude, newLatitude, newAltitude, SpatialReferences.Wgs84);
                MapPoint cameraPoint = new MapPoint(newLongitude, newLatitude - 0.1, newAltitude, SpatialReferences.Wgs84);
                CameraController cameraController = new OrbitLocationCameraController(targetPoint, cameraPoint);


                // Create the orbit camera controller to follow the missile
                _orbitCameraController = new OrbitGeoElementCameraController(missileGraphic, 15000.0)
                {
                    //CameraPitchOffset = 90.0,
                    //CameraHeadingOffset = 90.0,
                };
                //this.MyBodyView.InitialViewpoint = _orbitCameraController;
                // Set the CameraController on the SceneView instead of the Scene
                if (this.SceneView != null)
                {
                    this.SceneView.CameraController = cameraController; // _orbitCameraController;
                }


                // Use Task.Delay instead of Thread.Sleep to avoid blocking the main thread
                await Task.Delay(100);

            }
        }

        public async Task GoThroughTrajectoryBall(MapViewModel mapViewModel)
        {
            SetupCamera();

            for (int i = 0; i < DataStore.Trajectory.Count; i++)
            {
                await GoNextPt(i);

                var currentPoint = DataStore.Trajectory[i];

                // Add blue circle markers at each data point
                if (currentPoint.Altitude == 87.0)
                {
                    await AddPointToScene(currentPoint.Latitude, currentPoint.Longitude, currentPoint.Altitude * 1000.0, "Seperation");
                }

                if (currentPoint.Altitude == 400)
                {
                    await AddPointToScene(currentPoint.Latitude, currentPoint.Longitude, currentPoint.Altitude * 1000.0, "Apogea 400 km");
                }

                Esri.ArcGISRuntime.Geometry.MapPoint point = new Esri.ArcGISRuntime.Geometry.MapPoint(currentPoint.Longitude, currentPoint.Latitude);

                mapViewModel.AddTrajectoryToMap(point);

                // Use Task.Delay instead of Thread.Sleep to avoid blocking the main thread
                await Task.Delay(100);
            }
        }

        public void CalculateBodyAngles(int ind)
        {
            //for (int i = 0; i < DataStore.Trajectory.Count - 1; i++)
            int i = ind;
            {
                var currentPoint = DataStore.Trajectory[i];
                var nextPoint = DataStore.Trajectory[i + 1];

                double latDiff = nextPoint.Latitude - currentPoint.Latitude;
                double lonDiff = nextPoint.Longitude - currentPoint.Longitude;
                double altDiff = (nextPoint.Altitude - currentPoint.Altitude) * 1000.0;
                double forwardDiff = Math.Sqrt(latDiff * latDiff + lonDiff * lonDiff);
                forwardDiff = DegreesToMeters(forwardDiff, currentPoint.Latitude);

                double DEG2RAD = 0.017453292519943295;

                currentPoint.Heading = Math.Atan2(lonDiff * DEG2RAD, latDiff * DEG2RAD) * (180 / Math.PI); // Convert to degrees
                currentPoint.Pitch = Math.Atan2(altDiff, forwardDiff) * (180 / Math.PI); // Convert to degrees
                currentPoint.Roll = 0;
            }

            // Set the angles for the last point
            var lastPoint = DataStore.Trajectory[DataStore.Trajectory.Count - 1];
            lastPoint.Heading = 0;
            lastPoint.Pitch = 0;
            lastPoint.Roll = 0;
        }
        public double DegreesToMeters(double degrees, double latitude)
        {
            double earthRadius = 6371000; // Earth's radius in meters
            double radians = degrees * Math.PI / 180;
            double meters = earthRadius * radians * Math.Cos(latitude * Math.PI / 180);
            return meters;
        }




    }




}
