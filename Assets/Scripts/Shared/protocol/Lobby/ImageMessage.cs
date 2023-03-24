using UnityEngine;

namespace shared
{
	/**
	 * BIDIRECTIONAL Chat message for the lobby
	 */
	public class ImageMessage : ASerializable
	{
		public byte[] data;

		public override void Serialize(Packet pPacket)
		{
			pPacket.Write(data);
		}

		public override void Deserialize(Packet pPacket)
		{
			data = pPacket.ReadBytes();
		}
	}
}
