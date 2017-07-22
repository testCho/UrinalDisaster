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

        private List<CornerRail> corners = new List<CornerRail>();
        private List<EdgeRail> edges = new List<EdgeRail>();


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
            InitializeRails(); 
            PlaceCornerBlock();
            PlaceEdgeBlock();
            TryTwoUnitCornerBlock();
        }


        //sub
        private void InitializeRails()
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

        private void PlaceCornerBlock()
        {
            if (corners.Count == 0)
                return;

            foreach(CornerRail i in corners)
            {
                if (i.LinkedEdges.Count > 3)
                    TreatMultiLinkCorner(); //??


            }
        }

        private void PlaceEdgeBlock()
        {
        }

        private void TryTwoUnitCornerBlock()
        {

        }

        private void TreatMultiLinkCorner()
        { }

    }


    #region DataStructure

    //rail
    class CornerRail
    {
        //fields
        private List<ApartBlock> blocks = new List<ApartBlock>();
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
        private List<ApartBlock> blocks = new List<ApartBlock>();
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
            this.midPt = (edgeLine.From + edgeLine.To);
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


    class ApartBlock
    {
        private List<ApartUnit> units = new List<ApartUnit>();

        //constructor
        public ApartBlock()
        {

        }

        public ApartBlock(List<ApartUnit> units)
        {
            this.units = units;
        }


        //method
        private void RenewLink()
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
    }


    class ApartUnit
    {
        //fields
        private Point3d origin = new Point3d();
        private List<Point3d> connectionPts = new List<Point3d>();
        private Polyline outline = new Polyline();
        private Vector3d corridorAlign = new Vector3d();
        private Vector3d corridorPerp = new Vector3d();
        private double alignO = 0;
        private double alignI = 0;
        private double perpO = 0;
        private double perpI = 0;

        CoreTypeL coreType = CoreTypeL.Stair;
        private bool isFlip = false;

           
        //constructor
        public ApartUnit()
        {}

        public ApartUnit(CoreTypeL coreType, double alignO, double alignI, double perpO, double perpI)
        {
            this.coreType = coreType;
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


        //properties - type
        public CoreTypeL CoreType { get { return coreType; } }
        public bool IsFlip { get { return isFlip; } }

        
    }

    public enum CoreTypeL { Stair, Corridor, Tower }
    #endregion DataStructure

    #region TempDB
    class UnitDatabase
    {
        private static List<ApartUnit> units = new List<ApartUnit>();
        private static List<ApartBlock> blocks = new List<ApartBlock>();
        private static List<ApartBlock> selectedBlocks = new List<ApartBlock>();

        //method
        public static List<ApartBlock> GetBlocksByCoreType(string coreType)
        {
            return new List<ApartBlock>();
        }

        public static List<ApartBlock> GetBlocksByArea(double area)
        {
            return new List<ApartBlock>();
        }

        public static List<ApartBlock> GetBlocksByCoreAndArea(string coreType, double area)
        {
            return new List<ApartBlock>();
        }

        public static List<ApartUnit> GetUnitsByCoreType(string coreType)
        {
            return new List<ApartUnit>();
        }

        public static List<ApartUnit> GetUnitsByArea(double area)
        {
            return new List<ApartUnit>();
        }

        public static List<ApartUnit> GetUnitsByCoreAndArea(string coreType, double area)
        {
            return new List<ApartUnit>();
        }

        //properties
        public static List<ApartUnit> AllUnits { get { return units; } }
        public static List<ApartBlock> AllBlocks { get { return blocks; } }
        public static List<ApartBlock> SelectedBlocks { get { return selectedBlocks; } }
     }
    #endregion
}
