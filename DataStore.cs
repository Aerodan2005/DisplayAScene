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
    public static readonly List<TrajectoryPoint> Trajectory = new List<TrajectoryPoint>();
}
