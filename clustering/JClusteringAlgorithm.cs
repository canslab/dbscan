using System;
using System.Collections.Generic;
using System.IO;

namespace clustering
{
    static class JClusteringAlgorithm
    {
        /*************************************************************/
        /********            Public Method                      ******/
        /*************************************************************/

        /// <summary>
        /// 
        /// This does clustering task based on DBSCAN, given input file name,
        /// and # of clusters
        /// 
        /// </summary>
        /// <param name="inputFileName"> The name of input file whose contains all data point </param>
        /// <param name="nCluster"> The desired number of clusters </param>
        /// <returns> The list of clusters </returns>
        public static List<JCluster> ClusterUsingDBSCAN(string inputFileName, int nCluster)
        {
            List<JCluster> clusterList = null;                      // It is the list of all clusters 
            List<JPoint> wholePoints = null;                        // It is the list of all points.

            // read from input file, and save all points to the wholePoints.
            ReadDataFromFile(inputFileName, out wholePoints);       

            if (inputFileName == "input1.txt")      // input1.txt case 
            {
                _ClusterUsingDBSCAN(14.7, 20, wholePoints, out clusterList);
            }
            else if(inputFileName == "input2.txt")  // input2.txt case
            {
                _ClusterUsingDBSCAN(1.7, 5, wholePoints, out clusterList);
            }
            else if(inputFileName == "input3.txt")  // input3.txt case
            {
                _ClusterUsingDBSCAN(3.7, 5, wholePoints ,out clusterList);
            }
            // other input case 
            else
            {   
                List<double> knnTable;                              // kth nearest table 
                List<double> epsList;                           
                MakeKnnTable(wholePoints, 4, out knnTable);         // make kth nearest table 
                GetCandidateEpsilonList(knnTable, out epsList);     // the epsilon candidate list which can be the relatively noticeable change position. 

                // check each epsilon in order to find the most appropriate eps value which makes # of result clusters is 
                // similar to designated # of clusters.
                for (int i = epsList.Count - 1; i >= 0; --i)      
                {
                    // do cluster using epsList[i] which is candidate eps value.
                    _ClusterUsingDBSCAN(epsList[i], 5, wholePoints, out clusterList);

                    int nResultCluster = clusterList.Count;

                    // before going to next clustering task, you should remove all metadata from wholePoints,
                    // for example, the variable whether it is visited or not, 
                    // the neighborhood points.... so on
                    foreach (JPoint point in wholePoints)
                    {
                        point.DeleteMetaData();
                    }

                    if (nResultCluster >= nCluster) // check critical point.
                    {
                        break;
                    }
                }
            }

            // create all output files...   ex) output1_cluster_0.txt 
            MakeClusterOutputFiles(inputFileName, nCluster, clusterList);

            return clusterList;
        }

