# HoloToolkit-Extensions

At time of creation, Hololens V1 inluded a simple anchor placement mechanism which left a sort of "bookmark" in 3d space based on the SLAM mapping.  The anchors only stored a 3d position, so AnchorManager allows a way to encode Position, Rotation, and Scale by placing 3 anchors at relative distances to eachother to encode 3 3d Vectors needed.

I generified these scripts because I believe they may be of use to someone else or even to myself in the future.
