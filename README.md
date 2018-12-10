# Unity-Crowd-Visualizer
## Visualizer for crowd simulators

This visualizer takes a CSV file of agent trajectories as input and visualizes them

## Additional Assets for Recording 

Unity Recorder

## CSV file header

`TIME ,ID ,POS_X ,POS_Y, TAR_X, TAR_Y, AGENT_RADIUS, COLOR_R, COLOR_G, COLOR_B`

- Timestep (Positive Integer)
- Agent_Id (Positive Integer)
- X Position of Agent 
- Y Position of Agent
- X Position of Target (usually fixed)
- Y Position of Target (usually fixed)
- Radius of Agent (constant)
- Red component of Agent Color (Range [0, 1])
- Green component of Agent Color (Range [0, 1])
- Blue component of Agent Color (Range [0, 1])

## CSV file header for Walls

Walls are considered to be line segments. The header is structured like this :

`ID, P1_X, P1_Y, P2_X, P2_Y`

- ID : Id of wall
- P1 : Endpoint 1 of wall
- P2 : Endpoint 2 of wall

## Parameters in the Crowd Script

- Show Info : Shows Timestep info (Requires a text field to be set for `Info`).
- Animate : Animate the trajectories if checked.
- Show Targets : Show the target positions if checked.
- Write Statistics : Calculate some statistical properties about the trajectories and write them to a file if checked.
- Capture Screenshots : Capture a screenshot every timestep and save them to a folder named `Screenshots` if checked.
- Animate Interval : Seconds per frame.
- Colors : This parameter takes a value in the range [1, 3], If a single cluster has to be visualized, choose 1, if two distinct clusters need to be visualized, choose 2. If you want to assign a separate color to every agent, choose 3.
- Trail Length : Length of the trail to be left behind by every agent.

## Notes

- You can find some sample csv files for reference here : `Assets/Resources/csv/`

