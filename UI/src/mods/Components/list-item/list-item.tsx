import { ObjectsInEntity } from "domain/ObjectsInEntity";
import styles from "./list-item.module.scss";
import { Button } from "cs2/ui";
import { VanillaComponentResolver } from "mods/vanillacomponentresolver";
import classNames from "classnames";

export const ListItem = ({ object }: { object: ObjectsInEntity }) => {
  const text = `Entity ${object.ID}`;
  //   console.log(`${object.Name} => ${object.Thumbnail}`);
  return (
    <Button className={styles.buttons}>
      <div className={styles.listItems}>
        <div>{text}</div>
        {/* <img
          className={styles.gridThumbnail}
          src={object.Thumbnail ?? "Media/Placeholder.svg"}
        /> */}
        <div>{object.Name}</div>
      </div>
    </Button>
  );
};
