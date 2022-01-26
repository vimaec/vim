namespace Vim.ObjectModel
{
    public static class ObjectModelVersion
    {
        public static readonly SerializableVersion Current = SerializableVersion.Parse("4.1.0");
    }

    public partial class Entity
    {
        public int Index;
        public Document Document;
    }

    public partial class EntityWithElement : Entity
    {
        public Relation<Element> _Element;
    }

    [TableName(TableNames.Asset)]
    public partial class Asset : Entity
    {
        public string BufferName;
    }

    /// <summary>
    /// Defines how a parameter is displayed by indicating whether the value's type is a length, a volume, etc,
    /// and how that value should be shown via its spec (ex: as fractional inches, etc).
    /// </summary>
    [TableName(TableNames.DisplayUnit)]
    public partial class DisplayUnit : Entity, IStorageKey
    {
        public string Spec; // ex: "UT_Length" in Revit 2020 and prior, or "autodesk.spec.aec:length-1.0.0" in Revit 2021 and up.
        public string Type; // ex: "DUT_FEET_FRACTIONAL_INCHES" in Revit 2020 and prior, or "autodesk.unit.unit:feetFractionalInches-1.0.0" in Revit 2021 and up.
        public string Label; // The localized label, ex: "Feet and fractional inches"
    }

    /// <summary>
    /// Represents a parameter descriptor.
    /// </summary>
    [TableName(TableNames.ParameterDescriptor)]
    public partial class ParameterDescriptor : Entity, IStorageKey
    {
        public string Name;
        public string Group;
        public string ParameterType;
        public bool IsInstance;
        public bool IsShared;
        public bool IsReadOnly;
        public Relation<DisplayUnit> _DisplayUnit;
    }

    /// <summary>
    /// Represents a parameter associated to an element. An element can contain 0..* parameters.
    /// </summary>
    [TableName(TableNames.Parameter)]
    public partial class Parameter : EntityWithElement
    {
        /// <summary>
        /// A pipe-separated "NativeValue|DisplayValue" string.<br/>
        /// Pipe "|" or backslash "\" characters contained inside the NativeValue or DisplayValue parts are escaped with the backslash "\" character.
        /// If the value is not pipe-separated, it is both the NativeValue and the DisplayValue.
        /// </summary>
        public string Value;
        public Relation<ParameterDescriptor> _ParameterDescriptor;
    }

    [TableName(TableNames.Element)]
    public partial class Element : Entity
    {
        public int Id;
        public string Type;
        public string Name;
        public Vector3 Location;
        public string FamilyName;
        public Relation<Level> _Level;

        public Relation<Phase> _PhaseCreated;
        public Relation<Phase> _PhaseDemolished;
        public Relation<Category> _Category;
        public Relation<Workset> _Workset;
        public Relation<DesignOption> _DesignOption;
        public Relation<View> _OwnerView;
        public Relation<Group> _Group;
        public Relation<AssemblyInstance> _AssemblyInstance;
        public Relation<BimDocument> _BimDocument;
        public Relation<Room> _Room;
    }

    [TableName(TableNames.Workset)]
    public partial class Workset : Entity
    {
        public int Id;
        public string Name;
        public string Kind;
        public bool IsOpen;
        public bool IsEditable;
        public string Owner;
        public string UniqueId;
    }

    [TableName(TableNames.AssemblyInstance)]
    public partial class AssemblyInstance : EntityWithElement
    {
        public string AssemblyTypeName;
        public Vector3 Position;
    }

    [TableName(TableNames.Group)]
    public partial class Group : EntityWithElement
    {
        public string GroupType;
        public Vector3 Position;
    }

    [TableName(TableNames.DesignOption)]
    public partial class DesignOption : EntityWithElement
    {
        public bool IsPrimary;
    }

    [TableName(TableNames.Level)]
    public partial class Level : EntityWithElement
    {
        /// <summary>Retrieves or changes the elevation above or below the ground level.
        ///    This property retrieves or changes the elevation above or below the ground level of the
        ///    project. If the Elevation Base parameter is set to Project, the elevation is relative to project origin.
        ///    If the Elevation Base parameter is set to Shared, the elevation is relative to shared origin which can
        ///    be changed by relocate operation. The value is given in decimal feet.
        /// </summary>
        public double Elevation;
    }

    [TableName(TableNames.Phase)]
    public partial class Phase : EntityWithElement
    {
    }

    [TableName(TableNames.Room)]
    public partial class Room : EntityWithElement
    {
        public double BaseOffset;
        public double LimitOffset;
        public double UnboundedHeight;
        public double Volume;
        public double Perimeter;
        public double Area;
        public string Number;
        public Relation<Level> _UpperLimit;
    }

    [TableName(TableNames.BimDocument)]
    public partial class BimDocument : EntityWithElement
    {
        public string Title;
        public bool IsMetric;

        [IgnoreInEquality]
        //Ignore Guid in equality comparer because it gets exported different by revit when it should be the same.
        public string Guid;

        public int NumSaves;
        public bool IsLinked;
        public bool IsDetached;
        public bool IsWorkshared;
        public string PathName;
        public double Latitude;
        public double Longitude;
        public double TimeZone;
        public string PlaceName;
        public string WeatherStationName;
        public double Elevation;
        public string ProjectLocation;
        public string IssueDate;
        public string Status;
        public string ClientName;
        public string Address;
        public string Name;
        public string Number;
        public string Author;
        public string BuildingName;
        public string OrganizationName;
        public string OrganizationDescription;
        public string Product;
        public string Version;
        public string User;
        public Relation<View> _ActiveView;
        public Relation<Family> _OwnerFamily;
        public Relation<BimDocument> _Parent;
    }

    /// <summary>
    /// An associative table used to list the display units associated with a given bim document.
    /// </summary>
    [TableName(TableNames.DisplayUnitInBimDocument)]
    public partial class DisplayUnitInBimDocument : Entity, IStorageKey
    {
        public Relation<DisplayUnit> _DisplayUnit;
        public Relation<BimDocument> _BimDocument;
    }

    [TableName(TableNames.PhaseOrderInBimDocument)]
    public partial class PhaseOrderInBimDocument : Entity, IStorageKey
    {
        public int OrderIndex;

        public Relation<Phase> _Phase;
        public Relation<BimDocument> _BimDocument;
    }

    [TableName(TableNames.Category)]
    public partial class Category : Entity
    {
        public string Name;
        public int Id;
        public string CategoryType;
        public DVector3 LineColor;
        /// <summary>
        /// Represents the associated built-in category in Revit.
        /// </summary>
        public string BuiltInCategory;

        public Relation<Category> _Parent;
        public Relation<Material> _Material;
    }

    /// <summary>
    /// The Family element represents the entire family that consists of a collection of types, such as an 'I Beam'.
    /// You can think of that object as representing the entire family file. 
    /// </summary>
    [TableName(TableNames.Family)]
    public partial class Family : EntityWithElement
    {
        public string StructuralMaterialType;
        public string StructuralSectionShape;
        public bool IsSystemFamily;
        public Relation<Category> _FamilyCategory;
    }

    /// <summary>
    /// The Family object contains a number of FamilySymbol elements. The
    /// The FamilySymbol object represents a specific set of family settings within that Family and
    /// represents what is known in the Revit user interface as a Type, such as 'W14x32'. 
    /// </summary>
    [TableName(TableNames.FamilyType)]
    public partial class FamilyType : EntityWithElement
    {
        public bool IsSystemFamilyType;
        public Relation<Family> _Family;
        public Relation<CompoundStructure> _CompoundStructure;
    }

    /// <summary>
    /// The FamilyInstance object represents an actual instance of that
    /// type placed the Autodesk Revit project. For example there would
    /// be a single instance of a W14x32 column within the project. 
    /// </summary>
    [TableName(TableNames.FamilyInstance)]
    public partial class FamilyInstance : EntityWithElement
    {
        public bool FacingFlipped;
        public Vector3 FacingOrientation;
        public bool HandFlipped;
        public bool Mirrored;
        public bool HasModifiedGeometry;
        public float Scale;
        public Vector3 BasisX;
        public Vector3 BasisY;
        public Vector3 BasisZ;
        public Vector3 Translation;
        public Vector3 HandOrientation;
        public Relation<FamilyType> _FamilyType;
        public Relation<Element> _Host;
        public Relation<Room> _FromRoom;
        public Relation<Room> _ToRoom;
    }

    [TableName(TableNames.View)]
    public partial class View : EntityWithElement
    {
        public string Title;
        public string ViewType;
        public DVector3 Up;
        public DVector3 Right;
        public DVector3 Origin;
        public DVector3 ViewDirection;
        public DVector3 ViewPosition;
        public double Scale;
        public DAABox2D Outline;
        public int DetailLevel; // 0 = Undefined, 1 = Coarse, 2 = Medium, 3 = Fine
        public Relation<Camera> _Camera;
    }

    /// <summary>
    /// An associative table which binds elements to views.
    /// </summary>
    [TableName(TableNames.ElementInView)]
    public partial class ElementInView : EntityWithElement, IStorageKey
    {
        public Relation<View> _View;
    }

    /// <summary>
    /// An associative table which binds shapes to views.
    /// </summary>
    [TableName(TableNames.ShapeInView)]
    public partial class ShapeInView : Entity, IStorageKey
    {
        public Relation<Shape> _Shape;
        public Relation<View> _View;
    }

    /// <summary>
    /// An associative table which binds an asset to a View.
    /// </summary>
    [TableName(TableNames.AssetInView)]
    public partial class AssetInView : Entity, IStorageKey
    {
        public Relation<Asset> _Asset;
        public Relation<View> _View;
    }

    [TableName(TableNames.Camera)]
    public partial class Camera : Entity
    {
        public int Id;
        /// <summary>Identifies whether the projection is orthographic 0 or perspective 1</summary>
        public int IsPerspective;

        /// <summary>Distance between top and bottom planes on the target plane.</summary>
        public double VerticalExtent;

        /// <summary>Distance between left and right planes on the target plane.</summary>
        public double HorizontalExtent;

        /// <summary>
        ///    Distance from eye point to far plane of view frustum along the view direction.
        ///    This property together with NearDistance determines the depth restrictions of a view frustum.
        /// </summary>
        public double FarDistance;

        /// <summary>
        ///    Distance from eye point to near plane of view frustum along the view direction.
        ///    This property together with FarDistance determines the depth restrictions of a view frustum.
        /// </summary>
        public double NearDistance;

        /// <summary>Distance from eye point along view direction to target plane.
        ///    This value is appropriate for perspective views only.
        ///    Attempts to get this value for an orthographic view can
        ///    be made, but the obtained value is to be ignored.
        /// </summary>
        public double TargetDistance;

        /// <summary>
        ///    Distance that the target plane is offset towards the right
        ///    where right is normal to both Up direction and View direction.
        ///    This offset shifts both left and right planes.
        /// </summary>
        public double RightOffset;

        /// <summary>
        ///    Distance that the target plane is offset in the direction of
        ///    the Up direction. This offset shifts both top and bottom planes.
        /// </summary>
        public double UpOffset;
    }

    [TableName(TableNames.Material)]
    public partial class Material : EntityWithElement
    {
        /// <summary>
        /// Material name
        /// </summary>
        public string Name;

        /// <summary>
        /// The type of the category.
        /// </summary>
        public string MaterialCategory;

        /// <summary>
        /// The diffuse (albedo) color.
        /// </summary>
        public DVector3 Color;

        /// <summary>
        /// The asset representing the diffuse (albedo) color texture.
        /// </summary>
        public Relation<Asset> _ColorTextureFile;

        /// <summary>
        /// The UV scaling factor of the diffuse (albedo) color texture.
        /// </summary>
        public DVector2 ColorUvScaling = new DVector2(1, 1);

        /// <summary>
        /// The UV offset of the diffuse (albedo) color texture.
        /// </summary>
        public DVector2 ColorUvOffset;

        /// <summary>
        /// The asset representing the normal (bump) texture.
        /// </summary>
        public Relation<Asset> _NormalTextureFile;

        /// <summary>
        /// The UV scaling factor of the normal (bump) texture.
        /// </summary>
        public DVector2 NormalUvScaling = new DVector2(1, 1);

        /// <summary>
        /// The UV offset of the normal (bump) texture.
        /// </summary>
        public DVector2 NormalUvOffset;

        /// <summary>The magnitude of the normal texture effect.</summary>
        public double NormalAmount;

        /// <summary>The glossiness, defined in the domain [0..1].</summary>
        public double Glossiness;

        /// <summary>The smoothness, defined in the domain [0..1]</summary>
        public double Smoothness;

        /// <summary>The transparency, defined in the domain [0..1]</summary>
        public double Transparency;
    }

    public static class MaterialFunctionAssignment
    {
        public const string None = nameof(None); // Revit enum value: 0
        public const string Structure = nameof(Structure); // Revit enum value: 1
        public const string Substrate = nameof(Substrate); // Revit enum value: 2
        public const string Insulation = nameof(Insulation); // Revit enum value: 3
        public const string Finish1 = nameof(Finish1); // Revit enum value: 4
        public const string Finish2 = nameof(Finish2); // Revit enum value: 5
        public const string Membrane = nameof(Membrane); // Revit enum value: 100, i.e. 0x00000064
        public const string StructuralDeck = nameof(StructuralDeck); // Revit enum value: 200, i.e. 0x000000C8
    }

    [TableName(TableNames.CompoundStructureLayer)]
    public partial class CompoundStructureLayer : Entity
    {
        public int OrderIndex;
        public double Width;
        public string MaterialFunctionAssignment;

        public Relation<Material> _Material;
        public Relation<CompoundStructure> _CompoundStructure;
    }

    [TableName(TableNames.CompoundStructure)]
    public partial class CompoundStructure : Entity
    {
        /// <summary>
        /// If the structure is not vertically compound, then this is simply the sum of all layers' widths.
        /// If the structure is vertically compound, this is the width of the rectangular grid stored in the
        /// vertically compound structure. The presence of a layer with variable width has no effect on the
        /// value returned by this method. The value returned assumes that all layers have their specified
        /// width.<br/>
        /// See: <see href="https://www.revitapidocs.com/2020/dc1a081e-8dab-565f-145d-a429098d353c.htm">Revit API Docs - CompoundStructure.GetWidth()</see>.
        /// </summary>
        public double Width;
        /// <summary>
        /// Indicates the layer whose material defines the structural properties of the type for the purposes of analysis.<br/>
        /// See: <see href="https://www.revitapidocs.com/2020/cf4d771e-6ed2-ec6a-d32d-647fb5b649b3.htm">Revit API Docs - CompoundStructure.StructuralMaterialIndex</see>.
        /// </summary>
        public Relation<CompoundStructureLayer> _StructuralLayer;
    }

    [TableName(TableNames.Node)]
    [ElementIsOptional]
    public partial class Node : EntityWithElement
    {
    }

    [TableName(TableNames.Geometry)]
    public partial class Geometry : Entity
    {
        public AABox Box;
        public int VertexCount;
        public int FaceCount;
    }

    /// <summary>
    /// Shape entities represent the shapes defined in the g3d.
    /// A shape is a sequence of Vector3 points in world space.
    /// </summary>
    [TableName(TableNames.Shape)]
    public partial class Shape : EntityWithElement
    {
    }

    /// <summary>
    /// ShapeCollection entities represent a collection of shapes associated with an Element.
    /// Currently, these define the shapes representing the curve loops on a face on an element;
    /// faces may have a number of curve loops which may designate the contour of the face and its holes.
    /// </summary>
    [TableName(TableNames.ShapeCollection)]
    public partial class ShapeCollection : EntityWithElement
    {
    }

    /// <summary>
    /// ShapeInShapeCollection represents the optional association between a Shape and a ShapeCollection.
    /// </summary>
    [TableName(TableNames.ShapeInShapeCollection)]
    public partial class ShapeInShapeCollection : Entity, IStorageKey
    {
        public Relation<Shape> _Shape;
        public Relation<ShapeCollection> _ShapeCollection;
    }
}
