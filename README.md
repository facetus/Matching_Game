Independent repository for the Matching Minigame, originally created the AR application of Casa Bianca.
All the necessary prefabs, scripts and objects are included here, with an expcetion of a few UI elements that should not restrict the application's experience.

In order to modify the project follow these steps:

  1.The Matching Game Controller holds the required information about the game's mechanics. You can change from there how many pairs you want by modifying these values: "imagesPerText", "totalPairs". the first value will tell the script how many images to pair the first game object with and the second is the total number of pairs you want to have in your game.
  2. In the Unity Editor inside the canva element there is "ImageRow1" and "ImageRow2" which are my 2 rows containing 3 gameobjects each. You can freely add another row with 3 gameobjects if you desire to do so and copy paste the Image1 in order to have an exact replica to modify.
  3.The "Texts" gameobject holds the 2 gameobjects that have the one to many relation. In order to another one, you can simply duplicate either one and modify it.
