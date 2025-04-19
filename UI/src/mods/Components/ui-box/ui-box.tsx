import { Scrollable } from "cs2/ui";
import styles from "./ui-box.module.scss";
import classNames from "classnames";
import { useValue } from "cs2/api";
import { objectsInEntity$, objectSelected, rtpToolMode$ } from "mods/bindings";
import { ListItem } from "../list-item/list-item";
import { RTPToolModeEnum } from "domain/RTPToolMode";

export const UIBox = () => {
  const objectsInEntity = useValue(objectsInEntity$);
  let items: JSX.Element[];
  items = objectsInEntity.map((val) => <ListItem object={val} />);

  const rptToolMode = useValue(rtpToolMode$);
  const rptToolActive = rptToolMode == RTPToolModeEnum.Selected;
  return (
    <>
      {rptToolActive && (
        <>
          <div className={styles.container}>
            <Scrollable
              className={styles.list}
              vertical
              smooth
              trackVisibility="scrollable"
            >
              <div className={styles.header}>RTP Objects</div>
              <div className={styles.gridContainer}>{items}</div>
            </Scrollable>
          </div>
        </>
      )}
    </>
  );
};
