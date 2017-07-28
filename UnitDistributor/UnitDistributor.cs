using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace UnitDistributor
{
    interface IUnitDistibutor
    {
        bool IsAvailable();
        void Distribute();
    }

    class StairTypeDistributor: IUnitDistibutor
    {
        //fields
        private List<Line> aptLines = new List<Line>();
        private CoreTypeL coreType = CoreTypeL.Stair;
        private double maxAptDepth = 0;

        private List<int> targetUnitAreas = new List<int>();
        private List<CornerRail> corners = new List<CornerRail>();
        private List<EdgeRail> edges = new List<EdgeRail>();

        private Queue<ApartBlock> blockQueue = new Queue<ApartBlock>();


        //constructor
        public StairTypeDistributor()
        { }

        
        //main
        public bool IsAvailable()
        {
            //check cornerType.
            //check if cannot satisfy target.

            return true;
        }

        public void Distribute()
        {
            DetectAptLineTopology();
            AddEntryBlockOnEdge();
            PlaceAtThreeUnitCorner();
            DistributeAgain();
            OptimizeCoverage();
            OptimizeEntry();
        }


        //sub
        private void SetInitialUnitQueue()
        {
            //add All rail length.
            double railLengthSum = 0;

            foreach (Line i in aptLines)
                railLengthSum += i.Length;

            
            //caculate length ratio by Area
            List<double> allocatedLengthPerArea = new List<double>();


            //caculate max unit count

            
            //enqueue
        }

        private void DetectAptLineTopology()
        {
            List<Point3d> vertices = new List<Point3d>();
            List<CornerRail> cornerCandidates = new List<CornerRail>();

            //add edgeRails and coner candidates.
            foreach (Line edge in aptLines)
            {
                EdgeRail currentEdgeRail = new EdgeRail(edge);
                edges.Add(currentEdgeRail);


                //add startPt as corner.
                int startPtIndex = vertices.FindIndex(n => n.Equals(edge.From));

                if(startPtIndex != -1)
                {
                    CornerRail existingCornerRail = cornerCandidates[startPtIndex];
                    existingCornerRail.LinkedEdges.Add(currentEdgeRail);
                }
                else
                {
                    CornerRail currentCornerRail = new CornerRail(edge.From);
                    currentCornerRail.LinkedEdges.Add(currentEdgeRail);
                    cornerCandidates.Add(currentCornerRail);
                }


                //add endPt as corner.
                int endPtIndex = vertices.FindIndex(n => n.Equals(edge.To));

                if (endPtIndex != -1)
                {
                    CornerRail existingCornerRail = cornerCandidates[endPtIndex];
                    existingCornerRail.LinkedEdges.Add(currentEdgeRail);
                }
                else
                {
                    CornerRail currentCornerRail = new CornerRail(edge.From);
                    currentCornerRail.LinkedEdges.Add(currentEdgeRail);
                    cornerCandidates.Add(currentCornerRail);
                }
            }

            //seive dead end corner rails.
            foreach(CornerRail currentCandidate in cornerCandidates)
            {
                if (currentCandidate.LinkedEdges.Count < 2)
                    continue;

                currentCandidate.LinkCornerToEdge();
                corners.Add(currentCandidate);
            }
            
        }

        private void AddEntryBlockOnEdge()
        {

        }

        
        private void PlaceAtThreeUnitCorner()
        {
            if (corners.Count == 0)
                return;

            foreach(CornerRail i in corners)
            {
                if (i.LinkedEdges.Count > 3)
                    continue;
          

            }
        }

        private void DistributeAgain()
        {
            RemoveSurplusEntry();
            PlaceAtTwoUnitCorner();
        }

        private void OptimizeCoverage()
        { }

        private void OptimizeEntry()
        { }

        private void RemoveSurplusEntry()
        {
        }

        private void PlaceAtTwoUnitCorner()
        {
        }

    }


    #region DataStructure

    //rail
    class CornerRail
    {
        //fields
        private List<IApartBlock> blocks = new List<IApartBlock>();
        private List<EdgeRail> linkedEdges = new List<EdgeRail>();
        private List<Vector3d> linkDirections = new List<Vector3d>();
        private List<Line> linkDirectionLines = new List<Line>();
        private Point3d basePt = new Point3d();

        //constructor
        public CornerRail()
        {}

        public CornerRail(Point3d cornerPt)
        {
            this.basePt = cornerPt;
        }


        //method
        public void LinkCornerToEdge()
        {
            foreach (EdgeRail edge in linkedEdges)
                edge.LinkedCorners.Add(this);

        }

        public void LeaveCornerBlank()
        {
            foreach (EdgeRail edge in linkedEdges)
            {
                //+ 기본길이 제외 가져간 길이 돌려주기..
                blocks.Clear();
            }
        }

        public void GiveCornerToEdge()
        {
            //방향체크해서 같은 방향 Edge 연결
            //Link 처리            
        }

        public void ExchangeArea()
        {
           
        }


        //properties
        public List<EdgeRail> LinkedEdges { get { return linkedEdges; } set { linkedEdges = value as List<EdgeRail>; } }
        public Point3d BasePt { get { return basePt; } private set { basePt = value;} }
    }

    class EdgeRail
    {
        //fields
        private List<IApartBlock> blocks = new List<IApartBlock>();
        private List<CornerRail> linkedCorners = new List<CornerRail>();
        private Point3d midPt = new Point3d();
        private Line baseLine = new Line();
        private Line edgeLine = new Line();

        //constructor
        public EdgeRail()
        {}

        public EdgeRail(Line edgeLine)
        {
            this.baseLine = edgeLine;
            this.edgeLine = baseLine;
            this.midPt = (edgeLine.From + edgeLine.To)*0.5;
        }


        //method
        public void ExchangeArea()
        {

        }

        //properties
        public double LeftLength
        {
            get
            {
                return baseLine.Length;
            }
        }

        public List<CornerRail> LinkedCorners {get { return linkedCorners; } set { linkedCorners = value as List<CornerRail>; } }
        public Line BaseLine { get { return baseLine; } private set { baseLine = value; } }
        public Point3d MidPt { get { return midPt; } }
    }


    //Block
    interface IApartBlock
    {
        double ALength { get; }
        double ADepth { get; }
        double PLength { get; }
        double PDepth { get; }
    }

    class ApartBlock: IApartBlock
    { 
        private List<ApartUnit> units = new List<ApartUnit>();

        //constructor
        public ApartBlock()
        { 
        }

        public ApartBlock(string blockID)
        {
 

        }


        //method
        private void RenewConnectionPt()
        { }

        public void AddUnit(ApartUnit unit)
        {

        }

        public void ChangeUnit(double targetLength)
        {

        }


        //properties - units
        public int Count { get { return units.Count; } }
        public List<ApartUnit> Units { get { return units; } }


        //properties - dimension
        public double ALength
        {
            get
            {
                double aLength = 0;

                foreach (ApartUnit i in units)
                {
                   
                }

                return 0;
            }
        }

        public double ADepth
        {
            get
            {
                double aLength = 0;

                foreach (ApartUnit i in units)
                {

                }

                return 0;
            }
        }

        public double PLength
        {
            get
            {
                double aLength = 0;

                foreach (ApartUnit i in units)
                {

                }

                return 0;
            }
        }

        public double PDepth
        {
            get
            {
                double aLength = 0;

                foreach (ApartUnit i in units)
                {

                }

                return 0;
            }
        }

        public bool IsFlip
        {
            get
            {
                return false;
            }
        }
    }

    class EntryBlock: IApartBlock
    {
        private double aLength = 0;
        private double aDepth = 0;
        private double pLength = 0;
        private double pDepth = 0;

        public double ALength { get { return ALength; } }
        public double ADepth { get { return ADepth; } }
        public double PLength { get { return pLength; } }
        public double PDepth { get { return pDepth; }}
    }


    //Unit
    /// Unit code: {Area,Bay,Core,Deribed,Edge,Form,Ao,Po,Ai,Pi}
    /// ex) {74,3,S,0,E,0,10800,0,8000,0} => 74m2, 3bay, 계단형 0번 코어, 엣지타입 0번 유닛, Ao:10800, Ai: 0, Po: 8000, Pi: 0

    class ApartUnit
    {
        //fields
        private string unitID = "";
        private Point3d origin = new Point3d();
        private List<Point3d> connectionPts = new List<Point3d>();
        private Polyline outline = new Polyline();
        private Vector3d corridorAlign = new Vector3d();
        private Vector3d corridorPerp = new Vector3d();
        private double alignO = 0;
        private double alignI = 0;
        private double perpO = 0;
        private double perpI = 0;

        private bool isFlip = false;

           
        //constructor
        public ApartUnit()
        {}

        public ApartUnit(string unitID, Point3d origin, bool flip)
        {
            this.unitID = unitID;
            string[] splitCodes = unitID.Split(',');
            this.alignO = int.Parse(splitCodes[6]);
            this.alignI = int.Parse(splitCodes[7]);
            this.perpO = int.Parse(splitCodes[8]);
            this.perpI = int.Parse(splitCodes[9]);

            if (flip)
                Flip();
        }

        public ApartUnit(string unitID, double alignO, double alignI, double perpO, double perpI)
        {
            this.unitID = unitID;
            this.alignO = alignO;
            this.alignI = alignI;
            this.perpO = perpO;
            this.perpI = perpI;
        }


        //method
        public void AdjustAlignOutside(double targetLength)
        { }

        public void AdjustPerpOutside(double targetLength)
        { }

        public void AdjustPerpInside(double targetLength)
        { }

        public void Flip()
        {
            corridorAlign = -corridorAlign;
            isFlip = true;
        }


        //properties
        public double Area
        {
            get
            {
                return 0;
            }
        }


        //properties - direction
        public Vector3d Align { get { return corridorAlign; } private set { corridorAlign = value; } }
        public double AlignO { get { return alignO; } private set { alignO = value; } }
        public double AlignI { get { return alignI; } private set { alignI = value; } }

        public Vector3d Perp { get { return corridorPerp; } private set { corridorPerp = value; } }
        public double PerpO { get { return perpO; } private set { perpO = value; } }
        public double PerpI { get { return perpI; } private set { perpI = value; } }

        public Point3d Origin { get { return origin; } private set { origin = value; } }
        public bool IsFlip { get { return isFlip; } }


        //properties - type
        public string ID { get { return unitID; } }

        
    }

    public enum CoreTypeL { Stair, Corridor, Tower }
    #endregion DataStructure


    #region TempDB
    class UnitSearcher
    {
       
    }
    #endregion
}
