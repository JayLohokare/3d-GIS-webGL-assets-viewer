import React, { Component } from "react";
import { inject, observer } from "mobx-react";
class ModelsListElement extends Component {

  modelViewCallback = () => {
    this.props.store.setSelectedModel(this.props.model);
  };

  render() {
    return (
      <div className="menu-card" key={`${this.props.i}-${this.props.model.Equipement}`}>
        <div className="white darken-1">
            <div className="row valign-wrapper">

              <div className="col l10 s10 m10">
                <div className="left-align">
                  <p className="black-text left-align">{this.props.model.Equipement}</p>
                </div>
              </div>

              <div className="col l2 s2 m2">
                <button 
                style={{border: "none", backgroundColor: "transparent" }}
                className="blue-text" onClick={this.modelViewCallback.bind(this)}>
                  <i className="material-icons blue-text">call_made</i>
                </button>
              </div>
            </div>

        </div>
      </div>
    );
  }
}

export default inject("store")(observer(ModelsListElement));
