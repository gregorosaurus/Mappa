# Mappa

## Summary

Mappa is a command line tool used to generate map tiles of a given image so long as the image coordinates are known (West, North, East, South). 

> **Note:** Mappa uses the Microsoft / Bing Maps Y tile index, where (0,0) indicates the upper most left tile. 

## Build

```dotnet build src/Mappa.sln```

## Usage

```
-w, --west      Required. West longitude in decimal degrees

-n, --north     Required. North latitude in decimal degrees

-s, --south     Required. South latitude in decimal degrees

-e, --east      Required. East longitude in decimal degrees

-i, --image     Required. The path to the image to tile

-z, --zoom      (Default: 14) The zoom level to tile to (inclusive)

-o, --output    Required. The output directory in which to save the files
```

### Example

```dotnet Mappa.dll  -w -111.956177 -n 57.389756 -s 57.293046 -e -111.690445 -i /path/to/image.png  -z 16 -o /path/to/output/directory```

