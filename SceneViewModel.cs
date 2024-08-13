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
using System.Text;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DisplayAScene
{

    class SceneViewModel : INotifyPropertyChanged
    {

        public SceneViewModel()
        {
            SetupScene();
            OnPropertyChanged();
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
        }

        //// Create a new scene with an imagery basemap.
        //Scene scene = new Scene(BasemapStyle.ArcGISImageryStandard);

        //    //// Create an elevation source to show relief in the scene.
        //    //string elevationServiceUrl = "http://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer";
        //    //ArcGISTiledElevationSource elevationSource = new ArcGISTiledElevationSource(new Uri(elevationServiceUrl));
        //    //
        //    //// Create a Surface with the elevation data.
        //    //Surface elevationSurface = new Surface();            
        //    //elevationSurface.ElevationSources.Add(elevationSource);

        //    //// Add an exaggeration factor to increase the 3D effect of the elevation.
        //    ////elevationSurface.ElevationExaggeration = 2.5;

        //    //// Apply the surface to the scene.
        //    //scene.BaseSurface = elevationSurface;

        //    // Create a point that defines the observer's (camera) initial location in the scene.
        //    // The point defines a longitude, latitude, and altitude of the initial camera location.
        //    MapPoint cameraLocation = new MapPoint(32.0853, 33.909, 0.0, SpatialReferences.Wgs84);

        //    // Create a Camera using the point, the direction the camera should face (heading), and its pitch and roll (rotation and tilt).
        //    Camera sceneCamera = new Camera(locationPoint: cameraLocation, 
        //                          heading: 30.0, 
        //                          pitch: 72.0, 
        //                          roll: 0.0);

        //    //// Create the initial point to center the camera on (the Santa Monica mountains in Southern California).
        //    //// Longitude=118.805 degrees West, Latitude=34.027 degrees North
        //    //MapPoint sceneCenterPoint = new MapPoint(32.1853, 33.93, SpatialReferences.Wgs84);

        //    //// Set an initial viewpoint for the scene using the camera and observation point.
        //    //Viewpoint initialViewpoint = new Viewpoint(sceneCenterPoint, sceneCamera);
        //    //scene.InitialViewpoint = initialViewpoint;

        //    // Set the view model "Scene" property.
        //    this.Scene = scene;

        

    }

}

