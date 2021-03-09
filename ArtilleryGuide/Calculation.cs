using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtilleryGuide
{
    public static class Util
    {
        public static double RadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }

        public static double DegreesToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static double NormalizeHeading(double heading)
        {
            double ranged_heading = heading % 360.0;
            if (ranged_heading < 0.0)
            {
                return ranged_heading + 360.0;
            }
            else
            {
                return ranged_heading;
            }
        }
    }


    public struct Vector2D
    {
        public double x;
        public double y;

        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public void Normalize()
        {
            double length = Length();

            x /= length;
            y /= length;
        }

        public Vector2D Normalized()
        {
            double length = Length();

            return new Vector2D
            {
                x = x / length,
                y = y / length
            };
        }

        public double Heading()
        {
            double heading_radians = Math.Atan2(y, x);
            double heading_reverse_from_x = Util.RadiansToDegrees(heading_radians);
            double heading_from_x = Util.NormalizeHeading(-heading_reverse_from_x);
            double heading_from_y = Util.NormalizeHeading(heading_from_x + 90.0);
            return heading_from_y;
        }

        public static Vector2D FromHeading(double Heading)
        {
            double heading_from_x = Util.NormalizeHeading(Heading - 90);
            double reverse_heading_from_x = -heading_from_x;
            return new Vector2D
            {
                x = Math.Cos(Util.DegreesToRadians(reverse_heading_from_x)),
                y = Math.Sin(Util.DegreesToRadians(reverse_heading_from_x)),
            };
        }

        public static Vector2D operator + (Vector2D a, Vector2D b)
        {
            return new Vector2D
            {
                x = a.x + b.x,
                y = a.y + b.y
            };
        }

        public static Vector2D operator - (Vector2D a, Vector2D b)
        {
            return new Vector2D
            {
                x = a.x - b.x,
                y = a.y - b.y
            };
        }

        public static Vector2D operator - (Vector2D a)
        {
            return new Vector2D
            {
                x = -a.x,
                y = -a.y
            };
        }

        public static Vector2D operator * (Vector2D a, double b)
        {
            return new Vector2D
            {
                x = a.x * b,
                y = a.y * b
            };
        }

        public static Vector2D FromHeadingInformation(HeadingInformation info)
        {
            Vector2D unit_vector = FromHeading(info.heading);
            return unit_vector * info.distance;
        }
    }

    public struct HeadingInformation
    {
        public double heading;
        public double distance;

        public static HeadingInformation operator - (HeadingInformation a)
        {
            return new HeadingInformation
            {
                heading = Util.NormalizeHeading(a.heading - 180.0),
                distance = a.distance
            };
        }

        public static HeadingInformation operator + (HeadingInformation a, HeadingInformation b)
        {
            Vector2D a_vec = Vector2D.FromHeadingInformation(a);
            Vector2D b_vec = Vector2D.FromHeadingInformation(b);

            Vector2D total_vec = a_vec + b_vec;

            double distance = total_vec.Length();
            double heading = total_vec.Normalized().Heading();

            return new HeadingInformation
            {
                distance = distance,
                heading = heading
            };
        }
    }
}
