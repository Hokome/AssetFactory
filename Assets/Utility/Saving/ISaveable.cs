using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetFactory
{
	public interface ISaveable
	{
		void SaveFile(BinaryWriter writer);
		void LoadFile(BinaryReader reader);
	}
}
