# VIM v1.0.0
## Updated: August 8th, 2020

This documentation is Copyright 2020 VIMaec LLC. 
[Full copyright notice here](https://www.vimaec.com/copyright)

# About VIM

VIM is a modern and efficient open 3D data interchange format designed for construction and manufacturing data, that is optimized for efficient rendering on 
low-power devices. Unlike other 3D data formats, VIM is designed to carry extremely large amounts of complex relational data in an efficient and standardized manner. 

## Format Design Goals

The VIM format was originally designed for efficient rendering on multiple platforms using different languages and to transmit large amounts of BIM data 
present in real-world construction projects. As a result the format minimizes the processing needed before being able to hand the geometry data to a GPU for efficient rendering. 

### Comparison to Other Formats 

* Unlike Revit and FBX, VIM is an open and cross platform format and does not require an SDK to read or write
* Unlike IFC and STEP, VIM geometry is already triangulated and is in a GPU-friendly format
* Unlike glTF and USD, VIM supports structured BIM data in a compact relational form
* Unlike 3DXML and Collada, VIM is a binary format and requires minimal parsing 
* Unlike glTF, VIM is an opinionated format, that has fixed access patterns for data buffers, and can be extended in very precise ways to simplify parsing.
* Unlike FBX, VIM is easy to extend with new buffers. 
* Unlike glTF and FBX, VIM is not designed to specify animated assets  

# About this Specification 

This is the specification for version 1.0.0 of the VIM data format. It is divided up into two parts: 

1. VIM Format Binary Specification 
2. VIM Data Model Schema 

## About the Version 

The VIM format version uses the [Semantic Versioning](https://semver.org/) scheme: `MAJOR.MINOR.PATCH`.
* A `MAJOR` number increment indicates a breaking change. Older VIM tools are not expected to be capable of loading VIM files whose `MAJOR` version has been incremented.
* A `MINOR` number increment indicates a backwards-compatible change, for example: the addition of a new data field. Older VIM tools are expected to be capable of loading VIM files whose `MINOR` version has been incremented within the same `MAJOR` version.
* A `PATCH` number increment indicates a backwards-compatible change which does not affect the schema of the data, for example: a bug fix. Older VIM tools are expected to be capable of loading VIM files whose `PATCH` version has been incremented within the same `MAJOR` and `MINOR` version.

# 1. VIM Format Binary Specification

At the top-level a VIM document conforms to the [BFAST (Binary Format for Array Serialization and Transmission) binary data format](https://github.com/vimaec/bfast). 
A BFAST is similar to a ZIP or TAR archive without compression. 

## About BFAST 

The [BFAST data format](https://github.com/vimaec/bfast) is a useful and simple general purpose format for transporting named arrays of binary data between different 
languages and platforms.

Some properties of the BFAST format:
* 64 byte aligned data buffers 
* Data access table in the header 
* Support for big and little endian encoding
* Magic number to quickly identify BFAST format and endianess
* Support for data buffers larger than 2GB 

Some sample use cases: 
* files and folders 
* 2D images
* 2D movies  
* 3D meshes (see [G3D format](https://github.com/vimaec/g3d))
* 3D point clouds

## VIM Data Buffers

There are five expected top-level buffers in the VIM file with the following names. Their order is not important and only the header is optional. Additional buffers are allowed for custom purposes, 
but will not be parsed by most readers. 

* `header`
* `assets`
* `entities`
* `strings`
* `geometry`

## Header Buffer

The header section contains the VIM file version and additional meta data as a sequence of newline (`\n`) terminated sets of key/value pairs denoted by `<key>=<value>`.

The following is an example:

```
vim=1.0.0
id=03280421-595d-4a35-802e-83bb6739e7ae
revision=f7eaf6b2-55b2-4f55-87f4-cdc9a01aa406
generator=Vim.Revit.Exporter:v1.46:Revit2020
created=2020-05-20T16:31:19Z
```

The field names are case insensitive. The only required field is `vim` which must have the value in the format `<major>.<minor>.<patch>` representing the file format version. 
The `id` field should be a uniquely generated GUID created the first time a VIM is created, but is not changed when the file is modified or saved.  
The `revision` field if present should be a new GUID created anytime a VIM is modified or updated.
The `created` field, should contain a date in ISO_8601 format. 
The `generator` field contains the name of the program used to generate or edit the VIM. 

## Assets Buffer

The assets section of a BIM is also a BFAST container. It may contain any number of buffers with any names. Buffers prefixed with the name `textures\` are assumed to be texture files.   

## Geometry Buffer

The geometry section of a VIM contains the merged geometry and basic scene graph information for an entire VIM document using the [G3D format](https://github.com/vimaec/g3d).

### About G3D 

The [G3D format](https://github.com/vimaec/g3d) is a binary format for 3D geometry that encodes data in an array of attribute buffers. 
G3D is based on the BFAST binary layout, and uses a naming convention to identify the layout of each attribute buffer and how it is used. 

Each attribute buffer is associated with a component of a geometry:
* vertex
* face
* corner
* edge
* subgeometry 
* instance
* all
* none

G3D attributes have names to identify them (e.g. position or uv) and uses indices to distinguish when multiple attributes share the same name (e.g. uv:0 ... uv:8). 
They can one of the following core data datatypes: float32, float64, int8, int16, int32, int64.

More information on G3D is available on its Github repo at [https://github.com/vimaec/g3d](https://github.com/vimaec/g3d).

### VIM Geometry Attributes
The geometry in a VIM must contain the following attributes:

* `g3d:vertex:position:0:float32:3` - Position data arranged as 3 single precision floating point values per vertex
* `g3d:corner:index:0:int32:1` - The index buffer, which is a list of 32-bit integers 
* `g3d:subgeometry:indexoffset:0:int32:1` - The offset of the index buffer for a group. 
* `g3d:subgeometry:vertexoffset:0:int32:1` - The offset into the vertex buffer for a group
* `g3d:instance:subgeometry:0:int32:1`- The index of the subgeometry associated with a particular instance
* `g3d:instance:transform:0:float64:16`- The transform of a node encoded as a 4x4 matrix in row major order
* `g3d:face:material:0:int32:1` - The index of the material associated with the face. 
* `g3d:face:group:0:int32:1` - The index of the group associated with a face. 
* `g3d:vertex:uv:0:float32:2` - The UV buffer, which is a list of 2 single-precision floating point values per vertex 
* `g3d:instance:parent:0:uint32:1`- The index of a parent node 

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

The strings buffer contains a sequence of ordered strings of zero or more length, with no duplicates, delimited by the "NUL" character. The zero-based index of each string (typically the first string is the empty string) is used by keys and values in the key/value collections associated with entities, and the string columns of entity tables. 

# VIM Object Model

W.I.P.
