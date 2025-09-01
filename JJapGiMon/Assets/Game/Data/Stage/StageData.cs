using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class StageData
{

    // Stage Config
    public StageConfig stageConfig;

    // Stage Graph
    public StageGraph stageGraph;

    // Stage State
    public StageState stageState;
 

    public StageData()
    {
        stageConfig = new StageConfig();
        stageGraph = new StageGraph();
        stageState = new StageState();
    }

}