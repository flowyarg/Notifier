namespace Notifier.Matrix.Settings;

public record MatrixApiSettings(string AccessToken, string RefreshToken)
{
    public MatrixApiSettings() : this(default!, default!) { }
}