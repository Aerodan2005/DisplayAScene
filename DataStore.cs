using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace DisplayAScene;

public class TrajectoryPoint
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
    public double Heading { get; set; }
    public double Pitch { get; set; }
    public double Roll { get; set; }

    public TrajectoryPoint(double latitude, double longitude, double altitude, double heading, double pitch, double roll)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        Heading = heading;
        Pitch = pitch;
        Roll = roll;
    }
    public TrajectoryPoint(double latitude, double longitude, double altitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        Heading = 0;
        Pitch = 0;
        Roll = 0;
    }
}


public static class DataStore
{
    public static readonly List<TrajectoryPoint> Trajectory = new List<TrajectoryPoint>
            {

new TrajectoryPoint(35.725 , 51.382, 0      ),
new TrajectoryPoint(35.706 , 51.223, 12.564 ),
new TrajectoryPoint(35.688 , 51.064, 25.116 ),
new TrajectoryPoint(35.67 ,  50.905, 37.643 ),
new TrajectoryPoint(35.651 , 50.746, 50.133 ),
new TrajectoryPoint(35.633 , 50.587, 62.574 ),
new TrajectoryPoint(35.615 , 50.428, 74.953 ),
new TrajectoryPoint(35.596 , 50.27, 87.0 ),  // uri
new TrajectoryPoint(35.578 , 50.111, 99.476 ),
new TrajectoryPoint(35.56 ,  49.952, 111.6  ),
new TrajectoryPoint(35.541 , 49.793, 123.61 ),
new TrajectoryPoint(35.523 , 49.634, 135.5  ),
new TrajectoryPoint(35.504 , 49.475, 147.25 ),
new TrajectoryPoint(35.486 , 49.316, 158.86 ),
new TrajectoryPoint(35.468 , 49.158, 170.31 ),
new TrajectoryPoint(35.449 , 48.999, 181.6  ),
new TrajectoryPoint(35.431 , 48.84, 192.7  ),
new TrajectoryPoint(35.413 , 48.681, 203.62 ),
new TrajectoryPoint(35.394 , 48.522, 214.33 ),
new TrajectoryPoint(35.376 , 48.363, 224.83 ),
new TrajectoryPoint(35.358 , 48.204, 235.11 ),
new TrajectoryPoint(35.339 , 48.046, 245.16 ),
new TrajectoryPoint(35.321 , 47.887, 254.97 ),
new TrajectoryPoint(35.303 , 47.728, 264.52 ),
new TrajectoryPoint(35.284 , 47.569, 273.82 ),
new TrajectoryPoint(35.266 , 47.41, 282.84 ),
new TrajectoryPoint(35.247 , 47.251, 291.59 ),
new TrajectoryPoint(35.229 , 47.092, 300.04 ),
new TrajectoryPoint(35.211 , 46.934, 308.21 ),
new TrajectoryPoint(35.192 , 46.775, 316.06 ),
new TrajectoryPoint(35.174 , 46.616, 323.61 ),
new TrajectoryPoint(35.156 , 46.457, 330.83 ),
new TrajectoryPoint(35.137 , 46.298, 337.73 ),
new TrajectoryPoint(35.119 , 46.139, 344.3  ),
new TrajectoryPoint(35.101 , 45.98, 350.52 ),
new TrajectoryPoint(35.082 , 45.821, 356.4  ),
new TrajectoryPoint(35.064 , 45.663, 361.93 ),
new TrajectoryPoint(35.045 , 45.504, 367.1  ),
new TrajectoryPoint(35.027 , 45.345, 371.91 ),
new TrajectoryPoint(35.009 , 45.186, 376.35 ),
new TrajectoryPoint(34.99 ,  45.027, 380.42 ),
new TrajectoryPoint(34.972 , 44.868, 384.12 ),
new TrajectoryPoint(34.954 , 44.709, 387.43 ),
new TrajectoryPoint(34.935 , 44.551, 390.37 ),
new TrajectoryPoint(34.917 , 44.392, 392.91 ),
new TrajectoryPoint(34.899 , 44.233, 395.08 ),
new TrajectoryPoint(34.88 ,  44.074, 396.85 ),
new TrajectoryPoint(34.862 , 43.915, 398.22 ),
new TrajectoryPoint(34.843 , 43.756, 399.21 ),
new TrajectoryPoint(34.825 , 43.597, 399.8  ),
new TrajectoryPoint(34.807 , 43.439, 400    ),
new TrajectoryPoint(34.788 , 43.28, 399.8  ),
new TrajectoryPoint(34.77 ,  43.121, 399.21 ),
new TrajectoryPoint(34.752 , 42.962, 398.22 ),
new TrajectoryPoint(34.733 , 42.803, 396.85 ),
new TrajectoryPoint(34.715 , 42.644, 395.08 ),
new TrajectoryPoint(34.697 , 42.485, 392.91 ),
new TrajectoryPoint(34.678 , 42.327, 390.37 ),
new TrajectoryPoint(34.66 ,  42.168, 387.43 ),
new TrajectoryPoint(34.641 , 42.009, 384.12 ),
new TrajectoryPoint(34.623 , 41.85, 380.42 ),
new TrajectoryPoint(34.605 , 41.691, 376.35 ),
new TrajectoryPoint(34.586 , 41.532, 371.91 ),
new TrajectoryPoint(34.568 , 41.373, 367.1  ),
new TrajectoryPoint(34.55 ,  41.215, 361.93 ),
new TrajectoryPoint(34.531 , 41.056, 356.4  ),
new TrajectoryPoint(34.513 , 40.897, 350.52 ),
new TrajectoryPoint(34.495 , 40.738, 344.3  ),
new TrajectoryPoint(34.476 , 40.579, 337.73 ),
new TrajectoryPoint(34.458 , 40.42, 330.83 ),
new TrajectoryPoint(34.439 , 40.261, 323.61 ),
new TrajectoryPoint(34.421 , 40.102, 316.06 ),
new TrajectoryPoint(34.403 , 39.944, 308.21 ),
new TrajectoryPoint(34.384 , 39.785, 300.04 ),
new TrajectoryPoint(34.366 , 39.626, 291.59 ),
new TrajectoryPoint(34.348 , 39.467, 282.84 ),
new TrajectoryPoint(34.329 , 39.308, 273.82 ),
new TrajectoryPoint(34.311 , 39.149, 264.52 ),
new TrajectoryPoint(34.293 , 38.99, 254.97 ),
new TrajectoryPoint(34.274 , 38.832, 245.16 ),
new TrajectoryPoint(34.256 , 38.673, 235.11 ),
new TrajectoryPoint(34.238 , 38.514, 224.83 ),
new TrajectoryPoint(34.219 , 38.355, 214.33 ),
new TrajectoryPoint(34.201 , 38.196, 203.62 ),
new TrajectoryPoint(34.182 , 38.037, 192.7  ),
new TrajectoryPoint(34.164 , 37.878, 181.6  ),
new TrajectoryPoint(34.146 , 37.72, 170.31 ),
new TrajectoryPoint(34.127 , 37.561, 158.86 ),
new TrajectoryPoint(34.109 , 37.402, 147.25 ),
new TrajectoryPoint(34.091 , 37.243, 135.5  ),
new TrajectoryPoint(34.072 , 37.084, 123.61 ),
new TrajectoryPoint(34.054 , 36.925, 111.6  ),
new TrajectoryPoint(34.036 , 36.766, 99.476 ),
new TrajectoryPoint(34.017 , 36.608, 87.257 ),
new TrajectoryPoint(33.999 , 36.449, 74.953 ),
new TrajectoryPoint(33.98 ,  36.29, 62.574 ),
new TrajectoryPoint(33.962 , 36.131, 50.133 ),
new TrajectoryPoint(33.944 , 35.972, 37.643 ),
new TrajectoryPoint(33.925 , 35.813, 25.116 ),
new TrajectoryPoint(33.907 , 35.654, 12.564 ),
new TrajectoryPoint(33.889 , 35.495, 0)
            };


}
