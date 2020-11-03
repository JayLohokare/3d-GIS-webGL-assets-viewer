import React, { Component } from "react";
import Loader from "./Components/Loader";
import Error from "./Components/Error";
import ModelsView from "./modelDetails/modelsView";
import WebglView from "./webGL/mapView";
import { inject, observer } from "mobx-react";
import Materialize from "materialize-css";

class App extends Component {
  componentDidUpdate() {
    var sidenav = document.querySelectorAll(".sidenav");
    Materialize.Sidenav.init(sidenav, null);
  }

  async componentDidMount() {
    try {
      this.props.store.initiateMap();
    } catch (error) {
      console.log(error);
    }
  }

  withDataState = (renderer) => {
    return (
      <div>
        <button
          style={{border: "none", backgroundColor: "transparent" }}
          data-target="slide-out"
          className="sidenav-trigger mobileMenu"
        >
          <i className="material-icons black-text menu-icon">menu</i>
        </button>

        <ul id="slide-out" className="sidenav sidenav-fixed grey lighten-3">
          {renderer}
        </ul>

        <WebglView />
      </div>
    );
  };

  loadingState = () => <Loader />;
  errorState = () => <Error />;
  loadedState = () => <ModelsView />;

  render() {
    const { assetsLoadingState, errorState } = this.props.store;
    if (errorState === false) {
      if (assetsLoadingState === false) {
        return this.withDataState(this.loadedState());
      } else {
        return this.withDataState(this.loadingState());
      }
    } else {
      return this.withDataState(this.errorState());
    }
  }
}
export default inject("store")(observer(App));