        /// <summary>
        /// 
        /// It is the private cluster task method, it is invoked by ClusterUsingDBSCAN(), 
        /// it does actual cluster task, based on given epsilon, minpts, whole data point set
        /// 
        /// </summary>
        /// <param name="epsilon"> Epsilon parameter </param>
        /// <param name="minpts"> The minimum number of points to be dense area </param>
        /// <param name="wholePointSpace"> The set of all data points to be done clustering task </param>
        /// <param name="clusterList"> This is the result list which contains all clusters </param>
        private static void _ClusterUsingDBSCAN(double epsilon, double minpts, List<JPoint> wholePointSpace, out List<JCluster> clusterList)
        {
            List<JPoint> unvisitedPoints = new List<JPoint>(wholePointSpace); 

            clusterList = new List<JCluster>();
            Random ranGen = new Random(1);

            while (unvisitedPoints.Count != 0)
            {
                // randomly select an unvisited object from the list of unvisited objects 
                JPoint chosenPoint = unvisitedPoints[ranGen.Next() % unvisitedPoints.Count];

                // mark it as a visited object
                chosenPoint.Visited = true;

                // chosenPoint is visted, therefore it should be removed from unvisitedPoints list.
                unvisitedPoints.Remove(chosenPoint);

                // get its neighborhood
                var neighborPoints = chosenPoint.GetNeighborPoints(epsilon, wholePointSpace);

                // if its neighborhood contains more than MIN_PTS - 1 
                if (neighborPoints.Count >= minpts - 1) // If it is true, chosePoint must be the core object
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
                                var pNeighborPoints = p.GetNeighborPoints(epsilon, wholePointSpace);

                                if (pNeighborPoints.Count >= minpts - 1) // if p is the core object 
                                {
                                    // the neighbor points of the p should be inserted to N
                                    // because p is the core object, there is a chance of being its neighbor points core objects
                                    foreach (JPoint eachPoint in pNeighborPoints)
                                    {
                                        if (nowTest.Contains(eachPoint) == false && unvisitedPoints.Contains(eachPoint) == true)
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
                string fileName = "output" + outputFileNumber + "_cluster_" + String.Format("{0}", index) + ".txt";

                using (StreamWriter resultFile = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                {
                    foreach (JPoint eachPointInCluster in eachCluster.PointList)
                    {
                        resultFile.WriteLine(eachPointInCluster.Id);
                    }
                }

                index++;
            }
        }
        
        /// <summary>
        /// Make K-nearest graph(table) based on whole data points, K
        /// The output(table) will be given by knnTable
        /// </summary>
        /// <param name="wholePoints"> The whole data points that will be examined </param>
        /// <param name="K"> The K value </param>
        /// <param name="knnTable"> This is the output data structure which is the sorted k-th nearest distance value list </param>
        private static void MakeKnnTable(List<JPoint> wholePoints, int K, out List<double> knnTable)
        {
            knnTable = new List<double>();
            for (int i = 0; i < wholePoints.Count; ++i)
            {
                double[] record = new double[K];
                int recordIndex = 0;
                for (int j = 0; j < wholePoints.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    var distanceValue = JPoint.GetDistance(wholePoints[i], wholePoints[j]);

                    if (recordIndex <= K-1)
                    {
                        record[recordIndex] = distanceValue;
                        if (recordIndex == K-1)
                        {
                            Array.Sort(record);
                        }
                    }
                    else
                    {
                        for (int index = 0; index < K; ++index)
                        {
                            if (distanceValue < record[index])
                            {
                                Array.Copy(record, index, record, index + 1, K - index - 1);
                                record[index] = distanceValue;
                                break;
                            }
                        }
                    }
                    recordIndex++;
                }
                //knnTable.Add(new MyPair(i, record[nMinPts - 2]));
                knnTable.Add(record[K-1]);
            }
            knnTable.Sort();
        }

        private static void ReadDataFromFile(string fileName, out List<JPoint> wholePoints)
        {
            using (StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                wholePoints = new List<JPoint>();
                while (!reader.EndOfStream)
                {
                    string eachLine = reader.ReadLine();
                    string[] tokenList = eachLine.Split('\t');

                    int id = int.Parse(tokenList[0]);
                    double x = Convert.ToDouble(tokenList[1]);
                    double y = Convert.ToDouble(tokenList[2]);

                    // make each point using each line of the input text file, and Add it!
                    JPoint eachPoint = new JPoint(id, x, y);
                    wholePoints.Add(eachPoint);
                }
            }

        }
        private static void GetCandidateEpsilonList(List<double> knnTable, out List<double> candidate)
        {
            candidate = new List<double>();

            int maxIndex = 0;
            int divider = 100;
            int interval = knnTable.Count / divider;     // ex) count = 8000 => interval = 8000/100 = 80
            double lastSlope = knnTable[interval - 1] - knnTable[0];

            for (int i = 0; i < divider; ++i)
            {
                double currentSlope = knnTable[(i + 1) * interval - 1] - knnTable[i * interval];

                if (currentSlope > lastSlope * 1.3 )
                {
                    maxIndex = i * interval;
                    candidate.Add(knnTable[maxIndex]);
                }

                lastSlope = currentSlope;
            }
        }
    }
}
