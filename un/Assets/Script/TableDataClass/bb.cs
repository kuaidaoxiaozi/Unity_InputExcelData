using System.IO;
public class bb : TDBase {
	public int b2 {
		get;
		private set;
	}
	public int b3 {
		get;
		private set;
	}
	public int b4 {
		get;
		private set;
	}
	public int b5 {
		get;
		private set;
	}
	public int b6 {
		get;
		private set;
	}
	public override void updateData(BinaryReader br) {
		if (canInit == false)
			return;
		canInit = false;
		id = br.ReadInt32();
		b2 = br.ReadInt32();
		b3 = br.ReadInt32();
		b4 = br.ReadInt32();
		b5 = br.ReadInt32();
		b6 = br.ReadInt32();
	}
}
