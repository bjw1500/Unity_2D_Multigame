using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

//빌드 할 때는 앱에 딸려가지 않도록 if Unity_Editor을 해줘야 한다.

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor
{

#if UNITY_EDITOR

	// % (Ctrl), # (Shift), & (Alt)

	[MenuItem("Tools/GenerateMap %#g")]
	private static void GenerateMap()
	{

		GenerateByPath("Assets/Resources/Map");
		GenerateByPath("../Common/MapData");


	}

	private static void GenerateByPath(string pathPrefix)
    {

		GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");

		foreach (GameObject go in gameObjects)
		{


			Tilemap tmBase = Util.FindChild<Tilemap>(go, "Tilemap_Base", true);
			Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

			using (var writer = File.CreateText($"{pathPrefix}/{go.name}.txt"))
			{
				writer.WriteLine(tmBase.cellBounds.xMin);
				writer.WriteLine(tmBase.cellBounds.xMax);
				writer.WriteLine(tmBase.cellBounds.yMin);
				writer.WriteLine(tmBase.cellBounds.yMax);

				for (int y = tmBase.cellBounds.yMax; y >= tmBase.cellBounds.yMin; y--)
				{
					for (int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; x++)
					{
						TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
						if (tile != null)
						{
							string name = tile.name;
							if (name.Equals("Base Blue"))
								writer.Write("2");
							else if (name.Equals("Base Red"))
								writer.Write("3");
							else if (name.Equals("barrel")) //자원
								writer.Write("9");
							else
								writer.Write("1");

						}
						else
							writer.Write("0");

					}
					writer.WriteLine();
				}
			}
		}

	}

#endif

}
