**Radar Arrows**

In order to use Radar Arrows with the new UI system, place an instance of the
RadarArrows prefab as a child of your scene's canvas. Then add a reference to the
main camera in the Inspector of RadarArrows. You will probably want to
adjust at least the tags list on the RadarArrows script.

In order to use Legacy Radar Arrows, attach the RadarArrows script to the camera. 
Then attach a texture to use as your arrow.

------------------------------------------------------------

This is what the variables mean:
	
	
Cam
	Since the placement of the arrows is determined by the camera transform, you
	will need to attach a reference to the main camera.
	
Arrows Sprites
	Sprite textures to be spawned pointing to objects with certain tags. The sprite
	that appears depends on the tag of the object. Each sprite is matched to the tag
	with the same index in the array mentioned below.

Tags To Find
	Arrows will appear for each object with this tag. For example, if the Tag to Find
	is "Enemy", all objects with the "Enemy" tag will have arrows. The arrows sprite
	is the one with the matching index in the "Arrow Sprites" array.
	
Size
	Width (and height) of the arrow texture. The width and height should be the same,
	so enter just one length. The width/length of the included arrow is 30.
	
Hover On Screen
	If true, an arrow will appear above the object when it in on-screen. If false, arrows
	will only appear around the edge of the screen, pointing to objects off-screen.
	
Distance Above
	If "Hover On Screen" is checked, the arrows will hover this distance above the object.
	
Blind Spot
	Arrows usually jump around rapidly when the object is directly behind the camera. (A fraction
	of a degree could move it from the left side to the right.) It is usually better to leave a
	blind spot, where arrows vanish completely. The blind spot variable is a percentage of the
	camera dimensions. For example, if Blind Spot is 0.33, the arrow will disappear if it would
	be in the center third of the camera if the camera was rotated around 180 degrees.
	
Hover Angle
	The angle at which all arrows for on-screen objects should point. A 270 degree angle will
	result in an arrow appearing above, pointing down.
	
Project From Hover Point
	If true, arrows on the edge of the screen will point to where the tip of the arrow would
	have been if the object was on-screen. If false, it will point to the center of the object.

-----------------------------------------------------------------

Release notes for version 2.0:

The script has been almost entirely rewritten, although it should still function in much the same way.
The main difference is the 2.0 version uses Unity's new UI system. The RadarArrows prefab is designed
be placed as a child of a canvas object. The Canvas must be set to "Screen space - Overlay" to work
properly.

The Old Radar Arrows script is still included if it is still preferred for whatever reason. There is no
guarantee that it will be supported in future updates.

Also, there is now support for multiple arrows types. There should be one arrow texture for every tag.
Each arrow sprite will be matched up with the tag at the matching index of their respective arrays.

JavaScript support has been discontinued. All the code is now written in C#.