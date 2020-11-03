import React from 'react';
import { inject, observer } from "mobx-react";


const StreetViewModeButton = (props) => {
  
  return (
    <button
    data-position="left"
    data-tooltip={"Change to " + (props.store.mapViewPort.pitch === 0 ? "3D" : "2D") + " mode"}
    className=" btn-floating black-text btn-large tooltipped  waves-effect waves-dark white  street-view-button"
    onClick={props.toggleStreetView}
    >
    {props.store.mapViewPort.pitch  === 0 ? "3D" : "2D"}
    </button>
  );
};

export default inject("store")(observer(StreetViewModeButton));
