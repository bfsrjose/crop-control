using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using SQLite4Unity3d;
using UnityEngine.Rendering;

public class Init : MonoBehaviour
{
    class testTable
    {
        public int num;
    }

    struct Position
    {
        public int row;
        public int column;
    }

    class Mytable
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public string Name { get; set; }
        public int Num { get; set; }
    }

    class Bed
    {
        [PrimaryKey, AutoIncrement] public int BedId { get; set; }
        public string Name { get; set; }
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }
    }

    class Foot
    {
        [PrimaryKey, AutoIncrement] public int FootId { get; set; }
        public string Name { get; set; }
        public int BedId { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    class Grid
    {
        [PrimaryKey, AutoIncrement] public int GridId { get; set; }
        public string Name { get; set; }
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }
        public int RowStart { get; set; }
        public int ColumnStart { get; set; }
        public int SlotSize { get; set; }
    }

    class Alloc
    {
        [PrimaryKey, AutoIncrement] public int AllocId { get; set; }
        public int GridId { get; set; }
        public int FootId { get; set; }
    }

    class Slot
    {
        [PrimaryKey, AutoIncrement] public int SlotId { get; set; }
        public string Name { get; set; }
        public int GridId { get; set; }
        public int SlotIndex { get; set; }
    }

    class Plant
    {
        public int PlantId { get; set; }
        public string Name { get; set; }
        public int SlotId { get; set; }
        public int PlantDefinitionId { get; set; }
    }

    class PlantDefinition
    {
        public int PlantDefintionId { get; set; }
        public string Name { get; set; }
    }


    // Start is called before the first frame update
    void Start()
    {
        var db = new SQLiteConnection("./assets/dbstore/test_db", true);
        db.Trace = true;

        DropTables(db);
        CreateTables(db);

        PlantRadishes(db);
    }

    void PlantRadishes(SQLiteConnection db)
    {
        var radishes = new PlantDefinition {Name = "Radish"};
        db.Insert(radishes);

        var rows = 4;
        var cols = 4;
        var gridRowStart = 0;
        var gridColStart = 0;
        var gridRowSize = 1;
        var gridColSize = 1;
        var gridSlots = 16;

        //Create Bed

        var protoBed = new Bed {Name = "ProtoBed", RowSize = rows, ColumnSize = cols};
        db.Insert(protoBed);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                db.Insert(new Foot
                {
                    BedId = protoBed.BedId,
                    Row = row,
                    Column = col
                });
            }
        }

        // Make a Grid to put the radishes in
        var firstGrid = new Grid()
        {
            ColumnSize = gridColSize, ColumnStart = gridColStart, Name = "First Grid",
            RowSize = gridRowSize, RowStart = gridRowStart, SlotSize = gridSlots
        };
        db.Insert(firstGrid);

        // Find the feet to allocate to the grid and alloc them
        var positions = new HashSet<Position>();

        for (int row = 0; row < gridRowSize; row++)
        {
            for (int col = 0; col < gridColSize; col++)
            {
                positions.Add(new Position {row = row + gridRowStart, column = col + gridColStart});
            }
        }

//        var query = db.Table<Foot>().Where(s => positions.Contains(new Position {row = s.Row, column = s.Column}));
//
//        foreach (var foot in query)
//        {
//            db.Insert(new Alloc
//                {
//                    GridId = firstGrid.GridId,
//                    FootId = foot.FootId
//                }
//            );
//        }

        var query = db.Table<Foot>();

        foreach (var foot in query)
        {
            if (!positions.Contains(new Position {row = foot.Row, column = foot.Column}))
            {
                continue;
            }
            
            db.Insert(new Alloc
                {
                    GridId = firstGrid.GridId,
                    FootId = foot.FootId
                }
            );
        }

        // Make the slots for the Grid, put radishes in them

        for (int i = 0; i < gridSlots; i++)
        {
            var slot = new Slot
            {
                SlotIndex = i, GridId = firstGrid.GridId
            };

            db.Insert(slot);

            db.Insert(new Plant
                {
                    SlotId = slot.SlotId,
                    PlantDefinitionId = radishes.PlantDefintionId
                }
            );
        }
    }

    void DropTables(SQLiteConnection db)
    {
        db.DropTable<Mytable>();
        db.DropTable<Bed>();
        db.DropTable<Foot>();
        db.DropTable<Grid>();
        db.DropTable<Alloc>();
        db.DropTable<Slot>();
        db.DropTable<Plant>();
        db.DropTable<PlantDefinition>();
    }

    void CreateTables(SQLiteConnection db)
    {
        db.CreateTable<Mytable>();
        db.CreateTable<Bed>();
        db.CreateTable<Foot>();
        db.CreateTable<Grid>();
        db.CreateTable<Alloc>();
        db.CreateTable<Slot>();
        db.CreateTable<Plant>();
        db.CreateTable<PlantDefinition>();
    }

    void holder()
    {
        var db = new SQLiteConnection("./assets/dbstore/test_db", true);
        db.Trace = true;


        var testQuery = "select * from testTable";
//
//        var queryHandle = SQLite3.Prepare2(connection.Handle, testQuery);
//        
//        queryHandle.

//        var vals = connection.Query<testTable>(testQuery);
//        
//        Debug.Log(vals.Count);
//        Debug.Log(vals[0].num);
//        Debug.Log(vals[1].num);

//        connection.Get<testTable>()

//        var query = db.Table<testTable>().Where(s => s.Symbol.StartsWith("A"));
//        var query = db.Table<testTable>();
//
//        var result = query.ToList();
//
//        foreach (var s in result)
//            Console.WriteLine("num: " + s.num);

//        db.CreateTable<Mytable>();
//        db.Insert(new Mytable
//            {Num = 5});

        var query = db.Table<Mytable>();

        var result = query.ToList();

        foreach (var s in result)
            Debug.Log("num: " + s.Num);
    }

    // Update is called once per frame
    void Update()
    {
    }
}