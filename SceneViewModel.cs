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
        // Static trajectory overlay index - always use the first overlay for trajectory
        private readonly int TRAJECTORY_OVERLAY_INDEX = 0;

        public SceneViewModel()
        {
            SetupScene();
            Scene = new Scene(BasemapStyle.ArcGISImageryStandard);
            MyBodyView = new Scene(BasemapStyle.ArcGISImageryStandard);

            // Initialize fields
            missileGraphic = new Graphic();
            ballGraphic = new Graphic();

            // Initialize GraphicsOverlays
            GraphicsOverlays = new GraphicsOverlayCollection();
            
            // Create a default graphics overlay for trajectory
            GraphicsOverlay trajectoryOverlay = new GraphicsOverlay();
            trajectoryOverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
            trajectoryOverlay.IsVisible = true;
            trajectoryOverlay.Id = "TrajectoryOverlay"; // Give it a unique ID
            GraphicsOverlays.Add(trajectoryOverlay);
            
            Console.WriteLine("SceneViewModel initialized with graphics overlay");
            
            // Clear any default trajectory data
            DataStore.Trajectory.Clear();
            
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
            // Restore the original path that was working
            string scenePath = @"C:\Work\dataroot\dataroot\Maps\MED2.mspk";
            System.Diagnostics.Debug.WriteLine($"Loading scene package from: {scenePath}");

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

                // Set the basemap to the map - with null checks
                if (this.Scene != null)
                {
                    this.Scene.Basemap = basemap;
                }
                
                if (this.MyBodyView != null)
                {
                    this.MyBodyView.Basemap = basemap;
                    
                    // Add the second tiled layer as an operational layer
                    if (this.Scene?.OperationalLayers != null)
                    {
                        this.Scene.OperationalLayers.Add(tiledLayer2);
                    }
                    //this.MyBodyView.OperationalLayers.Add(tiledLayer2);
                }
                
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
            if (SceneView != null && MyBodyView != null)
            {
                SceneView.Scene = MyBodyView;
            }
        }

        async public void SetupCamera()
        {
            try
            {
                // Calculate trajectory extents for proper camera positioning
                double minLongitude = double.MaxValue;
                double maxLongitude = double.MinValue;
                double minLatitude = double.MaxValue;
                double maxLatitude = double.MinValue;
                double maxAltitude = 0;
                
                // Get the bounds of the trajectory
                foreach (var point in DataStore.Trajectory)
                {
                    minLongitude = Math.Min(minLongitude, point.Longitude);
                    maxLongitude = Math.Max(maxLongitude, point.Longitude);
                    minLatitude = Math.Min(minLatitude, point.Latitude);
                    maxLatitude = Math.Max(maxLatitude, point.Latitude);
                    maxAltitude = Math.Max(maxAltitude, point.Altitude);
                }
                
                Console.WriteLine($"Trajectory bounds: Lon({minLongitude} to {maxLongitude}), Lat({minLatitude} to {maxLatitude}), MaxAlt: {maxAltitude}km");
                
                // Calculate center point
                double centerLatitude = (minLatitude + maxLatitude) / 2.0;
                double centerLongitude = (minLongitude + maxLongitude) / 2.0;
                
                // Calculate the distance between min and max points (in degrees)
                double longitudeSpan = Math.Abs(maxLongitude - minLongitude);
                double latitudeSpan = Math.Abs(maxLatitude - minLatitude);
                
                // Add a buffer to ensure full trajectory visibility (20% buffer)
                double bufferFactor = 0.2;
                minLongitude -= longitudeSpan * bufferFactor;
                maxLongitude += longitudeSpan * bufferFactor;
                
                // Recalculate center and span with buffer
                centerLongitude = (minLongitude + maxLongitude) / 2.0;
                longitudeSpan = Math.Abs(maxLongitude - minLongitude);
                
                // For ballistic trajectories, we need a good side view
                // Set altitude based on max altitude and span to ensure the entire trajectory is visible
                double altitude = Math.Max(longitudeSpan * 111000 * 1.5, maxAltitude * 1000 * 2.5);
                
                // Ensure minimum altitude for good visibility
                altitude = Math.Max(altitude, 500000); // At least 500km altitude
                
                // For westward trajectories, position the camera to the east and looking west
                // This gives us a side view of the ballistic arc
                double cameraLongitude = maxLongitude + (longitudeSpan * 0.1);
                double cameraLatitude = centerLatitude;
                
                // Create camera with heading 270 (looking west across the trajectory)
                // For ballistic trajectory, use a smaller pitch angle to see the arc better
                Camera camera = new Camera(cameraLatitude, cameraLongitude, altitude, 270, 30, 0);
                // Heading 270 looks west, pitch 30 looks down at a 30-degree angle
                
                // Apply the camera to the scene view
                this.SceneView.SetViewpointCamera(camera);
                
                Console.WriteLine($"Camera setup for ballistic trajectory - viewing from east to west at altitude: {altitude}m");
                Console.WriteLine($"Camera position: Lat {cameraLatitude}, Long {cameraLongitude}");
                Console.WriteLine($"Trajectory spans: Long {minLongitude} to {maxLongitude}, MaxAlt: {maxAltitude}km");
                
                // Check if this is a Shahed trajectory based on altitude values
                bool isShahedTrajectory = maxAltitude < 10.0; // If max altitude is very small (< 10km), it's likely a Shahed with 1,000,000 scaling
                
                if (isShahedTrajectory)
                {
                    // For Shahed launched from Beirut, adjust camera accordingly
                    double beirutLat = 33.8938;
                    double beirutLon = 35.5018;
                    double targetDistance = 100.0; // km
                    
                    // Check if first point is close to Beirut (confirming it was relocated)
                    bool launchedFromBeirut = Math.Abs(DataStore.Trajectory[0].Latitude - beirutLat) < 1.0 &&
                                             Math.Abs(DataStore.Trajectory[0].Longitude - beirutLon) < 1.0;
                    
                    if (launchedFromBeirut)
                    {
                        Console.WriteLine("Detected Shahed launched from Beirut - adjusting camera");
                        
                        // Get first and last points to calculate direction
                        var firstPoint = DataStore.Trajectory[0];
                        var lastPoint = DataStore.Trajectory[DataStore.Trajectory.Count - 1];
                        
                        // Calculate camera position east of trajectory to look westward
                        double cameraAltKm = 100.0; // Higher altitude for Shahed trajectory
                        
                        // Position camera southeast of Beirut looking northwest
                        Camera shahedCamera = new Camera(
                            beirutLat - 1.0, // 1 degree south of Beirut
                            beirutLon + 1.0, // 1 degree east of Beirut
                            cameraAltKm * 1000, // Convert to meters
                            0, // Heading - looking west
                            45, // Pitch - looking down at 45 degrees
                            0  // Roll
                        );
                        
                        // Update camera
                        SceneView.SetViewpointCamera(shahedCamera);
                        return;
                    }
                }
                
                OnPropertyChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetupCamera: {ex.Message}");
            }
        }
        public void LoadTrajectoryData()
        {
            try
            {
                // Log the beginning of trajectory data loading
                Console.WriteLine($"LoadTrajectoryData called with {DataStore.Trajectory.Count} trajectory points");
                
                // Check if we have trajectory data to load
                if (DataStore.Trajectory == null || DataStore.Trajectory.Count < 2)
                {
                    Console.WriteLine("Warning: Insufficient trajectory data to load");
                    return;
                }
                
                // Only clear the trajectory graphics overlay
                if (GraphicsOverlays != null && GraphicsOverlays.Count > TRAJECTORY_OVERLAY_INDEX)
                {
                    // Clear only the trajectory overlay (index 0)
                    GraphicsOverlays[TRAJECTORY_OVERLAY_INDEX].Graphics.Clear();
                    Console.WriteLine("Trajectory graphics overlay cleared");
                }
                else
                {
                    // Create a new collection if it doesn't exist
                    Console.WriteLine("Creating new graphics overlay for trajectory");
                    if (GraphicsOverlays == null)
                    {
                        GraphicsOverlays = new GraphicsOverlayCollection();
                    }
                    
                    // Create a default graphics overlay for trajectory if it doesn't exist
                    if (GraphicsOverlays.Count <= TRAJECTORY_OVERLAY_INDEX)
                    {
                        GraphicsOverlay trajectoryOverlay = new GraphicsOverlay();
                        trajectoryOverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
                        trajectoryOverlay.IsVisible = true;
                        trajectoryOverlay.Id = "TrajectoryOverlay";
                        GraphicsOverlays.Add(trajectoryOverlay);
                    }
                }
                
                Console.WriteLine("Trajectory data loaded and ready for display");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadTrajectoryData: {ex.Message}");
            }
        }
        
        // Camera angle variables
        public double ThetaGeneral = 90.0;
        public double PsiGeneral = -90.0;
        public double PhiGeneral = 10.0;
        
        public async Task AddTrajectoryToScene()
        {
            // Check if the scene is null
            if (SceneView?.Scene == null)
            {
                Console.WriteLine("AddTrajectoryToScene - Scene is null, creating new scene");
                SceneView.Scene = new Scene(BasemapStyle.ArcGISImageryStandard);
            }

            try
            {
                Console.WriteLine($"AddTrajectoryToScene - Loading trajectory with {DataStore.Trajectory.Count} points");

                if (DataStore.Trajectory.Count < 2)
                {
                    Console.WriteLine("AddTrajectoryToScene - Not enough trajectory points to draw (minimum 2 required)");
                    return;
                }
                else
                {
                    Console.WriteLine($"AddTrajectoryToScene - Processing {DataStore.Trajectory.Count} trajectory points");
                    
                    // Ensure we have a trajectory overlay
                    if (GraphicsOverlays == null || GraphicsOverlays.Count <= TRAJECTORY_OVERLAY_INDEX)
                    {
                        Console.WriteLine("AddTrajectoryToScene - Creating trajectory overlay");
                        LoadTrajectoryData();  // This will create the overlay if needed
                    }
                    
                    // Always use the dedicated trajectory overlay
                    var trajectoryOverlay = GraphicsOverlays[TRAJECTORY_OVERLAY_INDEX];
                    
                    // Convert the list of custom trajectory points to a list of MapPoints
                    var trajPoints = DataStore.Trajectory;
                    var firstPoint = trajPoints[0];
                    
                    // Find max altitude point for highlighting
                    double maxAltitude = 0;
                    int maxAltitudeIndex = 0;
                    for (int i = 0; i < DataStore.Trajectory.Count; i++)
                    {
                        if (DataStore.Trajectory[i].Altitude > maxAltitude)
                        {
                            maxAltitude = DataStore.Trajectory[i].Altitude;
                            maxAltitudeIndex = i;
                        }
                    }
                    
                    // All trajectory types are now handled consistently with altitudes in kilometers
                    // No special detection for Shahed needed anymore 
                    
                    Console.WriteLine($"AddTrajectoryToScene - Max altitude: {maxAltitude} km at point {maxAltitudeIndex}");
                    
                    // Display altitude consistently in kilometers
                    string altitudeDisplay = $"Apogee ({maxAltitude:F1}km)";
                    
                    // Check if trajectory points need sorting to ensure linear path
                    bool requiresSorting = false;
                    if (DataStore.Trajectory.Count > 2)
                    {
                        // Calculate distances from first point as a heuristic
                        List<double> distances = new List<double>();
                        for (int i = 0; i < trajPoints.Count; i++)
                        {
                            double dist = Math.Sqrt(
                                Math.Pow(trajPoints[i].Latitude - firstPoint.Latitude, 2) + 
                                Math.Pow(trajPoints[i].Longitude - firstPoint.Longitude, 2));
                            distances.Add(dist);
                        }
                        
                        // Check if distances are monotonically increasing (or decreasing)
                        for (int i = 1; i < distances.Count; i++)
                        {
                            // If distance jumps back and forth, we need sorting
                            if ((distances[i] > distances[i-1] && i > 1 && distances[i-1] < distances[i-2]) ||
                                (distances[i] < distances[i-1] && i > 1 && distances[i-1] > distances[i-2]))
                            {
                                requiresSorting = true;
                                Console.WriteLine($"AddTrajectoryToScene - Non-linear trajectory detected at index {i}, requires sorting");
                                break;
                            }
                        }
                        
                        if (requiresSorting)
                        {
                            Console.WriteLine("AddTrajectoryToScene - Sorting trajectory points to ensure linear path...");
                            
                            // Sort by distance from start point as a heuristic approach
                            // This isn't perfect but helps in many cases
                            trajPoints = trajPoints.OrderBy(p => 
                                Math.Sqrt(
                                    Math.Pow(p.Latitude - firstPoint.Latitude, 2) + 
                                    Math.Pow(p.Longitude - firstPoint.Longitude, 2))
                            ).ToList();
                            
                            Console.WriteLine("AddTrajectoryToScene - Trajectory sorting complete");
                        }
                        else
                        {
                            Console.WriteLine("AddTrajectoryToScene - Trajectory appears to be properly ordered, no sorting needed");
                        }
                    }
                    
                    // Load the trajectory data
                    PolylineBuilder polylineBuilder = new PolylineBuilder(SpatialReferences.Wgs84);

                    foreach (var point in trajPoints)
                    {
                        double altitudeMeters;
                        
                        // Ensure first point is at ground level (0m elevation)
                        if (point == trajPoints[0])
                        {
                            // For the launch point (first point), set altitude to 0 for ground level start
                            altitudeMeters = 0;
                            Console.WriteLine($"DEBUG: Setting launch point altitude to 0m for ground level start (original was {point.Altitude}km)");
                        }
                        else
                        {
                            // For all other points, convert km to meters
                            altitudeMeters = point.Altitude * 1000.0; // Standard km to m conversion
                            Console.WriteLine($"DEBUG: point altitude: {point.Altitude}km -> {altitudeMeters}m (standard km to m conversion)");
                        }
                        
                        polylineBuilder.AddPoint(point.Longitude, point.Latitude, altitudeMeters);
                    }

                    Polyline polyline = polylineBuilder.ToGeometry();

                    if (polylineBuilder.Parts.Count > 0)
                    {
                        // Create a polyline from the builder with enhanced visibility
                        var polylineGraphic = new Graphic(polyline);
                        polylineGraphic.IsVisible = true;
                        
                        // Use a more visible blue color for the trajectory
                        polylineGraphic.Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 0, 0, 255), 8);
                        trajectoryOverlay.Graphics.Add(polylineGraphic);
                        
                        // Create positions for markers
                        double GetPointAltitude(TrajectoryPoint trajPoint)
                        {
                            // For the first point (launch point), force altitude to zero
                            if (trajPoint == trajPoints[0])
                            {
                                Console.WriteLine("Setting launch point marker altitude to 0 meters");
                                return 0.0;
                            }
                            
                            // All trajectory types now use the same standard conversion from km to meters
                            return trajPoint.Altitude * 1000.0; 
                        }
                        
                        MapPoint launchPoint = new MapPoint(
                            trajPoints[0].Longitude, 
                            trajPoints[0].Latitude, 
                            GetPointAltitude(trajPoints[0]), 
                            SpatialReferences.Wgs84);
                        
                        MapPoint impactPoint = new MapPoint(
                            trajPoints[trajPoints.Count - 1].Longitude, 
                            trajPoints[trajPoints.Count - 1].Latitude, 
                            GetPointAltitude(trajPoints[trajPoints.Count - 1]), 
                            SpatialReferences.Wgs84);
                        
                        // Create apogee point at max altitude
                        MapPoint apogeePoint = new MapPoint(
                            trajPoints[maxAltitudeIndex].Longitude, 
                            trajPoints[maxAltitudeIndex].Latitude, 
                            GetPointAltitude(trajPoints[maxAltitudeIndex]), 
                            SpatialReferences.Wgs84);
                        
                        // Marker for launch point (green)
                        var launchPointGraphic = new Graphic(launchPoint);
                        launchPointGraphic.Symbol = new SimpleMarkerSceneSymbol(SimpleMarkerSceneSymbolStyle.Sphere, 
                            System.Drawing.Color.LimeGreen, 2000, 2000, 2000, SceneSymbolAnchorPosition.Bottom);
                        trajectoryOverlay.Graphics.Add(launchPointGraphic);
                        
                        // Add a text label for the launch point
                        var launchTextSymbol = new TextSymbol("Launch Point", System.Drawing.Color.White, 12, 
                            Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Center, 
                            Esri.ArcGISRuntime.Symbology.VerticalAlignment.Bottom);
                        launchTextSymbol.OffsetY = 20; // Offset to position above the point
                        var launchTextGraphic = new Graphic(launchPoint, launchTextSymbol);
                        trajectoryOverlay.Graphics.Add(launchTextGraphic);
                        
                        // Marker for impact point (red)
                        var impactPointGraphic = new Graphic(impactPoint);
                        impactPointGraphic.Symbol = new SimpleMarkerSceneSymbol(SimpleMarkerSceneSymbolStyle.Sphere, 
                            System.Drawing.Color.Red, 2000, 2000, 2000, SceneSymbolAnchorPosition.Bottom);
                        trajectoryOverlay.Graphics.Add(impactPointGraphic);
                        
                        // Add a text label for the impact point
                        var impactTextSymbol = new TextSymbol("Impact Point", System.Drawing.Color.White, 12, 
                            Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Center, 
                            Esri.ArcGISRuntime.Symbology.VerticalAlignment.Bottom);
                        impactTextSymbol.OffsetY = 20; // Offset to position above the point
                        var impactTextGraphic = new Graphic(impactPoint, impactTextSymbol);
                        trajectoryOverlay.Graphics.Add(impactTextGraphic);
                        
                        // Marker for apogee point (yellow)
                        var apogeePointGraphic = new Graphic(apogeePoint);
                        apogeePointGraphic.Symbol = new SimpleMarkerSceneSymbol(SimpleMarkerSceneSymbolStyle.Sphere, 
                            System.Drawing.Color.Yellow, 10000, 10000, 10000, SceneSymbolAnchorPosition.Center);
                        trajectoryOverlay.Graphics.Add(apogeePointGraphic);
                        
                        // Add a text label for the apogee point
                        var apogeeTextSymbol = new TextSymbol(altitudeDisplay, System.Drawing.Color.Yellow, 12, 
                            Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Center, 
                            Esri.ArcGISRuntime.Symbology.VerticalAlignment.Bottom);
                        apogeeTextSymbol.OffsetY = 20; // Offset to position above the point
                        var apogeeTextGraphic = new Graphic(apogeePoint, apogeeTextSymbol);
                        trajectoryOverlay.Graphics.Add(apogeeTextGraphic);
                        
                        Console.WriteLine("AddTrajectoryToScene - Trajectory markers added: Launch, Impact, and Apogee points");
                    }

                    Console.WriteLine("AddTrajectoryToScene - Trajectory added successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddTrajectoryToScene - Failed to load the trajectory: " + ex.Message);
            }
        }

        // Modified CreateGraphics to accept a Graphic parameter
        private void CreateGraphics(Graphic anyGraphic)
        {
            // Check if the GraphicsOverlays collection is initialized, if not, initialize it.
            if (GraphicsOverlays == null)
            {
                GraphicsOverlays = new GraphicsOverlayCollection();
            }

            // Get or create the graphics overlay
            GraphicsOverlay graphicsOverlay;
            
            // Reuse existing overlay if available, or create a new one
            if (GraphicsOverlays.Count > 0)
            {
                graphicsOverlay = GraphicsOverlays[0];
            }
            else
            {
                graphicsOverlay = new GraphicsOverlay();
                graphicsOverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
                graphicsOverlay.IsVisible = true;
                GraphicsOverlays.Add(graphicsOverlay);
                Console.WriteLine("Created new graphics overlay");
            }

            // Add the graphic to the overlay
            if (anyGraphic != null)
            {
                graphicsOverlay.Graphics.Add(anyGraphic);
                Console.WriteLine($"Added graphic to overlay. Total graphics: {graphicsOverlay.Graphics.Count}");
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
            try
            {
                // First clear any existing trajectory data
                DataStore.Trajectory.Clear();
                
                // Then clear all graphics overlays
                if (GraphicsOverlays != null)
                {
                    foreach (var overlay in GraphicsOverlays)
                    {
                        overlay.Graphics.Clear();
                    }
                }
                
                Console.WriteLine("All graphics and trajectory data cleared from SceneViewModel");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing graphics: {ex.Message}");
            }
        }
        public Graphic missileGraphic;
        public ModelSceneSymbol missileSymbol;
        public Graphic ballGraphic;
        // Camera controller for centering the camera on the missile
        private OrbitGeoElementCameraController _orbitCameraController;

        private string _missileModelPath = "C:\\Users\\urika\\OneDrive\\THView\\dataroot\\3DModel\\singleX.obj"; // Default path
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
        
        private string _selectedMissileName = string.Empty;
        public string SelectedMissileName
        {
            get { return _selectedMissileName; }
            set
            {
                _selectedMissileName = value;
                OnPropertyChanged();
            }
        }

        // Properties and fields for camera position and orientation
        private double _cameraX;
        private double _cameraY;
        private double _cameraZ;
        private double _cameraHeading;
        private double _cameraPitch;
        private double _cameraRoll;

        // Field to store missile position geometry
        private MapPoint missile_geom;
        
        // Task delegate for loading the scene after initialization
        private Task _loadSceneTask;
        
        // Method to set missile information from external source (e.g., MetisDB)
        public void SetMissileInfo(int missileId, string missilePart, string modelPath, string missileName)
        {
            try
            {
                SelectedMissileId = missileId;
                SelectedMissilePart = missilePart;
                SelectedMissileName = missileName;
                
                if (!string.IsNullOrEmpty(modelPath) && File.Exists(modelPath))
                {
                    // Ensure the model path is in a valid format for URI creation
                    MissileModelPath = new Uri(modelPath).ToString();
                    System.Diagnostics.Debug.WriteLine($"SceneViewModel: Set missile info - ID={missileId}, Part={missilePart}, Name={missileName}, Path={modelPath}");
                    
                    // If we've already loaded the scene, update the missile model
                    if (SceneView?.GraphicsOverlays != null && missile_geom != null)
                    {
                        UpdateMissileModel();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SceneViewModel: Invalid model path: {modelPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SceneViewModel: Error setting missile info: {ex.Message}");
            }
        }
        
        private void UpdateMissileModel()
        {
            try
            {
                // Remove existing missile graphic
                if (missileGraphic != null && SceneView?.GraphicsOverlays?.FirstOrDefault()?.Graphics.Contains(missileGraphic) == true)
                {
                    SceneView.GraphicsOverlays.FirstOrDefault()?.Graphics.Remove(missileGraphic);
                }
                
                // Schedule task to update the missile model asynchronously
                UpdateMissileModelAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SceneViewModel: Error updating missile model: {ex.Message}");
            }
        }
        
        private async void UpdateMissileModelAsync()
        {
            try
            {
                // Create new missile symbol using the updated path
                missileSymbol = await ModelSceneSymbol.CreateAsync(new Uri(MissileModelPath));
                missileSymbol.AnchorPosition = SceneSymbolAnchorPosition.Center;
                missileSymbol.Height = 10;
                missileSymbol.Width = 10;
                missileSymbol.Depth = 10;
                
                // Create and add new missile graphic
                if (missile_geom != null)
                {
                    missileGraphic = new Graphic(missile_geom, missileSymbol);
                    SceneView?.GraphicsOverlays?.FirstOrDefault()?.Graphics.Add(missileGraphic);
                }
                
                System.Diagnostics.Debug.WriteLine($"SceneViewModel: Updated missile model with path: {MissileModelPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SceneViewModel: Error updating missile model async: {ex.Message}");
            }
        }

        public async Task GoNextPt(int ind = 0)
        {
            RemoveGraphic(missileGraphic);
            RemoveGraphic(ballGraphic);
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
                    
                    // If we've already loaded the scene, update the missile model
                    if (SceneView?.GraphicsOverlays != null && missile_geom != null)
                    {
                        UpdateMissileModel();
                    }
                }

                try
                {
                    // Determine if we need a larger scale factor for smaller models like Shahed
                    double scaleMultiplier = 1.0;
                    
                    // Check trajectory apogee to identify Shahed (low altitude trajectory)
                    bool isShahedModel = false;
                    double maxAltitude = DataStore.Trajectory.Max(p => p.Altitude);
                    if (maxAltitude < 10.0) // If max altitude is less than 10km, it's likely a Shahed
                    {
                        isShahedModel = true;
                        scaleMultiplier = 20.0; // Much larger scale for Shahed models
                        Console.WriteLine($"Detected Shahed model - applying larger scale factor: {scaleMultiplier}");
                    }
                    
                    // Create the model symbol with the proper orientation and scaling
                    missileSymbol = await ModelSceneSymbol.CreateAsync(new Uri(MissileModelPath), scaleMultiplier);
                    missileSymbol.Heading = DataStore.Trajectory[ind].Heading + PhiGeneral;
                    missileSymbol.Pitch = DataStore.Trajectory[ind].Pitch + ThetaGeneral;
                    missileSymbol.Roll = DataStore.Trajectory[ind].Roll;
                    
                    // Create a graphic using the missile symbol.
                    MapPoint point = new MapPoint(longitude, latitude, altitude * 1000, SpatialReferences.Wgs84);
                    missile_geom = point;
                    missileGraphic = new Graphic(missile_geom, missileSymbol);
                    CreateGraphics(missileGraphic);

                    // Add a ball (point) at the same location
                    MapPoint point2 = new MapPoint(longitude, latitude, altitude * 1000.0, SpatialReferences.Wgs84);
                    SimpleMarkerSymbol pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Red, 15);
                    ballGraphic = new Graphic(point2, pointSymbol);
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
                _orbitCameraController = new OrbitGeoElementCameraController(missileGraphic, 20000.0) // Reduced distance from 50000 to 20000 since coordinates are normalized
                {
                    TargetVerticalScreenFactor = 0.5, // Center the target in the view
                    CameraPitchOffset = 45.0, // Keep pitch offset to see more of the object
                    CameraHeadingOffset = 30.0, // Keep heading offset for better viewing angle
                };
                //this.MyBodyView.InitialViewpoint = _orbitCameraController;
                // Set the CameraController on the SceneView instead of the Scene
                if (this.SceneView != null)
                {
                    // Use the orbit camera controller for better viewing of the entire object
                    this.SceneView.CameraController = _orbitCameraController; // Changed from cameraController to _orbitCameraController
                    Console.WriteLine("Body view camera set to orbit controller with normalized coordinates");
                }


                // Use Task.Delay instead of Thread.Sleep to avoid blocking the main thread
                await Task.Delay(100);

            }
        }

        public async Task GoThroughTrajectoryBall(MapViewModel mapViewModel)
        {
            // Make sure the trajectory line is drawn before the animation begins
            try
            {
                // First ensure the camera is properly set
                SetupCamera();
                
                // Draw the complete trajectory line immediately
                await AddTrajectoryToScene();
                
                Console.WriteLine("Added trajectory line to scene before animation");
                
                // Give a small delay to ensure the line appears
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing trajectory line: {ex.Message}");
            }

            // Now go through the points for animation
            for (int i = 0; i < DataStore.Trajectory.Count; i++)
            {
                await GoNextPt(i);

                var currentPoint = DataStore.Trajectory[i];



                Esri.ArcGISRuntime.Geometry.MapPoint point = new Esri.ArcGISRuntime.Geometry.MapPoint(currentPoint.Longitude, currentPoint.Latitude);

                mapViewModel.AddTrajectoryToMap(point);

                // Slow down the animation to better visualize the trajectory
                // Increase delay from 100ms to 400ms per point
                await Task.Delay(100);
                
                // Log progress for debugging
//                 if (i % 5 == 0) // Log every 5th point to avoid console spam
//                 {
//                     Console.WriteLine($"Animation progress: point {i+1}/{DataStore.Trajectory.Count}");
//                 }
            }
        }

        public void CalculateBodyAngles(int ind = 0)
        {
            // Check if we have trajectory data
            if (DataStore.Trajectory == null || DataStore.Trajectory.Count < 2)
            {
                Console.WriteLine("CalculateBodyAngles - Not enough trajectory points (minimum 2 required)");
                return;
            }
            
            // Calculate angles for all points
            for (int i = 0; i < DataStore.Trajectory.Count - 1; i++)
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
            
            Console.WriteLine("CalculateBodyAngles - Angles calculated for all trajectory points");
        }
        public double DegreesToMeters(double degrees, double latitude)
        {
            double earthRadius = 6371000; // Earth's radius in meters
            double radians = degrees * Math.PI / 180;
            double meters = earthRadius * radians * Math.Cos(latitude * Math.PI / 180);
            return meters;
        }

        // Method to add a point to the scene with an optional model
        private async Task AddPointToScene(double latitude, double longitude, double altitude, string input, string modelPath = null)
        {
            // Create a point geometry
            MapPoint point = new MapPoint(longitude, latitude, altitude, SpatialReferences.Wgs84);
            
            // Store this point for missile positioning
            missile_geom = point;

            if (!string.IsNullOrEmpty(modelPath))
            {
                try
                {
                    // Create a 3D model symbol using the provided model path
                    ModelSceneSymbol pointModelSymbol = await ModelSceneSymbol.CreateAsync(new Uri(modelPath));
                    
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
                TextSymbol textSymbol = new TextSymbol(input, System.Drawing.Color.Blue, 24, 
                    Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Left, 
                    Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                Graphic textGraphic = new Graphic(point, textSymbol);
                CreateGraphics(textGraphic);
            }
        }
    }




}
