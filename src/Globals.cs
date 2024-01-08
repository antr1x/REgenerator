namespace REgenerator
{
    /// <summary>
    /// Contains global configurations and mappings used across the REgenerator.
    /// </summary>
    class Globals
    {
        /// <summary>
        /// A dictionary mapping function names to their respective IDs.
        /// This mapping is used to determine if a function requires the fix vectors operation
        /// in the Lua API invoker. Both the function name and ID are used for a precise match.
        /// </summary>
        public static readonly Dictionary<string, int> FixVectorsFunctions = new Dictionary<string, int>
        {
            { "GET_ENTITY_MATRIX", 761 },
            { "GET_CLOSEST_FIRE_POS", 985 },
            { "GET_INTERIOR_LOCATION_AND_NAMEHASH", 1909 },
            { "GET_GROUND_Z_AND_NORMAL_FOR_3D_COORD", 2051 },
            { "GET_SAVE_HOUSE_DETAILS_AFTER_SUCCESSFUL_LOAD", 2105 },
            { "GET_MODEL_DIMENSIONS", 2128 },
            { "GET_COORDS_OF_PROJECTILE_TYPE_IN_AREA", 2148 },
            { "GET_COORDS_OF_PROJECTILE_TYPE_IN_ANGLED_AREA", 2149 },
            { "GET_COORDS_OF_PROJECTILE_TYPE_WITHIN_DISTANCE", 2150 },
            { "GET_PROJECTILE_OF_PROJECTILE_TYPE_WITHIN_DISTANCE", 2151 },
            { "FIND_SPAWN_POINT_IN_DIRECTION", 2225 },
            { "GET_MOBILE_PHONE_ROTATION", 2310 },
            { "GET_MOBILE_PHONE_POSITION", 2312 },
            { "NETWORK_GET_RESPAWN_RESULT", 3328 },
            { "GET_COORDS_AND_ROTATION_OF_CLOSEST_OBJECT_OF_TYPE", 3585 },
            { "GET_SAFE_COORD_FOR_PED", 3784 },
            { "GET_CLOSEST_VEHICLE_NODE", 3785 },
            { "GET_CLOSEST_MAJOR_VEHICLE_NODE", 3786 },
            { "GET_CLOSEST_VEHICLE_NODE_WITH_HEADING", 3787 },
            { "GET_NTH_CLOSEST_VEHICLE_NODE", 3788 },
            { "GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING", 3790 },
            { "GET_NTH_CLOSEST_VEHICLE_NODE_ID_WITH_HEADING", 3791 },
            { "GET_NTH_CLOSEST_VEHICLE_NODE_FAVOUR_DIRECTION", 3792 },
            { "GET_VEHICLE_NODE_POSITION", 3795 },
            { "GET_CLOSEST_ROAD", 3798 },
            { "GET_RANDOM_VEHICLE_NODE", 3809 },
            { "GET_SPAWN_COORDS_FOR_VEHICLE_NODE", 3810 },
            { "GET_POS_ALONG_GPS_TYPE_ROUTE", 3817 },
            { "GET_ROAD_BOUNDARY_USING_HEADING", 3819 },
            { "GET_POSITION_BY_SIDE_OF_ROAD", 3820 },
            { "SET_PLAYER_WANTED_CENTRE_POSITION", 4513 },
            { "START_SHAPE_TEST_MOUSE_CURSOR_LOS_PROBE", 4828 },
            { "GET_SHAPE_TEST_RESULT", 4829 },
            { "GET_SHAPE_TEST_RESULT_INCLUDING_MATERIAL", 4830 },
            { "WAYPOINT_RECORDING_GET_COORD", 5591 },
            { "GENERATE_VEHICLE_CREATION_POS_FROM_PATHS", 6053 },
            { "GET_VEHICLE_SIZE", 6283 },
            { "TEST_PROBE_AGAINST_WATER", 6432 },
            { "GET_PED_LAST_WEAPON_IMPACT_COORD", 6492 }
        };
    }

}
