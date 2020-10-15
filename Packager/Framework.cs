namespace Packager
{
    public class Framework
    {
        public Framework(FrameworkType type, int major, int minor)
        {
            Type = type;
            Major = major;
            Minor = minor;
        }

        public FrameworkType Type { get; }
        public int Major { get; }
        public int Minor { get; }
    }
}
