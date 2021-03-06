using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmpathicQbt.ConsoleServer
{
    public class ConsoleInput
    {

        private BlockingCollection<string> inputQueue = new BlockingCollection<string>();
        private Thread inputThread = null;
        bool isInputTerminated = false;

        // Saved state, used to restore after reloading the configuration file.
        public string currentFavoritesList = null;

        public void Start()
        {
            inputThread = new Thread(ReadLineFromConsole);
            inputThread.Start();
        }

        private void ReadLineFromConsole()
        {
            while (true)
            {
                string input = Console.ReadLine();

                // input will be null when Skyrim terminated (stdin closed)
                if (input == null)
                {
                    isInputTerminated = true;
                    Trace.TraceInformation("Skyrim is terminated, console server will quit.");

                    // Notify the SkyrimInterop thread to exit
                    inputQueue.Add(null);

                    break;
                }

                inputQueue.Add(input);
            }
        }

        public bool IsInputTerminated() {
            return isInputTerminated;
        }

        public void WriteLine(string line) {
            inputQueue.Add(line);
        }

        public string ReadLine() {
            return inputQueue.Take();
        }

        public void RestoreSavedState() {
            if (currentFavoritesList != null) {
                inputQueue.Add(currentFavoritesList);
            }
        }
    }
}
