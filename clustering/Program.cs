using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace clustering
{
    class Program
    {
        private static void ReadDataFromFile(string fileName, ref List<JPoint> wholePoints)
        {
            using (StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                while(!reader.EndOfStream)
                {
                    string eachLine = reader.ReadLine();
                    string[] tokenList = eachLine.Split('\t');

                    int id = Convert.ToInt32(tokenList[0]);
                    double x = Convert.ToDouble(tokenList[1]);
                    double y = Convert.ToDouble(tokenList[2]);

                    JPoint eachPoint = new JPoint(id, x, y);
                    wholePoints.Add(eachPoint);
                }
            }
        }
        private static void ShowResult(string[] args, List<JCluster> clusterList)
        {
            int index = 0;

            using (StreamWriter resultFile = new StreamWriter(new FileStream("ress.txt", FileMode.Create)))
            {
                foreach (JCluster eachCluster in clusterList)
                {
                    if (index >= Convert.ToInt32(args[1]))
                    {
                        break;
                    }
                    resultFile.WriteLine(" Repre = {0}, # of members = {1}", eachCluster.Representative, eachCluster.Count);
                    resultFile.WriteLine("==========================================================");

                    foreach (JPoint eachPointInCluster in eachCluster.PointList)
                    {
                        resultFile.WriteLine("id = {0}, x = {1}, y = {2}", eachPointInCluster.Id, eachPointInCluster.X, eachPointInCluster.Y);
                    }

                    index++;
                }
            }
        }
        private static void MakeClusterOutputFiles(string inputTextFileName, int nCluster, List<JCluster> clusterList)
        {
            int index = 0;

            var k = inputTextFileName.LastIndexOf(".txt");
            var outputFileNumber = inputTextFileName[k - 1];

            foreach (JCluster eachCluster in clusterList)
            {
                if (index >= Convert.ToInt32(nCluster))
                {
                    break;
                }
                string prefixPath = "C:\\PA3\\";
                string fileName = prefixPath + "output" + outputFileNumber +"_cluster_" + String.Format("{0}", index) + ".txt";
                
                using (StreamWriter resultFile = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                {
                    foreach (JPoint eachPointInCluster in eachCluster.PointList)
                    {
                        resultFile.WriteLine(eachPointInCluster.Id);
                        //resultFile.WriteLine("{0}\t{1}", eachPointInCluster.X, eachPointInCluster.Y);
                    }
                }

                index++;
            }
        }

        static void DoJob(string[] args, List<JCluster> clusterList)
        {
            List<JPoint> unvisitedPoints = new List<JPoint>();

            Console.WriteLine("fileName" + args[0]);
            Console.WriteLine("the number of cluster = " + args[1]);

            ReadDataFromFile(args[0], ref unvisitedPoints);
            List<JPoint> wholePointSpace = new List<JPoint>(unvisitedPoints);

            Random ranGen = new Random();

            while (unvisitedPoints.Count != 0)
            {
                // randomly select an unvisited object from the list of unvisited objects 
                JPoint chosenPoint = unvisitedPoints[ranGen.Next() % unvisitedPoints.Count];

                // mark it as a visited object
                chosenPoint.Visited = true;

                // chosenPoint is visted, therefore it should be removed from unvisitedPoints list.
                unvisitedPoints.Remove(chosenPoint);

                // get its neighborhood
                var neighborPoints = chosenPoint.GetNeighborPoints(EPSILON, wholePointSpace);

                // if its neighborhood contains more than MIN_PTS - 1 
                if (neighborPoints.Count >= MIN_PTS - 1) // If it is true, chosePoint must be the core object
                {
                    // then make a new cluster, and expand this cluster as long as there exists points in N
                    JCluster newCluster = new JCluster();
                    newCluster.AddPoint(chosenPoint);

                    clusterList.Add(newCluster);
                    // N is the candidate set
                    HashSet<JPoint> nowTest = new HashSet<JPoint>(neighborPoints);
                    HashSet<JPoint> candidate = new HashSet<JPoint>();
                    
                    // N is the candidate set (whether the point is core or not)
                    while (nowTest.Count != 0)
                    {
                        foreach (JPoint p in nowTest)
                        {
                            // test for p 
                            if (p.Visited == false)
                            {
                                // mark it as a visited object 
                                p.Visited = true;
                                // remove p from unvisited points
                                unvisitedPoints.Remove(p);

                                // test whether p is the core object or not. So, get the neighbor points of it.
                                var pNeighborPoints = p.GetNeighborPoints(EPSILON, wholePointSpace);

                                if (pNeighborPoints.Count >= MIN_PTS - 1) // if p is the core object 
                                {
                                    // the neighbor points of the p should be inserted to N
                                    // because p is the core object, there is a chance of being its neighbor points core objects
                                    foreach (JPoint eachPoint in pNeighborPoints)
                                    {
                                        if(nowTest.Contains(eachPoint) == false && unvisitedPoints.Contains(eachPoint) == true)
                                        {
                                            candidate.Add(eachPoint);
                                        }
                                    }
                                }
                            }
                            // if it is not a member of any cluster, 
                            if (p.ParentExist == false)
                            { 
                                newCluster.AddPoint(p);
                            }
                        }
                        nowTest.Clear();
                        
                        // role swap
                        var k = nowTest;
                        nowTest = candidate;
                        candidate = k;
                    }
                }
                
            }

        }

        static void Main(string[] args)
        {
            List<JCluster> clusterList = new List<JCluster>();
            DoJob(args, clusterList);
            clusterList.Sort();

            ShowResult(args, clusterList);
            MakeClusterOutputFiles(args[0],Convert.ToInt32(args[1]), clusterList);
        }



        public static int MIN_PTS = 5;
        // 14 is good ,14.2 good, 14.4 개이득, 14.5 더이득,  14.7 더 이득
        // min pts = 20, epsilon = 14.7 --> best case (input1)
        // min pts = 5, epsiolon = 1.7 ---> best case (input2)
        // min pts = 5, epsilon = 3.7 ---> best case (input3)
        public static double EPSILON = 1.7; 

    }
}
