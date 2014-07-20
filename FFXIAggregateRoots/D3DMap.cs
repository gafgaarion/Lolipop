using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using KDTree;
using Commons;
using System.Drawing;

namespace FFXIAggregateRoots
{
    public class D3DMap
    {
        List<double> lx;
        List<double> lz;

        private bool mapLoaded;

        float pfAdjustmentX;
        float pfAdjustmentZ;

        Algorithms.IPathFinder pf;
        public D3DMap(float _pfAdjustmentX, float _pfAdjustmentZ)
        {
            pfAdjustmentX = _pfAdjustmentX;
            pfAdjustmentZ = _pfAdjustmentZ;
            mapLoaded = false;
        }

        private int smallestx;
        private int smallestz;
        private int rows;
        private int cols;
        private float[,] gridy;
        private byte[,] gridxz;

        private KDTree<double[]> tree;

        private void mapPriorities(int layers)
        {
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    if (this.gridy[i, j] == -90000)
                    {
                        for (int w = -layers; w <= layers; w++ )
                        {
                            for (int v = -layers; v <= layers; v++)
                            {
                                if (i + w < 0 || i + w > this.rows)
                                    continue;

                                if (j + v < 0 || j + v > this.cols)
                                    continue;

                                if (this.gridy[i+w, j+v] != -90000)
                                {
                                    gridxz[i+w, j+v] = byte.MaxValue; // add weight...
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Weights tiles in order to have bot attempt to avoid small objects 
        /// </summary>
        private void weightTerrains()
        {
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {

                    if (gridxz[i, j] > 0)
                    {
                        for (int w = -1; w <= 1; w++)
                        {
                            for (int v = -1; v <= 1; v++)
                            {
                                if (i + w < 0 || i + w > this.rows)
                                    continue;

                                if (j + v < 0 || j + v > this.cols)
                                    continue;

                                float dy = Math.Abs(this.gridy[i, j] - this.gridy[i + w, j + v]);

                                if (gridxz[i, j] < (byte)(dy * 100) && dy <= 1 && this.gridxz[i + w, j + v] != 0)
                                {
                                    gridxz[i, j] = (byte)(dy * 100); // add weight...
                                }
                            }
                        }
                    }
                }
            }
        }

        public int[] getClosest(int[] pt)
        {
            int[] closestpt = null;

            float distance = 90000;
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    if (this.gridy[i,j] != -90000)
                    {
                        float tempdist = (float)Math.Sqrt(Math.Pow(i - pt[0],2) + Math.Pow(j - pt[1],2));
                        if (tempdist < distance)
                        {
                            distance = tempdist;
                            closestpt = new int[2] { i, j };
                        }
                    }
                    
                }
            }

            return closestpt;
        }

        public Point getClosestFromFFxiCoord(float x, float z, out bool pointFound)
        {
            Point start = convertToGridSquare((float)Math.Round(x,0), (float)Math.Round(z,0));
            Point closestpt = new Point (0,0);
            pointFound = false;
            float distance = 90000;

            int range = 20;
            int rowLowBound = start.X - range ;
            int rowHighBound = start.X + range;
            int colLowBound = start.Y - range;
            int colHighBound = start.Y + range;

            if (rowLowBound < 0)
                rowLowBound = 0;
            if (rowHighBound > this.rows)
                rowHighBound = this.rows;
            if (colLowBound < 0)
                colLowBound = 0;
            if (colHighBound > this.cols)
                colHighBound = this.cols;

            for (int i = rowLowBound; i < rowHighBound; i++)
            {
                for (int j = colLowBound; j < colHighBound; j++)
                {
                    if (this.gridy[i, j] != -90000)
                    {
                        float tempdist = (float)Math.Sqrt(Math.Pow(i - (x- smallestx), 2) + Math.Pow(j - (z - smallestz), 2));
                        if (tempdist < distance)
                        {
                            distance = tempdist;
                            closestpt = new Point(i, j);
                            pointFound = true;
                        }
                    }

                }
            }

            return convertToFFxiCoord(closestpt);
        }

        public List<int[]> getPath(Point start, Point end, out List<double> _x, out List<double> _z)
        {
            List<int[]> path = null;
            _x = new List<double>();
            _z = new List<double>();


            List<Algorithms.PathFinderNode> nodes = this.pf.FindPath(start, end);

            if (nodes == null)
                return null;

            int ret_x = 0;
            int ret_z = 0;
            path = new List<int[]>();

            for (int i = 0; i < nodes.Count; i++) // Loop through List with for
	        {
                
                getCoordinatesFromPFNode(nodes[i], out ret_x, out ret_z);
                path.Add(new int[2] { ret_x, ret_z });



                _x.Add(ret_x);
                _z.Add(ret_z);
	        }
            return path;
        }

        public List<int[]> getPath(Point start, Point end)
        {
            List<int[]> path = null;


            List<Algorithms.PathFinderNode> nodes = this.pf.FindPath(start, end);

            if (nodes == null)
                return null;

            int ret_x = 0;
            int ret_z = 0;
            path = new List<int[]>();

            for (int i = 0; i < nodes.Count; i++) // Loop through List with for
            {

                getCoordinatesFromPFNode(nodes[i], out ret_x, out ret_z);
                path.Add(new int[2] { ret_x, ret_z });
            }
            return path;
        }

        public List<float[]> getPathFromFFxiCoord(Point start, Point end)
        {
            List<float[]> path = null;

            start = convertToGridSquare(start);
            end = convertToGridSquare(end);

            List<Algorithms.PathFinderNode> nodes = this.pf.FindPath(start, end);

            if (nodes == null)
            {

                // Second attempt
                start.X -= 1;
                start.Y -= 1;
                end.X -= 1;
                end.Y -= 1;

                nodes = this.pf.FindPath(start, end);
            }

            if (nodes == null)
            {

                // third attempt
                start.X += 2;
                start.Y += 2;
                end.X += 2;
                end.Y += 2;

                nodes = this.pf.FindPath(start, end);
            }

            if (nodes == null)
                return null;

            path = new List<float[]>();

            float outX;
            float outZ;
            for (int i = 0; i < nodes.Count; i++) // Loop through List with for
            {
                getCoordinatesFromPFNode(nodes[i], out outX, out outZ);

                if (i != 0)
                {
                    outX += pfAdjustmentX;
                    outZ += pfAdjustmentZ;
                }
                path.Add(new float[2] { outX, outZ });
            }
            return path;
        }

        public void convertToCoordinate(int currentIncrement, out double _x, out double _z)
        {
            _z = (double)(currentIncrement % cols) + smallestz;
            _x = (double)((int)(currentIncrement / cols)) + smallestx;
        }

        public void getCoordinatesFromPFNode(Algorithms.PathFinderNode pfnode, out int x, out int z)
        {
            z = pfnode.Y + smallestz;
            x = pfnode.X + smallestx;
        }

        public void getCoordinatesFromPFNode(Algorithms.PathFinderNode pfnode, out float x, out float z)
        {
            z = pfnode.Y + smallestz;
            x = pfnode.X + smallestx;
        }


        public Point convertToGridSquare(float _x, float _z)
        {
            _z = _z - smallestz;
            _x = _x - smallestx;
            return new Point((int)_x, (int)_z);
        }

        public Point convertToFFxiCoord(float _x, float _z)
        {
            _z = _z + smallestz;
            _x = _x + smallestx;
            return new Point((int)_x, (int)_z);
        }

        public Point convertToFFxiCoord(Point toConvert)
        {
            toConvert.Y += smallestz;
            toConvert.X += smallestx;
            return toConvert;
        }

        public Point convertToGridSquare(Point toConvert)
        {
            toConvert.Y -= smallestz;
            toConvert.X -= smallestx;
            return toConvert;
        }

        public bool isLoaded()
        {
            return this.mapLoaded;
        }

        public List<double> getCoordinatesX()
        {
            return lx;
        }

        public List<double> getCoordinatesZ()
        {
            return lz;
        }

        public bool LoadMap(short map, int lowerPrioritySquaresLayers, float XAdjus, float ZAdjus)
        {
            this.pfAdjustmentX = XAdjus;
            this.pfAdjustmentZ = ZAdjus;

            tree = new KDTree<double[]>(3);


            lx = new List<double>();
            lz = new List<double>();
            double[] dbl = null;
            int col_i = 0;
            int rows_i = 0;
            int i = 0;

            this.gridxz = new byte[2048, 2048];
            this.gridy = new float[2048, 2048];

            for (int k = 0; k < 2048; k++)
            {
                for (int j = 0; j < 2048; j++)
                {
                    this.gridxz[k, j] = 0;
                    this.gridy[k, j] = -90000;
                }
            }

            int nbwalkable = 0;
            try
            {
                using (Stream source = File.OpenRead("./BinaryMaps/" + map.ToString() + ".dat"))
                {
                    byte[] buffer = new byte[4];
                    int bytesRead;


                    // Smallest X unit
                    bytesRead = source.Read(buffer, 0, buffer.Length);
                    smallestx = (int)BitConverter.ToSingle(buffer, 0);

                    // Smallest Z unit
                    bytesRead = source.Read(buffer, 0, buffer.Length);
                    smallestz = (int)BitConverter.ToSingle(buffer, 0);

                    // Number of rows
                    bytesRead = source.Read(buffer, 0, buffer.Length);
                    rows = (int)BitConverter.ToSingle(buffer, 0);

                    // Number of columns
                    bytesRead = source.Read(buffer, 0, buffer.Length);
                    cols = (int)BitConverter.ToSingle(buffer, 0);


                    while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        float y = BitConverter.ToSingle(buffer, 0);

                        dbl = new double[3];
                        dbl[1] = System.Convert.ToDouble(y);
                        convertToCoordinate(i, out dbl[0], out dbl[2]);

                        if (y != -90000)
                        {

                            this.gridxz[rows_i, col_i] = 1;

                            // Adds coordinates for the graph
                            lx.Add(dbl[0]);
                            lz.Add(dbl[2]);
                            tree.AddPoint(dbl, dbl);
                            nbwalkable++;
                        }

                        this.gridy[rows_i, col_i] = y;

                        i++;
                        col_i = i % cols;
                        rows_i = (int)(i / cols);
                    }
                }
            }
            catch
            { return false; }

            this.weightTerrains();
            this.mapPriorities(lowerPrioritySquaresLayers);


            pf = new Algorithms.PathFinderFast(gridxz, gridy);
            pf.Diagonals = true;
            pf.HeavyDiagonals = false;
            pf.PunishChangeDirection = false;
            pf.SearchLimit = 1000000;
            pf.Formula = Algorithms.HeuristicFormula.Manhattan;
            pf.HeuristicEstimate = 2;
            pf.DebugProgress = false;

            mapLoaded = true;
            return true;
        }
    }
}
