// -------------------------------------------------------------------
// Department of Electrical and Computer Engineering
// University of Waterloo
//
// Student Name:     Anand Mathew
// Userid:           20560827
//
// Assignment:       PA 4
// Submission Date:  2014-12-01
// 
// I declare that, other than the acknowledgements listed below, 
// this program is my original work.
//
// Acknowledgements:
// ECE-150 Lecture Slides
// -------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

// -----------------------------------------------------------------------------
// A Drug object holds information about one fee-for-service outpatient drug 
// reimbursed by Medi-Cal (California's Medicaid program) to pharmacies.
class Drug : IComparable
{
    string name;            // brand name, strength, dosage form
    string id;              // national drug code number
    double size;            // package size
    string unit;            // unit of measurement
    double quantity;        // number of units dispensed
    double lowest;          // price Medi-Cal is willing to pay
    double ingredientCost;  // estimated ingredient cost
    int numTar;             // number of claims with a treatment auth. request
    double totalPaid;       // total amount paid
    double averagePaid;     // average paid per prescription
    int daysSupply;         // total days supply
    int claimLines;         // total number of claim lines
    
    // Properties providing read-only access to every field.
    public string Name { get { return name; } }               
    public string Id { get { return id; } }                 
    public double Size { get { return size; } }             
    public string Unit { get { return unit; } }             
    public double Quantity { get { return quantity; } }         
    public double Lowest { get { return lowest; } }             
    public double IngredientCost { get { return ingredientCost; } }    
    public int NumTar { get { return numTar; } }                
    public double TotalPaid { get { return totalPaid; } }          
    public double AveragePaid { get { return averagePaid; } }        
    public int DaysSupply { get { return daysSupply; } }            
    public int ClaimLines { get { return claimLines; } }            
    
    public Drug ( string name, string id, double size, string unit, 
        double quantity, double lowest, double ingredientCost, int numTar, 
        double totalPaid, double averagePaid, int daysSupply, int claimLines )
    {
        this.name = name;
        this.id = id;
        this.size = size;
        this.unit = unit;
        this.quantity = quantity;
        this.lowest = lowest;
        this.ingredientCost = ingredientCost;
        this.numTar = numTar;
        this.totalPaid = totalPaid;
        this.averagePaid = averagePaid;
        this.daysSupply = daysSupply;
        this.claimLines = claimLines;
    }

    // Simple string for debugging purposes, showing only selected fields.
    public override string ToString( )
    { 
        return string.Format( 
            "{0}: {1}, {2}", id, name, size ); 
    }

    //Compares the name of the Drug to the name of the Drug in the this instance, 
    //returning -1, 0, or 1, as per the Icomparable interface.
    public int CompareTo( object obj ) 
    {  
        if (obj is Drug)// checks if object is an instance of the Drug class
        {
            
            Drug newDrug = (Drug) obj;
            return (Name.CompareTo(newDrug.Name));
        }   
        
        else 
        {
            throw new ArgumentException ("Comparison Not Possible");
        }
        
    } 

}

// -----------------------------------------------------------------------------
// Linked list of Drugs.  A list object holds references to its head and tail
// Nodes and a count of the number of Nodes.
class DrugList
{
    // Nodes form the singly linked list.  Each node holds one Drug item.
    class Node : IComparable
    {
        Node next;
        Drug data;
        
        public Node( Drug data ) { next = null; this.data = data; }
        
        public Node Next{ get { return next; } set { next = value; } }
        public Drug Data{ get { return data; } }
        
        //Compares if the name of data stored in a node to the name of the data stored in the this instance.
        public int CompareTo( object obj )
        {
            if (obj is Node)// checks if the object is an instance of the Node class
            {
               
                Node newDrug = (Node) obj;
                return (this.Data.Name.CompareTo(newDrug.Data.Name));
            }  
            
            else 
            {
                throw new ArgumentException ("Comparison not possible");
            }
        }
    }
    
    Node tail;
    Node head;
    int count;
    
    public int Count { get { return count; } }
    
    // Constructors:
    public DrugList( ) { tail = null; head = null; count = 0; }
    public DrugList( string drugFile ) : this( ) { AppendFromFile( drugFile ); }
   
    // Methods which add elements:
    // Build this list from a specified drug file.
    public void AppendFromFile( string drugFile )
    {
        using( StreamReader sr = new StreamReader( drugFile ) )
        {
            while( ! sr.EndOfStream )
            {
                string line = sr.ReadLine( );
                
                // Extract drug information
                string name = line.Substring( 7, 30 ).Trim( );
                string id = line.Substring( 37, 13 ).Trim( );
                string temp = line.Substring( 50, 14 ).Trim( );
                double size 
                    = double.Parse( temp.Substring( 0 , temp.Length - 2 ) );
                string unit = temp.Substring( temp.Length - 2, 2 );
                double quantity = double.Parse( line.Substring( 64, 16 ) );
                double lowest = double.Parse( line.Substring( 80, 10 ) );
                double ingredientCost 
                    = double.Parse( line.Substring( 90, 12 ) );
                int numTar = int.Parse( line.Substring( 102, 8 ) );
                double totalPaid = double.Parse( line.Substring( 110, 14 ) );
                double averagePaid = double.Parse( line.Substring( 124, 10 ) );
                int daysSupply 
                    = ( int ) double.Parse( line.Substring( 134, 14 ) );
                int claimLines = int.Parse( line.Substring( 148 ) );
                
                // Put drug onto this list of drugs.
                Append( new Drug( name, id, size, unit, quantity, lowest, 
                    ingredientCost, numTar, totalPaid, averagePaid, 
                    daysSupply, claimLines ) );
            }
        }
    }
    
