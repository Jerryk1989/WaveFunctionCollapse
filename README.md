<h1>Wave Function Collapse</h1>

This will cover how I implemented the wave function collapse algorithm in this project.  I did not just implement wave function collapse.  I also implemented a way to press a few buttons in the unity editor and get all the 90 degree rotations for a prefab.  As well as that, I created a button that will go through all these prefab rotations and populate their neighbors.  What this means is that given your current prefab, for every 90 degree rotation it has, my script will get the valid tiles for each side of on every rotation and create those scriptable objects so you don't need to manually do it in the editor.

The solution isn't super solid, but it works pretty decently.  You just need to know quite a few things up front.

<br />

<h1>Which order should I run the scripts in</h1>
1.  Run the CreateRotatePrefabs script first.   This will create the rotation prefabs.
1.  Once you have the rotation prefabs created.  Run the CreateRotatedScriptableObjects to create the scriptable objects that have all their rotations

<h1>Create rotation prefabs button</h1>
This script is pretty simple, it only works for 3d projects.  All it does is go through a list of prefabs that you provide and create 4 rotations of that prefab and save them as new prefabs.  One at 0, 90, 180, and 270 degrees.

<h3>How to use the script</h3>

1.  Create an empty object in your scene and call it Wave Function Collapse or something like that.
1.  Select that empty object and click add component, select the CreateRotatedPrefabs script.
1.  You need to drag the prefabs you want to use into the "Prefabs To Rotate" list on this script.
1.  Once you have the list populated, click the "Create rotation prefabs" button.  (Note that the filepath is hard coded right now since I'm just making this for myself.  You may need to update it in the code or it will create it)
1.  That is all, you should have a folder filled with prefabs now.





There are two buttons I have made for the unity editor.

* "Create rotation prefabs" which comes from the CreateRotatedPrefabs class.
* "Create rotation data for each rotated prefab" which comes from the CreateRotatedScriptableObjects class.


  
