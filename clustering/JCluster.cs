using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clustering
{
    class JCluster : IComparable<JCluster>
    {
        public static int clusterIndex = 0;

        public JCluster()
        {
            PointList = new List<JPoint>();
        }
        
        public void AddPoint(JPoint point)
        {
            PointList.Add(point);
            point.SetParent(this);
        }

        // this < other ==> -1
        // this = other ==> 0
        // this > other ==> 1
        public int CompareTo(JCluster other)
        {
            if (Count > other.Count)
            {
                //return 1;
                return -1;
            }
            else if (Count == other.Count)
            {
                return 0;
            }
            else
            {
                //return -1;
                return 1;
            }
        }

        public KeyValuePair<double, double> Representative
        {
            get
            {
                double x = 0;
                double y = 0;
                foreach(JPoint point in PointList)
                {
                    x += point.X;
                    y += point.Y;
                }
                x /= Count;
                y /= Count;
                return new KeyValuePair<double, double>(x, y);
            }
        }

        public int Count
        {
            get { return PointList.Count; }
        }
        public List<JPoint> PointList { get; private set; }
    }
}
