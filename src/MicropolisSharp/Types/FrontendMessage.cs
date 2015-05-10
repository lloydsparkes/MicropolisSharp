using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicropolisSharp.Types
{
    public class FrontendMessage
    {
        public virtual void SendMessage(Micropolis sim)
        {
            throw new NotImplementedException();
        }
    }

    public class FrontendMessageMakeSound : FrontendMessage
    {
        private int dozeX;
        private int dozeY;
        private string channel;
        private string sound;

        public FrontendMessageMakeSound(string v1, string v2, int dozeX, int dozeY)
        {
            this.channel = v1;
            this.sound = v2;
            this.dozeX = dozeX;
            this.dozeY = dozeY;
        }

        public override void SendMessage(Micropolis sim)
        {
            sim.MakeSound(channel, sound, dozeX, dozeY);
        }
    }

    public class FrontendMessageDidTool : FrontendMessage
    {
        private string name;
        private short x;
        private short y;

        public FrontendMessageDidTool(string v, short x, short y)
        {
            this.name = v;
            this.x = x;
            this.y = y;
        }

        public override void SendMessage(Micropolis sim)
        {
            sim.DidTool(name, x, y);
        }
    }
}
