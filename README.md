# Windswept Demo Tooling

**Address**: `Windswept.exe+A23440`<br>
**Description**: Seems to be an offset of some sort for the global timer. Setting it to 0 increments the time by an unknown amount

Detailed Notes:
- Replacing this with NOP seems to cause the timer to increase at a rapid rate. I think this is an offset to resync the timer rate.

**Address**: `Windswept.exe+A42170:4` <br>
**Description**: Seems to be linked to the stage timer in some way. Setting to 0 does nothing

Detailed Notes:
- Possible investigation path. Look into other memory areas that access this one.
- Viewing the assembly, seems that the increment value (1) is stored in R15. 
  - Replacing the `mov` instruction operand to 0 seems to cause the above address to  be static. Stage counter continues to increment
- Might be total time game is open?

