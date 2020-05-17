using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public class GameEndHandler
    {
        public delegate void GameEnd();
        public event GameEnd OnGameEnded;

        public int N { get; private set; }
        private List<int> done;
        private List<int> axis;

        public GameEndHandler(int n)
        {
            done = new List<int>();
            axis = new List<int>();

            N = n;
        }

        private bool isEnded()
        {
            return done.Count == axis.Count;
        }

        public void addAxisDone(int x)
        {            
            done.Add(x);            
            if (isEnded())
            {
                OnGameEnded?.Invoke();
            }
        }
        public void addAxisAll(int x)
        {
            if (!axis.Contains(x))
            {
                axis.Add(x);
            }
        }

        public bool canAddAxis(int x)
        {
            return !axis.Contains(x);
        }
    }
}
