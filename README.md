# LightShow_Unity
Prototype of a 2-D music-driven bullet hell game with customizability made using Unity and Music Information Retrieval (MIR). This prototype is my first game made in Unity, and a continuation of my Linux SFML project of the same name. 
Demo Youtube link: https://youtu.be/T89ApJfXRbg

The concept of the game is as follows:
- Given a song file and the appropriate audio processing tools, we could automatically extract features from the song. These features could be the beat, the frequencies of different playing melodies, the chord, etc.
- Given a time-array of these musical features, and a design file that maps in-game events to these musical features, we can create a game where events and challenges are synced to the events of the music, resulting in a visually pleasing experience.

My prototype currently only implements the beat and the main melody frequencies to generate in-game obstacles. For future work, other musical features would include the chords, motifs, complimentary melodies, etc. Main melody frequency recognition is done using the [Melodia](https://www.upf.edu/web/mtg/melodia) VAMP Plugin, and the beat is specified by user input. Future goals are to implement more robust MIR algorhithms using the [Essentia](https://essentia.upf.edu/index.html) framework.


