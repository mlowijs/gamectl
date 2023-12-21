namespace Gamerun.Kde;

public record KscreenDoctorOutput(
    bool Connected,
    bool Enabled,
    KscreenDoctorMode[] Modes,
    int Priority,
    string Name);