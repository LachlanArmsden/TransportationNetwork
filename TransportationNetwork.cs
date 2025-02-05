//2024 CAB301 Assignment 3 
//TransportationNetwok.cs
//Assignment3B-TransportationNetwork

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public partial class TransportationNetwork
{

    private string[]? intersections; //array storing the names of those intersections in this transportation network design
    private int[,]? distances; //adjecency matrix storing distances between each pair of intersections, if there is a road linking the two intersections

    public string[]? Intersections
    {
        get {return intersections;}
    }

    public int[,]? Distances
    {
        get { return distances; }
    }


    //Read information about a transportation network plan into the system
    //Preconditions: The given file exists at the given path and the file is not empty
    //Postconditions: Return true, if the information about the transportation network plan is read into the system, the intersections are stored in the class field, intersections, and the distances of the links between the intersections are stored in the class fields, distances;
    //                otherwise, return false and both intersections and distances are null.
    public bool ReadFromFile(string filePath)
    {
        //To be completed by students
        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string total_lines = sr.ReadToEnd();

                // Uses total_lines to find all the unique letters and sorts them into alphabetical order.
                intersections = total_lines.Where(char.IsLetter).Select(c => c.ToString()).Distinct().OrderBy(c => c).ToArray();

                // Initialises a new temporary 2d array that has the length of intersections, which is to be used for later use.
                int[,] temp_matrix = new int[intersections.Length, intersections.Length];
                // Fills the temporary 2d array with the max value of Int32 (which indicates there is no connection between
                // the two intersections).
                for (int i = 0; i < temp_matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < temp_matrix.GetLength(1); j++)
                    {
                        temp_matrix[i, j] = Int32.MaxValue;
                    }
                }

                // Splits the entire text file into seperate lines (Example: A, B, 2)
                string[] total_split = total_lines.Split("\n");
                
                foreach (var split in total_split)
                {
                    // Each line is split into each component (Example: A)
                    string[] new_split = split.Split(", ");

                    // The two letter components are converted into indexes
                    int input1 = char.ToUpper(char.Parse(new_split[0])) - 65; // (Example: A = 0)
                    int input2 = char.ToUpper(char.Parse(new_split[1])) - 65; // (Example: B = 1)
                    // Converts the value to an int
                    int input3 = Int32.Parse(new_split[2]);

                    // The value is then inserted into position using input1 and input2
                    temp_matrix[input1, input2] = input3;
                }
                distances = temp_matrix;
                return true;
            }
        }
        catch (Exception e)
        {
            // If any error has occured, intersections and distances are made null and returns false.
            intersections = null;
            distances = null;
            return false;
        }
    }


    //Display the transportation network plan with intersections and distances between intersections
    //Preconditions: The given file exists at the given path and the file is not empty
    //Postconditions: The transportation netork is displayed in a matrix format
    public void DisplayTransportNetwork()
    {
        Console.Write("       ");
        for (int i = 0; i < intersections?.Length; i++)
        {
                    Console.Write(intersections[i].ToString().PadRight(5) + "  ");
        }
        Console.WriteLine();


        for (int i = 0; i < distances?.GetLength(0); i++)
        {
            Console.Write(intersections[i].ToString().PadRight(5) + "  ");
            for (int j = 0; j < distances?.GetLength(1); j++)
            {
                if (distances[i, j] == Int32.MaxValue)
                    Console.Write("INF  " + "  ");
                else
                    Console.Write(distances[i, j].ToString().PadRight(5)+"  ");
            }
            Console.WriteLine();
        }
    }


    //Check if this transportation network is strongly connected. A transportation network is strongly connected, if there is a path from any intersection to any other intersections in thihs transportation network. 
    //Precondition: Transportation network plan data have been read into the system.
    //Postconditions: return true, if this transpotation netork is strongly connected; otherwise, return false. This transportation network remains unchanged.
    public bool IsConnected()
    {
        //To be completed by students

        // Initialises counter for later use.
        int count = 0;

        // These for loops iterates through each column of distances.
        for (int col = 0; col < distances?.GetLength(1); col++)
        {
            for (int row = 0; row < distances?.GetLength(0); row++)
            {
                // If there is a connection found, then the counter increments and the innner for loop breaks early.
                if (distances[row, col] != Int32.MaxValue)
                {
                    count++;
                    break;
                }
            }
        }

        // If the amount of counts equals to the number of columns, then the graph is strongly connected since there is
        // at least one value in each column, otherwise it returns false.
        if (count == distances?.GetLength(1)) { return true; }
        else { return false; }
    }

    
    
    //Find the shortest path between a pair of intersections
    //Precondition: transportation network plan data have been read into the system
    //Postcondition: return the shorest distance between two different intersections; return 0 if there is no path from startVerte to endVertex; returns -1 if startVertex or endVertex does not exists. This transportation network remains unchanged.

    public int FindShortestDistance(string startVertex, string endVertex)
    {
        //To be completed by students

        // Converts string to int
        int int_start = char.ToUpper(char.Parse(startVertex)) - 65;
        int int_end = char.ToUpper(char.Parse(endVertex)) - 65;

        // Initialises values for use in Dijkstra's Algorthim
        int N = distances.GetLength(0);
        bool[] S = new bool[N];
        int[] W = new int[N];

        // Dijkstra's Algorithm
        for (int j = 0; j < N; j++)
        {
            W[j] = distances[int_start, j];
        }

        S[int_start] = true;

        for (int i = 0; i < N; i++)
        {
            int v = -1;
            int min_distance = Int32.MaxValue;

            for (int j = 0; j < N; j++)
            {
                if (!S[j] && W[j] < min_distance)
                {
                    min_distance = W[j];
                    v = j;
                }
            }

            if (v == -1) { break; }

            S[v] = true;

            for (int x = 0; x < N; x++)
            {
                if (!S[x] && distances[v, x] != Int32.MaxValue && W[v] + distances[v, x] < W[x])
                {
                    W[x] = W[v] + distances[v, x];
                }
            }
        }

        // If vertices could not be reached it returns 0, otherwise it returns the weight number of the shortest path.
        if (W[int_end] == Int32.MaxValue) { return 0; }
        else { return W[int_end]; }
    }


    //Find the shortest path between all pairs of intersections
    //Precondition: transportation network plan data have been read into the system
    //Postcondition: return the shorest distances between between all pairs of intersections through a two-dimensional int array and this transportation network remains unchanged

    public int[,] FindAllShortestDistances()
    {
        //To be completed by students

        // Initialises variables for use in Floyd's Algorithm
        int N = distances.GetLength(0);
        int [,] D = (int[,]) distances.Clone();

        // Floyd's Algorithm
        for (int k = 0; k < N; k++)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (D[i, k] != Int32.MaxValue && D[k, j] != Int32.MaxValue)
                    {
                        D[i, j] = Math.Min(D[i, j], D[i, k] + D[k, j]);
                    }
                }
            }
        }

        // Returns adjacency matrix of shortest paths.
        return D;
    }
}