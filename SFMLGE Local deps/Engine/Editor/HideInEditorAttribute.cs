﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.Editor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HideInEditorAttribute : Attribute { }
}