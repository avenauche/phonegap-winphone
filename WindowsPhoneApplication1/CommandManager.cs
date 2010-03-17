using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneGap
{
    class CommandManager
    {
        private Command[] commands = new Command[1];

        public CommandManager()
        {
            commands[0] = new NotificationCommand();
        }
        public string processInstruction(string instruction, string[] parameters)
        {
            for (int index = 0; index < commands.Length; index++)
            {
                Command command = (Command)commands[index];
                if (command.accept(instruction))
                {
                    try
                    {
                        return command.execute(instruction, parameters);
                    }
                    catch
                    {

                    }
                }
            }
            return null;
        }
    }
}