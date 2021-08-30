using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.PpsPro
{
    public class AreaLineCell
    {
        private int oriId;
        private int endId;
        private bool isStart;

        public int OriId { get { return OriId; } }
        public int EndId { get { return endId; } }
        public void Start(int id)
        {
            if (isStart) return;
            oriId = id;
            isStart = true;
        }

        public void End(int id)
        {
            endId = id - 1;
        }

        public void ReInit(int oriId, int endId)
        {
            this.oriId = oriId;
            this.endId = endId;
        }
    }
}
