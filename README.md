# Plinko
**Simple plinko simulation - c# console application**

Asks you for: 
- board width (recommended 50);
- board length (recommended 50);
- ball speed, determines how often ball positions update in seconds (recommended 0,1-0,3);
- ball amount;
- ball distance, determines how far away are balls spaced from each other (recommended 0-3);
- set hole point amounts, allows to set how much points will each place on the bottom add to the final score;
- set board appearance, allows to set how the plinko bars and balls are displayed (character), by default it's . for bars and * for balls

After inputting all the values a simulation of plinko will play, a ball will appear at the top, after the specified ball speed time it will move one space
down and randomly chosen to the left or to the right. When a ball reaches the bottom it falls into a "hole" each hole has a score value set by the user
or randomly generated from 0 to 100. If there are multiple balls, the first ball will move amount of time specified in ball distance before another ball 
will appear.
