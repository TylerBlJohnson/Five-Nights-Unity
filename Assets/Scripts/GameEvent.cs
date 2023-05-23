public static class GameEvent
{
    public const string CAMERA_FLIP = "CAMERA_FLIP"; //To be sent when the security monitor is being flipped up.
    public const string SWITCH_TO_OFFICE = "SWITCH_TO_OFFICE"; //To be sent when the game should swap to the office.
    public const string SWITCH_TO_MONITOR = "SWITCH_TO_MONITOR"; //To be sent when the game should swap to the security monitor, which happens after the monitor is flipped up.
    public const string MONITOR_CAN_FLIP = "MONITOR_CAN_FLIP"; //Allows the monitor to flip up.
    public const string CHANGE_CAMERA = "CHANGE_CAMERA"; //To be sent when the player clicks a monitor to view.
    public const string BONNIE_MOVED = "BONNIE_MOVED"; //To be sent when Bonnie moves to a new location.
    public const string CHICA_MOVED = "CHICA_MOVED"; //To be sent when Chica moves to a new location.
    public const string BONNIE_NEAR_CAMERA = "BONNIE_NEAR_CAMERA" ; //To be sent when Bonnie moves to a new location.
    public const string CHICA_NEAR_CAMERA = "CHICA_NEAR_CAMERA"; //To be sent when Chica moves to a new location.
    public const string FREDDY_MOVED = "FREDDY_MOVED"; //To be sent when Freddy moves to a new location.
    public const string FOXY_MOVED = "FOXY_MOVED"; //To be sent when Foxy moves to a new location.
    public const string DISABLE_CAMERAS = "DISABLE_CAMERAS"; //To be sent when the player is actively viewing a camera that either Bonnie or Chica are moving to or from
    public const string JUMPSCARE = "JUMPSCARE"; //To be sent when the player is getting jumpscared.
    public const string GAME_OVER = "GAME_OVER"; //To be sent once a jumpscare has finished playing.
    public const string TIME_CHANGE = "TIME_CHANGE"; //To be sent when an hour passes during the night.
    public const string GOLDEN_FREDDY_UPDATE = "GOLDEN_FREDDY_UPDATE"; //To be used when Golden Freddy's status is updated.
    public const string START_NIGHT = "START_NIGHT"; //Used in the main menu to tell the game to start the start night sequence
    public const string FORCE_CAMERA_LEFT = "FORCE_CAMERA_LEFT"; //Used when Foxy jumpscares the player to force the camera towards him
}