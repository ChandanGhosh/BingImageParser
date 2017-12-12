using System.Collections.Generic;

namespace BingImageParser.Core
{
    public class RootObject
    {
        public List<Image> images;

        public Tooltips tooltips;
    }

    public enum Style
    {
        Fill,
        Fit,
        Span,
        Stretch,
        Tile,
        Center
    }
}