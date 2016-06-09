using System;
using System.Collections.Generic;

namespace clustering
{
    class JCluster : IComparable<JCluster>
    {
        /************************************************/
        /*****          Constructor                 *****/
        /************************************************/
        public JCluster()
        {
            PointList = new List<JPoint>();
        }

        /************************************************/
        /*****          Public  Method              *****/
        /************************************************/
        public void AddPoint(JPoint point)
        {
            PointList.Add(point);
            point.SetParent(this);
        }

        /*************************************************/
        /***********    Interface Implemenation **********/
        /*************************************************/
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

        /*************************************************/
        /***********    Properties              **********/
        /*************************************************/
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
