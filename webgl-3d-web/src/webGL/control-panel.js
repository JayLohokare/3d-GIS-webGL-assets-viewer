import React, { PureComponent } from "react";
import { fromJS } from "immutable";
import MAP_STYLE from "../map-style-basic-v8.json";
import { inject, observer } from "mobx-react";
import Materialize from "materialize-css";

const defaultMapStyle = fromJS(MAP_STYLE);
const categories = [
  "labels",
  "roads",
  "buildings",
  "parks",
  "water",
  "background",
  "connectivity",
  "models",
];

// Layer id patterns by category
const layerSelector = {
  background: /background/,
  water: /water/,
  parks: /park/,
  buildings: /building/,
  roads: /bridge|road|tunnel/,
  labels: /label|place|poi/,
  connectivity: /trips-layer/,
  models: /model-layer/,
};

// Layer color class by type
const colorClass = {
  line: "line-color",
  fill: "fill-color",
  background: "background-color",
  symbol: "text-color",
};

class StyleControls extends PureComponent {
  constructor(props) {
    super(props);

    this._defaultLayers = defaultMapStyle.get("layers");

    this.state = {
      visibility: {
        water: true,
        parks: true,
        buildings: true,
        roads: true,
        labels: true,
        background: true,
        connectivity: false,
        models: true,
      },
      color: {
        water: "#DBE2E6",
        parks: "#E6EAE9",
        buildings: "#c0c0c8",
        roads: "#ffffff",
        labels: "#78888a",
        background: "#EBF0F0",
      },
    };
  }

  componentDidMount() {
    this._updateMapStyle(this.state);

    Materialize.Modal.init(document.querySelectorAll(".modal"));
    Materialize.Tooltip.init(document.querySelectorAll(".tooltipped"));
  }

  _onColorChange(name, event) {
    const color = { ...this.state.color, [name]: event.target.value };
    this.setState({ color });
    this._updateMapStyle({ ...this.state, color });
  }

  _onVisibilityChange(name, event) {
    const visibility = {
      ...this.state.visibility,
      [name]: event.target.checked,
    };
    this.setState({ visibility });
    this._updateMapStyle({ ...this.state, visibility });
    this.props.toggleParent(name, event.target.checked);
  }

  _updateMapStyle({ visibility, color }) {
    const layers = this._defaultLayers
      .filter((layer) => {
        const id = layer.get("id");
        return categories.every(
          (name) => visibility[name] || !layerSelector[name].test(id)
        );
      })
      .map((layer) => {
        const id = layer.get("id");
        const type = layer.get("type");
        const category = categories.find((name) =>
          layerSelector[name].test(id)
        );
        if (category && colorClass[type]) {
          return layer.setIn(["paint", colorClass[type]], color[category]);
        }
        return layer;
      });
    this.props.setMapStyle(defaultMapStyle.set("layers", layers));
  }

  _renderLayerControl(name) {
    const { visibility, color } = this.state;
    return (
      <div key={name}>
        <label className="input " width="200%">
          <div className="row">
            <input
              className="col s2 m2 l2"
              type="checkbox"
              checked={visibility[name]}
              onChange={this._onVisibilityChange.bind(this, name)}
            />

            <span className="col s7 m7 l4 center-aligned center offset-m1 offset-l3 offset-s1">
              {name}
            </span>

            {name === "connectivity" || name === "models" ? null : (
              <input
                className="col s2 m2 l1"
                type="color"
                value={color[name]}
                disabled={!visibility[name]}
                onChange={this._onColorChange.bind(this, name)}
              />
            )}
          </div>
        </label>
      </div>
    );
  }

  render() {
    return (
      <div>
        
        <div id="modal1" className="modal">
          <div className="layers-modal-header black">
            <h4 className="white-text center-align">Edit Layers</h4>
          </div>
          <div className="modal-content container center-align">
            <div>
              {categories.map((name) => this._renderLayerControl(name))}
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default inject("store")(observer(StyleControls));
