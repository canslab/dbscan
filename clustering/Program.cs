namespace clustering
{
    class Program
    {
        static void Main(string[] args)
        {
            JClusteringAlgorithm.ClusterUsingDBSCAN(args[0], int.Parse(args[1]));
        }
    }
}
