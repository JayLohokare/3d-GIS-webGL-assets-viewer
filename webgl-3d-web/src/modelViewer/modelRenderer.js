import React, { Component } from "react";
import { inject, observer } from "mobx-react";


class ModelRenderer extends Component {
  state = {
      model: this.props.model
    };

  render() {
    if (this.props.store.modelViewerState ){
      return (
        <div className="modelViewerContainer white">
                   
        </div>
      );
    }
    else{
      return (
        <div></div>
      );
    }
  }
}

export default inject("store")(observer(ModelRenderer));
