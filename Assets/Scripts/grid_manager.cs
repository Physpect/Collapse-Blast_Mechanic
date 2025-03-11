using System;
using System.Collections.Generic;
using UnityEngine;

public class grid_manager : MonoBehaviour
{
    public GameObject block_prefab;
    public Transform board_parent;
    public Sprite[] blue_block_sprites;
    public Sprite[] pink_block_sprites;
    public Sprite[] green_block_sprites;
    public Sprite[] yellow_block_sprites;
    public Sprite[] red_block_sprites;
    public Sprite[] purple_block_sprites;

    public bool allow_blue = true;
    public bool allow_pink = true;
    public bool allow_green = true;
    public bool allow_yellow = true;
    public bool allow_red = true;
    public bool allow_purple = true;

    public int rows = 10;
    public int columns = 12;
    public float block_size = 2f;
    public static GameObject[,] grid_array;

    void Start()
    {
        generate_grid();
        check_and_change_sprites();
    }

    void generate_grid()
    {
        grid_array = new GameObject[rows, columns];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 position = new Vector3(x * block_size, -y * block_size, 0);
                GameObject block = Instantiate(block_prefab, position, Quaternion.identity, board_parent);
                block.transform.localScale = new Vector3(0.45f, 0.45f, 1f);

                SpriteRenderer sprite_renderer = block.GetComponent<SpriteRenderer>();
                if (sprite_renderer != null)
                {
                    int color_index = get_random_color_index();
                    sprite_renderer.sprite = get_block_sprite_by_color(color_index);
                }

                grid_array[y, x] = block;
            }
        }
    }

    int get_random_color_index()
    {
        List<int> valid_color_indices = new List<int>();

        if (allow_blue) valid_color_indices.Add(0);
        if (allow_pink) valid_color_indices.Add(1);
        if (allow_green) valid_color_indices.Add(2);
        if (allow_yellow) valid_color_indices.Add(3);
        if (allow_red) valid_color_indices.Add(4);
        if (allow_purple) valid_color_indices.Add(5);

        if (valid_color_indices.Count > 0)
        {
            return valid_color_indices[UnityEngine.Random.Range(0, valid_color_indices.Count)];
        }
        else
        {
            return -1;
        }
    }

    public Sprite get_block_sprite_by_color(int color_index)
    {
        switch (color_index)
        {
            case 0: return blue_block_sprites[UnityEngine.Random.Range(0, blue_block_sprites.Length)];
            case 1: return pink_block_sprites[UnityEngine.Random.Range(0, pink_block_sprites.Length)];
            case 2: return green_block_sprites[UnityEngine.Random.Range(0, green_block_sprites.Length)];
            case 3: return yellow_block_sprites[UnityEngine.Random.Range(0, yellow_block_sprites.Length)];
            case 4: return red_block_sprites[UnityEngine.Random.Range(0, red_block_sprites.Length)];
            case 5: return purple_block_sprites[UnityEngine.Random.Range(0, purple_block_sprites.Length)];
            default: return null;
        }
    }

    void check_and_change_sprites()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (grid_array[y, x] != null)
                {
                    SpriteRenderer current_sprite = grid_array[y, x].GetComponent<SpriteRenderer>();
                    if (current_sprite != null)
                    {
                        current_sprite.sprite = get_default_sprite_for_block(current_sprite.sprite);
                    }
                }
            }
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (grid_array[y, x] != null)
                {
                    SpriteRenderer current_sprite = grid_array[y, x].GetComponent<SpriteRenderer>();
                    if (current_sprite != null)
                    {
                        int group_size = 1;
                        List<Vector2Int> group_positions = new List<Vector2Int> { new Vector2Int(x, y) };
                        Sprite current_color_sprite = current_sprite.sprite;

                        List<Vector2Int> group = find_connected_blocks(x, y, current_color_sprite);
                        group_size = group.Count;

                        if (group_size > 1)
                        {
                            Sprite new_sprite = get_sprite_for_group_size_and_color(group_size, current_color_sprite);
                            foreach (var pos in group)
                            {
                                if (grid_array[pos.y, pos.x] != null)
                                {
                                    grid_array[pos.y, pos.x].GetComponent<SpriteRenderer>().sprite = new_sprite;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    List<Vector2Int> find_connected_blocks(int x, int y, Sprite color_sprite)
    {
        List<Vector2Int> connected_blocks = new List<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Stack<Vector2Int> to_visit = new Stack<Vector2Int>();

        to_visit.Push(new Vector2Int(x, y));

        while (to_visit.Count > 0)
        {
            Vector2Int current = to_visit.Pop();
            if (visited.Contains(current)) continue;

            visited.Add(current);
            connected_blocks.Add(current);

            Vector2Int[] directions = new Vector2Int[] {
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0)
            };

            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;

                if (neighbor.x >= 0 && neighbor.x < columns && neighbor.y >= 0 && neighbor.y < rows)
                {
                    if (grid_array[neighbor.y, neighbor.x] != null)
                    {
                        Sprite neighbor_sprite = grid_array[neighbor.y, neighbor.x].GetComponent<SpriteRenderer>().sprite;
                        if (neighbor_sprite == color_sprite && !visited.Contains(neighbor))
                        {
                            to_visit.Push(neighbor);
                        }
                    }
                }
            }
        }

        return connected_blocks;
    }

    Sprite get_default_sprite_for_block(Sprite color_sprite)
    {
        if (color_sprite == null) return null;

        if (Array.Exists(blue_block_sprites, sprite => sprite == color_sprite))
        {
            return blue_block_sprites[0];
        }
        else if (Array.Exists(pink_block_sprites, sprite => sprite == color_sprite))
        {
            return pink_block_sprites[0];
        }
        else if (Array.Exists(green_block_sprites, sprite => sprite == color_sprite))
        {
            return green_block_sprites[0];
        }
        else if (Array.Exists(yellow_block_sprites, sprite => sprite == color_sprite))
        {
            return yellow_block_sprites[0];
        }
        else if (Array.Exists(red_block_sprites, sprite => sprite == color_sprite))
        {
            return red_block_sprites[0];
        }
        else if (Array.Exists(purple_block_sprites, sprite => sprite == color_sprite))
        {
            return purple_block_sprites[0];
        }

        return null;
    }

    Sprite get_sprite_for_group_size_and_color(int group_size, Sprite color_sprite)
    {
        if (color_sprite == null) return null;

        if (Array.Exists(blue_block_sprites, sprite => sprite == color_sprite))
        {
            return get_sprite_for_group_size(group_size, blue_block_sprites);
        }
        else if (Array.Exists(pink_block_sprites, sprite => sprite == color_sprite))
        {
            return get_sprite_for_group_size(group_size, pink_block_sprites);
        }
        else if (Array.Exists(green_block_sprites, sprite => sprite == color_sprite))
        {
            return get_sprite_for_group_size(group_size, green_block_sprites);
        }
        else if (Array.Exists(yellow_block_sprites, sprite => sprite == color_sprite))
        {
            return get_sprite_for_group_size(group_size, yellow_block_sprites);
        }
        else if (Array.Exists(red_block_sprites, sprite => sprite == color_sprite))
        {
            return get_sprite_for_group_size(group_size, red_block_sprites);
        }
        else if (Array.Exists(purple_block_sprites, sprite => sprite == color_sprite))
        {
            return get_sprite_for_group_size(group_size, purple_block_sprites);
        }

        return null;
    }

    Sprite get_sprite_for_group_size(int group_size, Sprite[] color_sprites)
    {
        if (group_size >= 10)
        {
            return color_sprites[3];
        }
        else if (group_size >= 8)
        {
            return color_sprites[2];
        }
        else if (group_size >= 5)
        {
            return color_sprites[1];
        }
        else
        {
            return color_sprites[0];
        }
    }

    public void destroy_connected_blocks(block clicked_block)
    {
        Vector2Int clicked_pos = get_block_position(clicked_block);
        if (clicked_pos == new Vector2Int(-1, -1)) return;

        Sprite clicked_sprite = clicked_block.GetComponent<SpriteRenderer>().sprite;
        List<Vector2Int> connected_blocks = find_connected_blocks(clicked_pos.x, clicked_pos.y, clicked_sprite);

        if (connected_blocks.Count >= 2)
        {
            foreach (var pos in connected_blocks)
            {
                Destroy(grid_array[pos.y, pos.x]);
                grid_array[pos.y, pos.x] = null;
            }

            drop_blocks();
            fill_empty_spaces();
        }
    }

    void drop_blocks()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = rows - 1; y >= 0; y--)
            {
                if (grid_array[y, x] == null)
                {
                    for (int above_y = y - 1; above_y >= 0; above_y--)
                    {
                        if (grid_array[above_y, x] != null)
                        {
                            grid_array[y, x] = grid_array[above_y, x];
                            grid_array[above_y, x] = null;
                            grid_array[y, x].transform.position = new Vector3(x * block_size, -y * block_size, 0);
                            break;
                        }
                    }
                }
            }
        }
    }

    void fill_empty_spaces()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = rows - 1; y >= 0; y--)
            {
                if (grid_array[y, x] == null)
                {
                    Vector3 position = new Vector3(x * block_size, -y * block_size, 0);
                    GameObject block = Instantiate(block_prefab, position, Quaternion.identity, board_parent);
                    block.transform.localScale = new Vector3(0.45f, 0.45f, 1f);

                    SpriteRenderer sprite_renderer = block.GetComponent<SpriteRenderer>();
                    if (sprite_renderer != null)
                    {
                        int color_index = get_random_color_index();
                        sprite_renderer.sprite = get_block_sprite_by_color(color_index);
                    }

                    grid_array[y, x] = block;
                }
            }
        }
    }

    Vector2Int get_block_position(block clicked_block)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (grid_array[y, x] == clicked_block.gameObject)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public bool is_no_more_matches()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (grid_array[y, x] != null)
                {
                    Sprite current_sprite = grid_array[y, x].GetComponent<SpriteRenderer>().sprite;
                    List<Vector2Int> connected_blocks = find_connected_blocks(x, y, current_sprite);
                    if (connected_blocks.Count >= 2)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    void destroy_blocks_in_color_order()
    {
        Sprite[] color_order = new Sprite[]
        {
            yellow_block_sprites[0],
            blue_block_sprites[0],
            green_block_sprites[0],
            pink_block_sprites[0],
            purple_block_sprites[0],
            red_block_sprites[0]
        };

        foreach (Sprite color_sprite in color_order)
        {
            while (is_no_more_matches())
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        if (grid_array[y, x] != null)
                        {
                            SpriteRenderer current_sprite = grid_array[y, x].GetComponent<SpriteRenderer>();
                            if (current_sprite != null && current_sprite.sprite == color_sprite)
                            {
                                Destroy(grid_array[y, x]);
                                grid_array[y, x] = null;
                            }
                        }
                    }
                }

                drop_blocks();
                fill_empty_spaces();
            }
        }
    }
    private void Update()
    {
        if (is_no_more_matches())
        {
            destroy_blocks_in_color_order();
        }
        check_and_change_sprites();

    }
}