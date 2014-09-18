#Generates random squiggles in Blender
#FIGURE OUT HOW TO EXPORT .obj FILES
#WORK INTO FOR LOOP

#Imports necessary packages
import bpy, math, random

#Defines how many times to subdivide the original cube at the start and finish
#Fewer initial subdivisions makes blobbier blobs. High subdivisions makes spiker blobs
subdivInit = 2
#Higher post subdivisions makes for fewer jaggies. It doesn't seem to make much difference past 3 or 4
subdivPost = 4

#Defines the power of the distortion. Lower values make it blobbier. Higher values pull out the distortions further
powerMin = 2.0
powerMax = 4.0

#Defines minimim and maximum noise values
noiseScaleMin = 0
noiseScaleMax = 2

#Defines the depth of the cloud noise texture Lowering below 1 seems to make fewer, more dramatic distortions
noiseDepth = 1

#################################

#Creates a cube mesh
bpy.ops.mesh.primitive_cube_add()

#Initializes a gameobject called "cube" that uses that mesh
cube = bpy.context.active_object

#Gives the cube object modifiers, which will be used to distort it
cube.modifiers.new('Subsurf1', 'SUBSURF')
cube.modifiers.new('Disp', 'DISPLACE')
cube.modifiers.new('Subsurf2', 'SUBSURF')

#Subdivides the cube according to the value defined earlier
cube.modifiers[0].levels = subdivInit

#Creates a new texture
bpy.ops.texture.new()
#Initializes a variable called "tex" and assigns it that texture
tex = bpy.data.textures[-1]
#Defines the name of the texture
tex.name = "CloudNoise"

#Defines the scale of the noise in the texture as a random value between min and max values
tex.noise_scale = random.random() * (noiseScaleMax-noiseScaleMin) + noiseScaleMin

#Defines the noise depth as the value set earlier
tex.noise_depth = noiseDepth
#Assigns the CloudNoise texture to the cube gameobject
cube.modifiers[1].texture = tex

#Defines the strength of the distortion as a random value between min and max values
cube.modifiers[1].strength = random.random() * (powerMax-powerMin) + powerMin

#Subdivides the mesh again, to smooth it out
cube.modifiers[2].levels = subdivPost
#Smooths the object
bpy.ops.object.shade_smooth()

#filepath = '~Downloads/Blobby'

#bpy.ops.export_scene.obj(filepath)

#filepath = "C:\\scl\\" + cube.name + "_Aldis_tits"
#print (filepath)
