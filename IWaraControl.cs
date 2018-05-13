namespace WaraUi
{
    interface IWaraControl
    {
        int Depth { get; set; }
        WaraSkinManager SkinManager { get; }
        MouseState MouseState { get; set; }

    }

    public enum MouseState
    {
        HOVER,
        DOWN,
        OUT
    }
}
