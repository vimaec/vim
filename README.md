# VIM v1.0.0

Author: VIMaec LLC
Updated: May 28th, 2020

# About VIM

VIM is a modern, efficient, and compact 3D open data interchange format to quickly transport design data and geometry from Revit, IFC, and other BIM sources to real-time engines, 3D editors, and mixed reality experiences.

## Format Design Goals

The VIM format was designed for efficient rendering on multiple platforms, and to transmit large amounts of BIM data. As a result the format aims to minimize the processing needed before being able to hand the geometry data to a GPU for efficient rendering. 

* Unlike Revit and FBX, VIM is cross platform and does not require an SDK to read or write
* Unlike IFC and STEP, VIM geometry is already triangulated and ready for rendering on a GPU
* Unlike glTF and USD, VIM supports structured BIM data in a compact relational form
* Unlike 3DXML and Collada, VIM is a binary format and requires minimal parsing 

# About this Specification 

This is the specification for version 1.0.0 of the VIM data format. It is divided up into two parts: 

1. VIM Format Binary Specification 
2. VIM Data Model Schema 

## About the Version 

The VIM format version now uses a <major>.<minor>.<revision> naming scheme. Changes to the VIM major number indicate a breaking change that will prevent older VIM tools from loading that VIM file. Changes to the minor number indicate that significant new data is present or no longer expected in the format, but that older VIM tools should still continue to function. Change to the revision number, indicate a small update to the object model only, with no significant impact to older tools.

# 1. VIM Format Binary Specification

A VIM document conforms to the BFAST (Binary Format for Array Serialization and Transmission) binary data format. A BFAST is similar to a ZIP (without compression) or a TAR archive and is a collection of named binary buffers, with an address table for fast access to the sub-assets. 

There are five expected top-level buffers in the VIM file with the following names. Their order is not critical and any section except the header is optional. 

* header
* assets
* entities
* strings
* geometry

## Header Buffer

The header section contains the VIM file version and additional meta data as a JSON-encoded object where the values are only allowed to be strings. String escaping is not allowed in keys or values. 

The following is an example:

```
{
  "vim" : "0.10.0",
  "id" : "03280421-595d-4a35-802e-83bb6739e7ae",
  "revision" : "f7eaf6b2-55b2-4f55-87f4-cdc9a01aa406",
  "generator" : "Vim.Revit.Exporter:v1.46:Revit2020",
  "created" : "2020-05-20T16:31:19Z",
}
```

The field names are case insensitive. The only required field is `vim` which must have the value in the format `<major>.<minor>.<patch>` representing the file format version. 
The `id` field should be a uniquely generated GUID created the first time a VIM is created, but is not changed when the file is modified or saved.  
The `revision` field if present should be a new GUID created anytime a VIM is modified or updated.
The `created` field, should contain a date in ISO_8601 format. 
The `generator` field contains the name of the program used to generate or edit the VIM. 

## Assets Buffer

The assets section of a BIM is also a BFAST container. It may contain any number of buffers with any names. Buffers prefixed with the name `textures\` are assumed to be texture files.   

### Geometry Buffer

The geometry section of a VIM contains the merged geometry and basic scene graph information for an entire using a shared vertex and index buffer. G3D is based on the BFAST binary layout, and uses a naming convention to identify the layout of each attribute buffer and how it is used. 

G3D attributes are arrays of data associated with different parts of the overall geometry. This may be:
* vertex
* face
* corner
* edge
* group 
* all
* none
* material
* instance

Group refers to "face group" and can be thought of as a sub-mesh. It is is a contiguous set of faces within the total vertex buffer. 

G3D attributes have names to identify them (e.g. position or uv) and uses indices to distinguish when multiple attributes share the same name (e.g. uv:0 ... uv:8). 

An attribute is one of the following core data datatypes: float32, float64, int8, int16, int32, int64, string. String data is encoded as a sequence of null terminated strings. 

The geometry in a VIM must contain the following attributes:

* `g3d:vertex:position:0:float32:3` -	Position data arranged as 3 single precision floating point values per vertex
* `g3d:corner:index:0:uint32:1` - The index buffer, which is a list of 32-bit integers 
* `g3d:group:material:0:uint32:1` - The index of the material associated with a face group. 
* `g3d:group:indexoffset:0:uint32:1` - The offset of the index buffer for a group. 
* `g3d:group:vertexoffset:0:uint32:1` - The offset into the vertex buffer for a group
* `g3d:material:name:0:string:1`- The name of each material 
* `g3d:material:texture:0:string:1`- The name of each texture, as specified in the textures section of the assets.
* `g3d:material:color:0:float32:4`- A RGBA color representing the diffuse color of a material
* `g3d:node:group:0:uint32:1`- The index of the face group associated with a particular node
* `g3d:node:position:0:float64:16`- The transform of a node encoded as a 4x4 matrix in row major order

The following attributes are optional:

* `g3d:vertex:uv:0:float32:2` - The UV buffer, which is a list of 2 single-precision floating point values per vertex 
* `g3d:vertex:color:0:float32:3` - The UV buffer, which is a list of 2 single-precision floating point values per vertex 
* `g3d:node:parent:0:uint32:1`- The index of a parent node 

Additional attributes are possible, but are ignored, and may or may not be written out by any tool that inputs and outputs VIM files.

Each face group is assumed to use consecutive sequences of indices and vertices. This implies that each member of the `indexoffset` and `vertexoffset` attributes are greater than or equal to the previous, and that the first value is zero. It is possible that a face group has zero indices and zero vertices. 

The values of the index buffer are not offset: they are relative to the beginning of the vertex buffer. 

*Recommendation:* It is good practice to offset the geometries so that their origin is either at the center of all points, or at the bottom center of all the points. 

### Note About Textures

An application may or may not be able to render a given texture type. Any referenced texture file is presumed to be contained as a buffer in the assets section with the prefix `textures\`. 

For example a reference to a texture file `wood.png` would be found as a named buffer in the assets section as `textures\wood.png`. 

## Entities Buffer

The entities section of a VIM contains a collection of entity tables. An entity table is a combination of a relational table alongside a collection of key-value pairs, about a particular data entity. Examples of data entities include: geometry, nodes, materials, cameras, assets, and various BIM elements such as Elements or Products. 

Each relational table consists of three types of columns: 

1. numerical - in which each value is a double precision (64 bit) floating point value
2. string - in which each value is an index into the string table, or a -1 to indicate no string
3. relation - in which each value is an index into another entity table indicated by the column name, or a -1 to indicate no entity

For more information on what they expected entities are and what columns they are expected to contain in this version of VIM see the VIM Data Model Section of the documentation. 

The entities section is encoded as a BFAST with each buffer containing an entity table. Each entity table is also encoded as a BFAST with each column as a named buffer, and a collection of key/value pairs encoded in its own column. 

## Strings Buffer

The strings buffer contains a sequence of strings of zero or more length, delimited by the "NUL" character. The zero-based index of each string (typically the first one is the empty string) is used by keys and values in the key/value collections associated with entities, and the string columns of entity tables. 

*Recommendation:* strings should be ordered alphabetically and should be unique.

# VIM Object Model

W.I.P.
