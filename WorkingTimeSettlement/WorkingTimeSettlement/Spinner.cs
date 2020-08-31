using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkingTimeSettlement
{

    public class Spinner : IDisposable
    {
        private string Sequence { get; }
        private int counterSequece = 0;
        private int counter = 0;
        private readonly int left;
        private readonly int top;
        private readonly int delay;
        private bool active;
        private readonly Thread thread;
        private bool isWriting = true;

        public Spinner(string sequence, int left, int top, int delay = 100)
        {
            this.Sequence = sequence;
            this.left = left;
            this.top = top;
            this.delay = delay;
            thread = new Thread(Spin);
        }

        public void Start()
        {
            active = true;
            if (!thread.IsAlive)
                thread.Start();
        }

        public void Stop()
        {
            active = false;
            OverrideText("DONE");
        }

        private void Spin()
        {
            while (active)
            {
                Turn();
                Thread.Sleep(delay);
            }
        }
        private void OverrideText (string s)
        {
            int i;
            isWriting = true;
            counter = 0;
            counterSequece = 0;
            for (i = 0; i < s.Length; i++)
            {
                Draw(s[i]);
            }
            Clear(i);
        }

        private void Clear(int j)
        {
            for (int i = j; i < Sequence.Length; i++)
            {
                Draw(' ');
            }
        }

        private void Draw(char c)
        {
            Console.SetCursorPosition(left + counterSequece, top);
            Console.ForegroundColor = ConsoleColor.Green;
            if (isWriting)
            {
                Console.Write(c);
            }
            else
            {
                Console.Write(" ");
            }

            if (counterSequece == Sequence.Length - 1)
            {
                counterSequece = 0;
                isWriting = !isWriting;
            }
            else
            {
                counterSequece++;
            }
        }

        private void Turn()
        {
            var index = counter % Sequence.Length;
            Draw(Sequence[index]);
            counter++;
        }

        public void Dispose()
        {
            Stop();
        }

    }
}
