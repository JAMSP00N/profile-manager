using Godot;

namespace PlayerProfiles
{

  public static class InputEventExt
  {
    public static string AsStringCode (this InputEvent evt) => evt switch
    {
      InputEventKey e => $"Key__{(int)e.PhysicalKeycode}",
      InputEventJoypadButton e => $"Button__{(int)e.ButtonIndex}",
      InputEventJoypadMotion e => $"Axis__{(int)e.Axis}__{e.AxisValue}",
      _ => "",
    };

    public static InputEvent ToInputEvent (this string code)
    {
      string [] parts = code.Split("__");
      return parts[0] switch
      {
        "Key" => new InputEventKey () { PhysicalKeycode = (Key)parts[1].ToInt() },
        "Button" => new InputEventJoypadButton () { ButtonIndex = (JoyButton)parts[1].ToInt() },
        "Axis" => new InputEventJoypadMotion () { Axis = (JoyAxis)parts[1].ToInt(), AxisValue = parts[2].ToFloat() },
        _ => null,
      };
    }
  }

}