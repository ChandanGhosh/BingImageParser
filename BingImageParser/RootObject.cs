using System.Collections.Generic;

namespace BingImageParser
{
    public class RootObject
    {
        public List<Image> images { get; set; }

        public Tooltips tooltips { get; set; }
    }

    public enum Style
    {
        Fill,
        Fit,
        Span,
        Stretch,
        Tile,
        Center,
    }
}
