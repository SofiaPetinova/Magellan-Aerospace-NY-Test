CREATE DATABASE Part;

\c Part;

CREATE TABLE item (
    id SERIAL PRIMARY KEY,
    item_name VARCHAR(50) NOT NULL,
    parent_item INTEGER REFERENCES item(id),
    cost INTEGER NOT NULL,
    req_date DATE NOT NULL
);


INSERT INTO item (id, item_name, parent_item, cost, req_date) VALUES
(1, 'Item1', null, 500, '2024-02-20'),
(2, 'Sub1', 1, 200, '2024-02-10'),
(3, 'Sub2', 1, 300, '2024-01-05'),
(4, 'Sub3', 2, 300, '2024-01-02'),
(5, 'Sub4', 2, 400, '2024-01-02'),
(6, 'Item2', null, 600, '2024-03-15'),
(7, 'Sub1', 6, 200, '2024-02-25');


CREATE OR REPLACE FUNCTION Get_Total_Cost(item_name_input VARCHAR(50))
RETURNS INTEGER AS $$
DECLARE
    total_cost INTEGER := 0;
BEGIN
    WITH RECURSIVE item_tree AS (
        SELECT id, item_name, parent_item, cost
        FROM item
        WHERE item_name = item_name_input
        
        UNION ALL
        
        SELECT i.id, i.item_name, i.parent_item, i.cost
        FROM item_tree it
        JOIN item i ON it.id = i.parent_item
    )
    SELECT INTO total_cost SUM(cost) FROM item_tree;
    
    RETURN total_cost;
END;
$$ LANGUAGE plpgsql;

