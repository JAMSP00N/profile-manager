using System.Collections.Generic;
using Godot;

using Logger;
using static PlayerProfiles.Constants;

namespace PlayerProfiles
{
  
  public partial class PlayerProfileManager : Node
  {
    public System.Collections.Generic.Dictionary<string, PlayerProfile> Profiles { get; } = new ();

    public override void _EnterTree ()
    {
      Log.Info("PPManager initialized");

      if (DirAccess.DirExistsAbsolute(PLAYER_PROFILE_DIR))
      {
        Log.Trace("PPManager", "Querying saved player profiles");
        string [] files = DirAccess.GetFilesAt(PLAYER_PROFILE_DIR);
        foreach (var file in files)
          LoadProfileToCache($"{PLAYER_PROFILE_DIR}{file}");

        if (!HasDefaultProfiles())
        {
          Log.Warn("PPManager", "Missing 1 or more default profiles");
          CreateDefaultProfiles();
        }
      }
      else
      {
        Log.Warn("PPManager", "No player_profiles/ directory found, creating one...");
        DirAccess.MakeDirRecursiveAbsolute(PLAYER_PROFILE_DIR);
        CreateDefaultProfiles();
      }
    }

    public PlayerProfile GetProfile (string name) => Profiles.ContainsKey(name) ? Profiles[name] : null;

    public void LoadProfileToCache (string absolutePath)
    {
      PlayerProfile profile = new ();
      profile.Load(absolutePath, PlayerProfile.Default("def"));

      if (profile == null)
      {
        Log.Error("PPManager", $"Unable to load profile {absolutePath}");
        return;
      }

      if (profile.Name != null && Profiles.ContainsKey(profile.Name))
      {
        Profiles.Remove(profile.Name);
        Log.Trace("PPManager", $"Removed cached profile {profile.Name}");
      }
      Profiles.Add(profile.Name, profile);
      Log.Info("PPManager", $"Loaded profile {profile.Name} to cached profiles");
    }

    bool HasDefaultProfiles ()
    {
      for (int idx = 1; idx <= 4; idx++)
        if (GetProfile($"P{idx}") == null)
          return false;

      return true;
    }
    void CreateDefaultProfiles ()
    {
      Log.Trace("PPManager", "Creating default player profile files...");

      for (int idx = 1; idx <= 4; idx++)
      {
        string path = $"{PLAYER_PROFILE_DIR}P{idx}{PLAYER_PROFILE_EXT}";

        if (FileAccess.FileExists(path) && Profiles.ContainsKey($"P{idx}"))
        {
          Log.Trace("PPManager", $"Default profile P{idx} already exists, continuing...");
          continue;
        }

        PlayerProfile profile = PlayerProfile.Default($"P{idx}");

        Error err = profile.Save(path);
        if (err == Error.Ok)
          Log.Info("PPManager", $"Saved default profile {profile.Name} to {path}");
        else
          Log.Error("PPManager", $"Could not save profile {profile.Name} to {path}, {err}");
      }
    }
  }

}