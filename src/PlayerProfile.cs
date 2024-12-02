using System.Collections.Generic;
using Godot;
using Logger;
using static PlayerProfiles.Constants;

namespace PlayerProfiles
{

  public partial class PlayerProfile
  {
	public string Name { get; internal set; }
	public string Path { get; internal set; }

	public Dictionary<string, List<InputEvent>> Bindings { get; internal set; } = new ();
	public Dictionary<string, Variant> UserData { get; internal set; } = new ();

	public bool IsInputEventBound (string evt)
	{
	  foreach (var key in Bindings.Keys)
		if (Bindings[key].Contains(evt.ToInputEvent()))
		  return true;
	  
	  return false;
	}
	public bool IsInputEventBound (InputEvent evt)
	{
	  foreach (var key in Bindings.Keys)
		if (Bindings[key].Contains(evt))
		  return true;

	  return false;
	}

	public StringName GetBindingWithInputEvent (string evt)
	{
	  foreach (var key in Bindings.Keys)
		if (Bindings[key].Contains(evt.ToInputEvent()))
		  return key;
	  
	  return "";
	}
	public StringName GetBindingWithInputEvent (InputEvent evt)
	{
	  foreach (var key in Bindings.Keys)
		if (Bindings[key].Contains(evt))
		  return key;

	  return "";
	}

	public static Error Save (PlayerProfile profile, string path)
	{
	  Log.Trace("PP - Save", path);
	  if (profile == null)
			return Error.InvalidParameter;

	  ConfigFile file = new ();

	  foreach (var key in profile.Bindings.Keys)
	  {
			List<InputEvent> binding = profile.Bindings[key];
			string [] events = new string [binding.Count];
			for (int idx = 0; idx < binding.Count; idx++)
				events[idx] = binding[idx].AsStringCode();

			Log.Debug("PP - Save", $"bindings > {key} > {events}");
			file.SetValue("bindings", key, events);
	  }

	  foreach (var key in profile.UserData.Keys)
	  {
			Variant value = profile.UserData[key];
			Log.Debug("PP - Save", $"user_data > {key} > {value}");
			file.SetValue("user_data", key, value);
	  }

	  Log.Debug("PP - Save", $"misc > name > {profile.Name}");
	  file.SetValue("misc", "name", profile.Name);

	  return file.Save(path);
	}
	public Error Save (string path) => Save(this, path);
	public Error Save () => Save(this, this.Path);
	
	public static Error Load (string path, out PlayerProfile profile, PlayerProfile defaultProfile = null)
	{
	  profile = null;

	  Log.Trace("PP - Load", path);

	  ConfigFile file = new ();
	  Error err = file.Load(path);
	  if (err != Error.Ok)
			return err;
	  if (!file.HasSection("bindings") || !file.HasSection("user_data") || !file.HasSection("misc"))
			return Error.Failed;

	  profile = new ()
	  {
			Bindings = new (),
			UserData = new (),
			Name = "",
			Path = path,
	  };

    foreach (var key in file.GetSectionKeys("bindings"))
    {
      List<InputEvent> events = new ();
      string [] codes = (string [])file.GetValue("bindings", key, System.Array.Empty<string>());
      foreach (var code in codes)
        events.Add(code.ToInputEvent());

      profile.Bindings.Add(key, events);
    }
    if (defaultProfile != null)
      foreach (var defbind in defaultProfile.Bindings.Keys)
      {
        if (profile.Bindings.ContainsKey(defbind))
          continue;

        profile.Bindings.Add(defbind, defaultProfile.Bindings[defbind]);
      }

	  foreach (var key in file.GetSectionKeys("user_data"))
		  profile.UserData.Add(key, file.GetValue("user_data", key));
    if (defaultProfile != null)
      foreach (var defdata in defaultProfile.UserData.Keys)
      {
        if (profile.UserData.ContainsKey(defdata))
          continue;

        profile.UserData.Add(defdata, defaultProfile.UserData[defdata]);
      }
	  
    string trimmed = path.Replace(PLAYER_PROFILE_DIR, "").Replace(PLAYER_PROFILE_EXT, "");
	  profile.Name = file.HasSectionKey("misc", "name") ? (string)file.GetValue("misc", "name") : trimmed;

    if (defaultProfile != null)
      profile.Save(path);
      
	  return Error.Ok;
	}
	public bool Load (string path, PlayerProfile defaultProfile = null)
	{
	  if (Load(path, out PlayerProfile profile, defaultProfile) == Error.Ok)
	  {
      Bindings = profile.Bindings;
      UserData = profile.UserData;
      Name = profile.Name;
      return true;
	  }

	  return false;
	}
  
	public static Error BuildFromDefault (out PlayerProfile profile, string name)
	{
	  Error err = Load(DEFAULT_PROFILE_PATH, out profile);
	  if (err != Error.Ok)
	  {
      profile = null;
      return err;
	  }

	  profile.Name = name;
	  return Error.Ok;
	}
	public static PlayerProfile Default (string name) =>
	  (BuildFromDefault(out PlayerProfile profile, name) == Error.Ok) ? profile : null;
  }

}
