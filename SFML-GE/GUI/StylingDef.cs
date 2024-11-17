using SFML.Graphics;
using SFML_GE.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.GUI
{
    /// <summary>
    /// Defines how <see cref="GUIPanel"/> objects are styled when first created.
    /// </summary>
    public class StylingDef
    {
        /// <summary>
        /// The default foreground color.
        /// </summary>
        public Color defaultForeground = new Color(0xE0EBF1FF);

        /// <summary>
        /// The default background color.
        /// </summary>
        public Color defaultBackground = new Color(0x474859FF) - new Color(0, 0, 0, 125);

        /// <summary>
        /// the default secondary color, usually used as an alt for hovering/outlines
        /// </summary>
        public Color defaultSecondary = new Color(0x61637BFF);

        /// <summary>
        /// the default primary color, usually used for outlines.
        /// </summary>
        public Color defaultPrimary = new Color(0x767997FF);

        /// <summary>
        /// the default color for pressed buttons and such.
        /// </summary>
        public Color defaultPressed = new Color(0x2C2D36FF);

        /// <summary>
        /// the name of the <see cref="FontResource"/> that guis will look for if one is not provided.
        /// </summary>
        public string defaultFontName = "Roboto-Regular";

        /// <summary>
        /// Creates a new default styling def, with just the font name given.
        /// </summary>
        /// <param name="fontName"></param>
        public StylingDef(string fontName)
        {
            this.defaultFontName = fontName;
        }

        /// <summary>
        /// Creates a new default styling def.
        /// </summary>
        public StylingDef() { return; }
    }
}
