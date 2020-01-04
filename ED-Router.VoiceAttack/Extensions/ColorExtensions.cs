using ED_Router.Services;

namespace ED_Router.VoiceAttack.Extensions
{
    public static class ColorExtensions
    {
        public static string ToLogColor(this MessageColor color)
        {
            return color.ToString().ToLower();
        }
    }
}
