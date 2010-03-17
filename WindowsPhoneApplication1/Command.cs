using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneGap
{
    public interface Command
    {
        string execute(string instruction, string[] parameters);
        bool accept(string instruction);
    }
}