import React from 'react';

const LayersButton = (props) => {
  return (
    <a
    data-position="left"
    data-tooltip="Edit Layers"
    className=" btn-floating btn-large tooltipped  waves-effect waves-dark white  modal-trigger layers-button"
    href="#modal1">
        <i className="material-icons  black-text">menu</i>
    </a>
  );
};

export default LayersButton