﻿namespace Mir_Utilities;

public class ClearFootprint
{
    //Todo: change this method to new robot object
    public static void ClearFootprintFromRobot(MirRobotApi.MiRRobot robot)
    {
        ApiCaller apiCaller = new ApiCaller(robot.Ip, robot.AuthId);
        
        //Get current user id to use in the request
        
        String id = apiCaller.GetApi("users/me").Result.guid;
        
        //Get all footprints for the user
        dynamic footprints = apiCaller.GetApi("footprints").Result;
        foreach (dynamic footprint in footprints )
        {
            string footprintCreatorId = apiCaller.GetApi("footprints/"+footprint.guid).Result.created_by_id;
            if (footprintCreatorId == id)
            {
                apiCaller.DeleteApi("footprints/"+footprint.guid);
            }
        }
    }
}