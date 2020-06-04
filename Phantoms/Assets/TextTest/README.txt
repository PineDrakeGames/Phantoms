Hi! This is my final project for Game Architecture, "Fancy Text"

It works by creating a text object in unity, then drag the script 'Fancytext.cs' on to it.
Then, drag a .txt file onto that and you are good to go!
Some settings can be changed in the editor, but it is recommended to do so in the text.
all commands are placed inside [] brackets, with a few other things in addition,
such as setting values or adding words to have an effect on.
Unity's rich text is also supported, as well as different fonts (theoretically).


Effects:
effects on individual letters or groups of letters
formatted as [effecttype=words to put the effect on>>StrengthOfEffect] (strength is optional)
current list:
Wavy	- makes letters move up and down like a wave
Jitter	- makes letters shake randomly
Pulse	- makes letters shrink and grow
Swivel	- makes letters swivel around center axis
Rainbow	- makes letters change colors, like a rainbow


Creators:
sets the animation to be played when letters appear
formatted as [creatortype=SpeedOfCreation]
current list:
Instant	- letters appear instantly
Pop		- letters grow to the correct size
Fadein	- letters fade in to the correct colors
Flip 	- letters flip into existance from the center

Speeds:
set the pacing of text appearance
formatted as [speedtype=value]
current list:
Speed	- sets the speed of text appearing (can be 0)
Pause	- waits for the given amount of time before continuing
Nlpause	- sets the time to pause for each new line (can be 0)

KNOWN BUGS:
-The text box must be big enough to fit all the text, otherwise it breaks.
this is because its trying to get a number of verts equal to the number of characters * 4,
but if the box is too small not all of the letters are there
-Thats all 


I hope you like it!
-CJ Legato