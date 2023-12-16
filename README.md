<h1>Wave Function Collapse</h1>

This will cover how I implemented the wave function collapse algorithm in this project.  I did not just implement wave function collapse.  I also implemented a way to press a few buttons in the unity editor and get all the 90 degree rotations for a prefab.  As well as that, I created a button that will go through all these prefab rotations and populate their neighbors.  What this means is that given your current prefab, for every 90 degree rotation it has, my script will get the valid tiles for each side of on every rotation and create those scriptable objects so you don't need to manually do it in the editor.

The solution isn't super solid, but it works pretty decently.  You just need to know quite a few things up front.

<br />

<h1>Prerequisites for running any scripts</h1>

1.  Every prefab you make needs to have the PrefabGroupingInfo component.  See the Grouping section on why this is important.

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


<h1>"Create rotation data for each rotated prefab button</h1>

This script is the complex one.  It has to do many things and at the time of this writing needs a lot of work still, but it does work.

First as a requriement.  You need to have all of your rotated prefabs created from the CreateRotatePrefabs script.

The script grabs all those prefabs and stores them into a list.


<h1>Grouping</h1>

I've implemented this concept of grouping into this project.  It's nothing revolutionary, it's just a way to put similar objects into a group.  For instance, roads that are not intersections are in the roads group.  Intersections are in the intersections group.

<h2>Why did I create the groupings</h2>

The problem that grouping is trying to solve is kind of long, but I'll try and explain it here.  While implementing Wave Function Collapse, I was running into the issue where whenever I created a new node to generate through the algorithm, I had to go back to my other nodes and update them to include it where it was valid to go.  For instance, a forward facing road would need to know that it now has another building it can have on it's back.  I also need to tell the building I'm creating that the road can go in it's front, and that a building can be on it's left, and that the building on the left knows this new building can be on the right.  And and... Hopefully you are seeing the issue.  Everytime I added anything, I spent 20 minutes going through and updating each node and worse yet, having to place the nodes into my world to make sure I had the right sides.  It was extremely tedious and slow and very prone to errors.

<br />

The problem I was trying to solve simply put is that I wanted a script to figure out, for each rotation of a node, which neighbor nodes were valid for it.  So given a building that just has 1 entrance, I wanted to be able to give this script all 4 rotations of that building and have it know that there will always be a road in front of it.  Since the buildings facing a different direction in each rotation, a different facing road would be needed at a different direction.  What I mean, is that forward is always forward.  So if you place a building facing forward, I place a road in front of it place forward.  Now if I rotate that building so it's facing right, i can't use the forward facing road and building, they would be facing the wrong way.  I instead need to use the right building and road.  I'd need to do this for all rotations of this building, for each neighbor it has in every rotation.  Every rotation has a forward, back, left, and right neighor.  Those neighbor's don't change.  For instance I only want that road in front of the building.  So for every rotation, I need my script to figure out which road goes there given the direction the building is facing.  Once it has done this, create the scriptable objects with all this information filled out so I don't have to do it.

<br />

While looking at this issue, I went through and created some rough idea's about what things would look like (see the notepad scribbles below).  This became useful later when I went to create the rules for this forward facing building.  I started writing the code and instantly realized calling out each prefab available was kind of clunky and not much better than doing it manually.  What happens when I want to add more road variations?  I'd have to update the script with every new addition, it would be really nice if I could just drop my new prefabs into a folder and let the script figure it out.  This line of thinking got me to realize that in front of the building I just want a road.  I dont' care if it's road 1, 2, or 3.  Just as long as it's a road.  So if I could somehow group all roads with a type of road, I could then just search for all roads facing a certain direction based off the way the bulding is facing and I'd have all the available roads neighbors.  This is how grouping came about in this project

> [!important]
> In order for the grouping to work, each prefab rotation needs to have the PrefabGroupingInfo component.  Just add it to the prefab that you run through the CreateRotatedPrefabs script and it will be there on the new ones

```
corner - forward

forward = [road - forward]
right = [road - right]
left = [bld - forward]
back = [bld - right]

corner - left

forward = [road - forward]
right = [bld - forward]
left = [road - left]
back = [bld - left]

corner - back

forward = [bld - left]
right = [bld - back]
left = [road - left]
back = [road - back]

corner - right

forward = [bld - right]
right = [road - right]
left = [bld - back]
back = [road - left]
```
