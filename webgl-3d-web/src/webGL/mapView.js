import React, { Component } from "react";
import DataView from "./dataView";
import { OBJLoader } from "@loaders.gl/obj";
import { registerLoaders } from "@loaders.gl/core";
import Materialize from "materialize-css";
import { SimpleMeshLayer } from "@deck.gl/mesh-layers";
import { PathLayer } from "@deck.gl/layers";
import { COORDINATE_SYSTEM, MapView } from "@deck.gl/core";
import { DirectionalLight, LightingEffect, PointLight, AmbientLight, _CameraLight as CameraLight } from "@deck.gl/core";
import { PathStyleExtension } from "@deck.gl/extensions";
import { DeckGL } from "@deck.gl/react";
import { StaticMap } from "react-map-gl";
import { inject, observer } from "mobx-react";
import { toJS } from "mobx";
import ControlPanel from "./control-panel";
import RefreshButton from "../Components/RefreshButton";
import LayersButton from "../Components/LayersButton";
import StreetViewModeButton from "../Components/StreetViewModeButton";

registerLoaders([OBJLoader]);

const accessToken =
  "pk.eyJ1IjoiamF5bG9ob2thcmUiLCJhIjoiY2tkcnI4OHU3MGZ6NDJ0bHFsOHRrcTljeiJ9.D9GoLNq2BgK16q5w3TjJ1g";

const modelFilesURL = "models/"


class WebglView extends Component {
  state = {
      model: this.props.model,
      mapStyle: "",
      modelLayersList: [],
      connectivityLayersList: [],
      showConnectivity: false,
      showModels: true,
      time: 0,
    };

  refreshAssets = async () => {
    if (this.props.store.mapViewPort.zoom < 12) {
      Materialize.toast({
        html: "Please zoom in more to load assets",
        displayLength: 2000,
        classes: "rounded",
      });
    }
    
      this.props.store.getModelList();
      this.props.store.refeshConnectivityLayer();
    
  };

  toggleStreetView = async () => {
    this.props.store.toggleStreetViewMode();
  };

  _onStyleChange = (mapStyle) => this.setState({ mapStyle });

  layerCheck = (name, isChecked) => {
    if (name === "models") {
      this.setState({
        showModels: isChecked,
      });
    } else if (name === "connectivity") {
      this.setState({
        showConnectivity: isChecked,
      });
    }
  };

  _renderEffects() {

    let effectsToRender = [];
    
    const ambientLight = new AmbientLight({
      color: [255, 255, 255],
      intensity: 1
    });
    effectsToRender.push(ambientLight);
    
    if(this.props.store.assets.toString() != "" &&  this.props.store.assets != []){
      this.props.store.assets.map((model) => {
        try{
          if(model){
            const light1 = new PointLight({
              color: [255, 255, 255],
              intensity: 2,
              position: [model.geometry.X, model.geometry.Y, 400]
            });
            effectsToRender.push(light1);

            const light2 = new PointLight({
              color: [255, 255, 255],
              intensity: 2,
              position: [model.geometry.X-20, model.geometry.Y-20, 400]
            });
            effectsToRender.push(light2);

            const light3 = new PointLight({
              color: [255, 255, 255],
              intensity: 2,
              position: [model.geometry.X+20, model.geometry.Y+20, 400]
            });
            effectsToRender.push(light3);

            const light4 = new PointLight({
              color: [255, 255, 255],
              intensity: 2,
              position: [model.geometry.X+20, model.geometry.Y-20, 400]
            });
            effectsToRender.push(light4);

            const light5 = new PointLight({
              color: [255, 255, 255],
              intensity: 2,
              position: [model.geometry.X-20, model.geometry.Y+20, 400]
            });
            effectsToRender.push(5);

          }
        }
        catch{}
      });
    }

    var effects = new LightingEffect(effectsToRender)
    return [effects];
  }

  _renderLayers() {
    let LayersList = [];

    const meshLayerMat = {};
    meshLayerMat.ambient = 1;
    meshLayerMat.diffuse = 0.2;
    meshLayerMat.shininess = 0;

    if(this.props.store.assets.toString() != "" &&  this.props.store.assets != []){
      this.props.store.assets.map((model) => {
        if(model){

          //DO NOT DELETE THIS LOG STATEMENT
          console.log(model.geometry.ModelName, model.Equipement, model.AssetId);

          try {
            let layer = new SimpleMeshLayer({
              id: `models_${model.AssetId}`,
              data: [
                {
                  position: [model.geometry.X, model.geometry.Y, model.geometry.Z],
                  zoom: 18,
                  orientation: [
                    model.geometry.RotationX,
                    model.geometry.RotationY,
                    model.geometry.RotationZ,
                  ],
                  model: model,
                },
              ],
              mesh: modelFilesURL + model.geometry.ModelName + ".obj",
              material: meshLayerMat,
              sizeScale: model.geometry.Scale,
              visible: this.state.showModels ? true : false,
              coordinateSystem: COORDINATE_SYSTEM.LNGLAT,
              getPosition: (d) => d.position,
              getOrientation: (d) => d.orientation,
              pickable: true,
              onClick: (event) => {
                this.props.store.setDataPanelModel(event.object.model);
              },
            });

            LayersList.push(layer);
          } catch {}
        }
      });
    }
    // eslint-disable-next-line
    toJS(this.props.store.subNetConectivity).map((x, i) => {
      const staticPathLayerSubNet = new PathLayer({
        id: `${i}-subnet-connectivity`,
        data: x,
        pickable: false,
        visible: this.state.showConnectivity ? true : false,
        widthMinPixels: 2,
        opacity: 0.6,
        getPath: (d) => d,
        positionFormat: 'XYZ',
        getColor: [253, 128, 93],
        getWidth: (d) => 1,
      });
      LayersList.push(staticPathLayerSubNet);
    });
    
    // eslint-disable-next-line
    toJS(this.props.store.stagedConnectivityData).map((x, i) => {
      const staticPathLayerSubNetStaged = new PathLayer({
        id: `${i}-subnet-connectivity-staged`,
        data: x,
        pickable: false,
        visible: this.state.showConnectivity ? true : false,
        widthMinPixels: 2,
        opacity: 0.76,
        getPath: (d) => d,
        positionFormat: 'XYZ',
        getColor: [246, 0, 0],
        getWidth: (d) => 1,
        getDashArray: [2, 3],
        dashJustified: true,
        extensions: [new PathStyleExtension({ dash: true })],
      });
      LayersList.push(staticPathLayerSubNetStaged);
    });

    return [LayersList];
  }

  render() {
    return (
      <div
      className="mapContainer" >
        <DeckGL
          initialViewState={this.props.store.mapViewPort}
          controller={true}
          onViewStateChange={({ viewState }) =>
            this.props.store.updateMapViewport(viewState)
          }
          effects={this._renderEffects()}
          layers={this._renderLayers()}
        >

          <MapView 
            id="mapView" 
            controller={true}>
            <StaticMap
              mapboxApiAccessToken={accessToken}
              mapStyle={this.state.mapStyle}
            />
          </MapView>

          <ControlPanel
            setMapStyle={this._onStyleChange}
            additionalLayers={this.state.layersList}
            toggleParent={this.layerCheck}
          />
        </DeckGL>

        <RefreshButton refreshClick={this.refreshAssets} />
        <LayersButton />
        <StreetViewModeButton toggleStreetView={this.toggleStreetView} />

        <DataView />
      </div>
    );
  }
}

export default inject("store")(observer(WebglView));
