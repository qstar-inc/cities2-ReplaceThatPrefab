import { ModRegistrar } from "cs2/modding";
import { HelloWorldComponent } from "mods/hello-world";
import TopLeft from "mods/Components/top-left";

const register: ModRegistrar = (moduleRegistry) => {
  moduleRegistry.append("Menu", HelloWorldComponent);
  moduleRegistry.append("GameTopLeft", TopLeft);
};

export default register;
