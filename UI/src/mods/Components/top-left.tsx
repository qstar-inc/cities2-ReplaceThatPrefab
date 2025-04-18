import classNames from "classnames";
import { Button, Tooltip } from "cs2/ui";
import { tool } from "cs2/bindings";
import { bindValue, trigger, useValue } from "cs2/api";
import mod from "mod.json";
import styles from "./top-left.module.scss";
import trafficIcon from "images/RTP_ModIcon.svg";
import trafficIconActive from "images/RTP_ModIconActive.svg";
import { RTPToolModeEnum } from "domain/RTPToolMode";
import { rtpToolMode$, toggleTool } from "mods/bindings";

export default () => {
  const rptToolMode = useValue(rtpToolMode$);
  return (
    <Tooltip tooltip={mod.name}>
      <Button
        variant="floating"
        className={classNames(
          {
            [styles.selected]: rptToolMode !== RTPToolModeEnum.None,
          },
          styles.toggle
        )}
        onSelect={toggleTool}
      >
        <img
          style={{
            maskImage: `url(${
              rptToolMode !== RTPToolModeEnum.None
                ? trafficIconActive
                : trafficIcon
            })`,
          }}
        />
      </Button>
    </Tooltip>
  );
};
