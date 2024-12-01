#if TOOLS

using Godot;
using Logger;

namespace PlayerProfiles
{

  public static class Constants
  {
    public readonly static string PLAYER_PROFILE_DIR = "user://player_profiles/";
    public readonly static string PLAYER_PROFILE_EXT = ".prfl";
    public readonly static string DEFAULT_PROFILE_PATH = "res://default_profile.prfl";
  }

  [Tool]
  public partial class PPManagerPlugin : EditorPlugin
  {

    public override void _EnterTree ()
    {
      AddAutoloadSingleton("PlayerProfileManager", "res://addons/profile-manager/src/PlayerProfileManager.cs");
    }

    public override void _ExitTree ()
    {
      RemoveAutoloadSingleton("PlayerProfileManager");
    }
  }

}

#endif