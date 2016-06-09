using System;
using System.Collections.Generic;

namespace clustering
{
    class JPoint 
    {
        public JPoint(int id,double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
            Visited = false;
            NeighborCalculated = false;
            mNeighborhood = new List<JPoint>();
            ParentExist = false;
            Parent = null;
        }

        /*******************************************/
        /*****      PUBLIC METHOD               ****/
        /*******************************************/
        public List<JPoint> GetNeighborPoints(double epsilon, List<JPoint> wholePoints)
        {
            if(wholePoints == null)
            {
                return null;
            }

            foreach(JPoint eachPoint in wholePoints)
            {
                double distance = 0.0f;
                
                if (eachPoint.Id == Id) // if this point(eachPoint) is equal to this point
                {
                    continue;
                }

                distance = Math.Sqrt(Math.Pow(eachPoint.X - X, 2) + Math.Pow(eachPoint.Y - Y, 2));
                if (distance <= epsilon)
                {
                    mNeighborhood.Add(eachPoint);
                }
            }
            NeighborCalculated = true;

            return mNeighborhood;
        }
        public void SetParent(JCluster parent)
        {
            Parent = parent;
            if(parent != null)
            {
                ParentExist = true;
            }
            else
            {
                ParentExist = false;
            }
        }
        public void DeleteMetaData()
        {
            Visited = false;
            NeighborCalculated = false;
            Parent = null;
            ParentExist = false;
            mNeighborhood.Clear();
        }

        /*******************************************/
        /****           Properties               ***/
        /*******************************************/
        public int Id { get;  }
        public double X { get;  }
        public double Y { get;  }
        public bool Visited { get; set; }
        public bool NeighborCalculated { get; private set; }
        public bool ParentExist { get; private set; }
        public JCluster Parent { get; private set; }

        /********************************************/
        /*******        Private member var.         */
        /********************************************/
        private List<JPoint> mNeighborhood ;

        /*********************************************/
        /******     Static Method               ******/
        /*********************************************/
        public static double GetDistance(JPoint pt1, JPoint pt2)
        {
            double retValue = -1;

            if (pt1 != null && pt2 != null)
            {
                retValue = Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
            }

            return retValue;
        }
    }
}
