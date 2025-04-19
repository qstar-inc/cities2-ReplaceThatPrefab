import { ModRegistrar } from "cs2/modding";
import { HelloWorldComponent } from "mods/hello-world";
import TopLeft from "mods/Components/top-left/top-left";
import { UIBox } from "mods/Components/ui-box/ui-box";

const register: ModRegistrar = (moduleRegistry) => {
  moduleRegistry.append("Menu", HelloWorldComponent);
  moduleRegistry.append("GameTopLeft", TopLeft);
  moduleRegistry.append("Game", () => UIBox());
};

export default register;
