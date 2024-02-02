# Lua Natives Generator

This documentation provides guidelines on how to use the **Lua Natives Generator** and the expected format for the native declarations file. The source code is comprehensively documented (commented), ensuring its utilization should be straightforward and trouble-free. Once generated, all Lua files named after their respective namespaces can be integrated into your Lua script using the require function.

## How to Use

1. **Ensure that Lua Natives Generator compiled and ready for use.**
2. **Start the REgenerator.exe file.**

## Example Usage
Suppose you have placed a Lua file named **PLAYER.lua** into the **SCRIPT_NAME/natives** directory. In your main.lua script:

```lua
require ("natives/PLAYER")

local player_id = PLAYER.PLAYER_ID()
local ped_id = PLAYER.GET_PLAYER_PED_SCRIPT_INDEX(player_id)
```