    // Adds a new Node with specified Drug data to the end of this linked list.
    public void Append( Drug data )
    {
        Node tmp = new Node (data); 
        
        if (tmp.Data == null)
            return;
        if (head == null)
        {
            head = tail = tmp; 
        }
        else
        {
            tail.Next = tmp;
            tail = tmp;
        }
         count++;
        
    }
    
    // Adds a new Drug in order based on a CompareTo method
    // The new Drug is inserted just before the Drug which is greater than it.
    public void InsertInOrder( Drug data )
    {
        Node newNode = new Node(data);
        if (newNode == null)
            return;
        if (head == null)
        {
            head = newNode;
            tail = newNode;
            newNode.Next = null;
            return;
        }
        Node tmp = head;
        Node previous = null;
        // searches for the index prior to the closest Drug which is larger than our Drug.
        while (tmp!= null && newNode.Data.CompareTo(tmp.Data) == 1)
        {
            previous = tmp;
            tmp = tmp.Next;
        }
        if (previous == null)
        {
            newNode.Next = head;
            head = newNode;
        }
        else
        {
            if (previous.Next == null)
            {
                tail = newNode;
            }
            newNode.Next = previous.Next;
            previous.Next = newNode;
        }
                
    }
    
    
    // Removes the first Drug from the linked list.
    public Drug RemoveFirst( )
    {
        Node loc;
         if (head == null)
            return null;
            
         else
        {
            loc = head;
            head = head.Next;
            return loc.Data;
        }    
    }
    
    // Remove the minimal Drug of the linked list.
   
    public Drug RemoveMin( )
    {
        Node previousMinimum = null;
        Node previous = head;
        Node currentMinimum = head;
        Node current;
        if( head == tail )
        {
            if( head == null )
            {
            // If the list is empty, return null
                return null;
            }
            else
            {
            // If the list contains one node, node is returned and list is reset.
                head = null;
                tail = null;
            return currentMinimum.Data;
            }
        }
        // This loop traverses the list and records a reference to the element with the smallest value
        current = head.Next;
        while( current != null )
        {
            if( current.Data.CompareTo(currentMinimum.Data) == -1 )
            {
        // The current node contains the smallest value found so far
                previousMinimum = previous;
                currentMinimum = current;
            }
            previous = current;
            current = current.Next;
        }
        if( head == currentMinimum )
        {
        // update head reference 
            head = currentMinimum.Next;
        }
        else
        {
            if( tail == currentMinimum )
            {
            // update tail reference
                tail = previousMinimum;
            }
            previousMinimum.Next = currentMinimum.Next;
        }
        return currentMinimum.Data;
    }
        
      
    
    // Methods which sort the list:
    // Sort this list by selection sort.
    // Continuously removes the minimal node and adds it to the end
    public void SelectSort( )
    {
        DrugList sortedList = new DrugList ();
        Drug tmp = RemoveMin();
        while (tmp!=null)
        {
            sortedList.Append(tmp);
            tmp = RemoveMin();
        }
        head = sortedList.head;
        tail = sortedList.tail;     
    }
    
    // Sort this list by insertion sort
    //Continuously removes the first node and the inserts it at a point 
    //where it is greater than the preceding node and less than the following node 
    public void InsertSort( )
    {
        DrugList sortedList = new DrugList ();
        Drug tmp = RemoveFirst();
        while (tmp!=null)
        {
            sortedList.InsertInOrder(tmp);
            tmp = RemoveFirst();
        }
        head = sortedList.head;
        tail = sortedList.tail;
    }
    
    // Methods which extract the Drugs:
    // Return, as an array, references to all the Drug objects on the list.
    public Drug[ ] ToArray( )
    {
        Drug[ ] result = new Drug[ count ];
        int nextIndex = 0;
        Node current = head;
        while( current != null )
        {
            result[ nextIndex ] = current.Data;
            nextIndex ++;
            current = current.Next;
        }
        return result;
    }
    
    // Return a collection of references to the Drug items on this list which 
    // can be used in a foreach loop.  Understanding enumerations and the 
    // 'yield return' statement is beyond the scope of the course.
    public IEnumerable< Drug > Enumeration
    {
        get
        {
            Node current = head;
            while( current != null )
            {
                yield return current.Data;
                current = current.Next;
            }
        }
    }
}

// -----------------------------------------------------------------------------
// Test the linked list of Drugs.
static class Program
{
    static void Main( )
    {
        DrugList drugs = new DrugList( "RXQT1402.txt" );
        drugs.InsertSort( );
        foreach( Drug d in drugs.ToArray( ) ) Console.WriteLine( d );
        
    }
}
