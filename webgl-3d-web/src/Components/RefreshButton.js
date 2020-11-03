import React from 'react';



const RefreshButton = (props) => {
  return (
    <div className="refresh-button">
      <button
        onClick={props.refreshClick}
        data-position="left" 
        data-tooltip="Search this location" 
        className="btn-floating  tooltipped  btn-large waves-effect waves-dark white "
      >
        <i className="material-icons  black-text">refresh</i>
      </button>
    </div>
  );
};

export default RefreshButton