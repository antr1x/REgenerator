# Lua Natives Generator

This documentation provides guidelines on how to use the **Lua Natives Generator** and the expected format for the native declarations file. The source code is comprehensively documented (commented), ensuring its utilization should be straightforward and trouble-free. Once generated, all Lua files named after their respective namespaces can be integrated into your Lua script using the require function.

## How to Use

1. **Ensure that Lua Natives Generator compiled and ready for use.**

2. **Prepare your native declarations file**. The file should contain native functions declarations formatted in a specific manner. Each declaration should be prefixed with the `NATIVE_DECL` macro.

3. **To use the utility** - simple drag and drop natives file onto compiled exe file.

## File Format

Your native declarations file should follow the specific format exemplified below:

```cpp
namespace SYSTEM
{
	NATIVE_DECL int START_NEW_SCRIPT(const char* scriptName, int stackSize) { return invoke<int>(0xE81651AD79516E48, scriptName, stackSize); } // 0xE81651AD79516E48 0x3F166D0E b323
    ... // other declarations
}

namespace APP
{
	NATIVE_DECL BOOL APP_DATA_VALID() { return invoke<BOOL>(0x846AA8E7D55EE5B6); } // 0x846AA8E7D55EE5B6 0x72BDE002 b323
    ... // other declarations
}
```

... and so on for other namespaces.

## Example Usage
Suppose you have placed a Lua file named **PLAYER.lua** into the **SCRIPT_NAME/natives** directory. In your main.lua script:

```lua
require ("natives/PLAYER")

local player_id = PLAYER_ID()
local ped_id = GET_PLAYER_PED_SCRIPT_INDEX(player_id)
```

## Important Notes
**Namespaces:** Ensure each group of native functions is encapsulated within its respective namespace.
**NATIVE_DECL:** This macro is essential for the utility to identify and process the native function declarations properly.
