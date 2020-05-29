# VIM v0.10.0

VIM is a modern, efficient, and compact 3D open data interchange format to quickly transport design data and geometry from Revit, IFC, and other BIM sources to real-time engines, 3D editors, and mixed reality experiences.

## About this Specification 

This is the specification for version 0.10 of the VIM data format. 

## Design Goals

The VIM format was designed for efficient rendering, and to transmit large amounts of BIM data. One goal of the format was to minimize the processing needed before being able to hand the geometry data to a GPU for efficient rendering. Another goal was to maintain as much information as possible about the. 

## Top Level Container Format

A VIM document conforms to the BFAST (Binary Format for Array Serialization and Transmission) binary data format. A BFAST is similar to a ZIP or TAR archive and is a collection of named binary buffers, with an address table for fast access to the sub-assets. 

There are five expected sections with the following names. Their order is not critical and any section except the header is optional. 

* header
* assets
* entities
* strings
* geometry

### Header Section

The Header Section contains information about the VIM 

### Assets Section

The assets section of a BIM is also a BFAST container. 

### Geometry Section

The geometry of a VIM conforms to the G3D data format for geometry data and the scene graph. G3D is based on the BFAST binary layout, and uses a naming convention to identify the layout of each attribute buffer and how it is used. 

The geometry in a VIM must contain the following attributes:

* `g3d:vertex:position:0:float32:3` -	Position data arranged as 3 single precision floating point values per vertex.
* `g3d:corner:index:0:int32:1` - The index buffer, which is a list of 32-bit integers 
* `g3d:submesh:material:0:int32:1` - The material ID 
* `g3d:submesh:indexoffset:0:int32:1` - 
* `g3d:submesh:indexcount:0:int32:1` - 
* `g3d:submesh:vertexoffset:0:int32:1` - 
* `g3d:submesh:vertexcount:0:int32:1` - 
* `g3d:material:name:0:string:1`-
* `g3d:material:texture:0:string:1`-
* `g3d:material:color:0:int8:4`-
* `g3d:instance:parent:0:int32:1`-
* `g3d:instance:objectid:0:int32:1`-
* `g3d:instance:transform:0:float64:16`-
* `g3d:instance:center:0:float32:3`-
* `g3d:instance:radius:0:float32:1`-

A VIM may contain the following attribute: 

* `g3d:vertex:uv:0:float32:2` - The UV buffer, which is a list of 2 single-precision floating point values per vertex 

### Entities Section

The entities section of a VIM contains additional information in either a relational table, or collection of key-value pairs, about the the geometry, nodes, materials, cameras, assets, and BIM elements. 

###

# VIM Object Model

