using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared
{
    /**
     * Send from SERVER to CLIENT to identify which player are playing the game
     */
    public class StartGame : ASerializable
    {
        public string player1Name;
        public string player2Name;
        public int movingPlayer;

        public override void Deserialize(Packet pPacket)
        {
            player1Name = pPacket.ReadString();
            player2Name = pPacket.ReadString();
            movingPlayer = pPacket.ReadInt();
        }

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(player1Name);
            pPacket.Write(player2Name);
            pPacket.Write(movingPlayer);
        }
    }
}
