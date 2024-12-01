profiles are designed with 2 main dictionaries:
- a `Bindings` dictionary (`Dictionary<string, InputEvent>`)
- a `UserData` dictionary (`Dictionary<string, Variant>`)

---

`InputEvent`s get internally serialized to string codes for the purposes of saving to disk

- `evt is InputEventKey` -> `"Key__{ (int)evt.PhysicalKeycode }"`
- `evt is InputEventJoypadButton` -> `"Button__{ (int)evt.ButtonIndex }"`
- `evt is InputEventJoypadMotion` -> `"Axis__{ (int)evt.Axis }__{ evt.AxisValue }"`

---

using this plugin ___requires___ an ini-style text file called `default_profile.prfl` in your godot project's root folder

(though i suppose you could change the `DEFAULT_PROFILE_PATH` in PPManagerPlugin.cs if you'd rather something else)

this default profile requires:

- a `[bindings]` section, for all actions and input events
  - these should be formatted as so: `ACTION_NAME=PackedStringArray("event code"...)`
- a `[user_data]` section
  - `key=value`, nothing special, so long as godot's `ConfigFile.GetValue(...)` can resolve your value to a `Variant`
- a `[misc]` section
  - `name=""`, so that the PlayerProfileManager autoload can cache your profile by name upon entering SceneTree

a rough example is linked in [default_profile.prfl](https://github.com/JAMSP00N/profile-manager/blob/master/example/default_profile.prfl)

---

default behavior is to load all profiles in `PLAYER_PROFILE_DIR` (`user://player_profiles/` by default) & then see if there are stock profiles for players 1-4 (stored as `P1.prfl`...)

(if the folder does not exist, the plugin will make it; if the profile(s) do not exist, the plugin will initialize them from the profile at `default_profile.prfl`)

---

if, throughout development, you need to add additional actions to `[bindings]` or tracked variables to `[user_data]`, simply add them to `default_profile.prfl`

upon the project's next run and the PlayerProfileManager autoload's next load to cache, any updates to the default profile will be appended to your already existing profiles
