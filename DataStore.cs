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

new TrajectoryPoint(35.725 , 53.382, 0      ), 
new TrajectoryPoint(35.724 , 53.223, 12.564 ),
new TrajectoryPoint(35.723 , 53.064, 25.116 ),
new TrajectoryPoint(35.722 , 52.905, 37.643 ),
new TrajectoryPoint(35.721 , 52.746, 50.133 ),
new TrajectoryPoint(35.72  , 52.587, 62.574 ),
new TrajectoryPoint(35.719 , 52.428, 74.953 ),
new TrajectoryPoint(35.718 , 52.27, 87.0 ),
new TrajectoryPoint(35.717 , 52.111, 99.476 ),
new TrajectoryPoint(35.716 , 51.952, 111.6  ),
new TrajectoryPoint(35.715 , 51.793, 123.61 ),
new TrajectoryPoint(35.714 , 51.634, 135.5  ),
new TrajectoryPoint(35.713 , 51.475, 147.25 ),
new TrajectoryPoint(35.712 , 51.316, 158.86 ),
new TrajectoryPoint(35.711 , 51.158, 170.31 ),
new TrajectoryPoint(35.71  , 50.999, 181.6  ),
new TrajectoryPoint(35.709 , 50.84, 192.7  ),
new TrajectoryPoint(35.708 , 50.681, 203.62 ),
new TrajectoryPoint(35.707 , 50.522, 214.33 ),
new TrajectoryPoint(35.706 , 50.363, 224.83 ),
new TrajectoryPoint(35.705 , 50.204, 235.11 ),
new TrajectoryPoint(35.704 , 50.046, 245.16 ),
new TrajectoryPoint(35.703 , 49.887, 254.97 ),
new TrajectoryPoint(35.702 , 49.728, 264.52 ),
new TrajectoryPoint(35.701 , 49.569, 273.82 ),
new TrajectoryPoint(35.7   , 49.41, 282.84 ),
new TrajectoryPoint(35.699 , 49.251, 291.59 ),
new TrajectoryPoint(35.698 , 49.092, 300.04 ),
new TrajectoryPoint(35.697 , 48.934, 308.21 ),
new TrajectoryPoint(35.696 , 48.775, 316.06 ),
new TrajectoryPoint(35.695 , 48.616, 323.61 ),
new TrajectoryPoint(35.694 , 48.457, 330.83 ),
new TrajectoryPoint(35.693 , 48.298, 337.73 ),
new TrajectoryPoint(35.692 , 48.139, 344.3  ),
new TrajectoryPoint(35.691 , 47.98, 350.52 ),
new TrajectoryPoint(35.69  , 47.821, 356.4  ),
new TrajectoryPoint(35.689 , 47.663, 361.93 ),
new TrajectoryPoint(35.688 , 47.504, 367.1  ),
new TrajectoryPoint(35.687 , 47.345, 371.91 ),
new TrajectoryPoint(35.686 , 47.186, 376.35 ),
new TrajectoryPoint(35.685 , 47.027, 380.42 ),
new TrajectoryPoint(35.684 , 46.868, 384.12 ),
new TrajectoryPoint(35.683 , 46.709, 387.43 ),
new TrajectoryPoint(35.682 , 46.551, 390.37 ),
new TrajectoryPoint(35.681 , 46.392, 392.91 ),
new TrajectoryPoint(35.68  , 46.233, 395.08 ),
new TrajectoryPoint(35.679 , 46.074, 396.85 ),
new TrajectoryPoint(35.678 , 45.915, 398.22 ),
new TrajectoryPoint(35.677 , 45.756, 399.21 ),
new TrajectoryPoint(35.676 , 45.597, 399.8  ),
new TrajectoryPoint(35.675 , 45.439, 400    ),
new TrajectoryPoint(35.674 , 45.28, 399.8  ),
new TrajectoryPoint(35.673 , 45.121, 399.21 ),
new TrajectoryPoint(35.672 , 44.962, 398.22 ),
new TrajectoryPoint(35.671 , 44.803, 396.85 ),
new TrajectoryPoint(35.67  , 44.644, 395.08 ),
new TrajectoryPoint(35.669 , 44.485, 392.91 ),
new TrajectoryPoint(35.668 , 44.327, 390.37 ),
new TrajectoryPoint(35.667 , 44.168, 387.43 ),
new TrajectoryPoint(35.666 , 44.009, 384.12 ),
new TrajectoryPoint(35.665 , 43.85, 380.42 ),
new TrajectoryPoint(35.664 , 43.691, 376.35 ),
new TrajectoryPoint(35.663 , 43.532, 371.91 ),
new TrajectoryPoint(35.662 , 43.373, 367.1  ),
new TrajectoryPoint(35.661 , 43.215, 361.93 ),
new TrajectoryPoint(35.66  , 43.056, 356.4  ),
new TrajectoryPoint(35.659 , 42.897, 350.52 ),
new TrajectoryPoint(35.658 , 42.738, 344.3  ),
new TrajectoryPoint(35.657 , 42.579, 337.73 ),
new TrajectoryPoint(35.656 , 42.42, 330.83 ),
new TrajectoryPoint(35.655 , 42.261, 323.61 ),
new TrajectoryPoint(35.654 , 42.102, 316.06 ),
new TrajectoryPoint(35.653 , 41.944, 308.21 ),
new TrajectoryPoint(35.652 , 41.785, 300.04 ),
new TrajectoryPoint(35.651 , 41.626, 291.59 ),
new TrajectoryPoint(35.65  , 41.467, 282.84 ),
new TrajectoryPoint(35.649 , 41.308, 273.82 ),
new TrajectoryPoint(35.648 , 41.149, 264.52 ),
new TrajectoryPoint(35.647 , 40.99, 254.97 ),
new TrajectoryPoint(35.646 , 40.832, 245.16 ),
new TrajectoryPoint(35.645 , 40.673, 235.11 ),
new TrajectoryPoint(35.644 , 40.514, 224.83 ),
new TrajectoryPoint(35.643 , 40.355, 214.33 ),
new TrajectoryPoint(35.642 , 40.196, 203.62 ),
new TrajectoryPoint(35.641 , 40.037, 192.7  ),
new TrajectoryPoint(35.64  , 39.878, 181.6  ),
new TrajectoryPoint(35.639 , 39.72, 170.31 ),
new TrajectoryPoint(35.638 , 39.561, 158.86 ),
new TrajectoryPoint(35.637 , 39.402, 147.25 ),
new TrajectoryPoint(35.636 , 39.243, 135.5  ),
new TrajectoryPoint(35.635 , 39.084, 123.61 ),
new TrajectoryPoint(35.634 , 38.925, 111.6  ),
new TrajectoryPoint(35.633 , 38.766, 99.476 ),
new TrajectoryPoint(35.632 , 38.608, 87.257 ),
new TrajectoryPoint(35.631 , 38.449, 74.953 ),
new TrajectoryPoint(35.63  , 38.29, 62.574 ),
new TrajectoryPoint(35.629 , 38.131, 50.133 ),
new TrajectoryPoint(35.628 , 37.972, 37.643 ),
new TrajectoryPoint(35.627 , 37.813, 25.116 ),
new TrajectoryPoint(35.626 , 37.654, 12.564 ),
new TrajectoryPoint(35.625 , 37.495, 0)
            };


}
