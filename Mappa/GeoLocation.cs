using System;
namespace Mappa
{
    [Serializable]
    public class GeoLocation
    {
        private double _radLat;  // latitude in radians
        private double _radLon;  // longitude in radians

        private double _degLat;  // latitude in degrees
        private double _degLon;  // longitude in degrees

        private static double MIN_LAT = -Math.PI / 2;
        private static double MAX_LAT = Math.PI / 2;
        private static double MIN_LON = -Math.PI;
        private static double MAX_LON = Math.PI;

        private const double earthRadius = 6371.01;

        public static double ConvertDegreesToRadians(double degrees)
        {
            return Math.Min(degrees, 90) * Math.PI / 180d;
        }

        public static double ConvertRadiansToDegrees(double radians)
        {
            return radians * 180d / Math.PI;
        }

        public double LatitudeDegrees
        {
            get
            {
                return _degLat;
            }
            set
            {
                _degLat = value;
                _radLat = ConvertDegreesToRadians(value);
            }
        }

        public double LongitudeDegrees
        {
            get
            {
                return _degLon;
            }
            set
            {
                _degLon = value;
                _radLon = ConvertDegreesToRadians(value);
            }
        }

        public bool IsPointInPolygon(GeoLocation[] polygon)
        {
            int i, j = polygon.Length - 1;
            bool oddNodes = false;

            for (i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].LatitudeDegrees < LatitudeDegrees && polygon[j].LatitudeDegrees >= LatitudeDegrees
                || polygon[j].LatitudeDegrees < LatitudeDegrees && polygon[i].LatitudeDegrees >= LatitudeDegrees)
                {
                    if (polygon[i].LongitudeDegrees + (LatitudeDegrees - polygon[i].LatitudeDegrees) / (polygon[j].LatitudeDegrees - polygon[i].LatitudeDegrees) * (polygon[j].LongitudeDegrees - polygon[i].LongitudeDegrees) < LongitudeDegrees)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }

        /// <summary>
        /// Return GeoLocation from Degrees
        /// </summary>
        /// <param name="latitude">The latitude, in degrees.</param>
        /// <param name="longitude">The longitude, in degrees.</param>
        /// <returns>GeoLocation in Degrees</returns>
        public static GeoLocation FromDegrees(double latitude, double longitude)
        {
            longitude = Math.Max(-180, Math.Min(180, longitude));
            latitude = Math.Max(-90, Math.Min(90, latitude));
            GeoLocation result = new GeoLocation
            {
                _radLat = ConvertDegreesToRadians(latitude),
                _radLon = ConvertDegreesToRadians(longitude),
                _degLat = latitude,
                _degLon = longitude
            };
            result.CheckBounds();
            return result;
        }

        /// <summary>
        /// Return GeoLocation from Radians
        /// </summary>
        /// <param name="latitude">The latitude, in radians.</param>
        /// <param name="longitude">The longitude, in radians.</param>
        /// <returns>GeoLocation in Radians</returns>
        public static GeoLocation FromRadians(double latitude, double longitude)
        {
            GeoLocation result = new GeoLocation
            {
                _radLat = latitude,
                _radLon = longitude,
                _degLat = ConvertRadiansToDegrees(latitude),
                _degLon = ConvertRadiansToDegrees(longitude)
            };
            result.CheckBounds();
            return result;
        }

        private void CheckBounds()
        {
            if (_radLat < MIN_LAT || _radLat > MAX_LAT ||
                    _radLon < MIN_LON || _radLon > MAX_LON)
                throw new Exception("Arguments are out of bounds");
        }

        /// <summary>
        /// Computes the great circle distance between this GeoLocation instance and the location argument.
        /// </summary>
        /// <param name="location">Location to act as the centre point</param>
        /// <returns>the distance, measured in the same unit as the radius argument.</returns>
        public double DistanceTo(GeoLocation location)
        {
            return Math.Acos(Math.Sin(_radLat) * Math.Sin(location._radLat) +
                    Math.Cos(_radLat) * Math.Cos(location._radLat) *
                    Math.Cos(_radLon - location._radLon)) * earthRadius;
        }
    }
}
