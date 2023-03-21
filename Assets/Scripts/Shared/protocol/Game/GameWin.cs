using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared
{
    /**
     * Send from SEVER to CLIENT to signify a game has been won by someone
     */
    public class GameWin : ASerializable
    {
        public int winningPlayerIndex;

        public override void Deserialize(Packet pPacket)
        {
            winningPlayerIndex = pPacket.ReadInt();
        }

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(winningPlayerIndex);
        }
    }
}
