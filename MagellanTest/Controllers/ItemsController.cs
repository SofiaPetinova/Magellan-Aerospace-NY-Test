using Microsoft.AspNetCore.Mvc;
using System;
using Npgsql;
using Dapper;


namespace MagellanTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
         private readonly string connectionString = "DefaultConnection";

        [HttpPost]
        public IActionResult CreateNewItem([FromBody] ItemRequest itemRequest)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var sql = @"INSERT INTO item (item_name, parent_item, cost, req_date)
                            VALUES (@ItemName, @ParentItem, @Cost, @ReqDate)
                            RETURNING id";

                var itemId = connection.ExecuteScalar<int>(sql, itemRequest);
                return Ok(new { id = itemId });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetItemById(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var sql = @"SELECT id, item_name, parent_item, cost, req_date
                            FROM item
                            WHERE id = @Id";

                var item = connection.QueryFirstOrDefault<Item>(sql, new { Id = id });

                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
            }
        }

        [HttpGet("totalCost/{itemName}")]
        public IActionResult GetTotalCost(string itemName)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT Get_Total_Cost(@ItemName)";

                var totalCost = connection.ExecuteScalar<int>(sql, new { ItemName = itemName });

                return Ok(new { totalCost });
            }
        }

        public class ItemRequest
        {
            public string ItemName { get; set; }
            public int? ParentItem { get; set; }
            public int Cost { get; set; }
            public DateTime ReqDate { get; set; }
        }

        public class Item
        {
            public int Id { get; set; }
            public string ItemName { get; set; }
            public int? ParentItem { get; set; }
            public int Cost { get; set; }
            public DateTime ReqDate { get; set; }
        }
    }
}
