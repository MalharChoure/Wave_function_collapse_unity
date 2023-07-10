// How to add detailed blocks without compromising performance

//Possible solutions:
// Lods

// problems with Lod's are: We are disabling the child meshes in chunks because the combined chunk is a single 
// parent object. If we start rendering individually Lod's are possible but the performance will take a massive hit because
// Each block will be rendered.


// Minecraft rendering system: 

//Problems with minecraft rendering system: UNity does not allow any changes to core scripts.
